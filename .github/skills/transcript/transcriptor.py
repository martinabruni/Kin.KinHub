#!/usr/bin/env python3
"""
export_csharp_transcript.py

Genera un "transcript" di repository:
- Produce:
  - header con metadati
  - tree stile "unix tree"
  - contenuti di ogni file delimitati da marker BEGIN/END
"""

from __future__ import annotations

import argparse
import os
from dataclasses import dataclass
from datetime import datetime
from pathlib import Path
from typing import Dict, List, Set, Tuple


ALLOWED_EXTENSIONS_DEFAULT = {".cs", ".csproj", ".json", ".tsx", ".ts", ".razor"}
EXCLUDE_DIRS_DEFAULT = {".git", "bin", "obj", "node_modules", "artifacts"}


@dataclass(frozen=True)
class FileEntry:
    relative_path: str
    full_path: Path


@dataclass(frozen=True)
class IndexEntry:
    section_key: str
    relative_path: str
    start_line: int
    end_line: int


class TreeNode:
    __slots__ = ("dirs", "files")

    def __init__(self) -> None:
        self.dirs: Dict[str, "TreeNode"] = {}
        self.files: List[str] = []


def is_excluded_path(relative_path: str, excluded_dirs: Set[str]) -> bool:
    # Split su / e \ in modo robusto
    parts = [p for p in relative_path.replace("\\", "/").split("/") if p]
    for seg in parts:
        if seg.casefold() in excluded_dirs:
            return True
    return False


def add_tree_path(tree: TreeNode, relative_path: str) -> None:
    parts = [p for p in relative_path.replace("\\", "/").split("/") if p]
    node = tree
    if len(parts) > 1:
        for d in parts[:-1]:
            if d not in node.dirs:
                node.dirs[d] = TreeNode()
            node = node.dirs[d]
    node.files.append(parts[-1])


def write_tree_node(node: TreeNode, prefix: str, out_lines: List[str]) -> None:
    dir_names = sorted(node.dirs.keys())
    file_names = sorted(node.files)

    children: List[Tuple[str, str]] = []
    children.extend([("dir", d) for d in dir_names])
    children.extend([("file", f) for f in file_names])

    for i, (kind, name) in enumerate(children):
        is_last = i == len(children) - 1
        connector = "`-- " if is_last else "|-- "
        suffix = "/" if kind == "dir" else ""
        out_lines.append(f"{prefix}{connector}{name}{suffix}")

        if kind == "dir":
            next_prefix = prefix + ("    " if is_last else "|   ")
            write_tree_node(node.dirs[name], next_prefix, out_lines)


def safe_read_text(path: Path) -> str:
    """
    Legge il file come testo, provando UTF-8 (con BOM), poi fallback permissivo.
    Mantiene comportamento simile a Get-Content -Raw, ma robusto su encoding misti.
    """
    try:
        return path.read_text(encoding="utf-8-sig", errors="strict")
    except UnicodeDecodeError:
        # Fallback: non fallire, ma preserva più contenuto possibile
        return path.read_text(encoding="utf-8", errors="replace")
    except Exception as e:
        return f"[ERROR READING FILE] {e}"


def default_outfile(resolved_root: Path) -> Path:
    out_dir = resolved_root / "transcripts"
    out_dir.mkdir(parents=True, exist_ok=True)
    return out_dir / f"latest-transcript.txt"


def default_index_file(out_path: Path) -> Path:
    return out_path.parent / "index.txt"


def collect_files(
    root: Path,
    allowed_exts: Set[str],
    excluded_dirs: Set[str],
) -> List[FileEntry]:
    selected: List[FileEntry] = []
    root_str = str(root)

    # rglob("*") + is_file() è cross-platform e semplice
    for p in root.rglob("*"):
        if not p.is_file():
            continue

        if p.suffix.casefold() not in allowed_exts:
            continue

        rel = os.path.relpath(str(p), root_str)
        if is_excluded_path(rel, excluded_dirs):
            continue

        selected.append(FileEntry(relative_path=rel.replace("\\", "/"), full_path=p))

    selected.sort(key=lambda e: e.relative_path)
    return selected


def derive_section_key(relative_path: str) -> str:
    parts = [p for p in relative_path.replace("\\", "/").split("/") if p]
    parent_parts = parts[:-1]

    dotted = [seg for seg in parent_parts if seg.count(".") >= 2]
    if dotted:
        return dotted[0]

    if len(parent_parts) >= 3:
        return ".".join(parent_parts[:3])

    if parent_parts:
        return ".".join(parent_parts)

    file_name = parts[-1] if parts else "root"
    stem = file_name.rsplit(".", 1)[0]
    return stem or "root"


def build_transcript(
    resolved_root: Path,
    selected: List[FileEntry],
    allowed_exts: Set[str],
) -> Tuple[str, List[IndexEntry]]:
    tree = TreeNode()
    for entry in selected:
        add_tree_path(tree, entry.relative_path)

    lines: List[str] = []
    index_entries: List[IndexEntry] = []
    lines.append("# C# Repository Transcript")
    lines.append("")
    lines.append(f"GeneratedAt: {datetime.now().isoformat(timespec='seconds')}")
    lines.append(f"ProjectRootPath: {resolved_root}")
    lines.append(f"IncludedExtensions: {', '.join(sorted(allowed_exts))}")
    lines.append(f"IncludedFiles: {len(selected)}")
    lines.append("")
    lines.append("## Tree")
    lines.append(".")
    write_tree_node(tree, "", lines)
    lines.append("")
    lines.append("## File Contents")

    for entry in selected:
        lines.append("")
        lines.append(f"===== BEGIN FILE: {entry.relative_path} =====")

        content_start_line = len(lines) + 1
        content = safe_read_text(entry.full_path)
        content_lines = content.splitlines()
        if content_lines:
            lines.extend(content_lines)
            content_end_line = len(lines)
        else:
            # File vuoto o errore convertito in stringa vuota: mantieni range minimo stabile
            content_end_line = content_start_line

        lines.append(f"===== END FILE: {entry.relative_path} =====")

        index_entries.append(
            IndexEntry(
                section_key=derive_section_key(entry.relative_path),
                relative_path=entry.relative_path,
                start_line=content_start_line,
                end_line=content_end_line,
            )
        )

    # Join con newline, come in PS
    return "\n".join(lines) + "\n", index_entries


def build_index_text(
    resolved_root: Path,
    transcript_path: Path,
    index_entries: List[IndexEntry],
) -> str:
    generated_at = datetime.now().isoformat(timespec="seconds")
    section_ranges: Dict[str, Tuple[int, int, int]] = {}

    for item in index_entries:
        if item.section_key in section_ranges:
            curr_start, curr_end, curr_count = section_ranges[item.section_key]
            new_start = min(curr_start, item.start_line)
            new_end = max(curr_end, item.end_line)
            section_ranges[item.section_key] = (new_start, new_end, curr_count + 1)
        else:
            section_ranges[item.section_key] = (item.start_line, item.end_line, 1)

    lines: List[str] = []
    lines.append("# Transcript Index")
    lines.append("")
    lines.append(f"GeneratedAt: {generated_at}")
    lines.append(f"ProjectRootPath: {resolved_root}")
    lines.append(f"TranscriptPath: {transcript_path}")
    lines.append("IndexFormat: <SectionKey> | <RelativePath> | <StartLine-EndLine>")
    lines.append("")
    lines.append("## Sections")

    for section_key in sorted(section_ranges.keys()):
        start_line, end_line, file_count = section_ranges[section_key]
        lines.append(
            f"- {section_key}: lines {start_line}-{end_line} ({file_count} files)"
        )

    lines.append("")
    lines.append("## File Ranges")
    for item in sorted(index_entries, key=lambda x: x.relative_path):
        lines.append(
            f"- {item.section_key} | {item.relative_path} | {item.start_line}-{item.end_line}"
        )

    return "\n".join(lines) + "\n"


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(
        description="Export a C# repository transcript (tree + contents)."
    )
    parser.add_argument(
        "--project-root",
        required=True,
        help="Path to repository root (equivalente di ProjectRootPath).",
    )
    parser.add_argument(
        "--out-file",
        default="",
        help="Path output transcript. Se vuoto: artifacts/transcripts/csharp-transcript-<timestamp>.txt",
    )
    parser.add_argument(
        "--exclude-dirs",
        nargs="*",
        default=sorted(EXCLUDE_DIRS_DEFAULT),
        help="Directory names to exclude (match per-segment).",
    )
    parser.add_argument(
        "--extensions",
        nargs="*",
        default=sorted(ALLOWED_EXTENSIONS_DEFAULT),
        help="Allowed file extensions (include leading dot).",
    )
    return parser.parse_args()


def main() -> int:
    args = parse_args()

    root = Path(args.project_root)
    if not root.exists():
        print(f"ERROR: ProjectRootPath not found: {root}")
        return 1

    resolved_root = root.resolve()

    excluded_dirs = {d.casefold() for d in args.exclude_dirs if d and d.strip()}
    allowed_exts = {e.casefold() for e in args.extensions if e and e.strip()}
    # Normalizza: assicurati che inizino con "."
    allowed_exts = {e if e.startswith(".") else f".{e}" for e in allowed_exts}

    out_path: Path
    if not args.out_file.strip():
        out_path = default_outfile(resolved_root)
    else:
        out_path = Path(args.out_file).expanduser()
        # Se path relativo, rendilo relativo alla cwd (come comportamento tipico)
        if not out_path.is_absolute():
            out_path = (Path.cwd() / out_path).resolve()
        out_path.parent.mkdir(parents=True, exist_ok=True)

    selected = collect_files(resolved_root, allowed_exts, excluded_dirs)
    transcript, index_entries = build_transcript(resolved_root, selected, allowed_exts)
    index_path = default_index_file(out_path)
    index_text = build_index_text(resolved_root, out_path, index_entries)

    out_path.write_text(transcript, encoding="utf-8")
    index_path.write_text(index_text, encoding="utf-8")
    print(f"Transcript generated: {out_path}")
    print(f"Index generated: {index_path}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())

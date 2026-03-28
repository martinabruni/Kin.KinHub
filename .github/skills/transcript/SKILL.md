---
name: transcript
description: >
  **CODEBASE CONTEXT SKILL** — Generate and query repository transcripts for answering architecture questions, 
  code location queries, dependency analysis, and cross-file understanding. USE FOR: "where is X defined?", 
  "show me all files related to Y", "explain the architecture", "how does feature Z work across files", 
  "what files contain X pattern". Executes transcriptor.py to generate indexed snapshots, then uses index.txt 
  to locate relevant sections in latest-transcript.txt efficiently. INVOKES: terminal (run Python script), 
  file system (read index and transcript portions). DO NOT USE FOR: single file questions (use grep or read_file 
  directly), real-time debugging, or questions about uncommitted changes.
---

# Transcript Skill

## Purpose

Generate and query indexed repository transcripts to answer questions requiring understanding of:

- File locations and organization
- Cross-file dependencies and relationships
- Architecture and structure
- Multi-file feature implementations
- Codebase patterns and conventions

## Workflow

```toon
transcript_skill:
  objective: Answer codebase questions using indexed repository transcript

  trigger_conditions:
    - user_asks_about_architecture
    - user_asks_where_code_is_located
    - user_asks_cross_file_question
    - user_asks_about_multiple_files
    - user_needs_codebase_overview

  do_not_trigger_when:
    - question_is_about_single_known_file
    - question_is_about_uncommitted_changes
    - real_time_debugging_needed
    - grep_or_semantic_search_sufficient

  process:
    steps[5]:
      1: check_transcript_freshness
      2: generate_or_update_transcript_if_needed
      3: read_index_to_locate_relevant_sections
      4: read_specific_line_ranges_from_transcript
      5: answer_question_with_context

  step_1_check_freshness:
    location: transcripts/latest-transcript.txt
    check_age: if_older_than_5_minutes_regenerate
    check_exists: if_missing_regenerate

  step_2_generate_transcript:
    script: .github/skills/transcript/transcriptor.py
    command: python .github/skills/transcript/transcriptor.py --project-root "."
    output_files[2]:
      - transcripts/latest-transcript.txt
      - transcripts/index.txt
    working_directory: repository_root
    wait_for_completion: true

  step_3_read_index:
    file: transcripts/index.txt
    parse_for:
      - section_keys
      - file_paths
      - line_ranges: start_line-end_line
    match_user_question_to_sections: use_keywords_and_patterns

  step_4_read_transcript_portions:
    file: transcripts/latest-transcript.txt
    strategy: read_only_relevant_line_ranges_from_index
    avoid: reading_entire_transcript
    example: if_index_shows_lines_150-300_read_those_only

  step_5_answer:
    use_context_from: transcript_portions
    cite_specific_files: yes
    provide_line_references: when_relevant
    offer_to_show_code: if_appropriate

rules:
  - always_check_transcript_exists_before_reading
  - always_use_index_to_find_sections_not_full_scan
  - regenerate_if_transcript_older_than_5_minutes
  - read_only_relevant_line_ranges_not_entire_file
  - if_script_fails_inform_user_clearly
  - preserve_file_paths_exactly_as_in_index
  - use_section_keys_to_group_related_files

index_format:
  sections:
    format: "- <section_key>: lines <start>-<end> (<count> files)"
    example: "- Kin.KinHub.Domain: lines 150-450 (12 files)"

  file_ranges:
    format: "- <section_key> | <relative_path> | <start_line>-<end_line>"
    example: "- Kin.KinHub.Domain | src/Domains/User/UserEntity.cs | 234-289"

script_configuration:
  default_extensions[6]:
    - .cs
    - .csproj
    - .json
    - .tsx
    - .ts
    - .razor

  excluded_dirs[5]:
    - .git
    - bin
    - obj
    - node_modules
    - artifacts

  customization: user_can_override_via_script_args

error_handling:
  if_script_not_found:
    - report: "Transcriptor script not found at .github/skills/transcript/transcriptor.py"
    - suggest: check_repository_structure

  if_script_fails:
    - report: full_error_message
    - check: python_installation
    - fallback: use_standard_search_tools

  if_index_missing:
    - regenerate_transcript
    - if_still_missing: report_issue

  if_no_relevant_sections_found:
    - inform_user: "No matching sections in transcript index"
    - suggest: use_grep_or_semantic_search_instead

examples:
  - question: "Where is the User authentication logic?"
    flow: |
      1. Check transcripts/latest-transcript.txt exists and is fresh
      2. Read transcripts/index.txt
      3. Search index for sections matching "auth", "user", "login"
      4. Identify line ranges: e.g., 450-620
      5. Read latest-transcript.txt lines 450-620
      6. Answer: "User authentication is in src/Auth/UserAuthService.cs..."

  - question: "Show me all domain entities"
    flow: |
      1. Generate transcript if needed
      2. Read index.txt
      3. Find all entries with paths matching "**/Domains/**/*.cs"
      4. Read those specific line ranges
      5. List all entity files with brief description

  - question: "How does feature X work across the codebase?"
    flow: |
      1. Ensure transcript is current
      2. Read index to find sections related to X
      3. Read multiple relevant file ranges
      4. Synthesize explanation from cross-file context

optimization:
  - cache_transcript_path_in_session_memory
  - avoid_regenerating_if_recent
  - read_index_first_always
  - use_section_keys_to_minimize_reads
  - read_multiple_relevant_sections_in_parallel

integration:
  works_with:
    - grep_search: for_finding_specific_patterns_first
    - semantic_search: for_concept_based_queries
    - read_file: for_deep_dive_after_locating_files

  supersedes:
    - reading_entire_repository_file_by_file
    - blind_searching_without_index
```

## Quick Reference

### When to Use

- Architecture questions spanning multiple files
- "Where is X?" queries when location unknown
- Understanding feature implementations across files
- Getting codebase overview
- Analyzing file organization and structure

### When NOT to Use

- Single file already identified → use `read_file`
- Simple keyword search → use `grep_search`
- Conceptual queries → use `semantic_search`
- Real-time uncommitted code → search tools

### Command Sequence

```bash
# 1. Generate transcript
python .github/skills/transcript/transcriptor.py --project-root "."

# 2. Read index
transcripts/index.txt

# 3. Locate relevant sections
# Find section keys and line ranges

# 4. Read specific portions
# Read only the line ranges from latest-transcript.txt

# 5. Answer with context
```

### Key Files

- **Script**: `.github/skills/transcript/transcriptor.py`
- **Output**: `transcripts/latest-transcript.txt` (full transcript)
- **Index**: `transcripts/index.txt` (navigation map)

## Advanced Usage

### Custom Extensions

```bash
python transcriptor.py --project-root "." --extensions .cs .fs .vb
```

### Custom Exclusions

```bash
python transcriptor.py --project-root "." --exclude-dirs .git bin obj temp
```

### Section Key Logic

Section keys are derived from file paths to group related files:

- `Kin.KinHub.Domain` → all domain layer files
- `Kin.KinHub.API.Controllers` → controller files
- Groups help locate related functionality efficiently

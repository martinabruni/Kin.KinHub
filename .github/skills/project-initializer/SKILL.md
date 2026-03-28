---
name: project-initializer
description: Initialize and manage DDD-structured .NET projects with standardized Domain, Business, Infrastructure, Presentation, and Test layers. Creates folder structure using dotnet CLI, and generates solution files (.slnx or .sln). Use for new projects, adding/removing layers, or restructuring existing codebases.
---

# DDD Project Initializer

A Python script that scaffolds Domain-Driven Design (DDD) structured .NET projects using the dotnet CLI. Creates a standardized folder hierarchy, C# projects, and solution files following DDD principles.

## When to Use This Skill

Use this skill when you need to:

- **Initialize new DDD projects** with proper layer separation
- **Add infrastructure projects** (SQL Server, PostgreSQL, OpenAI, Redis, etc.)
- **Add presentation projects** (API, Function, Blazor, Web, etc.)
- **Add test projects** organized by layer or type
- **Restructure existing codebases** to follow DDD conventions
- **Remove projects** from an existing DDD structure

## Prerequisites

- **Python 3.8+** installed and available in PATH
- **.NET SDK 8.0+** installed and available in PATH
- Run `dotnet --version` to verify dotnet CLI is available

## Project Structure

The script creates the following standard structure:

```
root/
├── docs/
│   ├── onboarding/
│   ├── features/
│   ├── shared/
│   └── artifacts/
├── ops/
│   ├── iac/
│   └── pipelines/
├── src/
│   ├── Domains/
│   │   └── Client.Project.Domain/
│   ├── Businesses/
│   │   └── Client.Project.Business/
│   ├── Infrastructures/
│   │   ├── Client.Project.SqlServer/
│   │   └── Client.Project.OpenAi/
│   ├── Presentations/
│   │   ├── Client.Project.Api/
│   │   └── Client.Project.Function/
│   └── Tests/
│       ├── Client.Project.UnitTests/
│       └── Client.Project.IntegrationTests/
└── Client.Project.slnx (or .sln)
```

## Naming Conventions

- **Domain/Business layers**: Use suffix pattern (`Client.Project.Domain`, `Client.Project.Business`)
- **Infrastructure/Presentation/Tests layers**: Use type pattern (`Client.Project.{Type}`)

## Usage Examples

### Basic Initialization

```powershell
python .github\skills\project-initializer\ddd_initializer.py --client Kin --project KinHub --presentations Api --infrastructures SqlServer,OpenAi
```

### With Domain Segment

```powershell
python .github\skills\project-initializer\ddd_initializer.py --client Contoso --project Sales --domain Orders --presentations Api,Function --infrastructures SqlServer,Redis --tests UnitTests,IntegrationTests
```

### Classic .sln Format

```powershell
python .github\skills\project-initializer\ddd_initializer.py --client Kin --project KinHub --presentations Api --solution-format sln
```

### Add Projects to Existing Structure

```powershell
python .github\skills\project-initializer\ddd_initializer.py --client Kin --project KinHub --root . --add-project Infrastructures:Redis
```

### Remove Projects

```powershell
python .github\skills\project-initializer\ddd_initializer.py --client Kin --project KinHub --root . --remove-project Infrastructures:Redis
```

## Command-Line Arguments

### Required

- `--client` Client identifier (alphanumeric only)
- `--project` Project identifier (alphanumeric only)

### Optional

- `--domain` Optional domain identifier for multi-domain projects
- `--root` Root directory to scaffold (default: current directory)
- `--target-framework`, `--tfm` Target framework (default: `net10.0`)
- `--solution-format` Solution format: `slnx` (default) or `sln`
- `--presentations`, `-p` Comma-separated presentation project types
- `--infrastructures`, `-i` Comma-separated infrastructure project types
- `--tests`, `-t` Comma-separated test project types
- `--add-project` Add a single project (format: `Category:Type`)
- `--remove-project` Remove a project (format: `Category:Type`)

## Solution Formats

### .slnx (Modern XML Format)

- Default format
- Modern Visual Studio 2022+ format
- Organizes projects in solution folders
- Includes docs/ and ops/ folders and files

### .sln (Classic Format)

- Traditional solution format
- Compatible with all Visual Studio versions
- Generated using `dotnet sln` commands

## Implementation Details

The script uses dotnet CLI commands:

1. **Project creation**: `dotnet new classlib -n <name> -o <path> -f <tfm>`
2. **Post-creation**: Edits .csproj to set `RootNamespace` and `AssemblyName`
3. **Cleanup**: Removes default `Class1.cs` files
4. **Solution (.sln)**: Uses `dotnet new sln` and `dotnet sln add`
5. **Solution (.slnx)**: Manually generates XML structure

## Troubleshooting

### dotnet command not found

Install the .NET SDK from https://dotnet.microsoft.com/download and ensure it's in your PATH.

### Python not found

Install Python from https://www.python.org/downloads/ and ensure it's added to PATH.

### Solution format issues

- For .slnx: Ensure you have Visual Studio 2022 version 17.0+
- For .sln: Use `--solution-format sln`

### Namespace conflicts

The script automatically sets `RootNamespace` and `AssemblyName` in .csproj files after creation.

## Cross-Platform Support

Both Python and dotnet CLI are cross-platform:

- **Windows**: PowerShell, Command Prompt
- **Linux**: Bash, Zsh
- **macOS**: Bash, Zsh

## Version History

- **v2.0.0** (2026-03-28) - Refactored to use dotnet CLI for project creation
  - Added subprocess integration with dotnet commands
  - Added solution format option (slnx/sln)
  - Improved error handling and validation
  - Added Class1.cs cleanup
  - Enhanced cross-platform support

- **v1.0.0** - Initial version with manual .csproj generation

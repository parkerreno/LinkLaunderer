# LinkLaunderer Copilot Instructions

## Project Overview

LinkLaunderer is an app to clean up URLs for sharing, built with .NET MAUI on .NET 10.

## Project Structure

The solution consists of three main projects:

- **LinkLaunderer.App** - The MAUI application (multi-platform UI)
  - Target frameworks: `net10.0-android`, `net10.0-ios`, `net10.0-maccatalyst`, `net10.0-windows10.0.19041.0`
  - Main executable project with MAUI resources (icons, splash screens, fonts)
- **LinkLaunderer.Lib** - Core library with link processing logic
  - Target frameworks: `net10.0`, `net10.0-android`, `net10.0-ios`, `net10.0-maccatalyst`, `net10.0-windows10.0.19041.0`
  - Contains platform-specific code under `Platforms/` subdirectories
- **LinkLaunderer.Test** - Unit tests using MSTest
  - Target framework: `net10.0`
  - Uses MSTest.Sdk 4.0.0-preview

## Platform Support Status

| Platform | Status |
|----------|--------|
| Android  | ⚠️ Currently in development, working but unstable |
| iOS      | ⛔ Future development |
| macOS    | ⛔ Future development |
| Windows  | ⛔ Future development |

**Current Focus**: Android development. While the project structure supports multiple platforms, prioritize Android compatibility and testing.

## Build & Test Commands

Based on the CI workflow (`.github/workflows/dotnet.yml`):

```bash
# Restore workloads (MAUI dependencies)
dotnet workload restore

# Restore NuGet packages
dotnet restore

# Build the solution
dotnet build --no-restore

# Run tests
dotnet test --no-build --verbosity normal

# Build Android APK (Release)
dotnet publish src/LinkLaunderer.App/LinkLaunderer.App.csproj -f net10.0-android -c Release
```

## Code Style & Conventions

### General Guidelines

- Follow the conventions defined in `.editorconfig`
- Use spaces for indentation (4 spaces for C#, 2 spaces for .csproj files)
- End of line: CRLF (Windows-style)
- Do NOT insert final newline (error level in .editorconfig)

### C# Coding Style

- **Variable declarations**: Do NOT use `var` - always use explicit types
- **Usings**: Place using directives inside namespace (error level)
- **Namespaces**: Use block-scoped namespaces (not file-scoped)
- **Braces**: Always use braces for code blocks
- **Naming**: 
  - Interfaces start with `I` (e.g., `ILinkProcessor`)
  - Types, methods, properties use PascalCase
- **Modifiers**: Always specify accessibility modifiers for non-interface members
- **this qualifier**: Use `this.` for properties and methods
- **Expression bodies**: 
  - Use for accessors, indexers, lambdas, and properties
  - Do NOT use for constructors, methods, or operators
- **Pattern matching**: Prefer pattern matching over `is` with cast and `as` with null check
- **Primary constructors**: Prefer primary constructors when appropriate
- **Top-level statements**: Prefer top-level statements where applicable

### File Organization

- Sort using directives with System namespaces first
- Place members with other members of the same kind
- Prefer readonly fields where applicable

## Technology Stack

- **.NET Version**: 10.0.0 (specified in `global.json`)
- **Test Framework**: MSTest with Microsoft.Testing.Platform
- **MAUI Version**: Specified via `$(MauiVersion)` variable
- **Nullable Reference Types**: Enabled across all projects
- **Implicit Usings**: Enabled

## Development Guidelines

1. **MAUI Workloads**: When making changes that affect MAUI dependencies, ensure workloads are restored before building
2. **Platform-Specific Code**: Use the `Platforms/` directory structure for platform-specific implementations
3. **Testing**: Add or update tests in LinkLaunderer.Test for any new functionality
4. **Android Focus**: When implementing new features, prioritize and test on Android first
5. **Build Verification**: Always ensure the project builds successfully on Windows (as per CI configuration)

## Common Patterns

- The solution uses `.slnx` format (Visual Studio XML solution file)
- MAUI resources are organized under `Resources/` with subdirectories for AppIcon, Splash, Fonts, and Raw assets
- The library project supports both standard .NET 10 and MAUI-specific target frameworks

## Notes for Copilot

- When suggesting code changes, ensure they follow the strict .editorconfig rules
- Test infrastructure exists - add tests for new functionality
- CI runs on Windows with .NET 10.x and MAUI Android workloads
- The project uses C# latest language version features
- Prefer simple using statements set to false - use traditional using blocks

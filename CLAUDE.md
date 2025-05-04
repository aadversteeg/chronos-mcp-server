# CLAUDE.md for chronos-mcp-server

This file provides specific guidance for Claude Code when working with this repository.

## C# Project Settings

### ImplicitUsings and GlobalUsings

- Do not use ImplicitUsings or GlobalUsings in project files
- Always add explicit using directives at the top of each file
- Remove the ImplicitUsings property from .csproj files
- Do not use GlobalUsings.cs files

### Code Style

- Add explicit namespace imports at the top of each file
- Use full namespace qualifications for all types
- For test files, include an explicit `using Xunit;` statement when using test attributes
- Format code with 4-space indentation

## Project Structure

- Follow established patterns in the codebase
- `Core.Application` contains shared services and interfaces
- `Core.Infrastructure.McpServer` contains the server implementation
- Unit test projects follow the "UnitTests.[ProjectName]" naming convention

## Service Implementation

- Implement DI-based services following existing patterns
- Register services using extension methods
- Use constructor injection for dependencies

## Testing

- Follow xUnit testing patterns with Fact/Theory attributes
- Use descriptive DisplayName attributes for tests
- Follow the Arrange/Act/Assert pattern in test methods
- Use Moq and FluentAssertions libraries for testing
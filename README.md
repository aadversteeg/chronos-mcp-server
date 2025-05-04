# Chronos MCP Server

A time-related server implementing the Model Context Protocol (MCP). This server provides timezone-aware date and time information through a simple MCP interface.

## Overview

The Chronos MCP server is built with .NET Core using the Model Context Protocol C# SDK ([github.com/modelcontextprotocol/csharp-sdk](https://github.com/modelcontextprotocol/csharp-sdk)). It provides tools for accessing date and time information in different timezones. The server is designed to be lightweight and demonstrates how to create a custom MCP server with practical functionality and proper error handling. It can be deployed either directly on a machine or as a Docker container.

## Features

- Get current date and time in any supported timezone
- Optional default timezone configuration through appsettings.json and environment variables
- Proper error handling for invalid timezone requests and missing default timezone
- Functional error handling using Result pattern

## Getting Started

### Prerequisites

- .NET 9.0 (for local development/deployment)
- Docker (for container deployment)


### Build Instructions (for development)

If you want to build the project from source:

1. Clone this repository:
   ```bash
   git clone https://github.com/aadversteeg/chronos-mcp-server.git
   ```

2. Navigate to the project root directory:
   ```bash
   cd chronos-mcp-server
   ```

3. Build the project using:
   ```bash
   dotnet build src/chronos.sln
   ```

4. Run the tests:
   ```bash
   dotnet test src/chronos.sln
   ```

## Docker Support

### DockerHub Image

The Chronos MCP Server is available on DockerHub at `aadversteeg/chronos-mcp-server`.

```bash
# Pull the latest version
docker pull aadversteeg/chronos-mcp-server:latest

# Or pull a specific version
docker pull aadversteeg/chronos-mcp-server:0.0.1
```

### Manual Docker Build

If you need to build the Docker image yourself:

```bash
# Navigate to the repository root
cd chronos-mcp-server

# Build the Docker image
docker build -f src/Core.Infrastructure.McpServer/Dockerfile -t chronos-mcp-server:latest src/

# Run the locally built image (with optional DefaultTimeZoneId)
docker run -d --name chronos-mcp -e "DefaultTimeZoneId=Europe/Amsterdam" chronos-mcp-server:latest
```

### Automated Builds with GitHub Actions

This repository includes a GitHub Actions workflow that automatically builds and pushes the Docker image to DockerHub when a version tag is created.

To trigger a new build:

1. Create and push a tag with a semantic version:
   ```bash
   git tag 1.0.0
   git push origin 1.0.0
   ```

2. The workflow will automatically build and push the Docker image with the tags:
   - `aadversteeg/chronos-mcp-server:latest`
   - `aadversteeg/chronos-mcp-server:1.0.0`

## MCP Protocol Usage

### Client Integration

To connect to the Chronos MCP Server from your applications:

1. Use the Model Context Protocol C# SDK or any MCP-compatible client
2. Configure your client to connect to the server's endpoint
3. Call the available tools described below

### Available Tools

#### get_current_date_and_time

Gets the current date and time in the specified timezone or the default timezone (if configured).

Parameters:
- `timezoneId` (optional if DefaultTimeZoneId is configured): The timezone identifier (e.g., 'America/New_York', 'Eastern Standard Time'). This parameter is required if no DefaultTimeZoneId is configured.

Example request:
```json
{
  "name": "get_current_date_and_time",
  "parameters": {
    "timezoneId": "Europe/London"
  }
}
```

Example response:
```json
{
  "date": "2023-12-25",
  "time": "12:00:00",
  "timezone": "Europe/London"
}
```

#### get_default_timezone_id

Gets the default timezone identifier configured for the server. Returns an error if no default timezone is configured.

Example request:
```json
{
  "name": "get_default_timezone_id",
  "parameters": {}
}
```

Example response (if configured):
```
Europe/Amsterdam
```

Example error response (if not configured):
```json
{
  "error": {
    "message": "DefaultTimeZoneId is not set.",
    "code": "NoDefaultTimeZoneId"
  }
}
```

## Configuration

### DefaultTimeZoneId

The `DefaultTimeZoneId` setting determines which timezone is used when no specific timezone is requested. This value must be a valid timezone identifier recognized by the operating system. This setting is optional - if not provided, the server will still function but will require a timezone ID parameter for each request.

You can set the DefaultTimeZoneId in two ways:

1. **appsettings.json** file (for local deployment):
```json
{
  "DefaultTimeZoneId": "America/New_York"
}
```

2. **Environment Variables** (for containerized deployments):
```bash
# When running the Docker container
docker run -e "DefaultTimeZoneId=Europe/Amsterdam" aadversteeg/chronos-mcp-server:latest
```

Valid timezone identifiers include:
- Standard IANA timezone names (e.g., "America/New_York", "Europe/London", "Asia/Tokyo")
- Windows timezone IDs (e.g., "Eastern Standard Time", "W. Europe Standard Time")

If the DefaultTimeZoneId setting is not specified, no default timezone will be used, and requests must explicitly provide a timezone ID parameter.

## Configuring Claude Desktop

### Using Local Installation

To configure Claude Desktop to use a locally installed Chronos server:

1. Add the server configuration to the `mcpServers` section in your Claude Desktop configuration:
```json
"chronos": {
  "command": "dotnet",
  "args": [
    "YOUR_PATH_TO_DLL\\Core.Infrastructure.McpServer.dll"
  ],
  "env": {
    "DefaultTimeZoneId": "Europe/Amsterdam"
  }
}
```

2. Save the file and restart Claude Desktop

### Using Docker Container

To use the Chronos server from a Docker container with Claude Desktop:

1. Add the server configuration to the `mcpServers` section in your Claude Desktop configuration:
```json
"chronos": {
  "command": "docker",
  "args": [
    "run",
    "--rm",
    "-i",
    "-e", "DefaultTimeZoneId=Europe/Amsterdam",
    "aadversteeg/chronos-mcp-server:latest"
  ]
}
```

2. Save the file and restart Claude Desktop

## Configuring Claude Code

To add the chronos server to Claude Code, use the following command:

```bash
claude mcp add chronos -- docker run -i --rm -e DefaultTimeZoneId=Europe/Amsterdam aadversteeg/chronos-mcp-server:latest
```

You can customize the default timezone by modifying the DefaultTimeZoneId environment variable.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
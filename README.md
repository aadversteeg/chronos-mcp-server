# Chronos MCP Server

A time-related server implementing the Model Context Protocol (MCP). This server provides timezone-aware date and time information through a simple MCP interface.

## Overview

The Chronos MCP server is built with .NET Core using the Model Context Protocol C# SDK ([github.com/modelcontextprotocol/csharp-sdk](https://github.com/modelcontextprotocol/csharp-sdk)). It provides tools for accessing date and time information in different timezones. The server is designed to be lightweight and demonstrates how to create a custom MCP server with practical functionality. It can be deployed either directly on a machine or as a Docker container.

## Features

- Get current date and time in any supported timezone
- Default timezone configuration through appsettings.json end environment
- Proper error handling for invalid timezone requests

## Getting Started

### Prerequisites

- .NET 9.0
- Docker (optional, for container deployment)

### Build Instructions

1. Clone this repository:
   ```
   git clone https://github.com/aadversteeg/chronos-mcp-server.git
   ```

2. Navigate to the project root directory:
   ```
   cd chronos-mcp-server
   ```

3. Build the project using:
   ```
   dotnet build src/chronos.sln
   ```

## Docker Support

### Manual Docker Build

```bash
# Build the Docker image
docker build -f src/Core.Infrastructure.McpServer/Dockerfile -t chronos-mcp-server:latest src/

# Push to a local registry
docker tag chronos-mcp-server:latest localhost:5000/chronos-mcp-server:latest
docker push localhost:5000/chronos-mcp-server:latest
```

### DockerHub Image

The Chronos MCP Server is available on DockerHub at `aadversteeg/chronos-mcp-server`.

```bash
# Pull the latest version
docker pull aadversteeg/chronos-mcp-server:latest

# Or pull a specific version
docker pull aadversteeg/chronos-mcp-server:1.0.0
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

For this to work, you need to set up these secrets in your GitHub repository:
- `DOCKERHUB_USERNAME`: Your Docker Hub username
- `DOCKERHUB_TOKEN`: Your Docker Hub access token (create at https://hub.docker.com/settings/security)

## Configuration

### DefaultTimeZoneId

The `DefaultTimeZoneId` setting determines which timezone is used when no specific timezone is requested. This value must be a valid timezone identifier recognized by the operating system.

You can set the DefaultTimeZoneId in two ways:

1. **appsettings.json** file:
```json
{
  "DefaultTimeZoneId": "America/New_York"
}
```

2. **Environment Variables** (useful for containerized deployments)

Valid timezone identifiers include:
- Standard IANA timezone names (e.g., "America/New_York", "Europe/London", "Asia/Tokyo")
- Windows timezone IDs (e.g., "Eastern Standard Time", "W. Europe Standard Time")

If not specified, the server defaults to "UTC".

## Available Tools

### get_current_date_and_time

Gets the current date and time in the specified timezone or the default timezone.

Parameters:
- `timezoneId` (optional): The timezone identifier (e.g., 'America/New_York', 'Eastern Standard Time').

Example response:
```json
{
  "date": "2023-12-25",
  "time": "12:00:00",
  "timezone": "America/New_York"
}
```

### get_default_timezone_id

Gets the default timezone identifier configured for the server.

Example response:
```
UTC
```

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

To use the Chronos server from a Docker container:

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

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
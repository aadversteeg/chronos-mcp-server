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

## Running the Server

After building, you can run the server using:

```
dotnet run --project src/Core.Infrastructure.McpServer/Core.Infrastructure.McpServer.csproj
```

## Docker Support

You can also build and run the server using Docker:

```
# Build the Docker image
docker build -f src/Core.Infrastructure.McpServer/Dockerfile -t chronos-mcp-server:latest src/

# Run the container
docker run -it --rm chronos-mcp-server:latest
```

To push to a local registry:

```
docker tag chronos-mcp-server:latest localhost:5000/chronos-mcp-server:latest
docker push localhost:5000/chronos-mcp-server:latest
```

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
    "localhost:5000/chronos-mcp-server:latest"
  ]
}
```

2. Save the file and restart Claude Desktop

This configuration assumes your Docker container is pushed to a local registry at `localhost:5000`. Adjust the URL as needed for your local registry configuration.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
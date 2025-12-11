#!/bin/bash
# Setup script for Citation Tool MCP Server
# This script builds the MCP server and configures it for use with Claude Desktop

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_DIR="$(dirname "$SCRIPT_DIR")"
MCP_PROJECT="$PROJECT_DIR/CitationTool.McpServer"
DATA_DIR="$HOME/.citation-tool"
DATA_FILE="$DATA_DIR/citations.json"

echo "=== Citation Tool MCP Server Setup ==="
echo ""

# Check for .NET SDK
if ! command -v dotnet &> /dev/null; then
    echo "Error: .NET SDK not found. Please install .NET 9 SDK."
    echo "Visit: https://dotnet.microsoft.com/download"
    exit 1
fi

DOTNET_VERSION=$(dotnet --version)
echo "Found .NET SDK: $DOTNET_VERSION"

# Build the MCP server
echo ""
echo "Building MCP Server..."
cd "$MCP_PROJECT"
dotnet build -c Release

# Create data directory
mkdir -p "$DATA_DIR"

# Check for data file
if [ ! -f "$DATA_FILE" ]; then
    echo ""
    echo "Note: No citation data file found at $DATA_FILE"
    echo "Please export your citations from the web app:"
    echo "  1. Open Citation Tool in your browser"
    echo "  2. Go to Settings > Data Backup"
    echo "  3. Click 'Download Backup'"
    echo "  4. Save the file to: $DATA_FILE"
fi

# Get the built executable path
MCP_EXE="$MCP_PROJECT/bin/Release/net9.0/citation-mcp-server"
if [ -f "$MCP_EXE.dll" ]; then
    MCP_EXE="$MCP_EXE.dll"
fi

echo ""
echo "=== Setup Complete ==="
echo ""
echo "MCP Server built at: $MCP_EXE"
echo "Data file location: $DATA_FILE"
echo ""
echo "To use with Claude Desktop, add this to your claude_desktop_config.json:"
echo ""
echo '{
  "mcpServers": {
    "citation-tool": {
      "command": "dotnet",
      "args": ["'$MCP_EXE'"],
      "env": {
        "CITATION_DATA_PATH": "'$DATA_FILE'"
      }
    }
  }
}'
echo ""
echo "Config file locations:"
echo "  macOS: ~/Library/Application Support/Claude/claude_desktop_config.json"
echo "  Windows: %APPDATA%\\Claude\\claude_desktop_config.json"
echo "  Linux: ~/.config/Claude/claude_desktop_config.json"

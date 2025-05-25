Collector App Specification
Overview
I want to build a .NET console application that collects Markdown (.md) files from various data sources and stores them in a PostgreSQL database.

Supported Data Sources

Specific folders in GitHub repositories.
Specific folders in Azure DevOps repositories.
Configuration

Data sources should be defined in a JSON configuration file included in the project. Each data source entry should include the following properties:

{
  "dataSourceUrl": "https://github.com/user/repo",
  "token": "your-pat-token",
  "folderName": "docs/specs",     // The folder to scan for markdown files
  "category": "engineering",
  "shortName": "repo1-docs"
}
Database

Use PostgreSQL as the storage backend.
Create a docker-compose.yml file to spin up the database.
Database schema should include a Documents table with the following structure:
Documents (
  id SERIAL PRIMARY KEY,
  source TEXT,
  category TEXT,
  shortName TEXT,
  content TEXT,
  fetchedAt TIMESTAMP
)
Markdown content should be stored raw in the content column.
Logging & Error Handling

Use Serilog for logging.
On error (e.g., failed authentication, missing folders), log the error but allow the process to continue.
Architecture Notes

The core logic (e.g., fetching data, parsing, storing) should be abstracted away from the console application's entry point.
Follow clean architecture principles where possible (e.g., separate concerns, use interfaces for services, etc.).
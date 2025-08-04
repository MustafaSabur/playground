# Playground - SQL Server 2025 with Full-Text Search

This repository contains a Docker setup for SQL Server 2025 (preview features based on SQL Server 2022) with Full-Text Search capabilities enabled and configured.

## Features

- SQL Server 2022 (latest) with 2025 preview features
- Full-Text Search enabled and configured
- Sample database with Full-Text Index
- Health checks configured
- Docker Compose setup for easy deployment

## Quick Start

### Using Docker Compose (Recommended)

```bash
# Clone the repository
git clone https://github.com/MustafaSabur/playground.git
cd playground

# Start SQL Server with Full-Text Search
docker-compose up -d

# Check the logs
docker-compose logs -f sqlserver2025
```

### Using Docker directly

```bash
# Build the image
docker build -t sqlserver2025-fulltext .

# Run the container
docker run -d \
  --name sqlserver2025-fulltext \
  -e ACCEPT_EULA=Y \
  -e MSSQL_SA_PASSWORD=YourStrong!Passw0rd \
  -p 1433:1433 \
  sqlserver2025-fulltext
```

## Connection Details

- **Server**: localhost,1433
- **Username**: sa
- **Password**: YourStrong!Passw0rd
- **Database**: PlaygroundDB (created automatically with sample data)

## Testing Full-Text Search

Once the container is running, connect to the database and try these queries:

```sql
-- Connect to PlaygroundDB
USE PlaygroundDB;

-- Test full-text search
SELECT Id, Title, Content 
FROM Documents 
WHERE CONTAINS(Content, 'SQL Server');

-- Search for phrases
SELECT Id, Title, Content 
FROM Documents 
WHERE CONTAINS(Content, '"full-text search"');

-- Search with wildcards
SELECT Id, Title, Content 
FROM Documents 
WHERE CONTAINS(Content, 'perform*');
```

## Configuration

- The SQL Server instance runs in Developer mode (free for development)
- Full-Text Search is automatically configured during container startup
- A sample database `PlaygroundDB` is created with full-text indexed content
- Health checks ensure the service is running properly

## Security Notes

⚠️ **Warning**: This setup uses a default password for demonstration purposes. In production:

1. Change the default SA password
2. Use environment variables or secrets management
3. Configure proper authentication and authorization
4. Use SSL/TLS encryption

## Customization

To modify the setup:

1. Edit `init-scripts/setup-fulltext.sql` to customize the database schema
2. Modify `Dockerfile` to add additional SQL Server features
3. Update `docker-compose.yml` to adjust container configuration

## Troubleshooting

- Check container logs: `docker-compose logs sqlserver2025`
- Verify Full-Text Search is installed: Connect and run `SELECT SERVERPROPERTY('IsFullTextInstalled')`
- Health check status: `docker ps` to see health status

## Requirements

- Docker 20.10+ 
- Docker Compose 1.29+
- At least 4GB RAM available for the container
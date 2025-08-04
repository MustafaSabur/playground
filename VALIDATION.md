# SQL Server 2025 Full-Text Search Validation

This document provides validation steps for the SQL Server 2025 with Full-Text Search Docker container.

## Validation Steps

### 1. Container Health Check
```bash
docker ps
# Should show sqlserver2025-fulltext as "healthy"
```

### 2. SQL Server Connection Test
```bash
docker exec sqlserver2025-fulltext /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P YourStrong!Passw0rd -C -Q "SELECT @@VERSION"
```

### 3. Full-Text Search Feature Test
```bash
docker exec sqlserver2025-fulltext /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P YourStrong!Passw0rd -C -Q "SELECT SERVERPROPERTY('IsFullTextInstalled')"
```

### 4. Database and Data Validation
```bash
docker exec sqlserver2025-fulltext /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P YourStrong!Passw0rd -C -d PlaygroundDB -Q "SELECT COUNT(*) FROM Documents"
```

### 5. Full-Text Search Query Tests
```sql
-- Simple word search
SELECT Title FROM Documents WHERE CONTAINS(Content, 'SQL Server');

-- Phrase search
SELECT Title FROM Documents WHERE CONTAINS(Content, '"full-text search"');

-- Boolean search
SELECT Title FROM Documents WHERE CONTAINS(Content, 'database AND performance');

-- Wildcard search
SELECT Title FROM Documents WHERE CONTAINS(Content, '"perform*"');

-- FREETEXT search
SELECT Title FROM Documents WHERE FREETEXT(Content, 'database management system');
```

## Expected Results

1. **Container Health**: Container should be running and healthy
2. **SQL Server Version**: Microsoft SQL Server 2022 (RTM-CU20) or later
3. **Full-Text Search**: SERVERPROPERTY('IsFullTextInstalled') should return 1
4. **Sample Data**: 5 documents in the PlaygroundDB.Documents table
5. **Search Results**: All search queries should return relevant documents

## Features Implemented

- ✅ SQL Server 2022 (2025 preview features)
- ✅ Full-Text Search enabled and configured
- ✅ Sample database with indexed content
- ✅ Multiple search query examples
- ✅ Docker Compose configuration
- ✅ Health checks and monitoring
- ✅ Persistent data storage
- ✅ Development-ready setup
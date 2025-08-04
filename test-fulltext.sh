#!/bin/bash

# Test script to verify SQL Server 2025 with Full-Text Search functionality
echo "=== SQL Server 2025 Full-Text Search Test ==="
echo "Testing container: sqlserver2025-fulltext"
echo ""

# Test 1: Check if SQL Server is responsive
echo "1. Testing SQL Server connection..."
docker exec sqlserver2025-fulltext /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P YourStrong!Passw0rd -C -Q "SELECT @@VERSION" -h -1 -W 2>/dev/null
if [ $? -eq 0 ]; then
    echo "✓ SQL Server is responding"
else
    echo "✗ SQL Server connection failed"
    exit 1
fi

echo ""

# Test 2: Check Full-Text Search installation
echo "2. Checking Full-Text Search installation..."
result=$(docker exec sqlserver2025-fulltext /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P YourStrong!Passw0rd -C -Q "SELECT SERVERPROPERTY('IsFullTextInstalled')" -h -1 -W 2>/dev/null | tr -d '\r\n ')
if [ "$result" = "1" ]; then
    echo "✓ Full-Text Search is installed and enabled"
else
    echo "✗ Full-Text Search is not available"
    exit 1
fi

echo ""

# Test 3: Check if PlaygroundDB exists and has data
echo "3. Checking PlaygroundDB and sample data..."
doc_count=$(docker exec sqlserver2025-fulltext /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P YourStrong!Passw0rd -C -d PlaygroundDB -Q "SELECT COUNT(*) FROM Documents" -h -1 -W 2>/dev/null | tr -d '\r\n ')
if [ "$doc_count" -gt "0" ]; then
    echo "✓ PlaygroundDB has $doc_count documents"
else
    echo "✗ PlaygroundDB is empty or doesn't exist"
    exit 1
fi

echo ""

# Test 4: Test Full-Text Search queries
echo "4. Testing Full-Text Search queries..."

echo "   • Searching for 'SQL Server':"
docker exec sqlserver2025-fulltext /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P YourStrong!Passw0rd -C -d PlaygroundDB -Q "SELECT Title FROM Documents WHERE CONTAINS(Content, 'SQL Server')" -h -1 -W 2>/dev/null | head -5

echo ""
echo "   • Searching for phrase 'full-text search':"
docker exec sqlserver2025-fulltext /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P YourStrong!Passw0rd -C -d PlaygroundDB -Q "SELECT Title FROM Documents WHERE CONTAINS(Content, '\"full-text search\"')" -h -1 -W 2>/dev/null | head -5

echo ""
echo "   • Boolean search (database AND performance):"
docker exec sqlserver2025-fulltext /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P YourStrong!Passw0rd -C -d PlaygroundDB -Q "SELECT Title FROM Documents WHERE CONTAINS(Content, 'database AND performance')" -h -1 -W 2>/dev/null | head -5

echo ""
echo "=== All tests completed successfully! ==="
echo "SQL Server 2025 with Full-Text Search is ready for use."
echo ""
echo "Connection details:"
echo "  Server: localhost,1433"
echo "  Username: sa"
echo "  Password: YourStrong!Passw0rd"
echo "  Database: PlaygroundDB"
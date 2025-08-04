-- SQL Server 2022/2025 Full-Text Search Setup
-- This script sets up a sample database with Full-Text Search capabilities

PRINT 'Starting Full-Text Search setup...';

-- Check if Full-Text Search is installed
IF SERVERPROPERTY('IsFullTextInstalled') = 1
BEGIN
    PRINT 'Full-Text Search is installed and available.';
END
ELSE
BEGIN
    PRINT 'WARNING: Full-Text Search is not available on this instance.';
END
GO

-- Create a sample database for testing Full-Text Search
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'PlaygroundDB')
BEGIN
    CREATE DATABASE PlaygroundDB;
    PRINT 'Created PlaygroundDB database.';
END
ELSE
BEGIN
    PRINT 'PlaygroundDB database already exists.';
END
GO

USE PlaygroundDB;
GO

-- Create a sample table for full-text search demonstration
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Documents]') AND type in (N'U'))
BEGIN
    CREATE TABLE Documents (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Title NVARCHAR(255) NOT NULL,
        Content NVARCHAR(MAX) NOT NULL,
        CreatedDate DATETIME2 DEFAULT GETDATE()
    );
    PRINT 'Created Documents table.';
END
ELSE
BEGIN
    PRINT 'Documents table already exists.';
END
GO

-- Insert sample data only if table is empty
IF NOT EXISTS (SELECT * FROM Documents)
BEGIN
    INSERT INTO Documents (Title, Content) VALUES
    ('Introduction to SQL Server', 'SQL Server is a relational database management system developed by Microsoft. It supports full-text search capabilities for advanced text searching and indexing.'),
    ('Full-Text Search Overview', 'Full-text search allows you to perform sophisticated searches against character-based data in SQL Server tables. You can search for words, phrases, and even linguistic variations with powerful query syntax.'),
    ('Database Performance Tuning', 'Optimizing database performance involves indexing strategies, query optimization, and proper database design. Full-text indexes can significantly improve search performance for large text columns.'),
    ('SQL Server 2022 Features', 'SQL Server 2022 introduces new features including enhanced security, improved performance, and better cloud integration. Full-text search continues to be a core feature.'),
    ('Advanced Search Techniques', 'Learn about proximity searches, weighted searches, and custom thesaurus files to enhance your full-text search capabilities in SQL Server applications.');
    
    PRINT 'Inserted sample data into Documents table.';
END
ELSE
BEGIN
    PRINT 'Sample data already exists in Documents table.';
END
GO

-- Create Full-Text Catalog if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.fulltext_catalogs WHERE name = 'PlaygroundCatalog')
BEGIN
    CREATE FULLTEXT CATALOG PlaygroundCatalog AS DEFAULT;
    PRINT 'Created Full-Text Catalog: PlaygroundCatalog';
END
ELSE
BEGIN
    PRINT 'Full-Text Catalog PlaygroundCatalog already exists.';
END
GO

-- Get the primary key constraint name
DECLARE @pkName NVARCHAR(128);
SELECT @pkName = kc.name 
FROM sys.key_constraints kc
INNER JOIN sys.tables t ON kc.parent_object_id = t.object_id
WHERE t.name = 'Documents' AND kc.type = 'PK';

-- Create Full-Text Index if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('Documents'))
BEGIN
    IF @pkName IS NOT NULL
    BEGIN
        DECLARE @sql NVARCHAR(MAX);
        SET @sql = 'CREATE FULLTEXT INDEX ON Documents(Content) KEY INDEX ' + QUOTENAME(@pkName) + ' WITH CHANGE_TRACKING AUTO;';
        EXEC sp_executesql @sql;
        PRINT 'Full-Text Index created successfully on Documents.Content column.';
    END
    ELSE
    BEGIN
        PRINT 'ERROR: Could not find primary key for Documents table.';
    END
END
ELSE
BEGIN
    PRINT 'Full-Text Index already exists on Documents table.';
END
GO

-- Test Full-Text Search functionality
PRINT 'Testing Full-Text Search functionality...';

-- Simple full-text search examples
PRINT 'Example 1: Searching for "SQL Server"';
SELECT Id, Title, Content 
FROM Documents 
WHERE CONTAINS(Content, 'SQL Server');

PRINT 'Example 2: Searching for "search" with wildcard';
SELECT Id, Title, Content 
FROM Documents 
WHERE CONTAINS(Content, '"search*"');

PRINT 'Example 3: Searching for phrase "database performance"';
SELECT Id, Title, Content 
FROM Documents 
WHERE CONTAINS(Content, '"database performance"');

-- Display Full-Text Search status
PRINT 'Full-Text Search setup and testing completed successfully!';
PRINT 'Database: PlaygroundDB';
PRINT 'Table: Documents with Full-Text Index on Content column';
PRINT 'Catalog: PlaygroundCatalog';

-- Show some useful queries for testing
PRINT '';
PRINT 'Useful Full-Text Search queries to try:';
PRINT '1. SELECT * FROM Documents WHERE CONTAINS(Content, ''Microsoft'');';
PRINT '2. SELECT * FROM Documents WHERE CONTAINS(Content, ''performance OR optimization'');';
PRINT '3. SELECT * FROM Documents WHERE CONTAINS(Content, ''"full-text search"'');';
PRINT '4. SELECT * FROM Documents WHERE FREETEXT(Content, ''database management system'');';

GO
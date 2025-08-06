-- Create the aiplaygrounddb database first
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'aiplaygrounddb')
BEGIN
    CREATE DATABASE [aiplaygrounddb];
END
GO

-- Switch to the newly created database
USE [aiplaygrounddb];
GO

-- Create a sample table for full-text search with explicit constraint name
CREATE TABLE Documents (
    Id int IDENTITY(1,1),
    Title nvarchar(255) NOT NULL,
    Content nvarchar(max) NOT NULL,
    CreatedDate datetime2 DEFAULT GETDATE(),
    CONSTRAINT PK_Documents PRIMARY KEY (Id)
);
GO

-- Create a full-text catalog
CREATE FULLTEXT CATALOG DocumentCatalog AS DEFAULT;
GO

-- Create a full-text index on the Documents table using the named constraint
CREATE FULLTEXT INDEX ON Documents(Title, Content)
    KEY INDEX PK_Documents
    WITH STOPLIST = SYSTEM;
GO

-- Insert some sample data
INSERT INTO Documents (Title, Content) VALUES 
    ('Getting Started with SQL Server', 'This document explains how to get started with SQL Server and its various features including full-text search capabilities.'),
    ('Advanced Queries', 'Learn advanced SQL queries and how to optimize them for better performance in production environments.'),
    ('Full-Text Search Guide', 'A comprehensive guide to implementing full-text search in SQL Server applications with practical examples.');
GO

-- Test that full-text search is working
SELECT FULLTEXTSERVICEPROPERTY('IsFullTextInstalled') AS IsFullTextInstalled;
GO

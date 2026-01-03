USE TaskManagement;
GO

IF NOT EXISTS (
    SELECT 1 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'Tasks' 
    AND COLUMN_NAME = 'LastReminderSentAt'
)
BEGIN
    ALTER TABLE Tasks
    ADD LastReminderSentAt DATETIME2 NULL;
END
GO

IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'IX_Tasks_LastReminderSentAt' 
    AND object_id = OBJECT_ID('Tasks')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_Tasks_LastReminderSentAt
    ON Tasks (LastReminderSentAt)
    INCLUDE (DueDate);
END
GO

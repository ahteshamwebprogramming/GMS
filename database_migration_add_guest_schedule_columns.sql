-- Add IsDeleted and IsCancelled columns to GuestSchedule table
-- Execute these queries in your SQL Server database

-- Check if columns already exist before adding them
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('GuestSchedule') AND name = 'IsDeleted')
BEGIN
    ALTER TABLE GuestSchedule
    ADD IsDeleted BIT NULL DEFAULT 0;
    
    PRINT 'IsDeleted column added to GuestSchedule table';
END
ELSE
BEGIN
    PRINT 'IsDeleted column already exists in GuestSchedule table';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('GuestSchedule') AND name = 'IsCancelled')
BEGIN
    ALTER TABLE GuestSchedule
    ADD IsCancelled BIT NULL DEFAULT 0;
    
    PRINT 'IsCancelled column added to GuestSchedule table';
END
ELSE
BEGIN
    PRINT 'IsCancelled column already exists in GuestSchedule table';
END
GO

-- Update existing records to set default values (optional - sets NULL values to 0)
UPDATE GuestSchedule
SET IsDeleted = 0
WHERE IsDeleted IS NULL;
GO

UPDATE GuestSchedule
SET IsCancelled = 0
WHERE IsCancelled IS NULL;
GO

PRINT 'Migration completed successfully';
GO


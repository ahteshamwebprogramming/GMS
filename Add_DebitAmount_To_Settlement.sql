-- Add DebitAmount and DebitNoteNumber columns to Settlement table
-- These columns are used for debit note settlements

-- Add DebitAmount column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Settlement') AND name = 'DebitAmount')
BEGIN
    ALTER TABLE Settlement
    ADD DebitAmount DECIMAL(18, 2) NULL;
    
    PRINT 'DebitAmount column added to Settlement table successfully.';
END
ELSE
BEGIN
    PRINT 'DebitAmount column already exists in Settlement table.';
END
GO

-- Add DebitNoteNumber column (if needed for tracking)
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Settlement') AND name = 'DebitNoteNumber')
BEGIN
    ALTER TABLE Settlement
    ADD DebitNoteNumber NVARCHAR(50) NULL;
    
    PRINT 'DebitNoteNumber column added to Settlement table successfully.';
END
ELSE
BEGIN
    PRINT 'DebitNoteNumber column already exists in Settlement table.';
END
GO


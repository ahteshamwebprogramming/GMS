-- Add approval and recovery fields to CreditDebitNoteAccount table
-- These fields are used for debit note approval workflow

-- Add IsApproved field
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CreditDebitNoteAccount') AND name = 'IsApproved')
BEGIN
    ALTER TABLE CreditDebitNoteAccount
    ADD IsApproved BIT NULL;
    
    PRINT 'IsApproved column added to CreditDebitNoteAccount table successfully.';
END
ELSE
BEGIN
    PRINT 'IsApproved column already exists in CreditDebitNoteAccount table.';
END
GO

-- Add ApprovedBy field
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CreditDebitNoteAccount') AND name = 'ApprovedBy')
BEGIN
    ALTER TABLE CreditDebitNoteAccount
    ADD ApprovedBy INT NULL;
    
    PRINT 'ApprovedBy column added to CreditDebitNoteAccount table successfully.';
END
ELSE
BEGIN
    PRINT 'ApprovedBy column already exists in CreditDebitNoteAccount table.';
END
GO

-- Add ApprovedOn field
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CreditDebitNoteAccount') AND name = 'ApprovedOn')
BEGIN
    ALTER TABLE CreditDebitNoteAccount
    ADD ApprovedOn DATETIME NULL;
    
    PRINT 'ApprovedOn column added to CreditDebitNoteAccount table successfully.';
END
ELSE
BEGIN
    PRINT 'ApprovedOn column already exists in CreditDebitNoteAccount table.';
END
GO

-- Add IsRecovered field
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CreditDebitNoteAccount') AND name = 'IsRecovered')
BEGIN
    ALTER TABLE CreditDebitNoteAccount
    ADD IsRecovered BIT NULL;
    
    PRINT 'IsRecovered column added to CreditDebitNoteAccount table successfully.';
END
ELSE
BEGIN
    PRINT 'IsRecovered column already exists in CreditDebitNoteAccount table.';
END
GO

-- Add RecoveredBy field
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CreditDebitNoteAccount') AND name = 'RecoveredBy')
BEGIN
    ALTER TABLE CreditDebitNoteAccount
    ADD RecoveredBy INT NULL;
    
    PRINT 'RecoveredBy column added to CreditDebitNoteAccount table successfully.';
END
ELSE
BEGIN
    PRINT 'RecoveredBy column already exists in CreditDebitNoteAccount table.';
END
GO

-- Add RecoveredOn field
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CreditDebitNoteAccount') AND name = 'RecoveredOn')
BEGIN
    ALTER TABLE CreditDebitNoteAccount
    ADD RecoveredOn DATETIME NULL;
    
    PRINT 'RecoveredOn column added to CreditDebitNoteAccount table successfully.';
END
ELSE
BEGIN
    PRINT 'RecoveredOn column already exists in CreditDebitNoteAccount table.';
END
GO


-- Add Debit Note Payment Method
-- This payment method should not appear in dropdowns but is used internally for debit note payments

IF NOT EXISTS (SELECT 1 FROM PaymentMethod WHERE PaymentMethodCode = 'DN')
BEGIN
    INSERT INTO PaymentMethod (PaymentMethodName, PaymentMethodCode, IsActive, CreatedDate)
    VALUES ('Debit Note', 'DN', 1, GETDATE())
END
GO


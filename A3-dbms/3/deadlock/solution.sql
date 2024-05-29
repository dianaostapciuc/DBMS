USE lab3
GO

-- solution: set deadlock priority to high for the second transaction
-- default priority is normal (0)

SET DEADLOCK_PRIORITY HIGH
BEGIN TRAN
UPDATE VET SET name = 'Transaction 2' WHERE vid = 15
WAITFOR DELAY '00:00:10'

UPDATE ANIMAL SET name = 'Transaction 2' WHERE aid = 15
COMMIT TRAN
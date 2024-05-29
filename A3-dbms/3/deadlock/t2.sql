USE lab3
GO

BEGIN TRAN
UPDATE VET SET name = 'Transaction 2' WHERE VID = 15
WAITFOR DELAY '00:00:10'
UPDATE ANIMAL SET name = 'Transaction 2' WHERE AID = 15
COMMIT TRAN
-- the tables will contain the values from T2
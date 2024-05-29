USE lab3
GO

--solution: set transaction isolation level to repeatable read
SET TRAN ISOLATION LEVEL REPEATABLE READ
BEGIN TRAN
SELECT * FROM Animal
WAITFOR DELAY '00:00:06'
-- now we see the value before the update
SELECT * FROM Animal
COMMIT TRAN


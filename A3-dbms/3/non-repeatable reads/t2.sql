USE lab3
GO

--part 2: the row is changed while T2 is in progress, so we will see both values for address
SET TRAN ISOLATION LEVEL READ COMMITTED
BEGIN TRAN
-- see first insert
SELECT * FROM Animal
WAITFOR DELAY '00:00:06'
SELECT * FROM Animal
COMMIT TRAN


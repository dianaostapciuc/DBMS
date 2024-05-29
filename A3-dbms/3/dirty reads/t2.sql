USE lab3
GO

-- part 2; we can read uncommited data (dirty read)
SET TRAN ISOLATION LEVEL READ UNCOMMITTED
BEGIN TRAN
-- see update
SELECT * FROM ANIMAL
WAITFOR DELAY '00:00:06'
-- update was rolled back
SELECT * FROM ANIMAL
COMMIT TRAN
USE lab3
GO

-- solution: set transaction isolation level to serializable
SET TRAN ISOLATION LEVEL SERIALIZABLE
BEGIN TRAN
SELECT * FROM ANIMAL
WAITFOR DELAY '00:00:06'
SELECT * FROM ANIMAL
COMMIT TRAN
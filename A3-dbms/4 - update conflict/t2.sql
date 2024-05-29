USE lab3
GO

-- update conflict
SET TRAN ISOLATION LEVEL SNAPSHOT
BEGIN TRAN

WAITFOR DELAY '00:00:05'

-- T1 has now updated and obtained a lock on this table
-- trying to update the same row will result in a error (process is blocked)
UPDATE ANIMAL SET name = 'New name 2' WHERE aid = 60
COMMIT TRAN
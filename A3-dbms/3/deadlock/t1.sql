USE lab3
GO

--INSERT INTO ANIMAL(aid, name, type_animal, age) VALUES(15, 'Yola', 'Bird', 6)
--INSERT INTO VET(vid, name, phone_number) VALUES (15, 'Dan', '0745282854')
--SELECT * FROM ANIMAL
--SELECT * FROM VET

--DELETE FROM ANIMAL WHERE aid = 15
--DELETE FROM VET WHERE vid = 15

BEGIN TRAN
UPDATE ANIMAL SET name = 'Transaction 1' WHERE AID = 15
WAITFOR DELAY '00:00:10'
UPDATE VET SET name = 'Transaction 1' WHERE VID = 15
COMMIT TRAN
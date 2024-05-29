USE lab3
GO

DROP TABLE IF EXISTS LogTable
CREATE TABLE LogTable(
	Lid int identity primary key,
	type_operation varchar(50),
	table_operation varchar(50),
	execution_date datetime
);

GO

-- use m:n relation animal - vet

-- string validation
CREATE OR ALTER FUNCTION ValidateString (@str VARCHAR(30))
RETURNS INT
AS
BEGIN
	DECLARE @return INT
	SET @return = 1
	IF(@str IS NULL OR LEN(@str) < 2 OR LEN(@str) > 30)
	BEGIN
		SET @return = 0
	END
	RETURN @return
END
GO

-- int validation
CREATE OR ALTER FUNCTION ValidateInt (@int integer)
RETURNS INT
AS
BEGIN
	DECLARE @return INT
	SET @return = 1
	IF(@int < 0)
	BEGIN
		SET @return = 0
	END
	RETURN @return
END
GO

CREATE OR ALTER PROCEDURE AddAnimal(@aid integer, @name VARCHAR(30), @type_animal VARCHAR(30), @age integer)
AS
	SET NOCOUNT ON
	IF (dbo.ValidateString(@name) <> 1)
	BEGIN
		RAISERROR('Name is invalid', 14, 1)
	END
	IF (dbo.ValidateString(@type_animal) <> 1)
	BEGIN
		RAISERROR('Type is invalid', 14, 1)
	END
	IF (dbo.ValidateInt(@age) <> 1)
	BEGIN
		RAISERROR('Age is invalid', 14, 1)
	END
	IF EXISTS (SELECT * FROM Animal a where a.aid = @aid)
	BEGIN
		RAISERROR('Animal already exists', 14, 1)
	END
	INSERT INTO Animal VALUES (@aid, @name, @type_animal, @age)
	INSERT INTO LogTable VALUES ('add', 'animal', GETDATE())
GO

CREATE OR ALTER PROCEDURE AddVet(@vid integer, @name VARCHAR(30), @phone_number integer)
AS
	SET NOCOUNT ON
	IF (dbo.ValidateString(@name) <> 1)
	BEGIN
		RAISERROR('Name is invalid', 14, 1)
	END
	IF (dbo.ValidateInt(@phone_number) <> 1)
	BEGIN
		RAISERROR('Number is invalid', 14, 1)
	END
	IF EXISTS (SELECT * FROM Vet v where v.vid = @vid)
	BEGIN
		RAISERROR('Vet already exists', 14, 1)
	END
	INSERT INTO Vet VALUES (@vid, @name, @phone_number)
	INSERT INTO LogTable VALUES ('add', 'vet', GETDATE())
GO

CREATE OR ALTER PROCEDURE AddConsultation(@aid integer, @vid integer, @week_day varchar(30))
AS
	SET NOCOUNT ON
	IF (dbo.ValidateString(@week_day) <> 1)
	BEGIN
		RAISERROR('Day is invalid', 14, 1)
	END
	IF EXISTS (SELECT * FROM Consultation c where c.aid = @aid AND c.vid = @vid)
	BEGIN
		RAISERROR('Consultation already exists', 14, 1)
	END
	INSERT INTO Consultation VALUES (@aid, @vid, @week_day)
	INSERT INTO LogTable VALUES ('add', 'consultation', GETDATE())
GO

CREATE OR ALTER PROCEDURE AddCommitScenario
AS
	BEGIN TRAN
	BEGIN TRY
		EXEC AddAnimal 1, 'Fluf', 'Cat', 2
		EXEC AddVet 1, 'Oliver', '0722000111'
		EXEC AddConsultation 1, 1, 'Monday'
		COMMIT TRAN
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN
		RETURN
	END CATCH
GO


CREATE OR ALTER PROCEDURE AddRollbackScenario
AS 
	BEGIN TRAN
	BEGIN TRY
		EXEC AddAnimal 1, 'Puff', 'Dog', 2
		EXEC AddVet 1, 'x', '0722000111' -- this will fail due to validation, so everything fails
		EXEC AddConsultation 1, 1, 'Friday'
		COMMIT TRAN
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN
		RETURN
	END CATCH
GO

EXEC AddRollbackScenario
EXEC AddCommitScenario

SELECT * FROM LogTable

SELECT * FROM Animal
SELECT * FROM Vet
SELECT * FROM Consultation

DELETE FROM LogTable
DELETE FROM Animal WHERE aid = 1
DELETE FROM Vet WHERE vid = 1
DELETE FROM Consultation WHERE aid = 1 AND vid = 1
USE lab3
GO

CREATE OR ALTER PROCEDURE AddAnimalRecover(@aid integer, @name VARCHAR(30), @type_animal VARCHAR(30), @age integer)
AS
	SET NOCOUNT ON
	BEGIN TRAN
	BEGIN TRY
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
		COMMIT TRAN
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN
	END CATCH
GO

CREATE OR ALTER PROCEDURE AddVetRecover(@vid integer, @name VARCHAR(30), @phone_number integer)
AS
	SET NOCOUNT ON
	BEGIN TRAN
	BEGIN TRY
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
		COMMIT TRAN
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN
	END CATCH
GO

CREATE OR ALTER PROCEDURE AddConsultationRecover(@aid integer, @vid integer, @week_day varchar(30))
AS
	SET NOCOUNT ON
	BEGIN TRAN
	BEGIN TRY
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
		COMMIT TRAN
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN
	END CATCH
GO

CREATE OR ALTER PROCEDURE BadAddScenario
AS
	EXEC AddAnimalRecover 2, 'Boop', 'Cat', 1
	EXEC AddVetRecover 2, 'g', '0741215512' -- this will fail, but the item added before will still be in the database
	EXEC AddConsultationRecover 2, 2, 'Tuesday'
GO

CREATE OR ALTER PROCEDURE GoodAddScenario
AS
	EXEC AddAnimalRecover 3, 'Juli', 'Dog', 5
	EXEC AddVetRecover 3, 'Anne', '0700222333' 
	EXEC AddConsultationRecover 3, 3, 'Friday'
GO

EXEC BadAddScenario
SELECT * FROM LogTable

EXEC GoodAddScenario
SELECT * FROM LogTable

SELECT * FROM Animal
SELECT * FROM Vet
SELECT * FROM Consultation

DELETE FROM LogTable
DELETE FROM Animal WHERE aid = 2
DELETE FROM Animal WHERE aid = 3
DELETE FROM Vet WHERE vid = 3
DELETE FROM Consultation WHERE aid = 3 AND vid = 3
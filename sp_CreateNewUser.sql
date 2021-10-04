CREATE PROCEDURE sp_CreateNewUser 
	@id INT OUTPUT,
	@firstName NVARCHAR(MAX),
	@lastName NVARCHAR(MAX),
	@Username NVARCHAR(20),
	@Password NVARCHAR(MAX),
	@Address NVARCHAR(MAX),
	@age INT
AS
BEGIN
	IF (@Username IS NULL OR @Username = '')
	raiserror('Username is null', 16, 1)

	INSERT INTO Users (FirstName, LastName, Username, Password, Address, Age)
	VALUES (@firstName, @lastName, @Username, @Password, @Address, @age)

	SET @id = SCOPE_IDENTITY()
END
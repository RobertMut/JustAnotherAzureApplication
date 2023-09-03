--Uses @prefix NVARCHAR(MAX) parameter
--Uses @dbSchema NVARCHAR(MAX) parameter

DECLARE @QUERY NVARCHAR(MAX)

BEGIN
	SET @QUERY = '
	DECLARE @existingGroups TABLE (id UNIQUEIDENTIFIER)
	DECLARE @existingUsers TABLE (id UNIQUEIDENTIFIER)
	DECLARE @existingFiles TABLE (Filename NVARCHAR(450))

	INSERT INTO @existingGroups
	SELECT Id FROM [' + @dbSchema + '].[Groups]
	WHERE Name LIKE '''+ @prefix +'''

	INSERT INTO @existingUsers
	SELECT Id FROM [' + @dbSchema + '].[Users]
	WHERE Username LIKE '''+ @prefix +'''

	INSERT INTO @existingFiles
	SELECT Filename FROM [' + @dbSchema + '].[Files]
	WHERE Filename LIKE '''+ @prefix +'''

	DELETE FROM [' + @dbSchema + '].[GroupShares]
	WHERE GroupId IN 
		(
			SELECT GS.GroupId
			FROM ' + @dbSchema + '.[GroupShares] GS
			INNER JOIN @existingGroups EG ON EG.Id = GS.GroupId
		)

	DELETE FROM [' + @dbSchema + '].[UserShares]
	WHERE UserId IN 
		(
			SELECT US.UserId
			FROM [' + @dbSchema + '].[UserShares] US
			INNER JOIN @existingUsers EU ON EU.Id = US.UserId
		)

	DELETE FROM [' + @dbSchema + '].[Files]
	WHERE Filename IN 
		(
			SELECT F.Filename
			FROM [' + @dbSchema + '].[Files] F
			INNER JOIN @existingFiles EF ON EF.Filename = F.Filename
		)

	DELETE FROM [' + @dbSchema + '].[GroupUsers]
	WHERE UserId IN 
		(
			SELECT GU.UserId
			FROM [' + @dbSchema + '].[GroupUsers] GU
			INNER JOIN @existingUsers EU ON EU.Id = GU.UserId
		)

    DELETE FROM [' + @dbSchema + '].[Users]
    WHERE Id IN
          (
              SELECT U.Id
              FROM [' + @dbSchema + '].[Users] U
                       INNER JOIN @existingUsers EU ON EU.Id = U.Id
          )

	DELETE FROM ' + @dbSchema + '.Groups
	WHERE Id IN 
		(
			SELECT G.Id
			FROM [' + @dbSchema + '].[Groups] G
			INNER JOIN @existingGroups EG ON EG.Id = G.Id
		)
	'
	EXEC sp_executesql @QUERY
END
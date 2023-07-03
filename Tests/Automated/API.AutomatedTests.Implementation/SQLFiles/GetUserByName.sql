--Uses @userName NVARCHAR(MAX) parameter
--Uses @dbSchema NVARCHAR(MAX) parameter

DECLARE @QUERY NVARCHAR(MAX)

BEGIN
	SET @QUERY = '
		SELECT Id
		FROM [' + @dbSchema + '].[Users]
		WHERE Username = ''' + @userName + '''
	'
	EXEC sp_executesql @QUERY
END
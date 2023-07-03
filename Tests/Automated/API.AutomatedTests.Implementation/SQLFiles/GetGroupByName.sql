--Uses @groupName NVARCHAR(MAX) parameter
--Uses @dbSchema NVARCHAR(MAX) parameter

DECLARE @QUERY NVARCHAR(MAX)

BEGIN
	SET @QUERY = '
		SELECT Id
		FROM [' + @dbSchema + '].[Groups]
		WHERE Name = ''' + @groupName + '''
	'
	EXEC sp_executesql @QUERY
END
--Uses @groupId NVARCHAR(MAX) parameter
--Uses @dbSchema NVARCHAR(MAX) parameter

DECLARE @QUERY NVARCHAR(MAX)

BEGIN
	SET @QUERY = '
		SELECT Id
		FROM [' + @dbSchema + '].[GroupShares]
		WHERE GroupId = ''' + @groupId + '''
	       AND Filename = '''+ @fileName +'''      
	'
	EXEC sp_executesql @QUERY
END
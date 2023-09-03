--Uses @groupName NVARCHAR(MAX) parameter
--Uses @dbSchema NVARCHAR(MAX) parameter

DECLARE @QUERY NVARCHAR(MAX)

BEGIN
	SET @QUERY = '
		SELECT (
			CASE
				WHEN COUNT(*) = 0
					THEN ''NotExists''
				ELSE
					''Exists''
			END
		)
		FROM [' + @dbSchema + '].[GroupShares]
		WHERE GroupId = ''' + @groupId + '''
	        AND Filename = '''+ @filename +'''
	'
	EXEC sp_executesql @QUERY
END
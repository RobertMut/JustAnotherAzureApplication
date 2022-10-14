--Uses @username NVARCHAR(MAX) parameter
--Uses @dbSchema NVARCHAR(MAX) parameter

DECLARE @QUERY NVARCHAR(MAX)

BEGIN
	SET @QUERY = '
		SELECT (
			CASE
				WHEN COUNT(*) > 0
					THEN ''Exists''
				ELSE
					''NotExists''
			END
		)
		FROM [' + @dbSchema + '].[Users]
		WHERE Username = ''' + @username + '''
	'
	EXEC sp_executesql @QUERY
END
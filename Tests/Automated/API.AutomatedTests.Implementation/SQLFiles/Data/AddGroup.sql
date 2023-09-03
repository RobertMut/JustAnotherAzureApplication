--Uses @prefix NVARCHAR(MAX) parameter
--Uses @dbSchema NVARCHAR(MAX) parameter

DECLARE @QUERY NVARCHAR(MAX)

BEGIN
	SET @QUERY = '
        INSERT INTO ['+@dbSchema+'].[Groups] (Id, Name, Description)
	        VALUES (NEWID(), N''t_testdata_CustomName'', ''t_testdata_Description'')
	'
	EXEC sp_executesql @QUERY
END
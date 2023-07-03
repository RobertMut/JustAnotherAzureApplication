--Uses @prefix NVARCHAR(MAX) parameter
--Uses @dbSchema NVARCHAR(MAX) parameter

DECLARE @QUERY NVARCHAR(MAX)

BEGIN
	SET @QUERY = '
        INSERT INTO ['+@dbSchema+'].[Groups] (Id, Name, Description)
	        VALUES (NEWID(), N''t_testdata_CustomGroup'', ''t_testdata_Description''),
	            (NEWID(), N''t_testdata_CustomGroup2'', ''t_testdata_Description2'')
        INSERT INTO ['+@dbSchema+'].[Users] (Id, Username, Password)
	        VALUES
	            (NEWID(), N''t_testdata_CustomUser1'', ''pass''),
	            (NEWID(), N''t_testdata_CustomUser2'', ''pass''),
                (NEWID(), N''t_testdata_CustomUser3'', ''pass'')

	    DECLARE @firstCustomUser TABLE (id UNIQUEIDENTIFIER) 
	    DECLARE @secondCustomUser TABLE (id UNIQUEIDENTIFIER) 

        INSERT INTO ['+@dbSchema+'].[GroupUsers] (UserId, GroupId)
        SELECT U.Id, G.Id FROM ['+@dbSchema+'].[Users] U, ['+@dbSchema+'].[Groups] G
        WHERE U.Username = ''t_testdata_CustomUser3''
        AND G.Name = ''t_testdata_CustomGroup''

        INSERT INTO ['+@dbSchema+'].[GroupUsers] (UserId, GroupId)
        SELECT U.Id, G.Id FROM ['+@dbSchema+'].[Users] U, ['+@dbSchema+'].[Groups] G
        WHERE U.Username = ''t_testdata_CustomUser2''
        AND G.Name = ''t_testdata_CustomGroup''

        INSERT INTO ['+@dbSchema+'].[GroupUsers] (UserId, GroupId)
        SELECT U.Id, G.Id FROM ['+@dbSchema+'].[Users] U, ['+@dbSchema+'].[Groups] G
        WHERE U.Username = ''t_testdata_CustomUser2''
        AND G.Name = ''t_testdata_CustomGroup2''

        INSERT INTO ['+@dbSchema+'].[Files] (Filename, OriginalName, UserId)
        SELECT ''t.testdata.groupShared1.jpg'', ''t.testdata.groupShared1.jpg'', U.Id FROM ['+@dbSchema+'].[Users] U
        WHERE U.Username = ''t_testdata_CustomUser1''

        INSERT INTO ['+@dbSchema+'].[Files] (Filename, OriginalName, UserId)
        SELECT ''t.testdata.userShared1.jpg'', ''t.testdata.userShared1.jpg'', U.Id FROM ['+@dbSchema+'].[Users] U
        WHERE U.Username = ''t_testdata_CustomUser3''

        INSERT INTO ['+@dbSchema+'].[Files] (Filename, OriginalName, UserId)
        SELECT ''t.testdata.default.jpg'', ''t.testdata.default.jpg'', U.Id FROM ['+@dbSchema+'].[Users] U
        WHERE U.Username = ''Default''  
	             
        INSERT INTO ['+@dbSchema+'].[GroupShares] (PermissionId, GroupId, Filename)
        SELECT P.Id, G.Id, ''t.testdata.groupShared1.jpg'' FROM  ['+@dbSchema+'].[Permissions] P, ['+@dbSchema+'].[Groups] G
        WHERE P.Name = ''Full'' AND G.Name = ''t_testdata_CustomGroup''
        
        INSERT INTO ['+@dbSchema+'].[UserShares] (PermissionId, UserId, Filename)
        SELECT P.Id, U.Id, ''t.testdata.userShared1.jpg'' FROM  ['+@dbSchema+'].[Permissions] P, ['+@dbSchema+'].[Users] U
        WHERE P.Name = ''Full'' AND U.Username = ''t_testdata_CustomUser2''
	'
	EXEC sp_executesql @QUERY
END
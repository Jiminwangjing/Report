
GO
/****** Object:  StoredProcedure [dbo].[uspSelectTable]    Script Date: 2/22/2023 6:48:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[uspSelectTable](@tableName VARCHAR(100)) 
--WITH ENCRYPTION
AS 
BEGIN 
	IF OBJECT_ID (@tableName, N'U') IS NULL 
	BEGIN
		RAISERROR ('Table name not found.', 11, 1) WITH NOWAIT
		RETURN;
	END

	DECLARE @tablenametable TABLE(tablename VARCHAR(100));
	INSERT INTO @tablenametable
	VALUES(@tableName);

	DECLARE dbcursor CURSOR	
	FOR
		SELECT tablename
		FROM @tablenametable
	OPEN dbcursor;
	FETCH NEXT FROM dbcursor INTO @tablename;
	WHILE @@FETCH_STATUS = 0
		BEGIN
			DECLARE @sql VARCHAR(MAX);
			SET @sql = 'SELECT * FROM '+ @tablename;
			EXEC(@sql);
			FETCH NEXT FROM dbcursor INTO @tablename;
		END;
	CLOSE dbcursor;
	DEALLOCATE dbcursor;
END;


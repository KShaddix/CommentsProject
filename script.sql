USE [master]
GO
/****** Object:  Database [commentsdb]    Script Date: 14.08.2020 18:31:59 ******/
CREATE DATABASE [commentsdb]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'commentsdb', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER2\MSSQL\DATA\commentsdb.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'commentsdb_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER2\MSSQL\DATA\commentsdb_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [commentsdb] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [commentsdb].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [commentsdb] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [commentsdb] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [commentsdb] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [commentsdb] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [commentsdb] SET ARITHABORT OFF 
GO
ALTER DATABASE [commentsdb] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [commentsdb] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [commentsdb] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [commentsdb] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [commentsdb] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [commentsdb] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [commentsdb] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [commentsdb] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [commentsdb] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [commentsdb] SET  ENABLE_BROKER 
GO
ALTER DATABASE [commentsdb] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [commentsdb] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [commentsdb] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [commentsdb] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [commentsdb] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [commentsdb] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [commentsdb] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [commentsdb] SET RECOVERY FULL 
GO
ALTER DATABASE [commentsdb] SET  MULTI_USER 
GO
ALTER DATABASE [commentsdb] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [commentsdb] SET DB_CHAINING OFF 
GO
ALTER DATABASE [commentsdb] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [commentsdb] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [commentsdb] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'commentsdb', N'ON'
GO
ALTER DATABASE [commentsdb] SET QUERY_STORE = OFF
GO
USE [commentsdb]
GO
/****** Object:  Table [dbo].[Articles]    Script Date: 14.08.2020 18:31:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Articles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Text] [nvarchar](2000) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Comments]    Script Date: 14.08.2020 18:31:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Comments](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Text] [nvarchar](200) NOT NULL,
	[ArticleId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[ParentId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 14.08.2020 18:31:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](32) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Comments]  WITH CHECK ADD FOREIGN KEY([ArticleId])
REFERENCES [dbo].[Articles] ([Id])
GO
ALTER TABLE [dbo].[Comments]  WITH CHECK ADD FOREIGN KEY([ParentId])
REFERENCES [dbo].[Comments] ([Id])
GO
ALTER TABLE [dbo].[Comments]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
/****** Object:  StoredProcedure [dbo].[CreateComment]    Script Date: 14.08.2020 18:31:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CreateComment]
	@text NVARCHAR(2000),
	@articleId INT,
	@userId INT,
	@parentId INT
AS
BEGIN
	INSERT INTO Comments (Text, ArticleId, UserId, ParentId) VALUES (@text, @articleId, @userId, @parentId);
	SELECT SCOPE_IDENTITY();
END;
GO
/****** Object:  StoredProcedure [dbo].[GetArticleInfoById]    Script Date: 14.08.2020 18:31:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetArticleInfoById]
	@id INT
AS
SELECT *, (SELECT COUNT(*) FROM Comments WHERE ArticleId = @id) AS CommentsCount FROM Articles WHERE Id = @id
GO
/****** Object:  StoredProcedure [dbo].[GetArticlesByPage]    Script Date: 14.08.2020 18:31:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetArticlesByPage]
	@page INT,
	@count INT
AS
SELECT *, (SELECT COUNT(*) FROM Comments WHERE ArticleId = a.Id) AS CommentsCount 
FROM Articles AS a
ORDER BY Id
	OFFSET (@page * @count) ROWS
	FETCH FIRST @count ROWS ONLY
GO
/****** Object:  StoredProcedure [dbo].[GetArticlesCount]    Script Date: 14.08.2020 18:31:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[GetArticlesCount]
AS SELECT COUNT(*) FROM Articles
GO
/****** Object:  StoredProcedure [dbo].[GetCommentById]    Script Date: 14.08.2020 18:31:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[GetCommentById]
	@id INT
AS
SELECT * FROM Comments WHERE Id = @id
GO
/****** Object:  StoredProcedure [dbo].[GetCommentsById]    Script Date: 14.08.2020 18:31:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetCommentsById]
	@id INT
AS
BEGIN
	WITH RecursComments AS
	(
		SELECT Id, ParentId
		FROM Comments 
		WHERE Id = @id
		UNION ALL
		SELECT c.Id, c.ParentId
		FROM Comments AS c
			JOIN RecursComments AS r ON r.Id = c.ParentId
	)

	SELECT * FROM RecursComments
END
GO
/****** Object:  StoredProcedure [dbo].[GetCommentsByPage]    Script Date: 14.08.2020 18:31:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetCommentsByPage]
	@articleId INT,
	@page INT,
	@count INT
AS
BEGIN
	WITH RecursComments AS
	(
		SELECT a.Id, Text, ArticleId, u.Name AS UserName, ParentId
		FROM Comments AS a
			JOIN Users AS u ON u.Id = UserId
		WHERE ArticleId = @articleId AND ParentId IS NULL
		ORDER BY a.Id
		OFFSET (@page * @count) ROWS
		FETCH FIRST @count ROWS ONLY
		UNION ALL
		SELECT c.Id, c.Text, c.ArticleId, u.Name AS UserName, c.ParentId
		FROM Comments AS c
			JOIN Users AS u ON u.Id = c.UserId
			JOIN RecursComments AS r ON r.Id = c.ParentId
	)

	SELECT * FROM RecursComments
END
GO
/****** Object:  StoredProcedure [dbo].[GetCommentsCountForArticle]    Script Date: 14.08.2020 18:31:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetCommentsCountForArticle] 
	-- Add the parameters for the stored procedure here
	@articleId INT
AS
BEGIN

    -- Insert statements for procedure here
	SELECT COUNT(*) FROM Comments WHERE ArticleId = @articleId AND ParentId IS NULL
END
GO
/****** Object:  StoredProcedure [dbo].[GetUser]    Script Date: 14.08.2020 18:31:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetUser]
	@id INT
AS
SELECT * FROM Users WHERE Id = @id
GO
USE [master]
GO
ALTER DATABASE [commentsdb] SET  READ_WRITE 
GO

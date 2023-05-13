USE [FoodLovers]
GO
/****** Object:  UserDefinedTableType [dbo].[ProductType]    Script Date: 2023/05/13 20:49:17 ******/
CREATE TYPE [dbo].[ProductType] AS TABLE(
	[PID] [int] NULL,
	[ID] [int] NULL,
	[ProductName] [varchar](500) NULL,
	[WeightedItem] [bit] NULL,
	[SuggestedSellingPrice] [decimal](10, 2) NULL,
	[UnitsInStock] [int] NULL,
	[DateAdded] [datetime] NULL,
	[DateUpdated] [datetime] NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[StoreProductType]    Script Date: 2023/05/13 20:49:17 ******/
CREATE TYPE [dbo].[StoreProductType] AS TABLE(
	[SID] [int] NULL,
	[PID] [int] NULL,
	[Active] [bit] NULL,
	[DateAdded] [datetime] NULL,
	[DateUpdated] [datetime] NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[StoreType]    Script Date: 2023/05/13 20:49:17 ******/
CREATE TYPE [dbo].[StoreType] AS TABLE(
	[SID] [int] NULL,
	[ID] [int] NULL,
	[StoreName] [varchar](500) NULL,
	[TelephoneNumber] [varchar](500) NULL,
	[OpenDate] [date] NULL,
	[DateAdded] [datetime] NULL,
	[DateUpdated] [datetime] NULL
)
GO
/****** Object:  Table [dbo].[ApiUser]    Script Date: 2023/05/13 20:49:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApiUser](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Username] [varchar](50) NULL,
	[Password] [varchar](50) NULL,
 CONSTRAINT [PK_ApiUser] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

SET IDENTITY_INSERT [FoodLovers].[dbo].[ApiUser] ON 
GO
INSERT [FoodLovers].[dbo].[ApiUser] ([ID], [Username], [Password]) VALUES (1, N'GoldinAPI', N'GoldinAPI')
GO
SET IDENTITY_INSERT [FoodLovers].[dbo].[ApiUser] OFF
Go
/****** Object:  Table [dbo].[Product]    Script Date: 2023/05/13 20:49:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Product](
	[PID] [int] IDENTITY(1,1) NOT NULL,
	[ID] [int] NULL,
	[ProductName] [varchar](500) NULL,
	[WeightedItem] [bit] NULL,
	[SuggestedSellingPrice] [decimal](10, 2) NULL,
	[UnitsInStock] [int] NULL,
	[DateAdded] [datetime] NULL,
	[DateUpdated] [datetime] NULL,
 CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED 
(
	[PID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Store]    Script Date: 2023/05/13 20:49:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Store](
	[SID] [int] IDENTITY(1,1) NOT NULL,
	[ID] [int] NULL,
	[StoreName] [varchar](500) NULL,
	[TelephoneNumber] [varchar](500) NULL,
	[OpenDate] [date] NULL,
	[DateAdded] [datetime] NULL,
	[DateUpdated] [datetime] NULL,
 CONSTRAINT [PK_Store] PRIMARY KEY CLUSTERED 
(
	[SID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StoreProduct]    Script Date: 2023/05/13 20:49:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StoreProduct](
	[SPID] [int] IDENTITY(1,1) NOT NULL,
	[SID] [int] NULL,
	[PID] [int] NULL,
	[Active] [bit] NULL,
	[DateAdded] [datetime] NULL,
	[DateUpdated] [datetime] NULL,
 CONSTRAINT [PK_StoreProduct] PRIMARY KEY CLUSTERED 
(
	[SPID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[InsertProducts]    Script Date: 2023/05/13 20:49:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[InsertProducts]
    @NewProducts ProductType READONLY
AS
BEGIN
    BEGIN TRY
        INSERT INTO [FoodLovers].[dbo].[Product] (ID,ProductName, WeightedItem, SuggestedSellingPrice, UnitsInStock, DateAdded, DateUpdated)
        SELECT ID,ProductName, WeightedItem, SuggestedSellingPrice, UnitsInStock, DateAdded, DateUpdated
        FROM @NewProducts
		Where ID not in (Select ID from [FoodLovers].[dbo].[Product]);
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH;
END
GO
/****** Object:  StoredProcedure [dbo].[InsertStores]    Script Date: 2023/05/13 20:49:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[InsertStores]
    @NewStores StoreType READONLY
AS
BEGIN
    BEGIN TRY
INSERT INTO [dbo].[Store]
           ([ID]
           ,[StoreName]
           ,[TelephoneNumber]
           ,[OpenDate]
           ,[DateAdded]
           ,[DateUpdated])
     
        SELECT [ID]
           ,[StoreName]
           ,[TelephoneNumber]
           ,[OpenDate]
           ,[DateAdded]
           ,[DateUpdated]
        FROM @NewStores
		Where ID not in (Select ID from [FoodLovers].[dbo].[Store]);
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH;
END
GO
/****** Object:  StoredProcedure [dbo].[InsertStoresProducts]    Script Date: 2023/05/13 20:49:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[InsertStoresProducts]
    @NewStoresProducts StoreProductType READONLY
AS
BEGIN
    BEGIN TRY
INSERT INTO [dbo].[StoreProduct]
           ([SID]
           ,[PID]
		   ,Active
		   ,DateAdded
		   ,DateUpdated
		   )
     
        SELECT [SID]
           ,[PID]
		   ,Active
		   ,DateAdded
		   ,DateUpdated
        FROM @NewStoresProducts
		Where not Exists (Select [SID],[PID] from [FoodLovers].[dbo].[StoreProduct]);
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH;
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateProducts]    Script Date: 2023/05/13 20:49:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE    PROCEDURE [dbo].[UpdateProducts]
    @ProductsToUpdate ProductType READONLY
AS
BEGIN
 BEGIN TRY
    UPDATE p
    SET p.ProductName = tvp.ProductName,
        p.WeightedItem = tvp.WeightedItem,
        p.SuggestedSellingPrice = tvp.SuggestedSellingPrice,
        p.UnitsInStock = tvp.UnitsInStock,
        p.DateUpdated = tvp.DateUpdated
    FROM [FoodLovers].[dbo].[Product] p
    JOIN @ProductsToUpdate tvp ON p.ID = tvp.ID and p.PID = p.PID; 
	  END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH;
END
GO

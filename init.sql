CREATE DATABASE [DotnetPracticalInterviewAssessment];
GO

USE [DotnetPracticalInterviewAssessment];
GO

-- 1. ORIGINAL SCHEMA (from assessment)
CREATE TABLE [dbo].[CustomerData](
	[CustomerId] [bigint] NOT NULL,
	[CustomerName] [varchar](100) NOT NULL,
	[CustomerType] [varchar](50) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
    CONSTRAINT [PK_CustomerData] PRIMARY KEY CLUSTERED ([CustomerId] ASC)
);

CREATE TABLE [dbo].[AccountData](
	[AccountId] [int] NOT NULL,
	[AccountNumber] [varchar](10) NOT NULL,
	[CustomerId] [bigint] NOT NULL,
	[AccountBalance] [decimal](18, 2) NOT NULL,
	[AccountOpenDate] [datetime] NOT NULL,
    CONSTRAINT [PK_AccountData] PRIMARY KEY CLUSTERED ([AccountId] ASC),
    CONSTRAINT [FK_AccountData_CustomerData] FOREIGN KEY([CustomerId]) REFERENCES [dbo].[CustomerData] ([CustomerId])
);

CREATE TABLE [dbo].[TransactionData](
	[TransactionId] [int] IDENTITY(1,1) NOT NULL,
	[AccountNumber] [varchar](10) NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[DiscountedAmount] [decimal](18, 2) NOT NULL,
	[Rate] [decimal](18, 2) NOT NULL,
	[TransactionDate] [datetime] NOT NULL,
    CONSTRAINT [PK_TransactionData] PRIMARY KEY CLUSTERED ([TransactionId] ASC)
);

-- 2. NEW SCHEMA (Banking Safeguards & Rewards)
CREATE TABLE [dbo].[CustomerRewardState] (
    [CustomerId] [bigint] NOT NULL,
    [RewardMonth] [varchar](7) NOT NULL,
    [TotalPoints] [int] NOT NULL DEFAULT 0,
    [QualifyingTransferCount] [int] NOT NULL DEFAULT 0,
    [HasReceivedCashbackThisMonth] [bit] NOT NULL DEFAULT 0,
    [PendingAirtimeRewardCount] [int] NOT NULL DEFAULT 0,
    [LastUpdated] [datetime] NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_CustomerRewardState] PRIMARY KEY CLUSTERED ([CustomerId] ASC, [RewardMonth] ASC),
    CONSTRAINT [FK_CustomerRewardState_Customer] FOREIGN KEY([CustomerId]) REFERENCES [dbo].[CustomerData] ([CustomerId])
);

CREATE TABLE [dbo].[AuditLogs] (
    [LogId] [bigint] IDENTITY(1,1) NOT NULL,
    [Action] [varchar](255) NOT NULL,
    [AccountNumber] [varchar](10) NULL,
    [Details] [nvarchar](max) NOT NULL,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_AuditLogs] PRIMARY KEY CLUSTERED ([LogId] ASC)
);

CREATE TABLE [dbo].[IdempotencyKeys] (
    [IdempotencyKey] [uniqueidentifier] NOT NULL,
    [RequestName] [varchar](100) NOT NULL,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_IdempotencyKeys] PRIMARY KEY CLUSTERED ([IdempotencyKey] ASC)
);
GO

-- 3. SEED DATA (from provided Data.sql)
INSERT [dbo].[CustomerData] ([CustomerId], [CustomerName], [CustomerType], [DateCreated]) VALUES (1344459, N'James Babalola', N'BUSINESS', CAST(N'1993-01-01T00:00:00.000' AS DateTime));
INSERT [dbo].[CustomerData] ([CustomerId], [CustomerName], [CustomerType], [DateCreated]) VALUES (2311159, N'Alfred Wisdom', N'BUSINESS', CAST(N'1995-01-01T00:00:00.000' AS DateTime));
INSERT [dbo].[CustomerData] ([CustomerId], [CustomerName], [CustomerType], [DateCreated]) VALUES (2344559, N'John Ole', N'RETAIL', CAST(N'1992-01-01T00:00:00.000' AS DateTime));
INSERT [dbo].[CustomerData] ([CustomerId], [CustomerName], [CustomerType], [DateCreated]) VALUES (3344659, N'Peter Mod', N'RETAIL', CAST(N'1994-01-01T00:00:00.000' AS DateTime));

INSERT [dbo].[AccountData] ([AccountId], [AccountNumber], [CustomerId], [AccountBalance], [AccountOpenDate]) VALUES (1, N'2344559772', 2344559, 9000000.00, CAST(N'2013-01-01T00:00:00.000' AS DateTime));
INSERT [dbo].[AccountData] ([AccountId], [AccountNumber], [CustomerId], [AccountBalance], [AccountOpenDate]) VALUES (2, N'1344459859', 1344459, 1400000.00, CAST(N'2021-12-28T00:00:00.000' AS DateTime));
GO
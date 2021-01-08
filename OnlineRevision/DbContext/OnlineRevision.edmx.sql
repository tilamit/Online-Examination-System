
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 12/17/2020 13:23:14
-- Generated from EDMX file: G:\RnD\C.V\Online-Examination-System\OnlineRevision\DbContext\OnlineRevision.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [OnlineRevision];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Answers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Answers];
GO
IF OBJECT_ID(N'[dbo].[AttemptedExam]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AttemptedExam];
GO
IF OBJECT_ID(N'[dbo].[Explanation]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Explanation];
GO
IF OBJECT_ID(N'[dbo].[PaymentDetails]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PaymentDetails];
GO
IF OBJECT_ID(N'[dbo].[Questions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Questions];
GO
IF OBJECT_ID(N'[dbo].[QuestionSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[QuestionSet];
GO
IF OBJECT_ID(N'[dbo].[Results]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Results];
GO
IF OBJECT_ID(N'[dbo].[SampleTable]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SampleTable];
GO
IF OBJECT_ID(N'[dbo].[StudentResults]', 'U') IS NOT NULL
    DROP TABLE [dbo].[StudentResults];
GO
IF OBJECT_ID(N'[dbo].[StudySection]', 'U') IS NOT NULL
    DROP TABLE [dbo].[StudySection];
GO
IF OBJECT_ID(N'[dbo].[sysdiagrams]', 'U') IS NOT NULL
    DROP TABLE [dbo].[sysdiagrams];
GO
IF OBJECT_ID(N'[dbo].[TrackUsers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TrackUsers];
GO
IF OBJECT_ID(N'[dbo].[UserDetails]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserDetails];
GO
IF OBJECT_ID(N'[dbo].[UserStatus]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserStatus];
GO
IF OBJECT_ID(N'[dbo].[UserType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserType];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Answers'
CREATE TABLE [dbo].[Answers] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [QuestionSetId] int  NULL,
    [QuestionId] int  NULL,
    [QuestionAnswers] nvarchar(max)  NULL,
    [Status] int  NULL,
    [CreatedOn] datetime  NULL,
    [CreatedBy] nvarchar(10)  NULL
);
GO

-- Creating table 'AttemptedExam'
CREATE TABLE [dbo].[AttemptedExam] (
    [AttemptedId] int IDENTITY(1,1) NOT NULL,
    [QuesType] int  NULL,
    [QuestionSetId] int  NULL,
    [QuestionId] int  NULL,
    [UserId] int  NULL,
    [QuestionAnswers] nvarchar(max)  NULL,
    [Status] int  NULL,
    [Details] varchar(max)  NULL,
    [AddedOn] datetime  NULL
);
GO

-- Creating table 'Explanation'
CREATE TABLE [dbo].[Explanation] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [QuestionSetId] int  NOT NULL,
    [QuestionId] int  NULL,
    [Details] nvarchar(max)  NULL,
    [Status] int  NULL,
    [CreatedOn] datetime  NULL
);
GO

-- Creating table 'PaymentDetails'
CREATE TABLE [dbo].[PaymentDetails] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [PaymentId] nvarchar(max)  NULL,
    [ValidTill] datetime  NULL,
    [email] nvarchar(100)  NULL,
    [AddedOn] datetime  NULL,
    [Details] nvarchar(max)  NULL,
    [Status] int  NULL
);
GO

-- Creating table 'Questions'
CREATE TABLE [dbo].[Questions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [QuestionType] int  NULL,
    [QuestionSetId] int  NULL,
    [QuestionId] int  NULL,
    [QuestionName] nvarchar(max)  NULL,
    [Options] nvarchar(max)  NULL,
    [Status] int  NULL,
    [CreatedOn] datetime  NULL
);
GO

-- Creating table 'QuestionSet'
CREATE TABLE [dbo].[QuestionSet] (
    [QuestionSetId] int IDENTITY(1,1) NOT NULL,
    [QuestionSetName] nvarchar(200)  NULL,
    [Timer] int  NULL,
    [Details] nvarchar(max)  NULL,
    [CreatedOn] datetime  NULL,
    [Status] int  NULL,
    [CreatedBy] int  NULL
);
GO

-- Creating table 'Results'
CREATE TABLE [dbo].[Results] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [QuestionSetId] int  NULL,
    [StudentId] int  NULL,
    [Percentage] float  NULL,
    [Correct] int  NULL,
    [Incorrect] int  NULL,
    [Status] int  NULL,
    [Incomplete] int  NULL,
    [CreatedOn] datetime  NULL
);
GO

-- Creating table 'SampleTable'
CREATE TABLE [dbo].[SampleTable] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [QuestionName] nvarchar(10)  NULL,
    [Options] nvarchar(10)  NULL,
    [QuestionAnswers] nvarchar(10)  NULL
);
GO

-- Creating table 'StudentResults'
CREATE TABLE [dbo].[StudentResults] (
    [ResultId] int IDENTITY(1,1) NOT NULL,
    [Id] int  NULL,
    [StudentId] int  NULL,
    [QuestionSetId] int  NULL,
    [QuestionId] int  NULL,
    [QuestionAnswers] nvarchar(max)  NULL,
    [ExamTaken] datetime  NULL,
    [Status] int  NULL
);
GO

-- Creating table 'StudySection'
CREATE TABLE [dbo].[StudySection] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [sectionId] nvarchar(20)  NULL,
    [Heading] nvarchar(100)  NULL,
    [StudyContent] nvarchar(max)  NULL,
    [AddedBy] int  NULL,
    [Status] int  NULL,
    [CreatedOn] datetime  NULL
);
GO

-- Creating table 'sysdiagrams'
CREATE TABLE [dbo].[sysdiagrams] (
    [name] nvarchar(128)  NOT NULL,
    [principal_id] int  NOT NULL,
    [diagram_id] int IDENTITY(1,1) NOT NULL,
    [version] int  NULL,
    [definition] varbinary(max)  NULL
);
GO

-- Creating table 'TrackUsers'
CREATE TABLE [dbo].[TrackUsers] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Email] nvarchar(40)  NULL,
    [Activity] nvarchar(10)  NULL,
    [LoginTime] datetime  NULL
);
GO

-- Creating table 'UserDetails'
CREATE TABLE [dbo].[UserDetails] (
    [UserId] int IDENTITY(1,1) NOT NULL,
    [PaymentId] nvarchar(max)  NULL,
    [ValidTill] datetime  NULL,
    [UserName] nvarchar(100)  NULL,
    [Password] nvarchar(100)  NULL,
    [Institution] nvarchar(100)  NULL,
    [Email] nvarchar(100)  NULL,
    [UserType] int  NULL,
    [Sex] int  NULL,
    [ImgPath] nvarchar(60)  NULL,
    [DateOfBirth] datetime  NULL,
    [YearContinuing] float  NULL,
    [Status] int  NULL,
    [CreatedOn] datetime  NULL
);
GO

-- Creating table 'UserStatus'
CREATE TABLE [dbo].[UserStatus] (
    [StatusId] int IDENTITY(1,1) NOT NULL,
    [StatusName] nvarchar(10)  NULL
);
GO

-- Creating table 'UserType'
CREATE TABLE [dbo].[UserType] (
    [TypeId] int IDENTITY(1,1) NOT NULL,
    [TypeName] nvarchar(20)  NULL,
    [Details] nvarchar(max)  NULL,
    [Status] int  NULL,
    [CreatedBy] int  NULL,
    [CreatedOn] datetime  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Answers'
ALTER TABLE [dbo].[Answers]
ADD CONSTRAINT [PK_Answers]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [AttemptedId] in table 'AttemptedExam'
ALTER TABLE [dbo].[AttemptedExam]
ADD CONSTRAINT [PK_AttemptedExam]
    PRIMARY KEY CLUSTERED ([AttemptedId] ASC);
GO

-- Creating primary key on [Id] in table 'Explanation'
ALTER TABLE [dbo].[Explanation]
ADD CONSTRAINT [PK_Explanation]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'PaymentDetails'
ALTER TABLE [dbo].[PaymentDetails]
ADD CONSTRAINT [PK_PaymentDetails]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Questions'
ALTER TABLE [dbo].[Questions]
ADD CONSTRAINT [PK_Questions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [QuestionSetId] in table 'QuestionSet'
ALTER TABLE [dbo].[QuestionSet]
ADD CONSTRAINT [PK_QuestionSet]
    PRIMARY KEY CLUSTERED ([QuestionSetId] ASC);
GO

-- Creating primary key on [Id] in table 'Results'
ALTER TABLE [dbo].[Results]
ADD CONSTRAINT [PK_Results]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SampleTable'
ALTER TABLE [dbo].[SampleTable]
ADD CONSTRAINT [PK_SampleTable]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [ResultId] in table 'StudentResults'
ALTER TABLE [dbo].[StudentResults]
ADD CONSTRAINT [PK_StudentResults]
    PRIMARY KEY CLUSTERED ([ResultId] ASC);
GO

-- Creating primary key on [Id] in table 'StudySection'
ALTER TABLE [dbo].[StudySection]
ADD CONSTRAINT [PK_StudySection]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [diagram_id] in table 'sysdiagrams'
ALTER TABLE [dbo].[sysdiagrams]
ADD CONSTRAINT [PK_sysdiagrams]
    PRIMARY KEY CLUSTERED ([diagram_id] ASC);
GO

-- Creating primary key on [Id] in table 'TrackUsers'
ALTER TABLE [dbo].[TrackUsers]
ADD CONSTRAINT [PK_TrackUsers]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [UserId] in table 'UserDetails'
ALTER TABLE [dbo].[UserDetails]
ADD CONSTRAINT [PK_UserDetails]
    PRIMARY KEY CLUSTERED ([UserId] ASC);
GO

-- Creating primary key on [StatusId] in table 'UserStatus'
ALTER TABLE [dbo].[UserStatus]
ADD CONSTRAINT [PK_UserStatus]
    PRIMARY KEY CLUSTERED ([StatusId] ASC);
GO

-- Creating primary key on [TypeId] in table 'UserType'
ALTER TABLE [dbo].[UserType]
ADD CONSTRAINT [PK_UserType]
    PRIMARY KEY CLUSTERED ([TypeId] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------
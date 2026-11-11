USE master;
GO
IF DB_ID('TradingCompany') IS NOT NULL
BEGIN
    ALTER DATABASE TradingCompany SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE TradingCompany;
END
GO
CREATE DATABASE TradingCompany;
GO
USE TradingCompany;
GO

CREATE TABLE Categories (
    Id   INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE Conditions (
    Id   INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE Currencies (
    Id   INT IDENTITY(1,1) PRIMARY KEY,
    Code NVARCHAR(10) NOT NULL UNIQUE,
    Name NVARCHAR(50) NOT NULL
);

CREATE TABLE Items (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    Name        NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,
    CategoryId  INT NOT NULL,
    ConditionId INT NOT NULL,
    IsActive    BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Items_Categories FOREIGN KEY (CategoryId)  REFERENCES Categories(Id),
    CONSTRAINT FK_Items_Conditions FOREIGN KEY (ConditionId) REFERENCES Conditions(Id)
);

CREATE TABLE Auctions (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    ItemId      INT NOT NULL,
    CurrencyId  INT NOT NULL,
    StartDate   DATETIME2 NOT NULL,
    EndDate     DATETIME2 NOT NULL,
    StartPrice  DECIMAL(18,2) NOT NULL,
    BuyoutPrice DECIMAL(18,2) NULL,
    IsActive    BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Auctions_Items      FOREIGN KEY (ItemId)     REFERENCES Items(Id),
    CONSTRAINT FK_Auctions_Currencies FOREIGN KEY (CurrencyId) REFERENCES Currencies(Id),
    CONSTRAINT CK_Auctions_Dates CHECK (EndDate > StartDate)
);

CREATE TABLE Bids (
    Id        INT IDENTITY(1,1) PRIMARY KEY,
    AuctionId INT NOT NULL,
    Amount    DECIMAL(18,2) NOT NULL,
    BidDate   DATETIME2 NOT NULL,
    CONSTRAINT FK_Bids_Auctions FOREIGN KEY (AuctionId) REFERENCES Auctions(Id)
);
GO

INSERT INTO Categories (Name)
SELECT CONCAT(N'Категорія ', n)
FROM (SELECT TOP 20 ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) n FROM sys.all_objects) t;

INSERT INTO Conditions (Name)
SELECT CASE n WHEN 1 THEN N'Новий' WHEN 2 THEN N'Б/в' WHEN 3 THEN N'Відновлений'
              WHEN 4 THEN N'Колекційний' ELSE CONCAT(N'Стан ', n) END
FROM (SELECT TOP 20 ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) n FROM sys.all_objects) t;

INSERT INTO Currencies (Code, Name)
SELECT CASE n WHEN 1 THEN 'UAH' WHEN 2 THEN 'USD' WHEN 3 THEN 'EUR'
              ELSE CONCAT('CU', n) END,
       CASE n WHEN 1 THEN N'Гривня' WHEN 2 THEN N'Долар США' WHEN 3 THEN N'Євро'
              ELSE CONCAT(N'Валюта ', n) END
FROM (SELECT TOP 20 ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) n FROM sys.all_objects) t;

INSERT INTO Items (Name, Description, CategoryId, ConditionId, IsActive)
SELECT CONCAT(N'Товар ', n), CONCAT(N'Опис товару ', n), n, n, 1
FROM (SELECT TOP 20 ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) n FROM sys.all_objects) t;

INSERT INTO Auctions (ItemId, CurrencyId, StartDate, EndDate, StartPrice, BuyoutPrice, IsActive)
SELECT n, ((n-1) % 3) + 1,
       DATEADD(DAY, 10 - n - 15, SYSDATETIME()),
       DATEADD(DAY, 10 - n, SYSDATETIME()),
       100 * n, 500 * n,
       CASE WHEN n <= 10 THEN 1 ELSE 0 END  
FROM (SELECT TOP 20 ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) n FROM sys.all_objects) t;

INSERT INTO Bids (AuctionId, Amount, BidDate)
SELECT n, 100 * n + 50, DATEADD(HOUR, -n, SYSDATETIME())
FROM (SELECT TOP 20 ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) n FROM sys.all_objects) t;
GO
-- ===== Автентифікація та ролі (IW2. WPF) =====
CREATE TABLE Roles (
    Id   INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE Users (
    Id           INT IDENTITY(1,1) PRIMARY KEY,
    Login        NVARCHAR(50)  NOT NULL UNIQUE,
    PasswordHash NVARCHAR(200) NOT NULL,
    RoleId       INT NULL,
    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);
GO

INSERT INTO Roles (Name) VALUES (N'AuctionManager'), (N'Viewer');

-- Паролі: manager / Manager123!   viewer / Viewer123!   guest / Guest123!
-- (зберігаються як PBKDF2-SHA256: ітерації.сіль.хеш)
INSERT INTO Users (Login, PasswordHash, RoleId) VALUES
 (N'manager', N'100000.BC3a9LiyrBEP0pBFdDvHFg==.dU8xFVDJpDPTWx4s9xikAj9wA2IhxpR9/QOO0ToBIis=', 1),
 (N'viewer',  N'100000.RXT933Zi9KSNtycuMTqRtg==.V0Q3cupoRJJ574JMJXaaLpRigPq0R/r9QmJwzxFIkow=', 2),
 (N'guest',   N'100000.MDRb0AuPyUbNgnLgS/PE1Q==.3UAvzIWNbjkhzbtFCdvM9kCv0AfwZUof8cL/P0BOv+0=', NULL);
GO

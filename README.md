"# ASIBasecodeCSharp2024" 

QUERIES
================================================


ALTER TABLE M_USER
ADD PasswordResetToken NVARCHAR(255),
    PasswordResetExpiration DATETIME;

ALTER TABLE M_USER
ALTER COLUMN Password VARCHAR(512) NOT NULL;
ALTER TABLE M_USER
ALTER COLUMN PasswordResetToken NVARCHAR(1000);

ALTER TABLE M_User
ADD IsVerified BIT NOT NULL DEFAULT 0;

ALTER TABLE [AsiBasecodeDb].[dbo].[M_User]
ADD VerificationToken NVARCHAR(255),
    VerificationTokenExpiration DATETIME;


================================================


CREATE TABLE M_Category (
    CategoryId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Type NVARCHAR(MAX) NOT NULL,
    Icon NVARCHAR(MAX) NOT NULL,
    Color NVARCHAR(MAX) NOT NULL,
    UserId INT NULL,
    IsGlobal BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_MCategory_MUser_UserId FOREIGN KEY (UserId) REFERENCES M_User(UserId)
);

SET IDENTITY_INSERT M_Category ON;

INSERT INTO M_Category (CategoryId, Name, Type, Icon, Color, IsGlobal)
VALUES 
    (1, N'Default Income', N'Income', N'💰', N'#00FF00', 1),
    (2, N'Default Expense', N'Expense', N'💵', N'#FF0000', 1),
    (3, N'Food', N'Expense', N'🍕', N'#FFA500', 1),
    (4, N'Shopping', N'Expense', N'🛍️', N'#FF69B4', 1),
    (5, N'Bills', N'Expense', N'⚡', N'#FFD700', 1),
    (6, N'Car', N'Expense', N'🚗', N'#0000FF', 1),
    (7, N'Income', N'Income', N'💸', N'#008000', 1);

SET IDENTITY_INSERT M_Category OFF;


================================================


CREATE TABLE M_Wallet (
    WalletId INT PRIMARY KEY IDENTITY(1,1),
    WalletName NVARCHAR(50) NOT NULL,
    WalletBalance DECIMAL(10, 2) NOT NULL,
    WalletIcon NVARCHAR(255) NOT NULL,
    WalletColor NVARCHAR(20) NOT NULL,
    UserId INT NOT NULL,
    CONSTRAINT FK_M_Wallet_UserId FOREIGN KEY (UserId) REFERENCES M_User(UserId)
); 


================================================


CREATE TABLE M_Transaction (
    TransactionId INT PRIMARY KEY IDENTITY(1,1),
    CategoryId INT NOT NULL,
    WalletId INT NOT NULL,
    UserId INT NOT NULL,
    TransactionType NVARCHAR(50) NOT NULL,
    Amount DECIMAL(10, 2) NOT NULL,
    TransactionDate DATE NOT NULL,
    Note NVARCHAR(255),
    TransactionSort NVARCHAR(50) NOT NULL,
    CONSTRAINT chk_amount CHECK (Amount > 0),
    CONSTRAINT fk_category_id FOREIGN KEY (CategoryId) REFERENCES M_Category(CategoryId),
    CONSTRAINT fk_wallet_id FOREIGN KEY (WalletId) REFERENCES M_Wallet(WalletId),
    CONSTRAINT fk_user_id FOREIGN KEY (UserId) REFERENCES M_User(UserId)
); 


================================================

SCAFFOLDING 

Scaffold-DbContext "Addr=(LocalDB)\MSSQLLocalDB;database=AsiBasecodeDb;Integrated Security=False;Trusted_Connection=True" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -ContextDir . -F


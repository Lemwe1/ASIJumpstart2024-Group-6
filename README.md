How to Setup the Code/Project Solution:
-------------------------------------------------------------------------------------------------
1. Setup the Database
	- Add the AsiBasecodeDb
	- Next, do these queries for the tables

ALTER TABLE M_USER
ADD PasswordResetToken NVARCHAR(255),
    PasswordResetExpiration DATETIME;

ALTER TABLE M_USER
ALTER COLUMN Password VARCHAR(512) NOT NULL;
ALTER TABLE M_USER
ALTER COLUMN PasswordResetToken NVARCHAR(1000);

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

INSERT INTO M_Category (CategoryId, Name, Type, Icon, Color, IsGlobal)
VALUES 
    (1, N'Default Income', N'Income', N'💰', N'#00FF00', 1),
    (2, N'Default Expense', N'Expense', N'💵', N'#FF0000', 1),
    (3, N'Food', N'Expense', N'🍕', N'#FFA500', 1),
    (4, N'Shopping', N'Expense', N'🛍️', N'#FF69B4', 1),
    (5, N'Bills', N'Expense', N'⚡', N'#FFD700', 1),
    (6, N'Car', N'Expense', N'🚗', N'#0000FF', 1),
    (7, N'Income', N'Income', N'💸', N'#008000', 1);

CREATE TABLE M_Wallet (
    WalletId INT PRIMARY KEY IDENTITY(1,1),
    WalletName NVARCHAR(50) NOT NULL,
    WalletBalance DECIMAL(10, 2) NOT NULL,
    WalletOriginalBalance DECIMAL(10, 2) NOT NULL DEFAULT 0,
    WalletIcon NVARCHAR(255) NOT NULL,
    WalletColor NVARCHAR(20) NOT NULL,
    UserId INT NOT NULL,
    CONSTRAINT FK_M_Wallet_UserId FOREIGN KEY (UserId) REFERENCES M_User(UserId)
);

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

CREATE TABLE M_Budgets (
    BudgetId INT PRIMARY KEY IDENTITY(1,1),
    BudgetName NVARCHAR(50) NOT NULL,
    CategoryId INT NOT NULL,
    UserId INT NOT NULL,
    MonthlyBudget DECIMAL(18,2) NOT NULL,
    RemainingBudget DECIMAL(18,2) NOT NULL,
    LastResetDate DATETIME NOT NULL,
    FOREIGN KEY (CategoryId) REFERENCES M_Category(CategoryId),
    FOREIGN KEY (UserId) REFERENCES M_User(UserId)
);



-------------------------------------------------------------------------------------------------
2. Pull the Git Repository 
https://github.com/Lemwe1/ASIJumpstart2024-Group-6
-------------------------------------------------------------------------------------------------
3. Make sure the Connection String in your appsettings.json is correct
Example: "DefaultConnection": "Server={connectionstring here};database=AsiBasecodeDb;Integrated Security=False;Trusted_Connection=True"
-------------------------------------------------------------------------------------------------
4. Do the Scaffold, Make sure the startup project is the ASI.Basecode.Data
Example: Scaffold-DbContext "Server={connectionstring here};Database=AsiBasecodeDb;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -ContextDir . -Force
-------------------------------------------------------------------------------------------------
5. Set ASI.Basecode.WebApp is set as Startup Project, and run the Project

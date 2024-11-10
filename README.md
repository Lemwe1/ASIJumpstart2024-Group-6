"# ASIBasecodeCSharp2024" 


================================================


ALTER TABLE M_USER
ADD PasswordResetToken NVARCHAR(255),
    PasswordResetExpiration DATETIME;

ALTER TABLE M_USER
ALTER COLUMN Password VARCHAR(512) NOT NULL;
ALTER TABLE M_USER
ALTER COLUMN PasswordResetToken NVARCHAR(1000);


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

-- Insert default global categories
INSERT INTO M_Category (CategoryId, Name, Type, Icon, Color, IsGlobal)
VALUES 
    (1, 'Default Income', 'Income', 'fas fa-wallet', '#00FF00', 1),
    (2, 'Default Expense', 'Expense', 'fas fa-money-bill-wave', '#FF0000', 1),
    (3, 'Food', 'Expense', 'fas fa-pizza-slice', '#FFA500', 1),
    (4, 'Shopping', 'Expense', 'fas fa-shopping-bag', '#FF69B4', 1),
    (5, 'Bills', 'Expense', 'fas fa-bolt', '#FFD700', 1),
    (6, 'Car', 'Expense', 'fas fa-car', '#0000FF', 1),
    (7, 'Income', 'Income', 'fas fa-sack-dollar', '#008000', 1);

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
    CONSTRAINT chk_amount CHECK (Amount > 0),
    CONSTRAINT fk_category_id FOREIGN KEY (CategoryId) REFERENCES M_Category(CategoryId),
    CONSTRAINT fk_wallet_id FOREIGN KEY (WalletId) REFERENCES M_Wallet(WalletId),
    CONSTRAINT fk_user_id FOREIGN KEY (UserId) REFERENCES M_User(UserId)
); 


================================================


Scaffold-DbContext "{Addr=(LocalDB)\MSSQLLocalDB;database=AsiBasecodeDb;Integrated Security=False;Trusted_Connection=True}" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -ContextDir . -F


"# ASIBasecodeCSharp2024" 

FOR QUERY:
create table M_ROLE (
    RoleId int NOT NULL,
    RoleName varchar(100) NOT NULL,
    CONSTRAINT PK_M_ROLE PRIMARY KEY (RoleId)
);

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

CREATE TABLE M_Category (
    CategoryId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Type NVARCHAR(MAX) NOT NULL,
    Icon NVARCHAR(MAX) NOT NULL,
    Color NVARCHAR(MAX) NOT NULL,
    UserId INT NOT NULL,
    CONSTRAINT FK_MCategory_MUser_UserId FOREIGN KEY (UserId) REFERENCES M_User(UserId)
); 


CREATE TABLE M_DebitLiab (
    DeLiId INT PRIMARY KEY IDENTITY(1,1),
    DeLiType NVARCHAR(50) NOT NULL,
    DeLiBalance DECIMAL(10, 2) NOT NULL,
    DeLiIcon NVARCHAR(255) NOT NULL,
    DeLiColor NVARCHAR(20) NOT NULL,
    DeLiHapp DATETIME,
    DeLiDue DATETIME,
    DeLiName NVARCHAR(50) NOT NULL,
    UserId INT NOT NULL,
    CONSTRAINT FK_M_DebitLiab_UserId FOREIGN KEY (UserId) REFERENCES M_User(UserId)
);



CREATE TABLE M_Transaction (
    TransactionId INT PRIMARY KEY IDENTITY(1,1),
    CategoryId INT NOT NULL,
    DeLiId INT NOT NULL,
    UserId INT NOT NULL,
    TransactionType NVARCHAR(50) NOT NULL,
    Description NVARCHAR(100),
    Amount DECIMAL(10, 2) NOT NULL,
    TransactionDate DATE NOT NULL,
    Note NVARCHAR(255),
    CONSTRAINT chk_amount CHECK (Amount > 0),
    CONSTRAINT fk_category_id FOREIGN KEY (CategoryId) REFERENCES M_Category(CategoryId),
    CONSTRAINT fk_deli_id FOREIGN KEY (DeLiId) REFERENCES M_DebitLiab(DeLiId),
    CONSTRAINT fk_user_id FOREIGN KEY (UserId) REFERENCES M_User(UserId)
);




-----------------------
For Scaffolding

Scaffold-DbContext "{Addr=(LocalDB)\MSSQLLocalDB;database=AsiBasecodeDb;Integrated Security=False;Trusted_Connection=True}" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -ContextDir . -F

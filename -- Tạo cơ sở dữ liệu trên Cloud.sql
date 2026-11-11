-- Chay script nay tren SQL Server, bat dau tu database master.
USE [master];
GO

DROP TRIGGER IF EXISTS dbo.TR_user_profile_updated_at;
GO

IF DB_ID(N'health_fitness_cloud_db') IS NULL
BEGIN
    CREATE DATABASE [health_fitness_cloud_db];
END;
GO

USE [health_fitness_cloud_db];
GO

IF OBJECT_ID(N'dbo.users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.users (
        id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_users PRIMARY KEY,
        user_id NVARCHAR(100) NOT NULL,
        username NVARCHAR(255) NOT NULL,
        password NVARCHAR(255) NOT NULL, -- Lưu mật khẩu thông thường
        created_at DATETIME2(3) NOT NULL CONSTRAINT DF_users_created_at DEFAULT SYSUTCDATETIME(),
        CONSTRAINT UQ_users_username UNIQUE (username)
    );
END;
GO

-- 1. Thong tin ca nhan cua nguoi dung
IF OBJECT_ID(N'dbo.user_profile', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.user_profile (
        id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_user_profile PRIMARY KEY,
        user_id NVARCHAR(100) NOT NULL,
        name NVARCHAR(255) NULL,
        age INT NULL,
        gender NVARCHAR(50) NULL,
        height FLOAT NULL,
        target_weight FLOAT NULL,
        activity_level NVARCHAR(100) NULL,
        created_at DATETIME2(3) NOT NULL CONSTRAINT DF_user_profile_created_at DEFAULT SYSUTCDATETIME(),
        updated_at DATETIME2(3) NOT NULL CONSTRAINT DF_user_profile_updated_at DEFAULT SYSUTCDATETIME(),
        CONSTRAINT UQ_user_profile_user_id UNIQUE (user_id)
    );
END;
GO

-- 2. Nhat ky an uong / dinh duong
IF OBJECT_ID(N'dbo.food_log', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.food_log (
        id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_food_log PRIMARY KEY,
        user_id NVARCHAR(100) NOT NULL,
        food_name NVARCHAR(255) NOT NULL,
        calories INT NOT NULL,
        protein FLOAT NULL,
        carbs FLOAT NULL,
        fat FLOAT NULL,
        logged_at DATETIME2(3) NOT NULL,
        synced_at DATETIME2(3) NOT NULL CONSTRAINT DF_food_log_synced_at DEFAULT SYSUTCDATETIME()
    );
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_food_log_user_id_logged_at'
      AND object_id = OBJECT_ID(N'dbo.food_log')
)
BEGIN
    CREATE INDEX IX_food_log_user_id_logged_at
        ON dbo.food_log (user_id, logged_at);
END;
GO

-- 3. Nhat ky uong nuoc
IF OBJECT_ID(N'dbo.water_log', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.water_log (
        id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_water_log PRIMARY KEY,
        user_id NVARCHAR(100) NOT NULL,
        amount_ml INT NOT NULL,
        logged_at DATETIME2(3) NOT NULL,
        synced_at DATETIME2(3) NOT NULL CONSTRAINT DF_water_log_synced_at DEFAULT SYSUTCDATETIME()
    );
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_water_log_user_id_logged_at'
      AND object_id = OBJECT_ID(N'dbo.water_log')
)
BEGIN
    CREATE INDEX IX_water_log_user_id_logged_at
        ON dbo.water_log (user_id, logged_at);
END;
GO

-- 4. Lich su can nang
IF OBJECT_ID(N'dbo.weight_entry', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.weight_entry (
        id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_weight_entry PRIMARY KEY,
        user_id NVARCHAR(100) NOT NULL,
        weight FLOAT NOT NULL,
        recorded_date DATE NOT NULL,
        synced_at DATETIME2(3) NOT NULL CONSTRAINT DF_weight_entry_synced_at DEFAULT SYSUTCDATETIME()
    );
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_weight_entry_user_id_recorded_date'
      AND object_id = OBJECT_ID(N'dbo.weight_entry')
)
BEGIN
    CREATE INDEX IX_weight_entry_user_id_recorded_date
        ON dbo.weight_entry (user_id, recorded_date);
END;
GO

-- 5. Nhat ky tap luyen
IF OBJECT_ID(N'dbo.workout_log', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.workout_log (
        id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_workout_log PRIMARY KEY,
        user_id NVARCHAR(100) NOT NULL,
        exercise_name NVARCHAR(255) NOT NULL,
        duration_minutes INT NOT NULL,
        calories_burned INT NULL,
        logged_at DATETIME2(3) NOT NULL,
        synced_at DATETIME2(3) NOT NULL CONSTRAINT DF_workout_log_synced_at DEFAULT SYSUTCDATETIME()
    );
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_workout_log_user_id_logged_at'
      AND object_id = OBJECT_ID(N'dbo.workout_log')
)
BEGIN
    CREATE INDEX IX_workout_log_user_id_logged_at
        ON dbo.workout_log (user_id, logged_at);
END;
GO

-- Cap nhat updated_at moi khi thong tin nguoi dung thay doi.
CREATE OR ALTER TRIGGER dbo.TR_user_profile_updated_at
ON dbo.user_profile
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF TRIGGER_NESTLEVEL() > 1
        RETURN;

    UPDATE profile
    SET updated_at = SYSUTCDATETIME()
    FROM dbo.user_profile AS profile
    INNER JOIN inserted AS changed ON changed.id = profile.id;
END;
GO

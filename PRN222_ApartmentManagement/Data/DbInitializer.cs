using Microsoft.EntityFrameworkCore;

namespace PRN222_ApartmentManagement.Data;

/// <summary>
/// Database initializer for automatic database creation and seeding
/// </summary>
public static class DbInitializer
{
    /// <summary>
    /// Initialize database - creates if not exists, applies migrations if exists
    /// </summary>
    public static async Task InitializeAsync(ApartmentDbContext context, ILogger logger)
    {
        try
        {
            // Check if database can connect
            var canConnect = await context.Database.CanConnectAsync();

            if (!canConnect)
            {
                logger.LogInformation("Database does not exist. Creating database...");
                
                // Create database with all tables
                await context.Database.EnsureCreatedAsync();
                
                logger.LogInformation("Database created successfully at {Time}", DateTime.Now);
                
                // Seed initial data if needed
                await SeedDataAsync(context, logger);
            }
            else
            {
                logger.LogInformation("Database already exists. Checking for pending migrations...");
                
                // Apply any pending migrations
                var pendingMigrations = (await context.Database.GetPendingMigrationsAsync()).ToList();
                if (pendingMigrations.Any())
                {
                    logger.LogInformation("Applying {Count} pending migrations...", pendingMigrations.Count);
                    await context.Database.MigrateAsync();
                    logger.LogInformation("Migrations applied successfully");
                }
                else
                {
                    logger.LogInformation("Database is up to date");
                }
            }

            // Always ensure default settings exist
            await SeedSettingsAsync(context, logger);
            await EnsureCommunityEngagementSchemaAsync(context, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the database");
            throw new InvalidOperationException("Database initialization failed. See inner exception for details.", ex);
        }
    }

    /// <summary>
    /// Seed initial data into database
    /// </summary>
    private static async Task SeedDataAsync(ApartmentDbContext context, ILogger logger)
    {
        // Check if data already exists
        if (await context.Users.AnyAsync())
        {
            logger.LogInformation("Database already contains data. Skipping seed.");
            return;
        }

        logger.LogInformation("Seeding initial data...");

        // Password hash for '123456' using BCrypt
        string passwordHash = BCrypt.Net.BCrypt.HashPassword("123456");

        var users = new List<Models.User>
        {
            new Models.User
            {
                Username = "admin",
                PasswordHash = passwordHash,
                FullName = "System Administrator",
                Email = "admin@apartment.com",
                Role = Models.UserRole.Admin,
                IsActive = true,
                CreatedAt = DateTime.Now
            },
            new Models.User
            {
                Username = "manager",
                PasswordHash = passwordHash,
                FullName = "BQL Manager",
                Email = "manager@apartment.com",
                Role = Models.UserRole.BQL_Manager,
                IsActive = true,
                CreatedAt = DateTime.Now
            },
            new Models.User
            {
                Username = "staff",
                PasswordHash = passwordHash,
                FullName = "BQL Staff",
                Email = "staff@apartment.com",
                Role = Models.UserRole.BQL_Staff,
                IsActive = true,
                CreatedAt = DateTime.Now
            },
            new Models.User
            {
                Username = "bqt_head",
                PasswordHash = passwordHash,
                FullName = "BQT Head",
                Email = "bqt_head@apartment.com",
                Role = Models.UserRole.BQT_Head,
                IsActive = true,
                CreatedAt = DateTime.Now
            },
            new Models.User
            {
                Username = "resident",
                PasswordHash = passwordHash,
                FullName = "Nguyen Van Resident",
                Email = "resident@apartment.com",
                Role = Models.UserRole.Resident,
                IsActive = true,
                CreatedAt = DateTime.Now
            },
            new Models.User
            {
                Username = "bqt_member",
                PasswordHash = passwordHash,
                FullName = "BQT Member",
                Email = "bqt_member@apartment.com",
                Role = Models.UserRole.BQT_Member,
                IsActive = true,
                CreatedAt = DateTime.Now
            }
        };

        context.Users.AddRange(users);

        // Add some service types
        var serviceTypes = new[]
        {
            new Models.ServiceType { ServiceTypeName = "Electricity", MeasurementUnit = "kWh", IsActive = true },
            new Models.ServiceType { ServiceTypeName = "Water", MeasurementUnit = "m³", IsActive = true },
            new Models.ServiceType { ServiceTypeName = "Management Fee", MeasurementUnit = "month", IsActive = true },
            new Models.ServiceType { ServiceTypeName = "Parking Fee", MeasurementUnit = "vehicle", IsActive = true },
            new Models.ServiceType { ServiceTypeName = "Internet", MeasurementUnit = "month", IsActive = true }
        };

        context.ServiceTypes.AddRange(serviceTypes);

        await context.SaveChangesAsync();

        logger.LogInformation("Initial data seeded successfully");
    }

    /// <summary>
    /// Seed default system settings into database
    /// </summary>
    private static async Task SeedSettingsAsync(ApartmentDbContext context, ILogger logger)
    {
        if (await context.SystemSettings.AnyAsync()) return;

        logger.LogInformation("Seeding default system settings...");

        var settings = new List<Models.SystemSetting>
        {
            new Models.SystemSetting { SettingKey = "ApartmentName", SettingValue = "ApartmentMS", Description = "Tên chung cư hiển thị trên hệ thống" },
            new Models.SystemSetting { SettingKey = "ApartmentLogo", SettingValue = "apartment", Description = "Tên Icon Material hiển thị ở Logo" },
            new Models.SystemSetting { SettingKey = "ApartmentAddress", SettingValue = "123 Đường ABC, Quận XYZ, TP. HCM", Description = "Địa chỉ chung cư" },
            new Models.SystemSetting { SettingKey = "ApartmentContact", SettingValue = "0123.456.789", Description = "Số điện thoại liên hệ" }
        };

        context.SystemSettings.AddRange(settings);
        await context.SaveChangesAsync();
    }

    private static async Task EnsureCommunityEngagementSchemaAsync(ApartmentDbContext context, ILogger logger)
    {
        const string sql = """
IF OBJECT_ID(N'[dbo].[CommunityCampaigns]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[CommunityCampaigns](
        [CampaignId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Title] NVARCHAR(200) NOT NULL,
        [Description] NVARCHAR(2000) NULL,
        [QuestionText] NVARCHAR(500) NOT NULL,
        [CampaignType] NVARCHAR(20) NOT NULL,
        [Status] NVARCHAR(20) NOT NULL,
        [AllowMultipleChoices] BIT NOT NULL DEFAULT 0,
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        [StartsAt] DATETIME2 NOT NULL,
        [EndsAt] DATETIME2 NOT NULL,
        [CreatedBy] INT NOT NULL,
        [CreatedAt] DATETIME2 NOT NULL,
        [UpdatedAt] DATETIME2 NULL,
        CONSTRAINT [FK_CommunityCampaigns_Users_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users]([UserId])
    );
END;

IF OBJECT_ID(N'[dbo].[CommunityCampaignOptions]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[CommunityCampaignOptions](
        [OptionId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [CampaignId] INT NOT NULL,
        [OptionText] NVARCHAR(300) NOT NULL,
        [DisplayOrder] INT NOT NULL,
        CONSTRAINT [FK_CommunityCampaignOptions_CommunityCampaigns_CampaignId] FOREIGN KEY ([CampaignId]) REFERENCES [dbo].[CommunityCampaigns]([CampaignId]) ON DELETE CASCADE
    );
    CREATE INDEX [IX_CommunityCampaignOptions_CampaignId_DisplayOrder] ON [dbo].[CommunityCampaignOptions]([CampaignId], [DisplayOrder]);
END;

IF OBJECT_ID(N'[dbo].[CommunityCampaignResponses]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[CommunityCampaignResponses](
        [ResponseId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [CampaignId] INT NOT NULL,
        [UserId] INT NOT NULL,
        [Comment] NVARCHAR(1000) NULL,
        [SubmittedAt] DATETIME2 NOT NULL,
        CONSTRAINT [FK_CommunityCampaignResponses_CommunityCampaigns_CampaignId] FOREIGN KEY ([CampaignId]) REFERENCES [dbo].[CommunityCampaigns]([CampaignId]) ON DELETE CASCADE,
        CONSTRAINT [FK_CommunityCampaignResponses_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([UserId])
    );
    CREATE UNIQUE INDEX [IX_CommunityCampaignResponses_CampaignId_UserId] ON [dbo].[CommunityCampaignResponses]([CampaignId], [UserId]);
END;

IF OBJECT_ID(N'[dbo].[CommunityCampaignResponseOptions]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[CommunityCampaignResponseOptions](
        [ResponseOptionId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [ResponseId] INT NOT NULL,
        [OptionId] INT NOT NULL,
        CONSTRAINT [FK_CommunityCampaignResponseOptions_CommunityCampaignResponses_ResponseId] FOREIGN KEY ([ResponseId]) REFERENCES [dbo].[CommunityCampaignResponses]([ResponseId]) ON DELETE CASCADE,
        CONSTRAINT [FK_CommunityCampaignResponseOptions_CommunityCampaignOptions_OptionId] FOREIGN KEY ([OptionId]) REFERENCES [dbo].[CommunityCampaignOptions]([OptionId])
    );
    CREATE UNIQUE INDEX [IX_CommunityCampaignResponseOptions_ResponseId_OptionId] ON [dbo].[CommunityCampaignResponseOptions]([ResponseId], [OptionId]);
END;
""";

        try
        {
            await context.Database.ExecuteSqlRawAsync(sql);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to ensure community engagement schema");
            throw;
        }
    }

    /// <summary>
    /// Check database health
    /// </summary>
    public static async Task<bool> CheckDatabaseHealthAsync(ApartmentDbContext context)
    {
        try
        {
            return await context.Database.CanConnectAsync();
        }
        catch
        {
            return false;
        }
    }
}

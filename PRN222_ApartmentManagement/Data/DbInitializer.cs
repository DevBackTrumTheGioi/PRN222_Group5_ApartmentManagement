using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Utils;

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
            var canConnect = await context.Database.CanConnectAsync();

            if (!canConnect)
            {
                logger.LogInformation("Database does not exist. Creating database...");
                await context.Database.EnsureCreatedAsync();
                logger.LogInformation("Database created successfully at {Time}", DateTime.Now);
                await SeedDataAsync(context, logger);
            }
            else
            {
                logger.LogInformation("Database already exists. Checking for pending migrations...");

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
        if (await context.Users.AnyAsync() &&
            await context.Apartments.AnyAsync() &&
            await context.ServiceTypes.AnyAsync() &&
            await context.Amenities.AnyAsync())
        {
            logger.LogInformation("Database already contains core data. Skipping automatic seed.");
            return;
        }

        logger.LogInformation("Seeding realistic initial data...");
        await DataSeeder.SeedAsync(context, logger);
        logger.LogInformation("Initial data seeded successfully");
    }

    /// <summary>
    /// Seed default system settings into database
    /// </summary>
    private static async Task SeedSettingsAsync(ApartmentDbContext context, ILogger logger)
    {
        if (await context.SystemSettings.AnyAsync())
        {
            return;
        }

        logger.LogInformation("Seeding default system settings...");

        context.SystemSettings.AddRange(
            new Models.SystemSetting
            {
                SettingKey = "ApartmentName",
                SettingValue = "Sunrise Riverside",
                Description = "Tên chung cư hiển thị trên hệ thống"
            },
            new Models.SystemSetting
            {
                SettingKey = "ApartmentLogo",
                SettingValue = "apartment",
                Description = "Tên Icon Material hiển thị ở Logo"
            },
            new Models.SystemSetting
            {
                SettingKey = "ApartmentAddress",
                SettingValue = "88 Nguyễn Hữu Thọ, Phường Tân Hưng, Quận 7, TP. Hồ Chí Minh",
                Description = "Địa chỉ chung cư"
            },
            new Models.SystemSetting
            {
                SettingKey = "ApartmentContact",
                SettingValue = "028 3888 6688",
                Description = "Số điện thoại liên hệ"
            });

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

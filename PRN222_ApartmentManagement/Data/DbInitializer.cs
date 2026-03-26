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

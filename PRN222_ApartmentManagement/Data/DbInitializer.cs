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

        // Add default admin user
        var adminUser = new Models.User
        {
            Username = "admin",
            PasswordHash = "AQAAAAEAACcQAAAAEJ5Z5Z5Z5Z5Z5Z5Z5Z5Z5Z5Z5Z5Z5Z5Z5Z5Z5Z5Z5Z5Z5Z==", // Default: admin123
            FullName = "System Administrator",
            Email = "admin@apartment.com",
            Role = "Admin",
            IsActive = true,
            CreatedAt = DateTime.Now
        };

        context.Users.Add(adminUser);

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


﻿using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Repositories.Interfaces;
using PRN222_ApartmentManagement.Services;

namespace PRN222_ApartmentManagement.Repositories.Implementations;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly ApartmentDbContext _context;
    protected readonly DbSet<T> _dbSet;
    protected readonly IActivityLogService _activityLog;

    public GenericRepository(ApartmentDbContext context, IActivityLogService activityLog)
    {
        _context = context;
        _dbSet = context.Set<T>();
        _activityLog = activityLog;
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public virtual async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();

        // Log CREATE action
        try
        {
            var entityId = GetEntityId(entity);
            await _activityLog.LogCreateAsync(
                entityName: typeof(T).Name,
                entityId: entityId ?? "N/A",
                newValues: entity,
                description: $"Tạo mới {typeof(T).Name}"
            );
        }
        catch
        {
            // Không crash nếu logging fails
        }
    }

    public virtual async Task AddRangeAsync(IEnumerable<T> entities)
    {
        var entityList = entities.ToList(); // Materialize to avoid multiple enumeration
        await _dbSet.AddRangeAsync(entityList);

        // Log bulk CREATE
        try
        {
            await _activityLog.LogCustomAsync(
                action: "BulkCreate",
                entityName: typeof(T).Name,
                description: $"Tạo mới {entityList.Count} {typeof(T).Name}",
                data: new { Count = entityList.Count }
            );
        }
        catch
        {
            // Không crash nếu logging fails
        }
    }

    public virtual async Task UpdateAsync(T entity)
    {
        // Get old values for logging
        var entityId = GetEntityId(entity);
        T? oldEntity = null;

        try
        {
            if (entityId != null && int.TryParse(entityId, out var id))
            {
                oldEntity = await _dbSet.AsNoTracking()
                    .FirstOrDefaultAsync(e => EF.Property<int>(e, GetIdPropertyName()) == id);
            }
        }
        catch
        {
            // Continue if we can't get old values
        }

        _dbSet.Update(entity);
        await _context.SaveChangesAsync();

        // Log UPDATE action
        try
        {
            await _activityLog.LogUpdateAsync(
                entityName: typeof(T).Name,
                entityId: entityId ?? "N/A",
                oldValues: (object?)oldEntity ?? new { Message = "Old values not available" },
                newValues: entity,
                description: $"Cập nhật {typeof(T).Name}"
            );
        }
        catch
        {
            // Không crash nếu logging fails
        }
    }

    public virtual async Task DeleteAsync(T entity)
    {
        var entityId = GetEntityId(entity);

        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();

        // Log DELETE action
        try
        {
            await _activityLog.LogDeleteAsync(
                entityName: typeof(T).Name,
                entityId: entityId ?? "N/A",
                oldValues: entity,
                description: $"Xóa {typeof(T).Name}"
            );
        }
        catch
        {
            // Không crash nếu logging fails
        }
    }

    public virtual async Task DeleteAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity != null)
        {
            await DeleteAsync(entity);
        }
    }

    public virtual async Task<bool> ExistsAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        return entity != null;
    }

    #region Helper Methods for Logging

    /// <summary>
    /// Lấy ID của entity để log (thử các tên property phổ biến)
    /// </summary>
    protected virtual string? GetEntityId(T entity)
    {
        try
        {
            var idPropertyName = GetIdPropertyName();
            var idProperty = typeof(T).GetProperty(idPropertyName);
            var idValue = idProperty?.GetValue(entity);
            return idValue?.ToString();
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Lấy tên property ID (thử theo convention)
    /// </summary>
    protected virtual string GetIdPropertyName()
    {
        // Try common ID property patterns
        var properties = typeof(T).GetProperties();

        // Pattern 1: {EntityName}Id (e.g., ApartmentId, ResidentId)
        var idProp = properties.FirstOrDefault(p => p.Name == $"{typeof(T).Name}Id");
        if (idProp != null) return idProp.Name;

        // Pattern 2: Id
        idProp = properties.FirstOrDefault(p => p.Name == "Id");
        if (idProp != null) return idProp.Name;

        // Pattern 3: UserId (for User table)
        idProp = properties.FirstOrDefault(p => p.Name == "UserId");
        if (idProp != null) return idProp.Name;

        // Default
        return "Id";
    }

    #endregion
}


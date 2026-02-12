﻿using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Models;

namespace PRN222_ApartmentManagement.Data;

/// <summary>
/// Database context for Apartment Management System
/// </summary>
public class ApartmentDbContext : DbContext
{
    public ApartmentDbContext(DbContextOptions<ApartmentDbContext> options)
        : base(options)
    {
    }

    // Core entities
    public DbSet<Apartment> Apartments { get; set; }
    public DbSet<Resident> Residents { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<ResidentCard> ResidentCards { get; set; }

    // Service entities
    public DbSet<ServiceType> ServiceTypes { get; set; }
    public DbSet<ServicePrice> ServicePrices { get; set; }
    public DbSet<MeterReading> MeterReadings { get; set; }
    public DbSet<ApartmentService> ApartmentServices { get; set; }

    // Invoice entities
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceDetail> InvoiceDetails { get; set; }
    public DbSet<PaymentTransaction> PaymentTransactions { get; set; }

    // Request entities
    public DbSet<Request> Requests { get; set; }
    public DbSet<RequestAttachment> RequestAttachments { get; set; }

    // Announcement entities
    public DbSet<Announcement> Announcements { get; set; }
    public DbSet<Document> Documents { get; set; }

    // Facility entities
    public DbSet<Visitor> Visitors { get; set; }
    public DbSet<Parcel> Parcels { get; set; }
    public DbSet<Amenity> Amenities { get; set; }
    public DbSet<AmenityBooking> AmenityBookings { get; set; }

    // Communication entities
    public DbSet<Notification> Notifications { get; set; }

    // Contract entities
    public DbSet<Contract> Contracts { get; set; }
    public DbSet<ContractMember> ContractMembers { get; set; }

    // Activity Log
    public DbSet<ActivityLog> ActivityLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .UseTptMappingStrategy();

        modelBuilder.Entity<Resident>()
            .ToTable("Residents");

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique()
            .HasFilter("[Email] IS NOT NULL");

        modelBuilder.Entity<Resident>()
            .HasIndex(r => r.IdentityCardNumber)
            .IsUnique()
            .HasFilter("[IdentityCardNumber] IS NOT NULL");

        modelBuilder.Entity<ResidentCard>()
            .HasIndex(rc => rc.CardNumber)
            .IsUnique();

        modelBuilder.Entity<Vehicle>()
            .HasIndex(v => v.LicensePlate)
            .IsUnique()
            .HasFilter("[LicensePlate] IS NOT NULL");

        modelBuilder.Entity<Invoice>()
            .HasIndex(i => i.InvoiceNumber)
            .IsUnique();

        modelBuilder.Entity<PaymentTransaction>()
            .HasIndex(pt => pt.TransactionCode)
            .IsUnique();

        modelBuilder.Entity<Request>()
            .HasIndex(r => r.RequestNumber)
            .IsUnique();

        modelBuilder.Entity<Visitor>()
            .HasIndex(v => v.QRCode)
            .IsUnique()
            .HasFilter("[QRCode] IS NOT NULL");

        modelBuilder.Entity<Parcel>()
            .HasIndex(p => p.TrackingNumber)
            .IsUnique()
            .HasFilter("[TrackingNumber] IS NOT NULL");

        modelBuilder.Entity<Contract>()
            .HasIndex(c => c.ContractNumber)
            .IsUnique();

        // Configure relationships with specific navigation properties
        modelBuilder.Entity<Resident>()
            .HasOne(r => r.Apartment)
            .WithMany(a => a.Residents)
            .HasForeignKey(r => r.ApartmentId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<ResidentCard>()
            .HasOne(rc => rc.Resident)
            .WithMany(r => r.ResidentCards)
            .HasForeignKey(rc => rc.ResidentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ResidentCard>()
            .HasOne(rc => rc.Vehicle)
            .WithMany(v => v.ResidentCards)
            .HasForeignKey(rc => rc.VehicleId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Vehicle>()
            .HasOne(v => v.Resident)
            .WithMany(r => r.Vehicles)
            .HasForeignKey(v => v.ResidentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MeterReading>()
            .HasOne(mr => mr.Staff)
            .WithMany(u => u.MeterReadings)
            .HasForeignKey(mr => mr.StaffId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Invoice>()
            .HasOne(i => i.Creator)
            .WithMany(u => u.CreatedInvoices)
            .HasForeignKey(i => i.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<InvoiceDetail>()
            .HasOne(id => id.MeterReading)
            .WithMany(mr => mr.InvoiceDetails)
            .HasForeignKey(id => id.MeterReadingId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<InvoiceDetail>()
            .HasOne(id => id.ServiceType)
            .WithMany(st => st.InvoiceDetails)
            .HasForeignKey(id => id.ServiceTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<InvoiceDetail>()
            .HasOne(id => id.ServicePrice)
            .WithMany(sp => sp.InvoiceDetails)
            .HasForeignKey(id => id.ServicePriceId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<PaymentTransaction>()
            .HasOne(pt => pt.Creator)
            .WithMany(u => u.PaymentTransactions)
            .HasForeignKey(pt => pt.CreatedBy)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Request>()
            .HasOne(r => r.Apartment)
            .WithMany(a => a.Requests)
            .HasForeignKey(r => r.ApartmentId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Request>()
            .HasOne(r => r.Resident)
            .WithMany(res => res.Requests)
            .HasForeignKey(r => r.ResidentId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Request>()
            .HasOne(r => r.AssignedUser)
            .WithMany(u => u.AssignedRequests)
            .HasForeignKey(r => r.AssignedTo)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Announcement>()
            .HasOne(a => a.Creator)
            .WithMany(u => u.Announcements)
            .HasForeignKey(a => a.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Document>()
            .HasOne(d => d.Uploader)
            .WithMany(u => u.Documents)
            .HasForeignKey(d => d.UploadedBy)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Visitor>()
            .HasOne(v => v.RegisteredByResident)
            .WithMany(r => r.RegisteredVisitors)
            .HasForeignKey(v => v.RegisteredBy)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Parcel>()
            .HasOne(p => p.ReceivedByUser)
            .WithMany(u => u.ReceivedParcels)
            .HasForeignKey(p => p.ReceivedBy)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Parcel>()
            .HasOne(p => p.PickedUpByResident)
            .WithMany(r => r.PickedUpParcels)
            .HasForeignKey(p => p.PickedUpBy)
            .OnDelete(DeleteBehavior.SetNull);


        modelBuilder.Entity<Contract>()
            .HasOne(c => c.Apartment)
            .WithMany(a => a.Contracts)
            .HasForeignKey(c => c.ApartmentId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Contract>()
            .HasOne(c => c.Creator)
            .WithMany(u => u.CreatedContracts)
            .HasForeignKey(c => c.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);

        // Configure computed columns
        modelBuilder.Entity<MeterReading>()
            .Property(mr => mr.Consumption)
            .HasComputedColumnSql("[CurrentReading] - [PreviousReading]", stored: true);

        modelBuilder.Entity<InvoiceDetail>()
            .Property(id => id.Amount)
            .HasComputedColumnSql("[Quantity] * [UnitPrice]", stored: true);
    }
}


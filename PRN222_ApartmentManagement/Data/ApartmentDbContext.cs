using Microsoft.EntityFrameworkCore;
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
    public DbSet<User> Users { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<ResidentCard> ResidentCards { get; set; }

    // Service entities
    public DbSet<ServiceType> ServiceTypes { get; set; }
    public DbSet<ServicePrice> ServicePrices { get; set; }
    public DbSet<ServiceOrder> ServiceOrders { get; set; }
    public DbSet<ApartmentService> ApartmentServices { get; set; }

    // Invoice entities
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceDetail> InvoiceDetails { get; set; }
    public DbSet<PaymentTransaction> PaymentTransactions { get; set; }

    // Request entities
    public DbSet<Request> Requests { get; set; }
    public DbSet<RequestAttachment> RequestAttachments { get; set; }
    public DbSet<RequestComment> RequestComments { get; set; }

    // Announcement entities
    public DbSet<Announcement> Announcements { get; set; }
    public DbSet<AnnouncementRead> AnnouncementReads { get; set; }
    public DbSet<AnnouncementAttachment> AnnouncementAttachments { get; set; }
    public DbSet<Document> Documents { get; set; }

    // Facility entities
    public DbSet<Visitor> Visitors { get; set; }
    public DbSet<AmenityType> AmenityTypes { get; set; }
    public DbSet<Amenity> Amenities { get; set; }
    public DbSet<AmenityBooking> AmenityBookings { get; set; }

    // Communication entities
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<CommunityCampaign> CommunityCampaigns { get; set; }
    public DbSet<CommunityCampaignOption> CommunityCampaignOptions { get; set; }
    public DbSet<CommunityCampaignResponse> CommunityCampaignResponses { get; set; }
    public DbSet<CommunityCampaignResponseOption> CommunityCampaignResponseOptions { get; set; }

    // Contract entities
    public DbSet<Contract> Contracts { get; set; }
    public DbSet<ContractMember> ContractMembers { get; set; }

    // Residency entities
    public DbSet<ResidentApartment> ResidentApartments { get; set; }

    // Activity Log
    public DbSet<ActivityLog> ActivityLogs { get; set; }
    public DbSet<SystemSetting> SystemSettings { get; set; }
    public DbSet<FaceAuthHistory> FaceAuthHistories { get; set; }
    public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<string>()
            .HasMaxLength(50);

        modelBuilder.Entity<User>()
            .Property(u => u.ResidentType)
            .HasConversion<string>()
            .HasMaxLength(50);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique()
            .HasFilter("[Email] IS NOT NULL");

        modelBuilder.Entity<User>()
            .HasIndex(u => u.IdentityCardNumber)
            .IsUnique()
            .HasFilter("[IdentityCardNumber] IS NOT NULL");

        modelBuilder.Entity<ResidentApartment>(entity =>
        {
            entity.Property(e => e.ResidencyType)
                .HasConversion<string>()
                .HasMaxLength(50);

            // 1 user + 1 apartment + 1 contract = unique
            entity.HasIndex(e => new { e.UserId, e.ApartmentId, e.ContractId })
                .IsUnique();

            entity.HasOne(e => e.User)
                .WithMany(u => u.ResidentApartments)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Apartment)
                .WithMany(a => a.ResidentApartments)
                .HasForeignKey(e => e.ApartmentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Contract)
                .WithMany(c => c.ResidentApartments)
                .HasForeignKey(e => e.ContractId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Announcement>()
            .Property(a => a.AnnouncementType)
            .HasConversion<string>()
            .HasMaxLength(50);

        modelBuilder.Entity<Announcement>()
            .Property(a => a.Priority)
            .HasConversion<string>()
            .HasMaxLength(20);

        modelBuilder.Entity<Document>()
            .Property(d => d.DocumentType)
            .HasConversion<string>()
            .HasMaxLength(50);

        modelBuilder.Entity<Request>()
            .Property(r => r.RequestType)
            .HasConversion<string>()
            .HasMaxLength(50);

        modelBuilder.Entity<Request>()
            .Property(r => r.Priority)
            .HasConversion<string>()
            .HasMaxLength(20);

        modelBuilder.Entity<Request>()
            .Property(r => r.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        modelBuilder.Entity<ResidentCard>()
            .Property(rc => rc.CardType)
            .HasConversion<string>()
            .HasMaxLength(50);

        modelBuilder.Entity<ResidentCard>()
            .Property(rc => rc.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        modelBuilder.Entity<ServiceOrder>()
            .Property(so => so.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        modelBuilder.Entity<Visitor>()
            .Property(v => v.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        modelBuilder.Entity<Invoice>()
            .Property(i => i.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        modelBuilder.Entity<Invoice>()
            .Property(i => i.ApprovalStatus)
            .HasConversion<string>()
            .HasMaxLength(20);

        modelBuilder.Entity<Notification>()
            .Property(n => n.NotificationType)
            .HasConversion<string>()
            .HasMaxLength(50);

        modelBuilder.Entity<Notification>()
            .Property(n => n.ReferenceType)
            .HasConversion<string>()
            .HasMaxLength(50);

        modelBuilder.Entity<Notification>()
            .Property(n => n.Priority)
            .HasConversion<string>()
            .HasMaxLength(20);

        modelBuilder.Entity<CommunityCampaign>()
            .Property(c => c.CampaignType)
            .HasConversion<string>()
            .HasMaxLength(20);

        modelBuilder.Entity<CommunityCampaign>()
            .Property(c => c.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        modelBuilder.Entity<Apartment>()
            .Property(a => a.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        modelBuilder.Entity<Contract>()
            .Property(c => c.ContractType)
            .HasConversion<string>()
            .HasMaxLength(50);

        modelBuilder.Entity<Contract>()
            .Property(c => c.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        modelBuilder.Entity<ContractMember>()
            .Property(cm => cm.SignatureStatus)
            .HasConversion<string>()
            .HasMaxLength(20);

        modelBuilder.Entity<ContractMember>()
            .Property(cm => cm.MemberRole)
            .HasConversion<string>()
            .HasMaxLength(50);

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


        modelBuilder.Entity<Contract>()
            .HasIndex(c => c.ContractNumber)
            .IsUnique();

        modelBuilder.Entity<UserRefreshToken>()
            .HasIndex(rt => rt.TokenHash)
            .IsUnique();

        modelBuilder.Entity<UserRefreshToken>()
            .HasIndex(rt => new { rt.UserId, rt.ExpiresAt });

        modelBuilder.Entity<CommunityCampaignOption>()
            .HasIndex(o => new { o.CampaignId, o.DisplayOrder });

        modelBuilder.Entity<CommunityCampaignResponse>()
            .HasIndex(r => new { r.CampaignId, r.UserId })
            .IsUnique();

        modelBuilder.Entity<CommunityCampaignResponseOption>()
            .HasIndex(ro => new { ro.ResponseId, ro.OptionId })
            .IsUnique();

        // Configure relationships with specific navigation properties
        modelBuilder.Entity<User>()
            .HasOne(u => u.Apartment)
            .WithMany(a => a.Residents)
            .HasForeignKey(u => u.ApartmentId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<UserRefreshToken>()
            .HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ResidentCard>()
            .HasOne(rc => rc.Resident)
            .WithMany(u => u.ResidentCards)
            .HasForeignKey(rc => rc.ResidentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Vehicle>()
            .HasOne(v => v.Resident)
            .WithMany(u => u.Vehicles)
            .HasForeignKey(v => v.ResidentId)
            .OnDelete(DeleteBehavior.Cascade);


        modelBuilder.Entity<Invoice>()
            .HasOne(i => i.Creator)
            .WithMany(u => u.CreatedInvoices)
            .HasForeignKey(i => i.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Invoice>()
            .HasOne(i => i.Approver)
            .WithMany()
            .HasForeignKey(i => i.ApprovedBy)
            .OnDelete(DeleteBehavior.SetNull);

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

        modelBuilder.Entity<InvoiceDetail>()
            .HasOne(id => id.ServiceOrder)
            .WithMany()
            .HasForeignKey(id => id.ServiceOrderId)
            .OnDelete(DeleteBehavior.SetNull);

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
            .WithMany(u => u.Requests)
            .HasForeignKey(r => r.ResidentId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Request>()
            .HasOne(r => r.AssignedUser)
            .WithMany(u => u.AssignedRequests)
            .HasForeignKey(r => r.AssignedTo)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Request>()
            .HasOne(r => r.EscalatedToUser)
            .WithMany()
            .HasForeignKey(r => r.EscalatedTo)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<RequestComment>()
            .HasOne(c => c.Request)
            .WithMany(r => r.Comments)
            .HasForeignKey(c => c.RequestId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RequestComment>()
            .HasOne(c => c.Author)
            .WithMany()
            .HasForeignKey(c => c.AuthorId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Announcement>()
            .HasOne(a => a.Creator)
            .WithMany(u => u.Announcements)
            .HasForeignKey(a => a.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<AnnouncementRead>()
            .HasIndex(ar => new { ar.AnnouncementId, ar.UserId })
            .IsUnique();

        modelBuilder.Entity<AnnouncementAttachment>()
            .HasIndex(aa => aa.AnnouncementId);

        modelBuilder.Entity<Announcement>()
            .HasOne(a => a.Creator)
            .WithMany(u => u.Announcements)
            .HasForeignKey(a => a.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<AnnouncementRead>()
            .HasOne(ar => ar.Announcement)
            .WithMany(a => a.AnnouncementReads)
            .HasForeignKey(ar => ar.AnnouncementId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AnnouncementRead>()
            .HasOne(ar => ar.User)
            .WithMany(u => u.AnnouncementReads)
            .HasForeignKey(ar => ar.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AnnouncementAttachment>()
            .HasOne(aa => aa.Announcement)
            .WithMany(a => a.Attachments)
            .HasForeignKey(aa => aa.AnnouncementId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Document>()
            .HasOne(d => d.Uploader)
            .WithMany(u => u.Documents)
            .HasForeignKey(d => d.UploadedBy)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Visitor>()
            .HasOne(v => v.Apartment)
            .WithMany(a => a.Visitors)
            .HasForeignKey(v => v.ApartmentId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Visitor>()
            .HasOne(v => v.RegisteredByUser)
            .WithMany(u => u.RegisteredVisitors)
            .HasForeignKey(v => v.RegisteredBy)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<AmenityBooking>()
            .HasOne(ab => ab.Resident)
            .WithMany(u => u.AmenityBookings)
            .HasForeignKey(ab => ab.ResidentId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<AmenityBooking>()
            .HasIndex(ab => new { ab.AmenityId, ab.BookingDate, ab.StartTime, ab.EndTime });

        modelBuilder.Entity<CommunityCampaign>()
            .HasOne(c => c.Creator)
            .WithMany()
            .HasForeignKey(c => c.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<CommunityCampaignOption>()
            .HasOne(o => o.Campaign)
            .WithMany(c => c.Options)
            .HasForeignKey(o => o.CampaignId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CommunityCampaignResponse>()
            .HasOne(r => r.Campaign)
            .WithMany(c => c.Responses)
            .HasForeignKey(r => r.CampaignId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CommunityCampaignResponse>()
            .HasOne(r => r.Respondent)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<CommunityCampaignResponseOption>()
            .HasOne(ro => ro.Response)
            .WithMany(r => r.SelectedOptions)
            .HasForeignKey(ro => ro.ResponseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CommunityCampaignResponseOption>()
            .HasOne(ro => ro.Option)
            .WithMany(o => o.ResponseSelections)
            .HasForeignKey(ro => ro.OptionId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<ContractMember>()
            .HasOne(cm => cm.Resident)
            .WithMany(u => u.ContractMembers)
            .HasForeignKey(cm => cm.ResidentId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<ServiceOrder>()
            .HasOne(so => so.Resident)
            .WithMany(u => u.ServiceOrders)
            .HasForeignKey(so => so.ResidentId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<FaceAuthHistory>()
            .HasOne(fah => fah.Resident)
            .WithMany()
            .HasForeignKey(fah => fah.ResidentId)
            .OnDelete(DeleteBehavior.Cascade);

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
            .HasOne(v => v.RegisteredByUser)
            .WithMany(u => u.RegisteredVisitors)
            .HasForeignKey(v => v.RegisteredBy)
            .OnDelete(DeleteBehavior.NoAction);



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
        modelBuilder.Entity<InvoiceDetail>()
            .Property(id => id.Amount)
            .HasComputedColumnSql("[Quantity] * [UnitPrice]", stored: true);

        // ServiceOrder configurations
        modelBuilder.Entity<ServiceOrder>()
            .HasIndex(so => so.OrderNumber)
            .IsUnique();

        modelBuilder.Entity<ServiceOrder>()
            .HasOne(so => so.Apartment)
            .WithMany(a => a.ServiceOrders)
            .HasForeignKey(so => so.ApartmentId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<ServiceOrder>()
            .HasOne(so => so.Resident)
            .WithMany(u => u.ServiceOrders)
            .HasForeignKey(so => so.ResidentId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<ServiceOrder>()
            .HasOne(so => so.ServiceType)
            .WithMany(st => st.ServiceOrders)
            .HasForeignKey(so => so.ServiceTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<ServiceOrder>()
            .HasOne(so => so.AssignedStaff)
            .WithMany(u => u.AssignedServiceOrders)
            .HasForeignKey(so => so.AssignedTo)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<ServiceOrder>()
            .HasOne(so => so.CompletedByUser)
            .WithMany(u => u.CompletedServiceOrders)
            .HasForeignKey(so => so.CompletedBy)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<ServiceOrder>()
            .HasOne(so => so.Invoice)
            .WithMany(i => i.ServiceOrders)
            .HasForeignKey(so => so.InvoiceId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<SystemSetting>()
            .HasIndex(s => s.SettingKey)
            .IsUnique();
    }
}

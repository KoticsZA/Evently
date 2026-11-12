using Evently.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Evently.Data
{
    public class EventlyDBContext : IdentityDbContext<AppUser>
    {
        public EventlyDBContext(DbContextOptions<EventlyDBContext> options)
            : base(options)
        {
        }

        public DbSet<Wallet> Wallets { get; set; }

        public DbSet<WalletTransaction> WalletTransactions { get; set; }

        public DbSet<EventOrganizer> EventOrganizers { get; set; }

        public DbSet<Event> Events { get; set; }

        public DbSet<OrganizerEvent> OrganizerEvents { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<TicketType> TicketTypes { get; set; }

        public DbSet<TicketOrder> TicketOrders { get; set; }

        public DbSet<Ticket> Tickets { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);



            builder.Entity<AppUser>(entity =>
            {
                entity.Property(u => u.FirstName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(u => u.LastName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(u => u.ProfilePictureUrl)
                    .HasMaxLength(500);

                entity.Property(u => u.Gender)
                    .HasMaxLength(30);

                entity.Property(u => u.DateOfBirth)
                    .IsRequired();

                entity.HasOne(u => u.Wallet)
                    .WithOne(w => w.User)
                    .HasForeignKey<Wallet>(w => w.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(u => u.OrganizedProfiles)
                    .WithOne(o => o.OwnerUser)
                    .HasForeignKey(o => o.OwnerUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(u => u.Orders)
                    .WithOne(o => o.User)
                    .HasForeignKey(o => o.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });



            builder.Entity<Wallet>(entity =>
            {
                entity.HasKey(w => w.Id);

                entity.Property(w => w.Balance)
                    .HasPrecision(18, 2)
                    .HasDefaultValue(0.00m);

                entity.Property(w => w.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(w => w.UserId)
                    .IsRequired();

                entity.HasIndex(w => w.UserId)
                    .IsUnique();
            });


            builder.Entity<WalletTransaction>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.Property(t => t.Amount)
                    .HasPrecision(18, 2)
                    .IsRequired();

                entity.Property(t => t.Type)
                    .IsRequired();

                entity.Property(t => t.Status)
                    .IsRequired();

                entity.Property(t => t.Reference)
                    .HasMaxLength(150);

                entity.Property(t => t.Description)
                    .HasMaxLength(500);

                entity.Property(t => t.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(t => t.Wallet)
                    .WithMany(w => w.Transactions)
                    .HasForeignKey(t => t.WalletId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(t => t.Reference)
                    .IsUnique()
                    .HasFilter("[Reference] IS NOT NULL");

                entity.ToTable(t =>
                    t.HasCheckConstraint(
                        "CK_WalletTransaction_Amount",
                        "[Amount] > 0"
                    ));
            });


            builder.Entity<EventOrganizer>(entity =>
            {
                entity.HasKey(o => o.Id);

                entity.Property(o => o.Name)
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(o => o.Description)
                    .HasMaxLength(2000)
                    .IsRequired();

                entity.Property(o => o.LogoUrl)
                    .HasMaxLength(500);

                entity.Property(o => o.CoverImageUrl)
                    .HasMaxLength(500);

                entity.Property(o => o.WebsiteUrl)
                    .HasMaxLength(500);

                entity.Property(o => o.ContactEmail)
                    .HasMaxLength(256);

                entity.Property(o => o.ContactNumber)
                    .HasMaxLength(20);

                entity.Property(o => o.OwnerUserId)
                    .IsRequired();

                entity.Property(o => o.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(o => o.OwnerUser)
                    .WithMany(u => u.OrganizedProfiles)
                    .HasForeignKey(o => o.OwnerUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(o => o.OwnerUserId);
            });



            builder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Title)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(e => e.Description)
                    .HasMaxLength(5000)
                    .IsRequired();

                entity.Property(e => e.CoverImageUrl)
                    .HasMaxLength(500);

                entity.Property(e => e.VenueName)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(e => e.StreetAddress)
                    .HasMaxLength(300)
                    .IsRequired();

                entity.Property(e => e.City)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.Province)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.Country)
                    .HasMaxLength(100)
                    .IsRequired()
                    .HasDefaultValue("South Africa");

                entity.Property(e => e.StartDate)
                    .IsRequired();

                entity.Property(e => e.EndDate)
                    .IsRequired();

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.IsPublished)
                    .HasDefaultValue(false);

                entity.HasOne(e => e.Category)
                    .WithMany(c => c.Events)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.ToTable(t =>
                    t.HasCheckConstraint(
                        "CK_Event_EventDates",
                        "[EndDate] >= [StartDate]"
                    ));

                entity.HasIndex(e => new
                {
                    e.City,
                    e.Province,
                    e.StartDate
                });

                entity.HasIndex(e => e.IsPublished);
            });



            builder.Entity<OrganizerEvent>(entity =>
            {
                entity.HasKey(oe => oe.Id);

                entity.HasIndex(oe => new
                {
                    oe.OrganizerId,
                    oe.EventId
                }).IsUnique();

                entity.HasOne(oe => oe.Organizer)
                    .WithMany(o => o.Events)
                    .HasForeignKey(oe => oe.OrganizerId);

                entity.HasOne(oe => oe.Event)
                    .WithMany(e => e.Organizers)
                    .HasForeignKey(oe => oe.EventId);
            });


            builder.Entity<TicketType>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.Property(t => t.Name)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(t => t.Description)
                    .HasMaxLength(500);

                entity.Property(t => t.Price)
                    .HasPrecision(18, 2)
                    .IsRequired();

                entity.Property(t => t.QuantityAvailable)
                    .IsRequired();

                entity.Property(t => t.QuantitySold)
                    .HasDefaultValue(0);

                entity.HasOne(t => t.Event)
                    .WithMany(e => e.TicketTypes)
                    .HasForeignKey(t => t.EventId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(t => new
                {
                    t.EventId,
                    t.Name
                }).IsUnique();

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint(
                        "CK_TicketType_Price",
                        "[Price] >= 0"
                    );

                    t.HasCheckConstraint(
                        "CK_TicketType_Quantity",
                        "[QuantityAvailable] > 0 AND " +
                        "[QuantitySold] >= 0 AND " +
                        "[QuantitySold] <= [QuantityAvailable]"
                    );

                    t.HasCheckConstraint(
                        "CK_TicketType_SalesDates",
                        "[SalesEndDate] IS NULL OR " +
                        "[SalesStartDate] IS NULL OR " +
                        "[SalesEndDate] >= [SalesStartDate]"
                    );
                });
            });


            builder.Entity<TicketOrder>(entity =>
            {
                entity.HasKey(o => o.Id);

                entity.Property(o => o.OrderNumber)
                    .HasMaxLength(32)
                    .IsRequired();

                entity.Property(o => o.TotalAmount)
                    .HasPrecision(18, 2)
                    .IsRequired();

                entity.Property(o => o.Status)
                    .IsRequired();

                entity.Property(o => o.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(o => o.User)
                    .WithMany(u => u.Orders)
                    .HasForeignKey(o => o.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.OrganizerEvent)
                    .WithMany(oe => oe.TicketOrders)
                    .HasForeignKey(t => t.OrganizerEventId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(o => o.Tickets)
                    .WithOne(t => t.TicketOrder)
                    .HasForeignKey(t => t.TicketOrderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(o => o.OrderNumber)
                    .IsUnique();

                entity.HasIndex(o => new
                {
                    o.UserId,
                    o.CreatedAt
                });

                entity.ToTable(t =>
                    t.HasCheckConstraint(
                        "CK_TicketOrder_TotalAmount",
                        "[TotalAmount] >= 0"
                    ));
            });


            builder.Entity<Ticket>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.Property(t => t.TicketNumber)
                    .HasMaxLength(32)
                    .IsRequired();

                entity.Property(t => t.QRCodeValue)
                    .HasMaxLength(64)
                    .IsRequired();

                entity.Property(t => t.Status)
                    .IsRequired();

                entity.Property(t => t.IssuedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(t => t.TicketType)
                    .WithMany(tt => tt.Tickets)
                    .HasForeignKey(t => t.TicketTypeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.TicketOrder)
                    .WithMany(o => o.Tickets)
                    .HasForeignKey(t => t.TicketOrderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(t => t.TicketNumber)
                    .IsUnique();

                entity.HasIndex(t => t.QRCodeValue)
                    .IsUnique();

                entity.HasIndex(t => new
                {
                    t.TicketTypeId,
                    t.Status
                });
            });

            builder.Entity<Event>();
        }
    }
}

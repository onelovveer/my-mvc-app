using Microsoft.EntityFrameworkCore;
using WebApplication1.Models.FitnessClub;

namespace WebApplication1.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

            // 👇 ДОБАВЬТЕ ЭТОТ БЛОК (решает проблему с датами) 👇
    modelBuilder.ConfigureConventions(config =>
    {
        config.Properties<DateTime>()
              .HaveColumnType("timestamp without time zone");
    });
    // 👆 КОНЕЦ БЛОКА 👆

        const string dbo = "dbo";

        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("Users", dbo);
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Username).IsUnique();
            e.Property(x => x.Username).HasMaxLength(100).IsRequired();
            e.Property(x => x.Password).HasMaxLength(100).IsRequired();
            e.Property(x => x.Role).HasMaxLength(20).IsRequired();
        });

        modelBuilder.Entity<Client>(e =>
        {
            e.ToTable("Clients", dbo);
            e.HasKey(x => x.Id);
            e.Property(x => x.FullName).HasMaxLength(200).IsRequired();
            e.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Service>(e =>
        {
            e.ToTable("Services", dbo);
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Type).HasMaxLength(100).IsRequired();
            e.Property(x => x.Price).HasColumnType("decimal(18,2)");
            
            e.Property(x => x.IsSpecialOffer).HasColumnType("boolean");
            
            e.Property(x => x.SpecialDescription).HasMaxLength(500);
        });

        modelBuilder.Entity<Subscription>(e =>
        {
            e.ToTable("Subscriptions", dbo);
            e.HasKey(x => x.Id);
            e.Property(x => x.OriginalPrice).HasColumnType("decimal(18,2)");
            e.Property(x => x.FinalPrice).HasColumnType("decimal(18,2)");
            e.Property(x => x.DiscountPercent).HasColumnType("decimal(5,2)");
            e.HasOne(x => x.Client)
                .WithMany()
                .HasForeignKey(x => x.ClientId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Service)
                .WithMany()
                .HasForeignKey(x => x.ServiceId);
        });
    }
}

using System.Data;
using System.Reflection;
using LibraryEcom.Application.Common.User;
using LibraryEcom.Application.Interfaces.Data;
using LibraryEcom.Application.Settings;
using LibraryEcom.Domain.Common.Base;
using LibraryEcom.Domain.Common.Property;
using LibraryEcom.Domain.Entities;
using LibraryEcom.Domain.Entities.Identity;
using LibraryEcom.Helper;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LibraryEcom.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUserService? currentUserService = null) :
    IdentityDbContext<User, Role, Guid, UserClaims, UserRoles, UserLogin, RoleClaims, UserToken>(options),
    IApplicationDbContext

{
    #region Identity Tables
    public override DbSet<User> Users { get; set; }

    public override DbSet<Role> Roles { get; set; }

    public override DbSet<UserRoles> UserRoles { get; set; }

    public override DbSet<UserClaims> UserClaims { get; set; }

    public override DbSet<RoleClaims> RoleClaims { get; set; }
    
    public DbSet<UserToken> UserToken { get; set; }

    public DbSet<UserLogin> UserLogin { get; set; }
    #endregion

    #region Other Tables
    public DbSet<Book> Books { get; set; }
    
    public DbSet<Author> Authors { get; set; }
    
    public DbSet<BookAuthor> BookAuthors { get; set; }
    
    public DbSet<Announcement> Announcements { get; set; }
    
    public DbSet<Cart> Carts { get; set; }
    
    public DbSet<CartItem> CartItems { get; set; }
    
    public DbSet<Discount> Discounts { get; set; }
    
    public DbSet<Order> Orders { get; set; }
    
    public DbSet<OrderItem> OrderItems { get; set; }
    
    public DbSet<Review> Reviews { get; set; }
    
    public DbSet<WhiteList> WhiteLists { get; set; }

    #endregion

    public override int SaveChanges()
    {
        UpdateLogs();

        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateLogs();

        return await base.SaveChangesAsync(cancellationToken);
    }

    public IDbConnection Connection => Database.GetDbConnection();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var basePath = AppContext.BaseDirectory;

        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        if (!Directory.Exists(basePath))
        {
            throw new DirectoryNotFoundException($"The directory '{basePath}' does not exist.");
        }
            
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
            .Build();

        var databaseSettings = new DatabaseSettings();

        configuration.GetSection("DatabaseSettings").Bind(databaseSettings);

        var connectionString = databaseSettings.DbProvider == Constants.DbProviderKeys.Npgsql
            ? databaseSettings.NpgSqlConnectionString
            : databaseSettings.SqlServerConnectionString;

        // optionsBuilder.UseLazyLoadingProxies();
        
        optionsBuilder = optionsBuilder.UseDatabase(databaseSettings.DbProvider, connectionString!);

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);

        #region Identity Entities Naming Conventions

        builder.Entity<User>().ToTable("Users");
        builder.Entity<Role>().ToTable("Roles");
        builder.Entity<UserToken>().ToTable("Tokens");
        builder.Entity<UserRoles>().ToTable("UserRoles");
        builder.Entity<RoleClaims>().ToTable("RoleClaims");
        builder.Entity<UserClaims>().ToTable("UserClaims");
        builder.Entity<UserLogin>().ToTable("LoginAttempts");

        #endregion

        #region User Model Configurations

        builder.Entity<Book>()
            .HasOne(t => t.CreatedUser)
            .WithMany()
            .HasForeignKey(t => t.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Book>()
            .HasOne(t => t.LastModifiedUser)
            .WithMany()
            .HasForeignKey(t => t.LastModifiedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Book>()
            .HasOne(t => t.DeletedUser)
            .WithMany()
            .HasForeignKey(t => t.DeletedBy)
            .OnDelete(DeleteBehavior.Restrict);

        #endregion


        #region Mappings

        builder.Entity<BookAuthor>().HasKey(ba => new { ba.BookId, ba.AuthorId });

        builder.Entity<BookAuthor>()
            .HasOne(ba => ba.Book)
            .WithMany()
            .HasForeignKey(ba => ba.BookId);

        builder.Entity<BookAuthor>()
            .HasOne(ba => ba.Author)
            .WithMany()
            .HasForeignKey(ba => ba.AuthorId);

        builder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany()
            .HasForeignKey(oi => oi.OrderId);

        builder.Entity<OrderItem>()
            .HasOne(oi => oi.Book)
            .WithMany()
            .HasForeignKey(oi => oi.BookId);

        builder.Entity<CartItem>()
            .HasOne(ci => ci.Cart)
            .WithMany()
            .HasForeignKey(ci => ci.CartId);

        builder.Entity<CartItem>()
            .HasOne(oi => oi.Book)
            .WithMany()
            .HasForeignKey(oi => oi.BookId);

        #endregion


    }

    private void UpdateLogs()
{
    var entries = ChangeTracker.Entries()
        .Where(e => e is { Entity: BaseEntity<Guid> or BaseEntity<string>, State: EntityState.Added or EntityState.Modified or EntityState.Deleted });

    if (currentUserService == null || currentUserService.GetUserId == Guid.Empty)
    {
        // Skip logging if no user is available (typically during data seeding)
        return;
    }

    var dateTime = DateTime.UtcNow;
    var userId = currentUserService.GetUserId;

    foreach (var entry in entries)
    {
        UpdateEntityLog(entry.Entity, entry.State, userId, dateTime);
    }
}


    private static void UpdateEntityLog<TEntity>(TEntity entity, EntityState state, Guid userId, DateTime dateTime)
        where TEntity : class
    {
        switch (state)
        {
            case EntityState.Added:
            {
                switch (entity)
                {
                    case BaseEntity<Guid> guidEntity:
                        guidEntity.CreatedAt = dateTime;
                        if (guidEntity.CreatedBy == Guid.Empty) guidEntity.CreatedBy = userId;
                        break;
                    case BaseEntity<string> stringEntity:
                        stringEntity.CreatedAt = dateTime;
                        if (stringEntity.CreatedBy == Guid.Empty) stringEntity.CreatedBy = userId;
                        break;
                }
                break;
            }

            case EntityState.Modified:
            {
                switch (entity)
                {
                    case BaseEntity<Guid> guidEntity:
                        guidEntity.LastModifiedAt = dateTime;
                        if (guidEntity.LastModifiedBy == Guid.Empty) guidEntity.LastModifiedBy = userId;
                        break;
                    case BaseEntity<string> stringEntity:
                        stringEntity.LastModifiedAt = dateTime;
                        if (stringEntity.LastModifiedBy == Guid.Empty) stringEntity.LastModifiedBy = userId;
                        break;
                }
                break;
            }

            case EntityState.Deleted:
            {
                switch (entity)
                {
                    case BaseEntity<Guid> guidEntity:
                        guidEntity.DeletedAt = dateTime;
                        if (guidEntity.DeletedBy == Guid.Empty) guidEntity.DeletedBy = userId;
                        break;
                    case BaseEntity<string> stringEntity:
                        stringEntity.DeletedAt = dateTime;
                        if (stringEntity.DeletedBy == Guid.Empty) stringEntity.DeletedBy = userId;
                        break;
                }
                break;
            }

            case EntityState.Detached:
            case EntityState.Unchanged:
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }
}
using System.Data;
using LibraryEcom.Application.Common.Service;
using LibraryEcom.Domain.Entities;
using LibraryEcom.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;


namespace LibraryEcom.Application.Interfaces.Data;

public interface IApplicationDbContext:IScopedService
{
    #region Identity
    DbSet<User> Users { get; set; }

    DbSet<Role> Roles { get; set; }

    DbSet<UserRoles> UserRoles { get; set; }

    DbSet<UserClaims> UserClaims { get; set; }

    DbSet<RoleClaims> RoleClaims { get; set; }

    DbSet<UserToken> UserToken { get; set; }

    DbSet<UserLogin> UserLogin { get; set; }
    #endregion
    
    DbSet<Book> Books { get; set; }
    
    DbSet<Author> Authors { get; set; }
    
    DbSet<BookAuthor> BookAuthors { get; set; }
    
    DbSet<Announcement> Announcements { get; set; }
    
    DbSet<Cart> Carts { get; set; }
    
    DbSet<CartItem> CartItems { get; set; }
    
    DbSet<Discount> Discounts { get; set; }
    
    DbSet<Order> Orders { get; set; }
    
    DbSet<OrderItem> OrderItems { get; set; }
    
    DbSet<Review> Reviews { get; set; }
    
    DbSet<WhiteList> WhiteLists { get; set; }
    
    
    #region Functions
    int SaveChanges();

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    #endregion

    #region Properties
    IDbConnection Connection { get; }
    #endregion
    
}
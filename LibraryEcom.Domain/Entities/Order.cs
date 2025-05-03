using System.ComponentModel.DataAnnotations.Schema;
using LibraryEcom.Domain.Common.Base;
using LibraryEcom.Domain.Entities.Identity;

namespace LibraryEcom.Domain.Entities;

public class Order: BaseEntity<Guid>
{
    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }
    
    public DateTime OrderDate { get; set; }
    
    public DateTime? CompletionDate { get; set; }
    
    public string Status { get; set; }
    
    public decimal Subtotal { get; set; }
    
    public decimal DiscountAmount { get; set; }
    
    public decimal LoyaltyDiscountAmount { get; set; }
    
    public decimal TotalAmount { get; set; }
    
    public string ClaimCode { get; set; }
    
    public DateTime? ClaimExpiry { get; set; }
    
    public bool IsClaimed { get; set; }

    public virtual User User { get; set; }
    
    
}
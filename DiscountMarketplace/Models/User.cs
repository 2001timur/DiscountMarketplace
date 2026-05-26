using System;
using System.Collections.Generic;

namespace DiscountMarketplace.Models;

public partial class User
{
    public int UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? Phone { get; set; }

    public string Role { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Business> Businesses { get; set; } = new List<Business>();

    public virtual ICollection<Coupon> Coupons { get; set; } = new List<Coupon>();
}

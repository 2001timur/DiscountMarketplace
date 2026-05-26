using System;
using System.Collections.Generic;

namespace DiscountMarketplace.Models;

public partial class Coupon
{
    public int CouponId { get; set; }

    public int DealId { get; set; }

    public int UserId { get; set; }

    public string Code { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime ExpirationDate { get; set; }

    public DateTime? UsedAt { get; set; }

    public virtual Deal Deal { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}

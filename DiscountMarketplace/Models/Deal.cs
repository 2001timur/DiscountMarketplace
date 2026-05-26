using System;
using System.Collections.Generic;

namespace DiscountMarketplace.Models;

public partial class Deal
{
    public int DealId { get; set; }

    public int BusinessId { get; set; }

    public int CategoryId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public decimal OriginalPrice { get; set; }

    public int DiscountPercent { get; set; }

    public decimal DiscountPrice { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int CouponLimit { get; set; }

    public string? ImageUrl { get; set; }

    public bool IsActive { get; set; }

    public virtual Business Business { get; set; } = null!;

    public virtual Servicecategory Category { get; set; } = null!;

    public virtual ICollection<Coupon> Coupons { get; set; } = new List<Coupon>();
}

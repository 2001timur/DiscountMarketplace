using System;
using System.Collections.Generic;

namespace DiscountMarketplace.Models;

public partial class Business
{
    public int BusinessId { get; set; }

    public int OwnerUserId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? LogoUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Deal> Deals { get; set; } = new List<Deal>();

    public virtual User OwnerUser { get; set; } = null!;
}

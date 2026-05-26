using System;
using System.Collections.Generic;

namespace DiscountMarketplace.Models;

public partial class Servicecategory
{
    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? IconUrl { get; set; }

    public virtual ICollection<Deal> Deals { get; set; } = new List<Deal>();
}

using DiscountMarketplace.Interfaces;
using DiscountMarketplace.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscountMarketplace.Services;

public class DealService: IDealService
{
    private readonly MarketplaceContext _context;

    public DealService(MarketplaceContext context)
    {
        _context = context;
    }

    public List<Deal> GetAll()
    {
        return _context.Deals
            .Include(d => d.Business)
            .Include(d => d.Category)
            .Include(d => d.Coupons)
            .ToList();
    }

    public Deal? GetById(int id)
    {
        return _context.Deals
            .Include(d => d.Business)
            .Include(d => d.Category)
            .Include(d => d.Coupons)
            .FirstOrDefault(d => d.DealId == id);
    }

    public List<Deal> GetActive()
    {
        return _context.Deals
            .Include(d => d.Business)
            .Include(d => d.Category)
            .Where(d => d.IsActive == true &&
                        d.StartDate <= DateTime.Now &&
                        d.EndDate >= DateTime.Now)
            .ToList();
    }

    public List<Deal> GetByBusiness(int businessId)
    {
        return _context.Deals
            .Include(d => d.Category)
            .Where(d => d.BusinessId == businessId)
            .ToList();
    }

    public List<Deal> GetByCategory(int categoryId)
    {
        return _context.Deals
            .Include(d => d.Business)
            .Where(d => d.CategoryId == categoryId)
            .ToList();
    }

    public void Create(Deal deal)
    {
        deal.DiscountPrice = CalculateDiscountPrice(deal.OriginalPrice, deal.DiscountPercent);
        deal.IsActive = true;

        _context.Deals.Add(deal);
        _context.SaveChanges();
    }

    public void Update(Deal deal)
    {
        var existing = _context.Deals.FirstOrDefault(d => d.DealId == deal.DealId);

        if (existing != null)
        {
            existing.BusinessId = deal.BusinessId;
            existing.CategoryId = deal.CategoryId;
            existing.Title = deal.Title;
            existing.Description = deal.Description;
            existing.OriginalPrice = deal.OriginalPrice;
            existing.DiscountPercent = deal.DiscountPercent;
            existing.DiscountPrice = CalculateDiscountPrice(deal.OriginalPrice, deal.DiscountPercent);
            existing.StartDate = deal.StartDate;
            existing.EndDate = deal.EndDate;
            existing.CouponLimit = deal.CouponLimit;
            existing.ImageUrl = deal.ImageUrl;
            existing.IsActive = deal.IsActive;

            _context.SaveChanges();
        }
    }

    public void Delete(int id)
    {
        var deal = _context.Deals
            .Include(d => d.Coupons)
            .FirstOrDefault(d => d.DealId == id);

        if (deal != null)
        {
            if (deal.CouponLimit == 0)
            {
                _context.Deals.Remove(deal);
                _context.SaveChanges();
            }
        }
        else
        {
            throw new Exception("Акцію не знайдено.");
        }
    }

    private decimal CalculateDiscountPrice(decimal originalPrice, int discountPercent)
    {
        return originalPrice - (originalPrice * discountPercent / 100);
    }
}
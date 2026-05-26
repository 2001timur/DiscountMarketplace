using DiscountMarketplace.Interfaces;
using DiscountMarketplace.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscountMarketplace.Services;

public class CouponService: ICouponService
{
    private readonly MarketplaceContext _context;

    public CouponService(MarketplaceContext context)
    {
        _context = context;
    }

    public List<Coupon> GetAll()
    {
        return _context.Coupons
        .Include(c => c.Deal)
        .Include(c => c.User)
        .OrderBy(c => c.CreatedAt) 
        .ToList();
    }

    public Coupon? GetById(int id)
    {
        return _context.Coupons
            .Include(c => c.Deal)
            .Include(c => c.User)
            .FirstOrDefault(c => c.CouponId == id);
    }

    public List<Coupon> GetByUser(int userId)
    {
        return _context.Coupons
            .Include(c => c.Deal)
            .Where(c => c.UserId == userId)
            .ToList();
    }

    public List<Coupon> GetByDeal(int dealId)
    {
        return _context.Coupons
            .Include(c => c.User)
            .Where(c => c.DealId == dealId)
            .ToList();
    }

    public void Create(Coupon coupon)
    {
        var deal = _context.Deals.FirstOrDefault(d => d.DealId == coupon.DealId);

        if (deal != null)
        {
            
            if (deal.CouponLimit > 0)
            {
                deal.CouponLimit -= 1;

                coupon.CreatedAt = DateTime.Now;
                coupon.Status = "Active";

                _context.Coupons.Add(coupon);
                _context.SaveChanges();
            }
            else
            {
                throw new Exception("Купони закінчилися.");
            }
        }
    }

    public void MarkAsUsed(int couponId)
    {
        var coupon = _context.Coupons.FirstOrDefault(c => c.CouponId == couponId);

        if (coupon != null && coupon.Status == "Active")
        {
            coupon.Status = "Used";
            coupon.UsedAt = DateTime.Now;

            _context.SaveChanges();
        }
    }

    public void ExpireOldCoupons()
    {
        var expiredCoupons = _context.Coupons
            .Where(c => c.Status == "Active" && c.ExpirationDate < DateTime.Now)
            .ToList();

        foreach (var coupon in expiredCoupons)
        {
            coupon.Status = "Expired";
        }

        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var coupon = _context.Coupons.FirstOrDefault(c => c.CouponId == id);

        if (coupon != null)
        {
            if (coupon.Status == "Used" || coupon.Status == "Expired")
            {
                _context.Coupons.Remove(coupon);
                _context.SaveChanges();
            }
            else
            {
                throw new Exception("Можна видаляти тільки використані купони. Поточний статус: " + coupon.Status);
            }
        }
        else
        {
            throw new Exception("Купон не знайдено.");
        }
    }
}

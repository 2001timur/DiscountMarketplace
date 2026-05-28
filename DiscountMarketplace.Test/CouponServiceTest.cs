using DiscountMarketplace.Models;
using DiscountMarketplace.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscountMarketplace.Test;

[TestClass]
public class CouponServiceTest
{
    private MarketplaceContext _context = null!;
    private CouponService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<MarketplaceContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new MarketplaceContext(options);
        _service = new CouponService(_context);
    }

    private User CreateUser()
    {
        return new User
        {
            FirstName = "Test",
            LastName = "User",
            Email = "user@test.com",
            PasswordHash = "123",
            Role = "User",
            CreatedAt = DateTime.Now
        };
    }

    private Deal CreateDeal(int businessId, int categoryId)
    {
        return new Deal
        {
            BusinessId = businessId,
            CategoryId = categoryId,
            Title = "Deal",
            OriginalPrice = 100,
            DiscountPercent = 10,
            DiscountPrice = 90,
            CouponLimit = 5,
            StartDate = DateTime.Now.AddDays(-1),
            EndDate = DateTime.Now.AddDays(10),
            IsActive = true
        };
    }

    private Coupon CreateCoupon(int dealId, int userId)
    {
        return new Coupon
        {
            DealId = dealId,
            UserId = userId,
            Code = "CODE-001",
            Status = "Active",
            CreatedAt = DateTime.Now,
            ExpirationDate = DateTime.Now.AddDays(10)
        };
    }

    private Business CreateBusiness(int userId)
    {
        return new Business
        {
            OwnerUserId = userId,
            Name = "Biz",
            CreatedAt = DateTime.Now
        };
    }

    private Servicecategory CreateCategory()
    {
        return new Servicecategory
        {
            Name = "Cat"
        };
    }

    private (User user, Deal deal) Seed()
    {
        var user = CreateUser();
        _context.Users.Add(user);

        var business = CreateBusiness(user.UserId);
        var category = CreateCategory();

        _context.Businesses.Add(business);
        _context.Servicecategories.Add(category);

        _context.SaveChanges();

        var deal = CreateDeal(business.BusinessId, category.CategoryId);
        _context.Deals.Add(deal);
        _context.SaveChanges();

        return (user, deal);
    }

    [TestMethod]
    public void Create_ShouldAddCoupon_AndDecreaseLimit()
    {
        var (user, deal) = Seed();

        var coupon = CreateCoupon(deal.DealId, user.UserId);

        _service.Create(coupon);

        var created = _context.Coupons.FirstOrDefault();

        Assert.IsNotNull(created);
        Assert.AreEqual("Active", created.Status);

        var updatedDeal = _context.Deals.First();

        Assert.AreEqual(4, updatedDeal.CouponLimit); // 5 → 4
    }

    [TestMethod]
    public void Create_ShouldThrow_WhenCouponLimitIsZero()
    {
        var (user, deal) = Seed();

        deal.CouponLimit = 0;
        _context.SaveChanges();

        var coupon = CreateCoupon(deal.DealId, user.UserId);

        Assert.ThrowsException<Exception>(() => _service.Create(coupon));
    }

    [TestMethod]
    public void GetAll_ShouldReturnCoupons()
    {
        var (user, deal) = Seed();

        _context.Coupons.Add(CreateCoupon(deal.DealId, user.UserId));
        _context.SaveChanges();

        var result = _service.GetAll();

        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void GetById_ShouldReturnCoupon()
    {
        var (user, deal) = Seed();

        var coupon = CreateCoupon(deal.DealId, user.UserId);
        _context.Coupons.Add(coupon);
        _context.SaveChanges();

        var result = _service.GetById(coupon.CouponId);

        Assert.IsNotNull(result);
        Assert.AreEqual(coupon.Code, result.Code);
    }

    [TestMethod]
    public void GetByUser_ShouldReturnCoupons()
    {
        var (user, deal) = Seed();

        _context.Coupons.Add(CreateCoupon(deal.DealId, user.UserId));
        _context.SaveChanges();

        var result = _service.GetByUser(user.UserId);

        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void GetByDeal_ShouldReturnCoupons()
    {
        var (user, deal) = Seed();

        _context.Coupons.Add(CreateCoupon(deal.DealId, user.UserId));
        _context.SaveChanges();

        var result = _service.GetByDeal(deal.DealId);

        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void MarkAsUsed_ShouldChangeStatusToUsed()
    {
        var (user, deal) = Seed();

        var coupon = CreateCoupon(deal.DealId, user.UserId);
        _context.Coupons.Add(coupon);
        _context.SaveChanges();

        _service.MarkAsUsed(coupon.CouponId);

        var updated = _context.Coupons.First();

        Assert.AreEqual("Used", updated.Status);
        Assert.IsNotNull(updated.UsedAt);
    }

    [TestMethod]
    public void ExpireOldCoupons_ShouldMarkAsExpired()
    {
        var (user, deal) = Seed();

        var coupon = CreateCoupon(deal.DealId, user.UserId);
        coupon.ExpirationDate = DateTime.Now.AddDays(-1); // expired

        _context.Coupons.Add(coupon);
        _context.SaveChanges();

        _service.ExpireOldCoupons();

        var updated = _context.Coupons.First();

        Assert.AreEqual("Expired", updated.Status);
    }

    [TestMethod]
    public void Delete_ShouldRemoveUsedCoupon()
    {
        var (user, deal) = Seed();

        var coupon = CreateCoupon(deal.DealId, user.UserId);
        coupon.Status = "Used";

        _context.Coupons.Add(coupon);
        _context.SaveChanges();

        _service.Delete(coupon.CouponId);

        var result = _context.Coupons.FirstOrDefault(c => c.CouponId == coupon.CouponId);

        Assert.IsNull(result);
    }

    [TestMethod]
    public void Delete_ShouldThrow_WhenCouponIsActive()
    {
        var (user, deal) = Seed();

        var coupon = CreateCoupon(deal.DealId, user.UserId);

        _context.Coupons.Add(coupon);
        _context.SaveChanges();

        Assert.ThrowsException<Exception>(() => _service.Delete(coupon.CouponId));
    }

    [TestMethod]
    public void Delete_ShouldThrow_WhenNotFound()
    {
        Assert.ThrowsException<Exception>(() => _service.Delete(999));
    }
}
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
public class DealServiceTest
{
    private MarketplaceContext _context = null!;
    private DealService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<MarketplaceContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new MarketplaceContext(options);
        _service = new DealService(_context);
    }

    private User CreateUser()
    {
        return new User
        {
            FirstName = "Owner",
            LastName = "Test",
            Email = "owner@test.com",
            PasswordHash = "123456",
            Role = "BusinessOwner",
            CreatedAt = DateTime.Now
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

    private Deal CreateDeal(int businessId, int categoryId)
    {
        return new Deal
        {
            BusinessId = businessId,
            CategoryId = categoryId,
            Title = "Deal 1",
            Description = "Desc",
            OriginalPrice = 100,
            DiscountPercent = 20,
            StartDate = DateTime.Now.AddDays(-1),
            EndDate = DateTime.Now.AddDays(10),
            CouponLimit = 10,
            ImageUrl = "img.png",
            IsActive = true
        };
    }


    [TestMethod]
    public void Create_ShouldCalculateDiscountPrice()
    {
        var user = CreateUser();
        _context.Users.Add(user);
        _context.SaveChanges();

        var business = CreateBusiness(user.UserId);
        _context.Businesses.Add(business);

        var category = CreateCategory();
        _context.Servicecategories.Add(category);

        _context.SaveChanges();

        var deal = CreateDeal(business.BusinessId, category.CategoryId);

        _service.Create(deal);

        var created = _context.Deals.FirstOrDefault();

        Assert.IsNotNull(created);
        Assert.AreEqual(80, created.DiscountPrice); // 100 - 20%
        Assert.IsTrue(created.IsActive);
    }

    [TestMethod]
    public void GetAll_ShouldReturnDeals()
    {
        SeedDeal();

        var result = _service.GetAll();

        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void GetById_ShouldReturnDeal()
    {
        var deal = SeedDeal();

        var result = _service.GetById(deal.DealId);

        Assert.IsNotNull(result);
        Assert.AreEqual(deal.Title, result.Title);
    }

    [TestMethod]
    public void GetActive_ShouldReturnOnlyActiveDeals()
    {
        var deal = SeedDeal();
        deal.IsActive = true;

        _context.SaveChanges();

        var result = _service.GetActive();

        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void GetByBusiness_ShouldReturnCorrectDeals()
    {
        var deal = SeedDeal();

        var result = _service.GetByBusiness(deal.BusinessId);

        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void GetByCategory_ShouldReturnCorrectDeals()
    {
        var deal = SeedDeal();

        var result = _service.GetByCategory(deal.CategoryId);

        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void Update_ShouldModifyDeal()
    {
        var deal = SeedDeal();

        deal.Title = "Updated";
        deal.DiscountPercent = 50;

        _service.Update(deal);

        var updated = _context.Deals.First();

        Assert.AreEqual("Updated", updated.Title);
        Assert.AreEqual(50, updated.DiscountPercent);
        Assert.AreEqual(50, updated.DiscountPrice); 
    }

    [TestMethod]
    public void Delete_ShouldNotRemoveDeal_WhenCouponsAreLessThanLimit()
    {
        var deal = SeedDeal();

        _service.Delete(deal.DealId);

        var result = _context.Deals.FirstOrDefault(d => d.DealId == deal.DealId);

        Assert.IsNotNull(result); // 
    }


 
    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void Delete_ShouldThrow_WhenDealNotFound()
    {
        _service.Delete(999);
    }


    [TestMethod]
    public void Delete_ShouldRemoveDeal_WhenCouponLimitIsZero()
    {
        var deal = SeedDeal();
        deal.CouponLimit = 0;

        _context.SaveChanges();

        _service.Delete(deal.DealId);

        var result = _context.Deals.FirstOrDefault(d => d.DealId == deal.DealId);

        Assert.IsNull(result);
    }


    private Deal SeedDeal()
    {
        var user = CreateUser();
        _context.Users.Add(user);

        var business = CreateBusiness(user.UserId);
        _context.Businesses.Add(business);

        var category = CreateCategory();
        _context.Servicecategories.Add(category);

        _context.SaveChanges();

        var deal = CreateDeal(business.BusinessId, category.CategoryId);

        _context.Deals.Add(deal);
        _context.SaveChanges();

        return deal;
    }
}
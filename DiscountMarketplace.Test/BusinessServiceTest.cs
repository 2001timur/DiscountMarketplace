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
public class BusinessServiceTest
{
    private MarketplaceContext _context = null!;
    private BusinessService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<MarketplaceContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new MarketplaceContext(options);
        _service = new BusinessService(_context);
    }

        private User CreateUser(string email = "owner@test.com")
    {
        return new User
        {
            FirstName = "Owner",
            LastName = "User",
            Email = email,
            PasswordHash = "password123",
            Role = "BusinessOwner",
            CreatedAt = DateTime.Now
        };
    }

    private Business CreateBusiness(int ownerId, string name = "Test Business")
    {
        return new Business
        {
            OwnerUserId = ownerId,
            Name = name,
            Description = "Test description",
            Address = "Lviv",
            Phone = "123456789",
            Email = "business@test.com",
            LogoUrl = "logo.png",
            CreatedAt = DateTime.Now
        };
    }
      
    [TestMethod]
    public void Create_ShouldAddBusiness()
    {
        var user = CreateUser();
        _context.Users.Add(user);
        _context.SaveChanges();

        var business = CreateBusiness(user.UserId);

        _service.Create(business);

        var created = _context.Businesses.FirstOrDefault(b => b.Name == "Test Business");

        Assert.IsNotNull(created);
        Assert.AreEqual("Test Business", created.Name);
        Assert.AreEqual(user.UserId, created.OwnerUserId);
    }

    [TestMethod]
    public void GetAll_ShouldReturnAllBusinesses()
    {
        var user = CreateUser();
        _context.Users.Add(user);
        _context.SaveChanges();

        _context.Businesses.Add(CreateBusiness(user.UserId, "B1"));
        _context.Businesses.Add(CreateBusiness(user.UserId, "B2"));
        _context.SaveChanges();

        var result = _service.GetAll();

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void GetById_ShouldReturnCorrectBusiness()
    {
        var user = CreateUser();
        _context.Users.Add(user);
        _context.SaveChanges();

        var business = CreateBusiness(user.UserId, "Single");
        _context.Businesses.Add(business);
        _context.SaveChanges();

        var result = _service.GetById(business.BusinessId);

        Assert.IsNotNull(result);
        Assert.AreEqual("Single", result.Name);
    }

    [TestMethod]
    public void Update_ShouldModifyBusiness()
    {
        var user = CreateUser();
        _context.Users.Add(user);
        _context.SaveChanges();

        var business = CreateBusiness(user.UserId, "Old Name");
        _context.Businesses.Add(business);
        _context.SaveChanges();

        var existing = _context.Businesses.First();

        existing.Name = "Updated Name";
        existing.Description = "Updated desc";
        existing.Address = "Kyiv";

        _service.Update(existing);

        var updated = _context.Businesses.First();

        Assert.AreEqual("Updated Name", updated.Name);
        Assert.AreEqual("Kyiv", updated.Address);
        Assert.AreEqual("Updated desc", updated.Description);
    }

    [TestMethod]
    public void Delete_ShouldRemoveBusiness()
    {
        var user = CreateUser();
        _context.Users.Add(user);
        _context.SaveChanges();

        var business = CreateBusiness(user.UserId);
        _context.Businesses.Add(business);
        _context.SaveChanges();

        _service.Delete(business.BusinessId);

        var deleted = _context.Businesses.FirstOrDefault(b => b.BusinessId == business.BusinessId);

        Assert.IsNull(deleted);
    }
}
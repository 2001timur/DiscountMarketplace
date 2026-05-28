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
public class ServiceCategoryServiceTest
{
    private MarketplaceContext _context = null!;
    private ServiceCategoryService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<MarketplaceContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new MarketplaceContext(options);
        _service = new ServiceCategoryService(_context);
    }

    private Servicecategory CreateCategory(string name = "Category 1")
    {
        return new Servicecategory
        {
            Name = name,
            Description = "Test description",
            IconUrl = "icon.png"
        };
    }

    [TestMethod]
    public void Create_ShouldAddCategory()
    {
        var category = CreateCategory();

        _service.Create(category);

        var created = _context.Servicecategories.FirstOrDefault(c => c.Name == "Category 1");

        Assert.IsNotNull(created);
        Assert.AreEqual("Category 1", created.Name);
    }

    [TestMethod]
    public void GetAll_ShouldReturnAllCategories()
    {
        _context.Servicecategories.Add(CreateCategory("C1"));
        _context.Servicecategories.Add(CreateCategory("C2"));
        _context.SaveChanges();

        var result = _service.GetAll();

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void GetById_ShouldReturnCorrectCategory()
    {
        var category = CreateCategory("Single");

        _context.Servicecategories.Add(category);
        _context.SaveChanges();

        var result = _service.GetById(category.CategoryId);

        Assert.IsNotNull(result);
        Assert.AreEqual("Single", result.Name);
    }

    [TestMethod]
    public void Update_ShouldModifyCategory()
    {
        var category = CreateCategory("Old");
        _context.Servicecategories.Add(category);
        _context.SaveChanges();

        var existing = _context.Servicecategories.First();

        existing.Name = "Updated";
        existing.Description = "Updated desc";
        existing.IconUrl = "new.png";

        _service.Update(existing);

        var updated = _context.Servicecategories.First();

        Assert.AreEqual("Updated", updated.Name);
        Assert.AreEqual("Updated desc", updated.Description);
        Assert.AreEqual("new.png", updated.IconUrl);
    }

    [TestMethod]
    public void Delete_ShouldRemoveCategory()
    {
        var category = CreateCategory("Delete me");

        _context.Servicecategories.Add(category);
        _context.SaveChanges();

        _service.Delete(category.CategoryId);

        var deleted = _context.Servicecategories.FirstOrDefault(c => c.CategoryId == category.CategoryId);

        Assert.IsNull(deleted);
    }
}

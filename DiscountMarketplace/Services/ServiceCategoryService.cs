using DiscountMarketplace.Interfaces;
using DiscountMarketplace.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscountMarketplace.Services;


public class ServiceCategoryService: IServiceCategoryService
{
    private readonly MarketplaceContext _context;

    public ServiceCategoryService(MarketplaceContext context)
    {
        _context = context;
    }

    public List<Servicecategory> GetAll()
    {
        return _context.Servicecategories
            .Include(c => c.Deals)
            .ToList();
    }

    public Servicecategory? GetById(int id)
    {
        return _context.Servicecategories
            .Include(c => c.Deals)
            .FirstOrDefault(c => c.CategoryId == id);
    }

    public void Create(Servicecategory category)
    {
        _context.Servicecategories.Add(category);
        _context.SaveChanges();
    }

    public void Update(Servicecategory category)
    {
        var existing = _context.Servicecategories.FirstOrDefault(c => c.CategoryId == category.CategoryId);

        if (existing != null)
        {
            existing.Name = category.Name;
            existing.Description = category.Description;
            existing.IconUrl = category.IconUrl;

            _context.SaveChanges();
        }
    }

    public void Delete(int id)
    {
        var category = _context.Servicecategories.FirstOrDefault(c => c.CategoryId == id);

        if (category != null)
        {
            _context.Servicecategories.Remove(category);
            _context.SaveChanges();
        }
    }
}
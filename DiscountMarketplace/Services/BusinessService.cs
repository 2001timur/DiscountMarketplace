using DiscountMarketplace.Interfaces;
using DiscountMarketplace.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscountMarketplace.Services;

public class BusinessService: IBusinessService
{
    private readonly MarketplaceContext _context;

    public BusinessService(MarketplaceContext context)
    {
        _context = context;
    }

    public List<Business> GetAll()
    {
        return _context.Businesses
            .Include(b => b.Deals)
            .Include(b => b.OwnerUser)
            .ToList();
    }

    public Business? GetById(int id)
    {
        return _context.Businesses
            .Include(b => b.Deals)
            .Include(b => b.OwnerUser)
            .FirstOrDefault(b => b.BusinessId == id);
    }

    public void Create(Business business)
    {
        business.CreatedAt = DateTime.Now;
        _context.Businesses.Add(business);
        _context.SaveChanges();
    }

    public void Update(Business business)
    {
      
        var existing = _context.Businesses.FirstOrDefault(b => b.BusinessId == business.BusinessId);

        if (existing != null)
        {
            existing.Name = business.Name;
            existing.Description = business.Description;
            existing.Address = business.Address;
            existing.Phone = business.Phone;
            existing.Email = business.Email;
            existing.LogoUrl = business.LogoUrl;
            existing.OwnerUserId = business.OwnerUserId;

            _context.SaveChanges();
        }
    }

    public void Delete(int id)
    {
        var business = _context.Businesses.Find(id);
        if (business != null)
        {
            _context.Businesses.Remove(business);
            _context.SaveChanges();
        }
    }
}
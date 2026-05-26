using DiscountMarketplace.Interfaces;
using DiscountMarketplace.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscountMarketplace.Services;

public class UserService: IUserService
{
    private readonly MarketplaceContext _context;

    public UserService(MarketplaceContext context)
    {
        _context = context;
    }

    public List<User> GetAll()
    {
        return _context.Users
            .Include(u => u.Businesses)
            .Include(u => u.Coupons)
            .ToList();
    }

     public User? GetById(int id)
    {
        return _context.Users
            .Include(u => u.Businesses)
            .Include(u => u.Coupons)
            .FirstOrDefault(u => u.UserId == id);
    }

    public void Create(User user)
    {
        user.CreatedAt = DateTime.Now;

        _context.Users.Add(user);
        _context.SaveChanges();
    }

    public void Update(User user)
    {
        var existingUser = _context.Users.FirstOrDefault(u => u.UserId == user.UserId);

        if (existingUser != null)
        {
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;
            existingUser.Phone = user.Phone;
            existingUser.Role = user.Role;

            _context.SaveChanges();
        }
    }

    public void Delete(int id)
    {
        var user = _context.Users.FirstOrDefault(u => u.UserId == id);

        if (user != null)
        {
            _context.Users.Remove(user);
            _context.SaveChanges();
        }
    }

    public User? GetByEmail(string email)
    {
        return _context.Users
            .Include(u => u.Businesses)
            .Include(u => u.Coupons)
            .FirstOrDefault(u => u.Email == email);
    }

    public bool IsAdmin(int userId)
    {
        var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
        return user?.Role == "Admin";
    }

    public bool IsBusinessOwner(int userId)
    {
        var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
        return user?.Role == "BusinessOwner";
    }
}
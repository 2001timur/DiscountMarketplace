using DiscountMarketplace.Models;

namespace DiscountMarketplace.Interfaces;

public interface IUserService
{
    List<User> GetAll();
    User? GetById(int id);
    void Create(User user);
    void Update(User user);
    void Delete(int id);
    User? GetByEmail(string email);
    bool IsAdmin(int userId);
    bool IsBusinessOwner(int userId);
    User? Authenticate(string email, string password);
}
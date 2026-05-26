using DiscountMarketplace.Models;

namespace DiscountMarketplace.Interfaces;

public interface IBusinessService
{
    List<Business> GetAll();
    Business? GetById(int id);
    void Create(Business business);
    void Update(Business business);
    void Delete(int id);
}
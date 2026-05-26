using DiscountMarketplace.Models;

namespace DiscountMarketplace.Interfaces;

public interface IDealService
{
    List<Deal> GetAll();
    Deal? GetById(int id);
    List<Deal> GetActive();
    List<Deal> GetByBusiness(int businessId);
    List<Deal> GetByCategory(int categoryId);
    void Create(Deal deal);
    void Update(Deal deal);
    void Delete(int id);
}
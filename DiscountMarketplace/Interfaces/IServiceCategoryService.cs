using DiscountMarketplace.Models;

namespace DiscountMarketplace.Interfaces;

public interface IServiceCategoryService
{
    List<Servicecategory> GetAll();
    Servicecategory? GetById(int id);
    void Create(Servicecategory category);
    void Update(Servicecategory category);
    void Delete(int id);
}

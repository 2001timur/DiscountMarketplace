using DiscountMarketplace.Models;

namespace DiscountMarketplace.Interfaces;

public interface ICouponService
{
    List<Coupon> GetAll();
    Coupon? GetById(int id);
    List<Coupon> GetByUser(int userId);
    List<Coupon> GetByDeal(int dealId);
    void Create(Coupon coupon);
    void MarkAsUsed(int couponId);
    void ExpireOldCoupons();
    void Delete(int id);
}

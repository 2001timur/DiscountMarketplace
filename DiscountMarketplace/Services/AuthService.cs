using DiscountMarketplace.Models;

namespace DiscountMarketplace.Services;

public class AuthService
{
    public User? CurrentUser { get; set; }

    public event Action? OnNotify;

    public void Login(User user)
    {
        CurrentUser = user;
        OnNotify?.Invoke(); 
    }

    public void Logout()
    {
        CurrentUser = null;
        OnNotify?.Invoke();
    }

    public bool IsAdmin =>
        CurrentUser?.Role == "Admin";

    public bool IsCustomer =>
        CurrentUser?.Role == "Customer";
    public bool IsBusinessOwner =>
        CurrentUser?.Role == "BusinessOwner";
}
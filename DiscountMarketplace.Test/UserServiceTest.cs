using DiscountMarketplace.Models;
using DiscountMarketplace.Services;
using Microsoft.EntityFrameworkCore;

namespace DiscountMarketplace.Test;

[TestClass]
public class UserServiceTest
{
    private MarketplaceContext _context = null!;
    private UserService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<MarketplaceContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new MarketplaceContext(options);
        _service = new UserService(_context);
    }

    private User CreateUser(
        string email = "test@test.com",
        string password = "password123")
    {
        return new User
        {
            FirstName = "Test",
            LastName = "User",
            Email = email,
            PasswordHash = password,
            Phone = "123456789",
            Role = "User",
            CreatedAt = DateTime.Now
        };
    }

    [TestMethod]
    public void Create_ShouldAddUser_AndHashPassword()
    {
        var user = CreateUser();

        _service.Create(user);

        var created = _context.Users.FirstOrDefault(x => x.Email == user.Email);

        Assert.IsNotNull(created);
        Assert.AreEqual("Test", created.FirstName);
        Assert.AreEqual("User", created.LastName);
        Assert.AreEqual("User", created.Role);
        Assert.AreNotEqual("password123", created.PasswordHash); // перевірка хешу
    }

    [TestMethod]
    public void GetAll_ShouldReturnAllUsers()
    {
        _context.Users.Add(CreateUser("u1@test.com"));
        _context.Users.Add(CreateUser("u2@test.com"));
        _context.SaveChanges();

        var result = _service.GetAll();

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void GetById_ShouldReturnCorrectUser()
    {
        var user = CreateUser("id@test.com");

        _context.Users.Add(user);
        _context.SaveChanges();

        var result = _service.GetById(user.UserId);

        Assert.IsNotNull(result);
        Assert.AreEqual("id@test.com", result.Email);
    }

    [TestMethod]
    public void GetByEmail_ShouldReturnCorrectUser()
    {
        var user = CreateUser("email@test.com");

        _context.Users.Add(user);
        _context.SaveChanges();

        var result = _service.GetByEmail("email@test.com");

        Assert.IsNotNull(result);
        Assert.AreEqual(user.Email, result.Email);
    }

    [TestMethod]
    public void Authenticate_ShouldReturnUser_WhenPasswordCorrect()
    {
        var user = CreateUser("auth@test.com");

        _service.Create(user);

        var result = _service.Authenticate("auth@test.com", "password123");

        Assert.IsNotNull(result);
        Assert.AreEqual("auth@test.com", result.Email);
    }

    [TestMethod]
    public void Authenticate_ShouldReturnNull_WhenPasswordIncorrect()
    {
        var user = CreateUser("authfail@test.com");

        _service.Create(user);

        var result = _service.Authenticate("authfail@test.com", "wrongpassword");

        Assert.IsNull(result);
    }

    [TestMethod]
    public void Update_ShouldModifyUser()
    {
        var user = CreateUser("old@test.com");

        _context.Users.Add(user);
        _context.SaveChanges();

        var existing = _context.Users.First();

        existing.FirstName = "Updated";
        existing.LastName = "Name";
        existing.Email = "new@test.com";
        existing.Role = "Admin";

        _service.Update(existing);

        var updated = _context.Users.First();

        Assert.AreEqual("Updated", updated.FirstName);
        Assert.AreEqual("Name", updated.LastName);
        Assert.AreEqual("Admin", updated.Role);
        Assert.AreEqual("new@test.com", updated.Email);
    }

    [TestMethod]
    public void Delete_ShouldRemoveUser()
    {
        var user = CreateUser("delete@test.com");

        _context.Users.Add(user);
        _context.SaveChanges();

        _service.Delete(user.UserId);

        var deleted = _context.Users.FirstOrDefault(x => x.UserId == user.UserId);

        Assert.IsNull(deleted);
    }

    [TestMethod]
    public void IsAdmin_ShouldReturnTrue_WhenRoleIsAdmin()
    {
        var user = CreateUser("admin@test.com");
        user.Role = "Admin";

        _context.Users.Add(user);
        _context.SaveChanges();

        var result = _service.IsAdmin(user.UserId);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsBusinessOwner_ShouldReturnTrue_WhenRoleIsBusinessOwner()
    {
        var user = CreateUser("owner@test.com");
        user.Role = "BusinessOwner";

        _context.Users.Add(user);
        _context.SaveChanges();

        var result = _service.IsBusinessOwner(user.UserId);

        Assert.IsTrue(result);
    }
}
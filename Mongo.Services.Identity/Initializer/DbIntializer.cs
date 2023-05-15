using IdentityModel;

using Microsoft.AspNetCore.Identity;

using Mongo.Services.Identity.DbContext;
using Mongo.Services.Identity.Models;

using System.Security.Claims;

namespace Mongo.Services.Identity.Initializer;

public class DbIntializer : IDbIntializer
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public DbIntializer(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _db = db;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public void Initialize()
    {
        if (_roleManager.FindByNameAsync(SD.Admin).Result == null)
        {
            _roleManager.CreateAsync(new IdentityRole(SD.Admin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Customer)).GetAwaiter().GetResult();
        }
        else { return; }

        ApplicationUser adminUser = new ApplicationUser()
        {
            UserName = "admin1@gmail.com",
            Email = "admin1@gmail.com",
            EmailConfirmed = true,
            PhoneNumber = "0971469785",
            FirstName = "Denis",
            LastName = "Maltsev"
        };

        _userManager.CreateAsync(adminUser, "Admin123*").GetAwaiter().GetResult();
        _userManager.AddToRoleAsync(adminUser, SD.Admin).GetAwaiter().GetResult();

        var temp1 = _userManager.AddClaimsAsync(adminUser, new Claim[] { 
            new Claim(JwtClaimTypes.Name, adminUser.FirstName+" "+adminUser.LastName),
            new Claim(JwtClaimTypes.GivenName, adminUser.FirstName),
            new Claim(JwtClaimTypes.FamilyName, adminUser.LastName),
            new Claim(JwtClaimTypes.Role, SD.Admin)
        }).Result;

        ApplicationUser custumerUser = new ApplicationUser()
        {
            UserName = "customer1@gmail.com",
            Email = "customer1@gmail.com",
            EmailConfirmed = true,
            PhoneNumber = "0971469785",
            FirstName = "Denis",
            LastName = "Cust"
        };

        _userManager.CreateAsync(custumerUser, "Admin123*").GetAwaiter().GetResult();
        _userManager.AddToRoleAsync(custumerUser, SD.Customer).GetAwaiter().GetResult();

        var temp2 = _userManager.AddClaimsAsync(custumerUser, new Claim[] {
            new Claim(JwtClaimTypes.Name, custumerUser.FirstName+" "+custumerUser.LastName),
            new Claim(JwtClaimTypes.GivenName, custumerUser.FirstName),
            new Claim(JwtClaimTypes.FamilyName, custumerUser.LastName),
            new Claim(JwtClaimTypes.Role, SD.Customer)
        }).Result;
    }
}

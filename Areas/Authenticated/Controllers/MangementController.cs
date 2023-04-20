using System.Runtime.InteropServices;
using System.Security.Claims;
using FPT_BOOKSTORE.Data;
using FPT_BOOKSTORE.Utility.cs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FPT_BOOKSTORE.Controllers;

[Area(Constraintt.AuthenticatedArea)]
[Authorize(Roles = Constraintt.StoreOwnerRole + "," + Constraintt.CustomerRole)]

public class ManagementController : Controller
{
    private readonly ApplicationDbContext _db;

    public ManagementController(ApplicationDbContext db)
    {
        _db = db;
    }
        
    // GET
    [HttpGet]
    public IActionResult Index()
    {
        List<int> listId = new List<int>();

        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        
        // check if user has the "storeowner" role
        bool isStoreOwner = User.IsInRole(Constraintt.StoreOwnerRole);
        
        if (isStoreOwner)
        {
            // if user has "storeowner" role, display all orders
            var orderList = _db.Orders
                .Include(o => o.User)
                .ToList();
            
            return View(orderList);
        }
        else
        {
            // if user has any other role, display only orders belonging to that user
            var orderList = _db.Orders
                .Where(x => x.UserId == claims.Value)
                .Include(o => o.User)
                .ToList();
            
            return View(orderList);
        }
        

    }
        
    // detail of management
    [HttpGet]
    public IActionResult Detail(int managementId)
    {
        var managementDetail = _db.OrderDetails
            .Where(d => d.OrderId == managementId)
            .Include(d => d.Book)
            .ToList();


        return View(managementDetail);
    }
}
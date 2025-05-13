using BestStore.Models;
using BestStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BestStore_Application.Controllers
{
    [Authorize(Roles = "cleint")]
    public class ClientOrdersController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public ClientOrdersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var currentUser = await userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Index", "Home");
            }

            IQueryable<Order> query = context.Orders
                .Include(o => o.Items)
                .OrderByDescending(o => o.Id)
                .Where(o => o.ClientId == currentUser.Id);

            var orders = context.Orders
                .Include(q => q.Cient)
                .Include(q => q.Items)
                .OrderByDescending(q => q.Id)
                .ToList();

            ViewBag.Orders = orders;
            return View();
        }
    }
}

using BestStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BestStore_Application.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("/Admin/Orders/{action=Index}/{id?}")]
    public class AdminOrdersController : Controller
    {
        private readonly ApplicationDbContext context;

        public AdminOrdersController(ApplicationDbContext context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {

            var orders = context.Orders
                .Include(q => q.Cient)
                .Include(q => q.Items)
                .OrderByDescending(q => q.Id)
                .ToList();

            ViewBag.Orders = orders;
            return View();
        }

        public IActionResult Details(int id)
        {
            var order = context.Orders
                .Include(o => o.Cient)
                .Include(o => o.Items)
                .ThenInclude(o => o.Product).FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.NumOrders = context.Orders.Where(o => o.ClientId == order.ClientId).Count();

            return View(order);
        }

        [HttpPost("/Admin/Orders/ChangeStatus")]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeStatus(int id, string newStatus)
        {
            var order = context.Orders.FirstOrDefault(o => o.Id == id);

            if (order != null)
            {
                order.PaymentStatus = newStatus;
                context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

    }
}

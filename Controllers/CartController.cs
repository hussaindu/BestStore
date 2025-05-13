using BestStore.Models;
using BestStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BestStore_Application.Controllers
{
    //[Authorize(Roles = "cleint")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IConfiguration configuration;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly decimal ShippingFee;

        public CartController(ApplicationDbContext context,
                              IConfiguration configuration,
                              UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.configuration = configuration;
            this.userManager = userManager;
            ShippingFee = configuration.GetValue<decimal>("CartSettings:ShippingFee");
        }
        public IActionResult Index()
        {
            List<OrderItem> cartItems = CartHelper.GetCartItems(Request, Response, context);
            decimal subtotal = CartHelper.GetSubtotal(cartItems);

            ViewBag.CartItems = cartItems;
            ViewBag.ShippingFee = ShippingFee;
            ViewBag.Subtotal = subtotal;
            ViewBag.total = subtotal + ShippingFee;
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Index(CheckOutDto model)
        {
            List<OrderItem> cartItems = CartHelper.GetCartItems(Request, Response, context);
            decimal subtotal = CartHelper.GetSubtotal(cartItems);

            ViewBag.CartItems = cartItems;
            ViewBag.ShippingFee = ShippingFee;
            ViewBag.Subtotal = subtotal;
            ViewBag.total = subtotal + ShippingFee;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (cartItems.Count == 0)
            {
                ViewBag.ErrorMesssage = "Your cart is empty";
                return View(model);
            }

            TempData["DeliveryAddress"] = model.DeliveryAddress;
            TempData["PaymentMethod"] = model.PaymentMethod;

            return RedirectToAction("Confirm");
        }

        public IActionResult Confirm()
        {
            List<OrderItem> cartItems = CartHelper.GetCartItems(Request, Response, context);
            decimal total = CartHelper.GetSubtotal(cartItems) + ShippingFee;

            int cartSize = 0;
            foreach (var item in cartItems)
            {
                cartSize += item.Quantity;
            }

            string deliveryAddress = TempData["DeliveryAddress"] as string ?? "";
            string paymentMethod = TempData["PaymentMethod"] as string ?? "";
            TempData.Keep();

            if (cartSize == 0 || deliveryAddress.Length == 0 || paymentMethod.Length == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.DeliveryAddress = deliveryAddress;
            ViewBag.PaymentMethod = paymentMethod;
            ViewBag.TotalAmount = total;
            ViewBag.cartSize = cartSize;

            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Confirm(int any)
        {

            var cartItems = CartHelper.GetCartItems(Request, Response, context);
            string deliveryAddress = TempData["DeliveryAddress"] as string ?? "";
            string paymentMethod = TempData["PaymentMethod"] as string ?? "";

            TempData.Keep();


            if (cartItems.Count == 0 || deliveryAddress.Length == 0 || paymentMethod.Length == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            var appUser = await userManager.GetUserAsync(User);
            if (appUser == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var order = new Order
            {
                ClientId = appUser.Id,
                Items = cartItems,
                ShippingFee = ShippingFee,
                DeliveryAddress = deliveryAddress,
                PaymentMethod = paymentMethod,
                PaymentStatus = "pending",
                PaymentDetails = "",
                OrderStatus = "Created",
                CreateTime = DateTime.Now,
            };

            context.Orders.Add(order);
            context.SaveChanges();


            //delete

            Response.Cookies.Delete("shopping_cart");

            ViewBag.Success = "Order created Successfully";

            return View();
        }
    }
}

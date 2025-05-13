using BestStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BestStore.Controllers
{
    [Authorize(Roles ="admin")]
    [Route("/Admin/[controller]/{action=Index}/{id?}")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
        public IActionResult Index()
        {
            var users = userManager.Users.OrderByDescending(x => x.CreateAt).ToList();
            return View(users);
        }

        public async Task<IActionResult> Details(string? id)
        {
            if(id == null)
            {
                return RedirectToAction("Index","Users");
            }

            var appUser = await userManager.FindByIdAsync(id);

            if (appUser == null)
            {
                return RedirectToAction("Index", "Users");
            }
            ViewBag.Roles=await userManager.GetRolesAsync(appUser);

            return View(appUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return RedirectToAction("Index");
            }

            var result = await userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "User deleted successfully.";
            }
            else
            {
                TempData["Error"] = "Something went wrong while deleting the user.";
            }

            return RedirectToAction("Index");
        }

    }
}

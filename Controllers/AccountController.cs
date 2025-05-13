using BestStore.Models;
using BestStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BestStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }   
        public IActionResult Register()
        {
            if(signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index","Home");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }
           
            if (!ModelState.IsValid) 
            {
                return View(registerDto);
            }

            //create a new account and authenticate the user
            var user = new ApplicationUser()
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                UserName = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                Address = registerDto.Address,
                CreateAt = DateTime.Now
            };

            var result=await userManager.CreateAsync(user,registerDto.Password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "cleint");

                await signInManager.SignInAsync(user, false);

                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors) 
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(registerDto);
        }

        public async Task<IActionResult> Logout()
        {
            if(signInManager.IsSignedIn(User))
            {
                await signInManager.SignOutAsync();
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Login()
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }
           
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }
         
            if (!ModelState.IsValid) 
            {
                return View(loginDto);
            }
            var result = await signInManager.PasswordSignInAsync
                (loginDto.Email, loginDto.Password, loginDto.RememberMe, false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid login attemped";
            }

            return View(loginDto);
        }
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            return View();
        }

        public async Task<IActionResult> AccessDenied()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}

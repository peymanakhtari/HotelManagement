using hotelsimorgh.Data;
using hotelsimorgh.Data.DbModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace hotelsimorgh.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        [AllowAnonymous]
        public IActionResult Login()
        {
          
            
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password )
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("ReservesTable", "Reserve");
            using (UnitOfWork db = new UnitOfWork())
            {
                var user = db.UserRepository.Get(x => x.UserName == username && x.Password == password).FirstOrDefault();
                if (user == null)
                {
                    ViewBag.userNotFound = true; return View();
                }

                var claims = new List<Claim>
                {
                 new Claim(ClaimTypes.Name, user.UserName),
                 new Claim("FullName", user.Name),
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                };
                await HttpContext.SignInAsync(
                  CookieAuthenticationDefaults.AuthenticationScheme,
                 new ClaimsPrincipal(claimsIdentity),
                    authProperties);
                db.UserLoginRepository.Insert(new UserLogin { UserId = user.Id, datetime = DateTime.Now });
                db.Save();
                return RedirectToAction("ReservesTable", "Reserve");
            }
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}

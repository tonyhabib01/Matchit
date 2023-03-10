using MatchIt.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MatchIt.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            //var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Username);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "Invalid credentials!";
                    return RedirectToAction("Login", "Account");
                }
                // if the user has no password redirect him to change password
                if (string.IsNullOrEmpty(user.PasswordHash))
                {
                    TempData["InfoMessage"] = "In order to proceed, you must change your password!";
                    return RedirectToAction("ChangePassword", "Account", new { userId = user.Id });
                }

                if (user != null && !string.IsNullOrEmpty(model.Password))
                {
                    if (await _userManager.CheckPasswordAsync(user, model.Password))
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);

                        return RedirectToAction("List", "Semester");
                    }
                }
            }
            TempData["ErrorMessage"] = "Invalid credentials!";
            ModelState.AddModelError("", "Invalid credentials!");
            return View(model);
        }

        public async Task<IActionResult> ChangePassword (string? userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "Invalid user!";
                return RedirectToAction("Login", "Account");

            }

            var user = await _userManager.FindByIdAsync(userId);

            if ((!string.IsNullOrEmpty(user.PasswordHash) && !User.Identity.IsAuthenticated) || user == null)
            {
                TempData["ErrorMessage"] = "You are not allowed to change the password for this user!";
                return RedirectToAction("Login", "Account");
            }

            var model = new ChangePasswordViewModel { UserId = userId };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);

                if (user != null) {
                    IdentityResult result;
                    if (User.Identity.IsAuthenticated)
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                        result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
                    }
                    else
                    {
                        result = await _userManager.AddPasswordAsync(user, model.NewPassword);
                    }

                    if (result.Succeeded)
                    {
                        TempData["SuccessMessage"] = "Password changed successfully!";

                        if (User.Identity.IsAuthenticated)
                            return RedirectToAction("Logout", "Account");

                        return RedirectToAction("Login", "Account");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    TempData["ErrorMessage"] = result.Errors.ToList()[0].Description;
                    return RedirectToAction("ChangePassword", "Account", new { userId = model.UserId });
                }
            } else
            {
                var errorMessages = ModelState.Values.SelectMany(v => v.Errors)
                                            .Select(e => e.ErrorMessage);
                TempData["ErrorMessage"] = errorMessages.ToList()[0];
                return RedirectToAction("ChangePassword", "Account", new { userId = model.UserId });

            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}

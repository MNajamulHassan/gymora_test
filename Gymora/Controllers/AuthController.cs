using Gymora.Data.Entities;
using Gymora.Models.ViewModels.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Gymora.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        private static readonly Guid DemoTenantId = new Guid("11111111-1111-1111-1111-111111111111");

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET /Auth/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (_signInManager.IsSignedIn(User))
                return RedirectBasedOnRole();

            return View(new LoginViewModel());
        }

        // POST /Auth/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains("GymOwner") || roles.Contains("Receptionist"))
                        return RedirectToAction("Index", "Dashboard");

                    if (roles.Contains("Member"))
                        return RedirectToAction("Dashboard", "Portal", new { area = "Member" });
                }
                return RedirectToAction("Dashboard", "Portal", new { area = "Member" });
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Account locked. Try again in 5 minutes.");
                return View(model);
            }

            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(model);
        }

        // GET /Auth/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            if (_signInManager.IsSignedIn(User))
                return RedirectBasedOnRole();

            return View(new RegisterViewModel());
        }

        // POST /Auth/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existing = await _userManager.FindByEmailAsync(model.Email);
            if (existing != null)
            {
                ModelState.AddModelError("Email", "An account with this email already exists.");
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                Gender = model.Gender,
                DateOfBirth = model.DateOfBirth,
                FitnessGoals = model.FitnessGoals,
                HealthConditions = model.HealthConditions,
                TenantId = DemoTenantId,
                JoinDate = DateTime.Today,
                IsActive = true,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Member");
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Dashboard", "Portal", new { area = "Member" });
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        // POST /Auth/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        // GET /Auth/AccessDenied
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // ─── Helpers ───────────────────────────────────────────────────────
        private IActionResult RedirectBasedOnRole()
        {
            if (User.IsInRole("GymOwner") || User.IsInRole("Receptionist"))
                return RedirectToAction("Index", "Dashboard");

            return RedirectToAction("Dashboard", "Portal", new { area = "Member" });
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using My_Uni_Hub.Models;
using My_Uni_Hub.Models.Pages;
using My_Uni_Hub.Models.ViewModels.UserViewModel;
using My_Uni_Hub.Services.Implementations;
using My_Uni_Hub.Services.Interfaces;

namespace My_Uni_Hub.Controllers.Admin
{
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILocalFileStorageService _fileStorageService;
        private readonly IStudentService _students;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AuthController> _logger;
        private readonly MyUniDbContext _db;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,ILocalFileStorageService fileStorageService,IStudentService students, 
            MyUniDbContext db, ILogger<AuthController> logger, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _fileStorageService = fileStorageService;
            _students = students;
            _roleManager = roleManager;
            _logger = logger;
            _db = db;
        }

        // GET: /Auth/Login
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !user.IsActive)
            {
                ModelState.AddModelError("", "Invalid email or password");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Invalid email or password");
                return View(model);
            }

            var roles = await _userManager.GetRolesAsync(user);

            bool isAdmin = roles.Contains("Admin");
            bool isStudent = roles.Contains("Student") || roles.Contains("User");

            
            if (model.Role == "Admin" && !isAdmin)
            {
                await _signInManager.SignOutAsync();
                ModelState.AddModelError("", "You are not authorized to login as Admin");
                return View(model);
            }

           
            if (model.Role == "Student" && (isStudent || isAdmin))
            {
                return RedirectToAction("Dashboard", "User");
            }

            
            if (model.Role == "Admin" && isAdmin)
            {
                return RedirectToAction("Index", "AdminDashboard");
            }

            await _signInManager.SignOutAsync();
            ModelState.AddModelError("", "Unauthorized login attempt");
            return View(model);
        }



        // GET: /Auth/Register
        [HttpGet]
        public async Task<IActionResult> RegisterAsync()
        {
            var departments = await _db.Departments
                                       .AsNoTracking()
                                       .OrderBy(d => d.Name)
                                       .ToListAsync();

            var faculties = await _db.Faculties
                                     .AsNoTracking()
                                     .OrderBy(f => f.Name)
                                     .ToListAsync();
            ViewBag.Departments = new SelectList(departments, "Id", "Name");
            ViewBag.Faculties = new SelectList(faculties, "Id", "Name");

            return View();
        
        }

        // POST: /Auth/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View(model);

            string profileImagePath = null;
            try
            {
                if (model.ProfileImage != null)
                    profileImagePath = await _fileStorageService.UploadAsync(model.ProfileImage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Profile upload failed");
                ModelState.AddModelError("", "Profile image upload failed.");
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                Address = model.Address,
                DateOfBirth = model.DateOfBirth,
                ProfileImagePath = profileImagePath,
                PhoneNumber = model.PhoneNumber
            };

            var createRes = await _userManager.CreateAsync(user, model.Password);
            if (!createRes.Succeeded)
            {
                foreach (var err in createRes.Errors)
                    ModelState.AddModelError("", err.Description);
                return View(model);
            }

            var role = string.IsNullOrWhiteSpace(model.SelectedRole) ? "User" : model.SelectedRole;
            var roleExists = await _roleManager.RoleExistsAsync(role);
            if (!roleExists)
            {
                await _userManager.DeleteAsync(user);
                ModelState.AddModelError("", $"Role '{role}' does not exist.");
                return View(model);
            }

            var addRoleRes = await _userManager.AddToRoleAsync(user, role);
            if (!addRoleRes.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                foreach (var err in addRoleRes.Errors)
                    ModelState.AddModelError("", err.Description);
                return View(model);
            }
            Student createdStudent = null;
            try
            {
                var student = new Student
                {
                    FullName = string.IsNullOrWhiteSpace(model.FullName) ? user.UserName ?? user.Email : model.FullName,
                    Email = user.Email,
                    UserId = user.Id,
                    PhotoUrl = profileImagePath,
                    CreatedAt = DateTime.UtcNow,
                    IsVerified = true,
                    DepartmentId = model.DepartmentId,
                    FacultyId = model.FacultyId,
                    AcademicYear = model.AcademicYear,
                    PhoneNumber = model.PhoneNumber,
                    Bio = null
                };

                createdStudent = await _students.CreateAsync(student, ct);
                await _students.AssignAllCoursesToStudentAsync(createdStudent.Id, ct);
                TempData["dbg_studentId"] = createdStudent.Id.ToString();
                TempData["dbg_assigned"] = "true";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed creating Student or assigning courses for user {UserId}", user.Id);


                ModelState.AddModelError("", "Registration succeeded but creating/assigning student profile failed. Contact admin.");
                return View(model);
            }
            await _signInManager.SignInAsync(user, isPersistent: false);

            return RedirectToAction("student", "admin");
        }




        // POST: /Auth/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Auth");
        }

        // GET: /Auth/ChangePassword
        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        // POST: /Auth/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["SuccessMessage"] = "Password changed successfully!";
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        // GET: /Auth/Profile
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var model = new EditProfileViewModel
            {
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };

            return View(model);
        }

        // POST: /Auth/Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Profile(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction(nameof(Login));
            }

            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;

            if (user.Email != model.Email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                if (!setEmailResult.Succeeded)
                {
                    foreach (var error in setEmailResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }

                var setUsernameResult = await _userManager.SetUserNameAsync(user, model.Email);
                if (!setUsernameResult.Succeeded)
                {
                    foreach (var error in setUsernameResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
            }

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction(nameof(Profile));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // /Auth/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null || !user.IsActive)
            {
                ViewBag.SuccessMessage = "If your email is registered, you will receive a password reset link shortly.";
                return View();
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var callbackUrl = Url.Action(
                "ResetPassword",
                "Auth",
                new { userId = user.Id, token = token },
                protocol: Request.Scheme
            );
            return View();
        }
        // GET: /Auth/ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                ViewBag.ErrorMessage = "Invalid password reset link.";
                return View("Error");
            }

            var model = new ResetPasswordViewModel
            {
                Token = token, 
                UserId = userId
            };

            return View(model);
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                ViewBag.ErrorMessage = "User not found.";
                return View(model);
            }


            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Password has been reset successfully! You can now login.";
                return RedirectToAction(nameof(Login));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }



        // GET: /Auth/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}

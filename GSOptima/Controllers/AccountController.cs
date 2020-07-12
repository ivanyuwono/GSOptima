using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using GSOptima.Models;
using GSOptima.Models.AccountViewModels;
using GSOptima.Services;
using GSOptima.ViewModels;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using MimeKit;

namespace GSOptima.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private IHostingEnvironment _env;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ILogger<AccountController> logger,
           RoleManager<IdentityRole> roleManager,
           IHostingEnvironment env)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _roleManager = roleManager;
            _env = env;
        }

        [TempData]
        public string ErrorMessage { get; set; }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;


            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true

                //*IVY 5 Nov 2019 Penambahan logic pengecekan expired user
                ApplicationUser user = await _userManager.FindByEmailAsync(model.Email);
                bool valid = false;
                if (user != null)
                {
                    if (user.EndDate >= DateTime.Today)
                    {
                        valid = true;
                    }
                }

                if(!valid)
                {
                    ModelState.AddModelError(string.Empty, "Your account is expired.");
                    return View(model);
                }



                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return RedirectToLocal(returnUrl);
                }


                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(LoginWith2fa), new { returnUrl, model.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToAction(nameof(Lockout));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWith2fa(bool rememberMe, string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            var model = new LoginWith2faViewModel { RememberMe = rememberMe };
            ViewData["ReturnUrl"] = returnUrl;

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model, bool rememberMe, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, model.RememberMachine);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} logged in with 2fa.", user.Id);
                return RedirectToLocal(returnUrl);
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                _logger.LogWarning("Invalid authenticator code entered for user with ID {UserId}.", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithRecoveryCode(string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty);

            var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} logged in with a recovery code.", user.Id);
                return RedirectToLocal(returnUrl);
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                _logger.LogWarning("Invalid recovery code entered for user with ID {UserId}", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl, _env.EnvironmentName);

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation("User created a new account with password.");
                    return RedirectToLocal(returnUrl);
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToAction(nameof(Login));
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in with {Name} provider.", info.LoginProvider);
                return RedirectToLocal(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                return View("ExternalLogin", new ExternalLoginViewModel { Email = email });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    throw new ApplicationException("Error loading external login information during confirmation.");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(nameof(ExternalLogin), model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userId}'.");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);
                await _emailSender.SendEmailAsync(model.Email, "Reset Password",
                   $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>", _env.EnvironmentName);
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                throw new ApplicationException("A code must be supplied for password reset.");
            }
            var model = new ResetPasswordViewModel { Code = code };
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
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            AddErrors(result);
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }


        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }
        #endregion


        //added code
        public async Task<IActionResult> Index(string filter, int? page, string sort, string nowsort)
        {
            
            if (Request?.Headers != null &&
             Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return ViewComponent("UserList", new { filter = filter, page = page, sort = sort, nowsort = nowsort });

            }
            Paging<RegisterViewModel> p = new Paging<RegisterViewModel>();
            p.attribute.filter = "";
            return View(p);
        }

        // GET: Account/EditUser/5
        [HttpGet]
        //[SessionTimeout]
        public async Task<IActionResult> EditUser(string id, string returnUrl = "Index")
        {
            //var email = id;

            ViewData["ReturnUrl"] = returnUrl;

            if (id == null)
            {
                return NotFound();
            }
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);

            UpdateViewModel reg = new UpdateViewModel()
            {
                Email = user.Email,
                Name = user.Name,
                //BirthDate = user.BirthDate,
                //Address = user.Address,
                EndDate = user.EndDate,
                StartDate = user.StartDate,
                Roles = roles.Count > 0 ? roles.First() : "",
                RoleList = new SelectList(_roleManager.Roles, "Name", "Name")
            };
            return PartialView("_EditUser", reg);
        }


        // POST: Account/EditUser/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //public async Task<IActionResult> Edit(Guid id, [Bind("Id,BBLower,BBUpper,BigWave,Close,Date,EMA12,EMA26,Frequency,GSLine,High,Low,MA20,MA60,MACD,Open,Resistance,SignalLine,StockID,Support,TrendHigh,TrendLow,Volume")] StockPrice stockPrice)
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[SessionTimeout]
        public async Task<IActionResult> EditUser(string id, UpdateViewModel updatedUser, string returnUrl = null)
        {
            //var email = id;
            if (id != updatedUser.Id)
            {
                return NotFound();
            }
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {

                ApplicationUser user = await _userManager.FindByIdAsync(updatedUser.Id);
                user.Email = updatedUser.Email;
                user.Name = updatedUser.Name;
                //user.BirthDate = updatedUser.BirthDate;
                //user.Address = updatedUser.Address;
                user.BirthDate = new DateTime(1900, 1, 1);
                user.Address = "";
                user.EndDate = updatedUser.EndDate;
                user.StartDate = (DateTime)updatedUser.StartDate;
                await _userManager.UpdateAsync(user);

                var roles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, roles);
                await _userManager.AddToRoleAsync(user, updatedUser.Roles);

            }
            //ViewData["StockID"] = new SelectList(_context.Stock, "StockID", "StockID", stockPrice.StockID);
            //return RedirectToAction(returnUrl);
            return Content("");
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ApplicationUser user = await _userManager.FindByIdAsync(id);


            if (user == null)
            {
                return NotFound();
            }

            UpdateViewModel reg = new UpdateViewModel()
            {
                Id = id,
                Email = user.Email,
                Name = user.Name,
                //Address = user.Address,
                EndDate = user.EndDate,
                StartDate = user.StartDate,
            };

            return PartialView(reg);
        }

        // POST: Account/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //[SessionTimeout]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {

            var currentUser = await GetCurrentUserAsync();
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                if (user == currentUser)
                {
                    await _signInManager.SignOutAsync();
                    _logger.LogInformation(4, "User logged out.");
                    return Content("logoff");
                }
                //await LogOff();
                else
                    return Content("");
            }
            else
            {
                return PartialView();
            }


            //return RedirectToAction("UserList");
        }
        [HttpPost]
        public async Task<ActionResult> SendEmail(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user.EmailConfirmed)
                return Json("Email has been confirmed. No need to verify.");
            else
            {
                var webRoot = _env.WebRootPath; //get wwwroot Folder
                                                //Get TemplateFile located at wwwroot/Templates/EmailTemplate/Register_EmailTemplate.html  
                var pathToFile = _env.WebRootPath
                        + Path.DirectorySeparatorChar.ToString()
                        + "Templates"
                        + Path.DirectorySeparatorChar.ToString()
                        + "EmailTemplate"
                        + Path.DirectorySeparatorChar.ToString()
                        + "Confirm_Account_Registration.html";

                var builder = new BodyBuilder();
                using (StreamReader SourceReader = System.IO.File.OpenText(pathToFile))
                {

                    builder.HtmlBody = SourceReader.ReadToEnd();

                }
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);

                string messageBody = string.Format(builder.HtmlBody,
                        user.Email,
                        user.StartDate.ToString("dd/MM/yyyy"),
                        user.EndDate.ToString("dd/MM/yyyy"),
                        callbackUrl
                        );




                //await _emailSender.SendEmailAsync(user.Email, "Confirm your account",
                //  $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>");
                await _emailSender.SendEmailAsync(user.Email, "User ID & Aktivasi GS Optima",
                    messageBody, _env.EnvironmentName);

                return Json("Email has been sent");
            }
        }
        [HttpGet]
        public IActionResult CreateUser(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["RolesID"] = new SelectList(_roleManager.Roles, "Name", "Name");

            RegisterViewModel rm = new RegisterViewModel();
            //rm.Roles = new List<string>();

            //rm.Roles.Add("Administrator");
            //rm.Roles.Add("User");
            //rm.Roles = new string[] { "Administrator", "User" };
            rm.RoleList = new SelectList(_roleManager.Roles, "Name", "Name");
            //rm.Roles = "Administrator";
            //rm.Email = "ivanyuwono3@yahoo.com";
            rm.Address = "";
            rm.BirthDate = new DateTime(1980, 1, 1);
            rm.Roles = "User";

            rm.EndDate = DateTime.Today.AddYears(1);
            rm.StartDate = DateTime.Today;
            //rm.Password = "Masterclass#1";
            //rm.ConfirmPassword = "Masterclass#1";
            return PartialView("_CreateUser", rm);
        }
        [HttpPost]
        //ini yang dipakai
        public async Task<IActionResult>
            CreateUser2(RegisterViewModel model)
        {

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Name = model.Name,
                    Email = model.Email,
                    //Address = model.Address,
                    Address = "",
                    StartDate = (DateTime)model.StartDate,
                    EndDate = model.EndDate,
                    //BirthDate = model.BirthDate,
                    BirthDate = new DateTime(1900, 1, 1),
                    EmailConfirmed = false,
                    MembershipType = model.MembershipType


                };

                var result = await _userManager.CreateAsync(user, model.Password);



                IdentityResult result2 = IdentityResult.Success;
                if (result.Succeeded)
                {
                    result2 = await _userManager.AddToRoleAsync(user, model.Roles);
                    //result2 = await _userManager.AddToRolesAsync(user, new List<string>() {"Administrator" });
                    if (result2.Succeeded)
                    {
                        await SendEmail(user.Id);

                        //// For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                        //// Send an email with this link
                        //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                        //await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
                        ////await _emailSender.SendEmailAsync("ivanyuwono@yahoo.com", "Confirm your account",
                        //    $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>");


                        ////await _signInManager.SignInAsync(user, isPersistent: false);
                        ////_logger.LogInformation(3, "User created a new account with password.");
                        ////_logger.LogInformation(3, "User added to roles.");

                        ////return RedirectToLocal(returnUrl);

                        ////return RedirectToLocal("");
                    }

                    _logger.LogInformation(3, "Created a new account " + model.Email);
                    //return RedirectToAction("Index");
                    return Content("");
                }

                AddErrors(result);
                //AddErrors(result2);

            }

            model.RoleList = new SelectList(_roleManager.Roles, "Name", "Name");
            return PartialView("_CreateUser", model);
            //return null;
            //return Json("Duplicate User Name");
        }
    }
}

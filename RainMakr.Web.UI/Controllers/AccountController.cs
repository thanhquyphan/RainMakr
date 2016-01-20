using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Owin;
using RainMakr.Web.UI.Models;

namespace RainMakr.Web.UI.Controllers
{
    using RainMakr.Web.Core;
    using RainMakr.Web.Interfaces.Manager.Command;
    using RainMakr.Web.Models;
    using RainMakr.Web.UI.Core;

    /// <summary>
    /// The account controller.
    /// </summary>
    [Authorize]
    public class AccountController : BaseController
    {
        /// <summary>
        /// The sign in manager.
        /// </summary>
        private readonly SignInManager<Person, string> signInManager;

        /// <summary>
        /// The email manager.
        /// </summary>
        private readonly IEmailManager emailManager;

        /// <summary>
        /// The person manager.
        /// </summary>
        private readonly UserManager<Person> personManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        /// <param name="personManager">
        /// The person manager <see cref="UserManager{Person}"/>
        /// </param>
        /// <param name="emailManager">
        /// The email Manager.
        /// </param>
        public AccountController(
            UserManager<Person> personManager,
            IEmailManager emailManager)
        {
            this.signInManager = new SignInManager<Person, string>(personManager, System.Web.HttpContext.Current.GetOwinContext().Authentication);
            this.personManager = personManager;
            this.emailManager = emailManager;
        }

        /// <summary>
        /// The login.
        /// </summary>
        /// <param name="returnUrl">
        /// The return url.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (this.HttpContext.User.Identity.IsAuthenticated)
            {
                return this.Redirect(returnUrl);
            }

            this.ViewBag.ReturnUrl = returnUrl;
            return this.View();
        }

        /// <summary>
        /// The login.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="returnUrl">
        /// The return url.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var person = await this.personManager.FindAsync(model.Email, model.Password);
            if (person != null)
            {
                if (!person.EmailConfirmed)
                {
                    await this.GenerateEmailConfirmationAsync(person.Id);
                    return this.View("AccountUnconfirmed");
                }
            }

            var result = await this.signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, true);
            switch (result)
            {
                case SignInStatus.Success:
                    return this.RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return this.View("Lockout");
                case SignInStatus.RequiresVerification:
                    return this.RedirectToAction("SendCode", new { ReturnUrl = returnUrl, model.RememberMe });
                default:
                    this.ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return this.View(model);
            }
        }

        /// <summary>
        /// The verify code.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="returnUrl">
        /// The return url.
        /// </param>
        /// <param name="rememberMe">
        /// The remember me.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the person has already logged in via username/password or external login
            if (!await this.signInManager.HasBeenVerifiedAsync())
            {
                return this.View("Error");
            }

            return this.View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        /// <summary>
        /// The verify code.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a person enters incorrect codes for a specified amount of time then the person account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await this.signInManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return this.RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return this.View("Lockout");
                default:
                    this.ModelState.AddModelError(string.Empty, "Invalid code.");
                    return this.View(model);
            }
        }

        /// <summary>
        /// The register.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [AllowAnonymous]
        public ActionResult Register()
        {
            return this.View();
        }

        /// <summary>
        /// The register.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var person = new Person { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName };
                var result = await this.personManager.CreateAsync(person, model.Password);
                if (result.Succeeded)
                {
                    result = await this.personManager.AddToRoleAsync(person.Id, "User");
                    if (result.Succeeded)
                    {
                        if (result.Succeeded)
                        {
                            await this.signInManager.SignInAsync(person, false, false);

                            await this.GenerateEmailConfirmationAsync(person.Id);
                            return this.View("AccountConfirmationSent");
                        }
                    }
                }

                this.AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return this.View(model);
        }

        /// <summary>
        /// The confirm email.
        /// </summary>
        /// <param name="personId">
        /// The person id.
        /// </param>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string personId, string code)
        {
            if (personId == null || code == null)
            {
                return this.View("Error");
            }

            var result = await this.personManager.ConfirmEmailAsync(personId, code);
            return this.View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        /// <summary>
        /// The forgot password.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return this.View();
        }

        /// <summary>
        /// The forgot password.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var person = await this.personManager.FindByNameAsync(model.Email);
                if (person == null || !person.EmailConfirmed)
                {
                    // Don't reveal that the person does not exist or is not confirmed
                    return this.View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                var code = await this.personManager.GeneratePasswordResetTokenAsync(person.Id);
                var email = await this.personManager.GetEmailAsync(person.Id);
                await this.emailManager.SendPasswordResetAsync(person.Id, code, email);
                return this.RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return this.View(model);
        }

        /// <summary>
        /// The forgot password confirmation.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return this.View();
        }

        /// <summary>
        /// The reset password.
        /// </summary>
        /// <param name="personId">
        /// The person Id.
        /// </param>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [AllowAnonymous]
        public ActionResult ResetPassword(string personId, string code)
        {
            return string.IsNullOrWhiteSpace(personId) || string.IsNullOrWhiteSpace(code) ? this.View("Error") : this.View(new ResetPasswordViewModel(personId, code));
        }

        /// <summary>
        /// The reset password.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var person = await this.personManager.FindByIdAsync(model.PersonId);
            if (person == null)
            {
                // Don't reveal that the person does not exist
                return this.RedirectToAction("ResetPasswordConfirmation", "Account");
            }

            var result = await this.personManager.ResetPasswordAsync(person.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return this.RedirectToAction("ResetPasswordConfirmation", "Account");
            }

            this.AddErrors(result);
            return this.View();
        }

        /// <summary>
        /// The reset password confirmation.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return this.View();
        }

        ///// <summary>
        ///// The external login.
        ///// </summary>
        ///// <param name="provider">
        ///// The provider.
        ///// </param>
        ///// <param name="returnUrl">
        ///// The return url.
        ///// </param>
        ///// <returns>
        ///// The <see cref="ActionResult"/>.
        ///// </returns>
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult ExternalLogin(string provider, string returnUrl)
        //{
        //    // Request a redirect to the external login provider
        //    return new ChallengeResult(provider, this.Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        //}

        /// <summary>
        /// The send code.
        /// </summary>
        /// <param name="returnUrl">
        /// The return url.
        /// </param>
        /// <param name="rememberMe">
        /// The remember me.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await this.signInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return this.View("Error");
            }

            var userFactors = await this.personManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return this.View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        /// <summary>
        /// The send code.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            // Generate the token and send it
            if (!await this.signInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return this.View("Error");
            }

            return this.RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, model.ReturnUrl, model.RememberMe });
        }

        ///// <summary>
        ///// The external login callback.
        ///// </summary>
        ///// <param name="returnUrl">
        ///// The return url.
        ///// </param>
        ///// <returns>
        ///// The <see cref="ActionResult"/>.
        ///// </returns>
        //[AllowAnonymous]
        //public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        //{
        //    var loginInfo = await this.signInManager.AuthenticationManager.GetExternalLoginInfoAsync();
        //    if (loginInfo == null)
        //    {
        //        return this.RedirectToAction("Login");
        //    }

        //    // Sign in the person with this external login provider if the person already has a login
        //    var result = await this.signInManager.ExternalSignInAsync(loginInfo, false);
        //    switch (result)
        //    {
        //        case SignInStatus.Success:
        //            return this.RedirectToLocal(returnUrl);
        //        case SignInStatus.LockedOut:
        //            return this.View("Lockout");
        //        case SignInStatus.RequiresVerification:
        //            return this.RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
        //        default:
        //            string firstName = null;
        //            string lastName = null;

        //            switch (loginInfo.Login.LoginProvider)
        //            {
        //                case "Facebook":
        //                    var accessToken = loginInfo.ExternalIdentity.Claims.First(x => x.Type == "FacebookAccessToken");
        //                    var fb = new FacebookClient(accessToken.Value);
        //                    dynamic myInfo = fb.Get("/me?fields=email,first_name,last_name"); // specify the email field
        //                    loginInfo.Email = myInfo.email;
        //                    firstName = myInfo.first_name;
        //                    lastName = myInfo.last_name;
        //                    break;
        //                case "Microsoft":
        //                    firstName = loginInfo.ExternalIdentity.Claims.First(x => x.Type == "urn:microsoftaccount:first_name").Value;
        //                    lastName = loginInfo.ExternalIdentity.Claims.First(x => x.Type == "urn:microsoftaccount:last_name").Value;
        //                    break;
        //                case "Google":
        //                    firstName = loginInfo.ExternalIdentity.Claims.First(x => x.Type == "urn:googleaccount:first_name").Value;
        //                    lastName = loginInfo.ExternalIdentity.Claims.First(x => x.Type == "urn:googleaccount:last_name").Value;
        //                    break;
        //            }

        //            this.ViewBag.ReturnUrl = returnUrl;
        //            return this.View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { SubmitModel = new ExternalLoginConfirmationSubmitModel { Email = loginInfo.Email, FirstName = firstName, LastName = lastName }, LogInProvider = loginInfo.Login.LoginProvider });
        //    }
        //}

        ///// <summary>
        ///// The external login confirmation.
        ///// </summary>
        ///// <param name="model">
        ///// The model.
        ///// </param>
        ///// <param name="returnUrl">
        ///// The return url.
        ///// </param>
        ///// <param name="logInProvider">
        ///// The log in provider.
        ///// </param>
        ///// <returns>
        ///// The <see cref="ActionResult"/>.
        ///// </returns>
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ExternalLoginConfirmation([Bind(Prefix = "SubmitModel")] ExternalLoginConfirmationSubmitModel model, string returnUrl, string logInProvider)
        //{
        //    if (this.User.Identity.IsAuthenticated)
        //    {
        //        return this.RedirectToAction("Index", "Manage");
        //    }

        //    if (this.ModelState.IsValid)
        //    {
        //        // Get the information about the person from the external login provider
        //        var info = await this.signInManager.AuthenticationManager.GetExternalLoginInfoAsync();
        //        if (info == null)
        //        {
        //            return this.View("ExternalLoginFailure");
        //        }

        //        var person = new Person { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName, EmailConfirmed = true };
        //        var result = await this.personManager.CreateAsync(person, model.Password);
        //        if (result.Succeeded)
        //        {
        //            result = await this.personManager.AddToRoleAsync(person.Id, "User");
        //            if (result.Succeeded)
        //            {
        //                result = await this.personManager.AddLoginAsync(person.Id, info.Login);
        //                if (result.Succeeded)
        //                {
        //                    await this.signInManager.SignInAsync(person, false, false);
        //                    return this.RedirectToLocal(returnUrl);
        //                }
        //            }
        //        }

        //        this.AddErrors(result);
        //    }

        //    this.ViewBag.ReturnUrl = returnUrl;
        //    return this.View(new ExternalLoginConfirmationViewModel { SubmitModel = model, LogInProvider = logInProvider });
        //}

        /// <summary>
        /// The log off.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            this.signInManager.AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return this.RedirectToAction("Index", "Home");
        }

        ///// <summary>
        ///// The external login failure.
        ///// </summary>
        ///// <returns>
        ///// The <see cref="ActionResult"/>.
        ///// </returns>
        //[AllowAnonymous]
        //public ActionResult ExternalLoginFailure()
        //{
        //    return this.View();
        //}

        /// <summary>
        /// Generates email confirmation.
        /// </summary>
        /// <param name="personId">
        /// The person id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task GenerateEmailConfirmationAsync(string personId)
        {
            var code = await this.personManager.GenerateEmailConfirmationTokenAsync(personId);
            var email = await this.personManager.GetEmailAsync(personId);
            await this.emailManager.SendAccountConfirmationAsync(personId, code, email);
        }

        /// <summary>
        /// The add errors.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError(string.Empty, error);
            }
        }

        /// <summary>
        /// The redirect to local.
        /// </summary>
        /// <param name="returnUrl">
        /// The return url.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (this.Url.IsLocalUrl(returnUrl))
            {
                return this.Redirect(returnUrl);
            }

            return this.RedirectToAction("Index", "Home");
        }
    }
}
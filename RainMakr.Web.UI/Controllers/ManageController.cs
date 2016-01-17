// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManageController.cs" company="BringDream">
//   BringDream 2016
// </copyright>
// <summary>
//   Defines the ManageController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RainMakr.Web.UI.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;

    using RainMakr.Web.Models;
    using RainMakr.Web.Core;
    using RainMakr.Web.UI.Core;
    using RainMakr.Web.UI.Models;

    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin.Security;

    using RainMakr.Web.Models;

    /// <summary>
    /// The manage controller.
    /// </summary>
    [Authorize]
    public class ManageController : BaseController
    {
        /// <summary>
        /// The sign in manager.
        /// </summary>
        private readonly SignInManager<Person, string> signInManager;
        
        /// <summary>
        /// The person manager.
        /// </summary>
        private readonly UserManager<Person> personManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManageController"/> class.
        /// </summary>
        /// <param name="personManager">
        /// The person manager <see cref="UserManager{Person}"/>
        /// </param>
        public ManageController(
            UserManager<Person> personManager)
        {
            this.signInManager = new SignInManager<Person, string>(personManager, System.Web.HttpContext.Current.GetOwinContext().Authentication);
            this.personManager = personManager;
        }

        /// <summary>
        /// The index.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            this.ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
                : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
                : string.Empty;

            var user = await this.personManager.FindByIdAsync(this.User.Identity.GetUserId());
            var model = new IndexViewModel
            {
                HasPassword = user.PasswordHash != null,
                PhoneNumber = user.PhoneNumber,
                TwoFactor = user.TwoFactorEnabled,
                Logins = await this.personManager.GetLoginsAsync(user.Id),
                BrowserRemembered = await this.signInManager.AuthenticationManager.TwoFactorBrowserRememberedAsync(user.Id)
            };

            return this.View(model);
        }

        /// <summary>
        /// The remove login.
        /// </summary>
        /// <param name="loginProvider">
        /// The login provider.
        /// </param>
        /// <param name="providerKey">
        /// The provider key.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result = await this.personManager.RemoveLoginAsync(this.User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await this.personManager.FindByIdAsync(this.User.Identity.GetUserId());
                if (user != null)
                {
                    await this.signInManager.SignInAsync(user, false, false);
                }

                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }

            return this.RedirectToAction("ManageLogins", new { Message = message });
        }
        
        /// <summary>
        /// The add phone number.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult AddPhoneNumber()
        {
            return this.View();
        }

        /// <summary>
        /// The add phone number.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            // Generate the token and send it
            var code = await this.personManager.GenerateChangePhoneNumberTokenAsync(this.User.Identity.GetUserId(), model.Number);
            if (this.personManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.Number,
                    Body = "Your security code is: " + code
                };
                await this.personManager.SmsService.SendAsync(message);
            }

            return this.RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
        }

        /// <summary>
        /// The enable two factor authentication.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            await this.personManager.SetTwoFactorEnabledAsync(this.User.Identity.GetUserId(), true);
            var user = await this.personManager.FindByIdAsync(this.User.Identity.GetUserId());
            if (user != null)
            {
                await this.signInManager.SignInAsync(user, false, false);
            }

            return this.RedirectToAction("Index", "Manage");
        }

        /// <summary>
        /// The disable two factor authentication.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            await this.personManager.SetTwoFactorEnabledAsync(this.User.Identity.GetUserId(), false);
            var user = await this.personManager.FindByIdAsync(this.User.Identity.GetUserId());
            if (user != null)
            {
                await this.signInManager.SignInAsync(user, false, false);
            }

            return this.RedirectToAction("Index", "Manage");
        }

        /// <summary>
        /// The verify phone number.
        /// </summary>
        /// <param name="phoneNumber">
        /// The phone number.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var code = await this.personManager.GenerateChangePhoneNumberTokenAsync(this.User.Identity.GetUserId(), phoneNumber);

            // Send an SMS through the SMS provider to verify the phone number
            return phoneNumber == null ? this.View("Error") : this.View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        /// <summary>
        /// The verify phone number.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var result = await this.personManager.ChangePhoneNumberAsync(this.User.Identity.GetUserId(), model.PhoneNumber, model.Code);
            if (result.Succeeded)
            {
                var user = await this.personManager.FindByIdAsync(this.User.Identity.GetUserId());
                if (user != null)
                {
                    await this.signInManager.SignInAsync(user, false, false);
                }

                return this.RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
            }

            // If we got this far, something failed, redisplay form
            this.ModelState.AddModelError(string.Empty, "Failed to verify phone");
            return this.View(model);
        }

        /// <summary>
        /// The remove phone number.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public async Task<ActionResult> RemovePhoneNumber()
        {
            var result = await this.personManager.SetPhoneNumberAsync(this.User.Identity.GetUserId(), null);
            if (!result.Succeeded)
            {
                return this.RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }

            var user = await this.personManager.FindByIdAsync(this.User.Identity.GetUserId());
            if (user != null)
            {
                await this.signInManager.SignInAsync(user, false, false);
            }

            return this.RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        }

        /// <summary>
        /// The change password.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult ChangePassword()
        {
            return this.View();
        }

        /// <summary>
        /// The change password.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var result = await this.personManager.ChangePasswordAsync(this.User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await this.personManager.FindByIdAsync(this.User.Identity.GetUserId());
                if (user != null)
                {
                    await this.signInManager.SignInAsync(user, false, false);
                }

                return this.RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }

            this.AddErrors(result);
            return this.View(model);
        }

        /// <summary>
        /// The manage logins.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            this.ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : string.Empty;
            var user = await this.personManager.FindByIdAsync(this.User.Identity.GetUserId());
            if (user == null)
            {
                return this.View("Error");
            }

            var userLogins = await this.personManager.GetLoginsAsync(this.User.Identity.GetUserId());
            var otherLogins = this.signInManager.AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            this.ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return this.View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }
        
        /// <summary>
        /// The link login.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, this.Url.Action("LinkLoginCallback", "Manage"), this.User.Identity.GetUserId());
        }

        /// <summary>
        /// The link login callback.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await this.signInManager.AuthenticationManager.GetExternalLoginInfoAsync(ChallengeResult.XsrfKey, this.User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return this.RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }

            var result = await this.personManager.AddLoginAsync(this.User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded ? this.RedirectToAction("ManageLogins") : this.RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
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
        /// The manage message id.
        /// </summary>
        public enum ManageMessageId
        {
            /// <summary>
            /// The add phone success.
            /// </summary>
            AddPhoneSuccess,

            /// <summary>
            /// The change password success.
            /// </summary>
            ChangePasswordSuccess,

            /// <summary>
            /// The set two factor success.
            /// </summary>
            SetTwoFactorSuccess,

            /// <summary>
            /// The set password success.
            /// </summary>
            SetPasswordSuccess,

            /// <summary>
            /// The remove login success.
            /// </summary>
            RemoveLoginSuccess,

            /// <summary>
            /// The remove phone success.
            /// </summary>
            RemovePhoneSuccess,

            /// <summary>
            /// The error.
            /// </summary>
            Error
        }
    }
}
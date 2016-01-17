using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RainMakr.Web.UI.Core
{
    using System.Web.Mvc;

    using Microsoft.Owin.Security;

    public class ChallengeResult : HttpUnauthorizedResult
    {
        /// <summary>
        /// The XSRF key. Used for XSRF protection when adding external logins
        /// </summary>
        public const string XsrfKey = "XsrfId";

        /// <summary>
        /// Initializes a new instance of the <see cref="ChallengeResult"/> class.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="redirectUri">
        /// The redirect uri.
        /// </param>
        public ChallengeResult(string provider, string redirectUri)
            : this(provider, redirectUri, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChallengeResult"/> class.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="redirectUri">
        /// The redirect uri.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        public ChallengeResult(string provider, string redirectUri, string userId)
        {
            this.LoginProvider = provider;
            this.RedirectUri = redirectUri;
            this.UserId = userId;
        }

        /// <summary>
        /// Gets or sets the login provider.
        /// </summary>
        public string LoginProvider { get; set; }

        /// <summary>
        /// Gets or sets the redirect uri.
        /// </summary>
        public string RedirectUri { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// The execute result.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public override void ExecuteResult(ControllerContext context)
        {
            var properties = new AuthenticationProperties { RedirectUri = this.RedirectUri };
            if (this.UserId != null)
            {
                properties.Dictionary[XsrfKey] = this.UserId;
            }

            context.HttpContext.GetOwinContext().Authentication.Challenge(properties, this.LoginProvider);
        }
    }
}
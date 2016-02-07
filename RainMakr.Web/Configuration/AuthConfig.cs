// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthConfig.cs" company="BringDream">
//   BringDream 2016
// </copyright>
// <summary>
//   Defines the AuthConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RainMakr.Web.Configuration
{
    using System;
    using System.Configuration;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin;
    using Microsoft.Owin.Security.Cookies;
    using Microsoft.Owin.Security.DataProtection;
    using Microsoft.Owin.Security.Facebook;
    using Microsoft.Owin.Security.Google;
    using Microsoft.Owin.Security.MicrosoftAccount;

    using Owin;

    using RainMakr.Web.Models;

    /// <summary>
    /// The authentication config.
    /// </summary>
    public static class AuthConfig
    {
        /// <summary>
        /// The configure user manager.
        /// </summary>
        /// <param name="userStore">
        /// The user store.
        /// </param>
        /// <param name="app">
        /// The app.
        /// </param>
        /// <returns>
        /// The <see cref="UserManager{Person}"/>.
        /// </returns>
        public static UserManager<Person> ConfigureUserManager(UserStore<Person> userStore, IAppBuilder app)
        {
            var userManager = new UserManager<Person>(userStore);
            userManager.UserValidator = new UserValidator<Person>(userManager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            userManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configure user lockout defaults
            userManager.UserLockoutEnabledByDefault = true;
            userManager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            userManager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            userManager.RegisterTwoFactorProvider(
                "Phone Code",
                new PhoneNumberTokenProvider<Person>
                {
                    MessageFormat = "Your security code is {0}"
                });

            userManager.RegisterTwoFactorProvider(
                "Email Code",
                new EmailTokenProvider<Person>
                {
                    Subject = "Security Code",
                    BodyFormat = "Your security code is {0}"
                });

            ////this.EmailService = new EmailService();
            ////this.SmsService = new SmsService();

            if (app != null)
            {
                var dataProtectionProvider = app.GetDataProtectionProvider();
                if (dataProtectionProvider != null)
                {
                    userManager.UserTokenProvider =
                        new DataProtectorTokenProvider<Person>(dataProtectionProvider.Create("ASP.NET Identity"));
                }
            }

            return userManager;
        }

        /// <summary>
        /// The configure authentication.
        /// </summary>
        /// <param name="app">
        /// The app.
        /// </param>
        public static void ConfigureAuth(IAppBuilder app)
        {
            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<UserManager<Person>, Person>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => manager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie))
                }
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);
            
            //var microsoftAuthenticationOptions = new MicrosoftAccountAuthenticationOptions
            //{
            //    ClientId = ConfigurationManager.AppSettings["Authentication:Microsoft:ClientId"],
            //    ClientSecret = ConfigurationManager.AppSettings["Authentication:Microsoft:ClientSecret"]
            //};
            //microsoftAuthenticationOptions.Scope.Add("wl.basic");
            //microsoftAuthenticationOptions.Scope.Add("wl.emails");
            //microsoftAuthenticationOptions.Provider = new MicrosoftAccountAuthenticationProvider
            //{
            //    OnAuthenticated = context =>
            //    {
            //        context.Identity.AddClaim(new Claim("urn:microsoftaccount:access_token", context.AccessToken));

            //        foreach (var claim in context.User)
            //        {
            //            var claimType = string.Format("urn:microsoftaccount:{0}", claim.Key);
            //            var claimValue = claim.Value.ToString();
            //            if (!context.Identity.HasClaim(claimType, claimValue))
            //            {
            //                context.Identity.AddClaim(new Claim(claimType, claimValue, "XmlSchemaString", "Microsoft"));
            //            }
            //        }

            //        return Task.FromResult(true);
            //    }
            //};

            //app.UseMicrosoftAccountAuthentication(microsoftAuthenticationOptions);

            ////app.UseTwitterAuthentication(
            ////   consumerKey: "",
            ////   consumerSecret: "");
            
            ////
            ////ConfigurationManager.AppSettings["Authentication:Facebook:ClientId"]

            //app.UseFacebookAuthentication(new FacebookAuthenticationOptions
            //{
            //    AppId = ConfigurationManager.AppSettings["Authentication:Facebook:ClientId"],
            //    AppSecret = ConfigurationManager.AppSettings["Authentication:Facebook:ClientSecret"],
            //    Provider = new FacebookAuthenticationProvider
            //    {
            //        OnAuthenticated = context =>
            //        {
            //            context.Identity.AddClaim(new Claim("FacebookAccessToken", context.AccessToken));
            //            return Task.FromResult(true);
            //        }
            //    }
            //});

            //var googleAuthenticationOptions = new GoogleOAuth2AuthenticationOptions
            //{
            //    ClientId = ConfigurationManager.AppSettings["Authentication:Google:ClientId"],
            //    ClientSecret = ConfigurationManager.AppSettings["Authentication:Google:ClientSecret"],
            //    Provider = new GoogleOAuth2AuthenticationProvider()
            //    {
            //        OnAuthenticated = context =>
            //        {
            //            context.Identity.AddClaim(new Claim("urn:googleaccount:first_name", context.User.GetValue("name")["givenName"].ToString()));
            //            context.Identity.AddClaim(new Claim("urn:googleaccount:last_name", context.User.GetValue("name")["familyName"].ToString()));
            //            return Task.FromResult(true);
            //        }
            //    }
            //};
            //googleAuthenticationOptions.Scope.Add("email");
            //app.UseGoogleAuthentication(googleAuthenticationOptions);
        }
    }
}
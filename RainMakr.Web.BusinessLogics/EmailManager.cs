// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmailManager.cs" company="BringDream">
//   BringDream 2016
// </copyright>
// <summary>
//   The email manager.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RainMakr.Web.BusinessLogics
{
    using System;
    using System.Threading.Tasks;

    using Mvc.Mailer;

    using RainMakr.Web.Interfaces.Manager.Command;
    using RainMakr.Web.Models.Core;
    using RainMakr.Web.Models.Notifications;

    /// <summary>
    /// The email manager.
    /// </summary>
    public class EmailManager : MailerBase, IEmailManager
    {
        /// <inheritdoc />
        public Task SendAccountConfirmationAsync(string personId, string code, string email)
        {
            var model = new AccountConfirmationNotification
            {
                Code = code,
                PersonId = personId
            };

            this.ViewData.Model = model;
            var message = this.Populate(
                x =>
                    {
                        x.Subject = EmailSubjects.AccountConfirmation;
                        x.ViewName = "AccountConfirmation";
                        x.To.Add(email);
                    });

            return this.SendMessageAsync(message);
        }

        /// <inheritdoc />
        public Task SendPasswordResetAsync(string personId, string code, string email)
        {
            var model = new PasswordResetNotification
            {
                Code = code,
                PersonId = personId
            };

            this.ViewData.Model = model;
            var message = this.Populate(
                x =>
                {
                    x.Subject = EmailSubjects.PasswordReset;
                    x.ViewName = "PasswordReset";
                    x.To.Add(email);
                });

            return this.SendMessageAsync(message);
        }
        
        /// <summary>
        /// Attempts to send an email message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <exception cref="EmailSendFailureException">
        /// All errors relating to sending the message will result in this exception.
        /// This allows for mailing errors to be easily handled.
        /// </exception>
        /// <remarks>
        /// Sending emails can result in a wide variety of exceptions.
        /// To make it easier to handle these exceptions upstream, all exceptions
        /// relating to the actual sending of the message are caught and wrapped into
        /// one exception. This is acceptable because code upstream doesn't care
        /// about how the message failed to send, only that it did fail.
        /// </remarks>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private Task SendMessageAsync(MvcMailMessage message)
        {
            try
            {
                return message.SendAsync();
            }
            catch (Exception ex)
            {
                throw new EmailSendFailureException(ex);
            }
        }
    }
}

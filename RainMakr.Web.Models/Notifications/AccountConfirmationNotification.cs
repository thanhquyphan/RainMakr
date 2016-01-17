// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountConfirmationNotification.cs" company="BringDream">
//   BringDream 2016
// </copyright>
// <summary>
//   Defines the AccountConfirmationNotification type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RainMakr.Web.Models.Notifications
{
    /// <summary>
    /// The account confirmation notification.
    /// </summary>
    public class AccountConfirmationNotification
    {
        /// <summary>
        /// Gets or sets the person id.
        /// </summary>
        public string PersonId { get; set; }

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        public string Code { get; set; }
    }
}

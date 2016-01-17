// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PasswordResetNotification.cs" company="BringDream">
//   BringDream 2016
// </copyright>
// <summary>
//   The password reset notification.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RainMakr.Web.Models.Notifications
{
    /// <summary>
    /// The password reset notification.
    /// </summary>
    public class PasswordResetNotification
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

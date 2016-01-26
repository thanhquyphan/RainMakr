// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEmailManager.cs" company="BringDream">
//   BringDream 2016
// </copyright>
// <summary>
//   Defines the IEmailManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RainMakr.Web.Interfaces.Manager.Command
{
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    /// <summary>
    /// The EmailManager interface.
    /// </summary>
    public interface IEmailManager : ICommandManager
    {
        /// <summary>
        /// Sends account confirmation.
        /// </summary>
        /// <param name="personId">
        /// The person id.
        /// </param>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task SendAccountConfirmationAsync(string personId, string code, string email);

        /// <summary>
        /// Sends password reset.
        /// </summary>
        /// <param name="personId">
        /// The person id.
        /// </param>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task SendPasswordResetAsync(string personId, string code, string email);
    }
}

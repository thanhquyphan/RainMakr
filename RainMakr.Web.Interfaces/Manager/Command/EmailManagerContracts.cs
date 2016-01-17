// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmailManagerContracts.cs" company="BringDream">
//   BringDream 2016
// </copyright>
// <summary>
//   The email manager contracts.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RainMakr.Web.Interfaces.Manager.Command
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    /// <summary>
    /// The email manager contracts.
    /// </summary>
    [ContractClassFor(typeof(IEmailManager))]
    public abstract class EmailManagerContracts : IEmailManager
    {
        /// <inheritdoc />
        public Task SendAccountConfirmationAsync(string personId, string code, string email)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(personId));
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(code));
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(email));

            return default(Task);
        }

        /// <inheritdoc />
        public Task SendPasswordResetAsync(string personId, string code, string email)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(personId));
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(code));
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(email));

            return default(Task);
        }
    }
}
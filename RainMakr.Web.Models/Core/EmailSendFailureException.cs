using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainMakr.Web.Models.Core
{
    /// <summary>
    /// Used to catch exceptions in email manager.
    /// </summary>
    /// <remarks>
    /// Needed so that all email related errors can be picked up by the controller
    /// which needs to notify the user that what they were doing was successful but
    /// failed to email. (EG. payment was successful but failed to email receipt).
    /// </remarks>
    public class EmailSendFailureException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailSendFailureException"/> class.
        /// </summary>
        /// <param name="innerException">
        /// The specific exception that caused the email sending to fail.
        /// </param>
        public EmailSendFailureException(Exception innerException)
            : base("An attempt to send an email failed", innerException)
        {
        }
    }
}

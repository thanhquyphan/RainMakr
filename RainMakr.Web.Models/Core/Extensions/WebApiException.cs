namespace RainMakr.Web.Models.Core.Extensions
{
    using System.Net;

    /// <summary>
    /// The web API exception.
    /// </summary>
    public class WebApiException : WebException
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="WebApiException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <param name="statusCode">
        /// The status code.
        /// </param>
        public WebApiException(string message, WebExceptionStatus status, HttpStatusCode statusCode)
            : base(message, status)
        {
            this.StatusCode = statusCode;
        }

        /// <summary>
        /// Gets the status code.
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }
    }
}
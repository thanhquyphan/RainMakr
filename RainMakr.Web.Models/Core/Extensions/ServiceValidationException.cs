namespace RainMakr.Web.Models.Core.Extensions
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The <see cref="ServiceValidationException"/>
    ///     class identifies a validation failure from the service.
    /// </summary>
    [Serializable]
    public class ServiceValidationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceValidationException"/> class.
        ///     Initialises a new instance of the <see cref="ServiceValidationException"/> class.
        /// </summary>
        public ServiceValidationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceValidationException"/> class.
        ///     Initialises a new instance of the <see cref="ServiceValidationException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public ServiceValidationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceValidationException"/> class.
        ///     Initialises a new instance of the <see cref="ServiceValidationException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="inner">
        /// The inner.
        /// </param>
        public ServiceValidationException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceValidationException"/> class.
        ///     Initialises a new instance of the <see cref="ServiceValidationException"/> class.
        /// </summary>
        /// <param name="info">
        /// The serialisation info.
        /// </param>
        /// <param name="context">
        /// The streaming context.
        /// </param>
        protected ServiceValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
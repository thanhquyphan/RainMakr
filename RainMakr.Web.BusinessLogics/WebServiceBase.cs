using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainMakr.Web.BusinessLogics
{
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Net;

    using RainMakr.Web.Models.Core.Extensions;

    using RestSharp;

    public abstract class WebServiceBase
    {
        /// <summary>
        /// Stores the service client.
        /// </summary>
        private readonly RestClient _serviceClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebServiceBase"/> class.
        ///     Initialises a new instance of the <see cref="WebServiceBase"/> class.
        /// </summary>
        /// <param name="serviceClient">
        /// The service client.
        /// </param>
        /// <param name="serviceLocation">
        /// The service location.
        /// </param>
        protected WebServiceBase(RestClient serviceClient)
        {
            Contract.Requires<ArgumentNullException>(serviceClient != null);

            this._serviceClient = serviceClient;
        }

        /// <summary>
        /// Executes the specified service location.
        /// </summary>
        /// <param name="serviceLocation">
        /// The service location.
        /// </param>
        /// <param name="method">
        /// The service method.
        /// </param>
        /// <param name="configureRequest">
        /// The configure request.
        /// </param>
        /// <param name="processResponse">
        /// The process response.
        /// </param>
        /// <exception cref="System.Net.WebException">
        /// Thrown when a
        ///     <see cref="HttpStatusCode"/>
        ///     in the 4xx or 5xx range is returned.
        /// </exception>
        protected virtual void Execute(
            string serviceLocation,
            Method method,
            Action<IRestRequest> configureRequest,
            Action<IRestResponse> processResponse)
        {
            Contract.Requires<ArgumentNullException>(string.IsNullOrWhiteSpace(serviceLocation) == false);

            var request = this.BuildRequest(serviceLocation, method, configureRequest);

            var response = this._serviceClient.Execute(request);

            this.ProcessResponse(response, processResponse);
        }

        /// <summary>
        /// Executes the specified service location.
        /// </summary>
        /// <typeparam name="T">
        /// The type of value to return.
        /// </typeparam>
        /// <param name="serviceLocation">
        /// The service location.
        /// </param>
        /// <param name="method">
        /// The service method.
        /// </param>
        /// <param name="configureRequest">
        /// The configure request.
        /// </param>
        /// <param name="processResponse">
        /// The process response.
        /// </param>
        /// <returns>
        /// A <typeparamref name="T"/> value.
        /// </returns>
        /// <exception cref="System.Net.WebException">
        /// Thrown when a
        ///     <see cref="HttpStatusCode"/>
        ///     in the 4xx or 5xx range is returned.
        /// </exception>
        protected virtual T Execute<T>(
            string serviceLocation,
            Method method,
            Action<IRestRequest> configureRequest,
            Action<IRestResponse<T>> processResponse) where T : new()
        {
            Contract.Requires<ArgumentNullException>(string.IsNullOrWhiteSpace(serviceLocation) == false);

            var request = this.BuildRequest(serviceLocation, method, configureRequest);

            var response = this._serviceClient.Execute<T>(request);

            ProcessResponse(response, processResponse);

            if (typeof(T).IsValueType)
            {
                // HACK: RestSharp JsonDeserializer currently doesn't handle value types
                var content = response.Content;

                return (T)Convert.ChangeType(content, typeof(T));
            }

            return response.Data;
        }

        /// <summary>
        /// Builds the request.
        /// </summary>
        /// <param name="serviceLocation">
        /// The service location.
        /// </param>
        /// <param name="method">
        /// The service method.
        /// </param>
        /// <param name="configureRequest">
        /// The configure request.
        /// </param>
        /// <returns>
        /// A <see cref="RestRequest"/> value.
        /// </returns>
        private RestRequest BuildRequest(string serviceLocation, Method method, Action<IRestRequest> configureRequest)
        {
            var request = new RestRequest(method)
            {
                Resource = serviceLocation,
                Credentials = CredentialCache.DefaultNetworkCredentials,
                RequestFormat = DataFormat.Json,
                DateFormat = "yyyy-MM-ddTHH:mm:ss"
            };

            if (configureRequest != null)
            {
                configureRequest(request);
            }

            return request;
        }

        /// <summary>
        /// Processes the response.
        /// </summary>
        /// <typeparam name="T">
        /// The type of data returned in the response.
        /// </typeparam>
        /// <param name="response">
        /// The response.
        /// </param>
        /// <param name="processResponse">
        /// The process response.
        /// </param>
        private void ProcessResponse<T>(IRestResponse<T> response, Action<IRestResponse<T>> processResponse)
        {
            if (processResponse == null)
            {
                return;
            }

            Action<IRestResponse> wrappedAction = x => processResponse((IRestResponse<T>)x);

            this.ProcessResponse((IRestResponse)response, wrappedAction);
        }

        /// <summary>
        /// Processes the response.
        /// </summary>
        /// <param name="response">
        /// The response.
        /// </param>
        /// <param name="processResponse">
        /// The process response action.
        /// </param>
        /// <exception cref="System.Net.WebException">
        /// Thrown when a
        ///     <see cref="HttpStatusCode"/>
        ///     in the 4xx or 5xx range is returned.
        /// </exception>
        private void ProcessResponse(IRestResponse response, Action<IRestResponse> processResponse)
        {
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new ServiceValidationException(response.Content);
            }

            var statusCode = (int)response.StatusCode;

            if (statusCode >= 400)
            {

                throw new WebApiException(response.StatusDescription, WebExceptionStatus.ProtocolError, response.StatusCode);
            }

            // HACK: The InvalidCastException check below attempts to ignore JsonDeserializer inability to work with value types
            // We will assume that continuing to process the response will be successful
            if (response.ErrorException != null && response.ErrorException is InvalidCastException == false)
            {

                throw response.ErrorException;
            }

            if (processResponse != null)
            {
                processResponse(response);
            }
        }

    }
}
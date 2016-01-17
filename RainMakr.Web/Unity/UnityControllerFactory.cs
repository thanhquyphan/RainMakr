// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnityControllerFactory.cs" company="BringDream">
//   BringDream 2016
// </copyright>
// <summary>
//   The unity controller factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RainMakr.Web.Unity
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    using Microsoft.Practices.Unity;

    /// <summary>
    /// The unity controller factory.
    /// </summary>
    public class UnityControllerFactory : DefaultControllerFactory
    {
        /// <summary>
        /// The unity container to resolve through.
        /// </summary>
        private readonly UnityContainer container;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityControllerFactory"/> class.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        public UnityControllerFactory(UnityContainer container)
        {
            this.container = container;
        }
        
        /// <inheritdoc />
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            UnityController controller;
            if (controllerType == null)
            {
                var errorMessage =
                    string.Format("The controller for path '{0}' could not be found or it does not implement RainMakrController.", requestContext.HttpContext.Request.Path);
                throw new HttpException(404, errorMessage);
            }

            if (!typeof(UnityController).IsAssignableFrom(controllerType))
            {
                throw new ArgumentException(
                    string.Format("Type requested is not a RainMakrController controller: {0}", controllerType.Name),
                    "controllerType");
            }

            try
            {
                controller = this.container.Resolve(controllerType) as UnityController;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    string.Format("Error resolving controller {0}", controllerType.Name),
                    ex);
            }

            if (controller == null)
            {
                throw new InvalidOperationException("Error resolving controller");
            }

            controller.Container = this.container;
            return controller;
        }
        
    }
}
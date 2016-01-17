// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnityFilterAttributeFilterProvider.cs" company="BringDream">
//   BringDream 2016
// </copyright>
// <summary>
//   Defines the UnityFilterAttributeFilterProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RainMakr.Web.Unity
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using Microsoft.Practices.Unity;

    /// <summary>
    /// The unity filter attribute filter provider.
    /// </summary>
    public class UnityFilterAttributeFilterProvider : FilterAttributeFilterProvider
    {
        /// <summary>
        /// The container.
        /// </summary>
        private readonly IUnityContainer container;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityFilterAttributeFilterProvider"/> class.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        public UnityFilterAttributeFilterProvider(IUnityContainer container)
        {
            this.container = container;
        }

        /// <summary>
        /// The get action attributes.
        /// </summary>
        /// <param name="controllerContext">
        /// The controller context.
        /// </param>
        /// <param name="actionDescriptor">
        /// The action descriptor.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        protected override IEnumerable<FilterAttribute> GetActionAttributes(
            ControllerContext controllerContext,
            ActionDescriptor actionDescriptor)
        {
            var attributes = base.GetActionAttributes(controllerContext, actionDescriptor).ToList();
            this.BuildUpAttributes(attributes);

            return attributes;
        }

        /// <summary>
        /// The get controller attributes.
        /// </summary>
        /// <param name="controllerContext">
        /// The controller context.
        /// </param>
        /// <param name="actionDescriptor">
        /// The action descriptor.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        protected override IEnumerable<FilterAttribute> GetControllerAttributes(
            ControllerContext controllerContext,
            ActionDescriptor actionDescriptor)
        {
            var attributes = base.GetControllerAttributes(controllerContext, actionDescriptor).ToList();
            this.BuildUpAttributes(attributes);

            return attributes;
        }

        /// <summary>
        /// The build up attributes.
        /// </summary>
        /// <param name="attributes">
        /// The attributes.
        /// </param>
        private void BuildUpAttributes(IEnumerable attributes)
        {
            foreach (FilterAttribute attribute in attributes)
            {
                this.container.BuildUp(attribute.GetType(), attribute);
            }
        }
    }
}
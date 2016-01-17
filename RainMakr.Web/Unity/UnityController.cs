// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnityController.cs" company="BringDream">
//   BringDream 2016
// </copyright>
// <summary>
//   The unity controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RainMakr.Web.Unity
{
    using System.Web.Mvc;

    using Microsoft.Practices.Unity;

    /// <summary>
    /// The unity controller.
    /// </summary>
    public class UnityController : Controller
    {
        /// <summary>
        /// Gets or sets the unity container.
        /// </summary>
        public UnityContainer Container { get; set; }
    }
}

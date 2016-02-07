using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Practices.Unity;

namespace RainMakr.Web.Unity
{
    public class ApiUnityController : ApiController
    {
        /// <summary>
        /// Gets or sets the unity container.
        /// </summary>
        public UnityContainer Container { get; set; }
    }
}

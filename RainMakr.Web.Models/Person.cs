using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainMakr.Web.Models
{
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    public class Person : IdentityUser, IUser
    {
        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        [StringLength(100)]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        [StringLength(100)]
        public string LastName { get; set; }

        public ICollection<Device> Devices { get; set; }
    }
}

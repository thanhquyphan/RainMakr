using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainMakr.Web.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Device
    {
        public Device()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        public string PersonId { get; set; }

        public string IpAddress { get; set; }

        [Required]
        [MaxLength(17)]
        public string MacAddress { get; set; }

        public virtual Person Person { get; set; }

        public ICollection<Schedule> Schedules { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SportsStore.Domain.Entities
{
    public class ShippingDetails
    {
        [Required(ErrorMessage = "Please enter a name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter the first addres line")]
        [Display(Name="Line 1")] //до этого отображалось без пробела Line1--> property.DisplayName ?? ... вызывает Line 1
        public string Line1 { get; set; }
        [Display(Name="Line 2")]
        public string Line2 { get; set; }
        [Display(Name="Line 3")]
        public string Line3 { get; set; }

        [Required(ErrorMessage = "Please enter a city name")]
        public string City { get; set; }

        [Required(ErrorMessage ="Please enter a state name")]
        public string State { get; set; }
        public string Zip { get; set; }

        [Required(ErrorMessage ="Please enter a country name")]
        public string Country { get; set; }
        public bool GiftWrap { get; set; }
    }
}

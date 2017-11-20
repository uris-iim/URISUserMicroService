using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace URISUserMicroService.Models
{
    /// <summary>
    /// UserAddress web api
    /// </summary>
    public class UserAddress
    {
        /// <summary>
        /// UserAddress Id
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// User that is associated with the address
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// The address
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Location zip code
        /// </summary>
        public string ZipCode { get; set; }

        /// <summary>
        /// City part of the address
        /// </summary>
        public string CityName { get; set; }

        /// <summary>
        /// Country part of the address
        /// </summary>
        public string CountryName { get; set; }

        /// <summary>
        /// Geographic latitude of the address
        /// </summary>
        public string Latitude { get; set; }

        /// <summary>
        /// Geographic longitude of the address
        /// </summary>
        public string Longitude { get; set; }

        /// <summary>
        /// Order number of the address within one user
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Indicator if the address is a default one
        /// </summary>
        public bool Default { get; set; }

        /// <summary>
        /// Indicator if the address is active and can be used
        /// </summary>
        public bool Active { get; set; }
    }
}
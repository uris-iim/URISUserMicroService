using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace URISUserMicroService.Models
{
    public class User
    {
        /// <summary>
        /// User Id
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// User name
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Users e-mail address
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Users address
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Zip code
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
        /// User's phone number
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Type of user where he belongs to
        /// </summary>
        [Required]
        public int UserTypeId { get; set; }

        /// <summary>
        /// Hashed password value
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Indicator if the user is active and can be user of the system
        /// </summary>
        [Required]
        public bool Active { get; set; }

        /// <summary>
        /// List of user addresses
        /// </summary>
        public List<UserAddress> UserAddresses { get; set; }

        /// <summary>
        /// Last name of user
        /// </summary>
        public string LastName { get; set; }
    }
}
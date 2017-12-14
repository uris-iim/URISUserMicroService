using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace URISUserMicroService.Models
{
    public class UserType
    {
        /// <summary>
        /// Id of UserType
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of UserType
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Flag to indicate if 
        /// the UserType active or not
        /// </summary>
        public bool Active { get; set; }

    }
}
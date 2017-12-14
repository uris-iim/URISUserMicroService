using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using URISUserMicroService.DataAccess;
using URISUserMicroService.Models;

namespace URISUserMicroService.Controllers
{
    public class UserTypeController
    {
        [Route("/api/UserType"), HttpGet]
        public List<UserType> GetUserTypes()
        {
            return UserTypeDB.GetUserTypes();
        }
    }
}
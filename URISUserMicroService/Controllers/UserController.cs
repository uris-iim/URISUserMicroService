using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using URISUserMicroService.DataAccess;
using URISUserMicroService.Models;
using URISUtil.DataAccess;

namespace URISUserMicroService.Controllers
{
    public class UserController : ApiController
    {
        /// <summary>
        /// Gets all users based on filters
        /// </summary>
        /// <param name="userType">User type</param>
        /// <param name="userName">User name</param>
        /// <param name="active">Indicates if the user is active or not</param>
        /// <param name="order">Ordering</param>
        /// <param name="orderDirection">Order direction (asc/desc)</param>
        /// <returns>List of users</returns>
        [Route("api/User"), HttpGet]
        public IEnumerable<User> GetUsers([FromUri]string userType = null, [FromUri]string userName = null, [FromUri]ActiveStatusEnum active = ActiveStatusEnum.Active, 
                                          [FromUri]UserOrderEnum order = UserOrderEnum.Id, 
                                          [FromUri]OrderEnum orderDirection = OrderEnum.Asc)
        {
            return UserDB.GetUsers(userType, userName, active, order, orderDirection);
        }

        /// <summary>
        /// Get single user based on id
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>Single user</returns>
        [Route("api/User/{id}"), HttpGet]
        public User GetUser(int id)
        {
            return UserDB.GetUser(id);
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="user">User as json</param>
        /// <returns>Created user</returns>        
        [Route("api/User"), HttpPost]
        public User CreateUser([FromBody]User user)
        {
            return UserDB.CreateUser(user);
        }

        /// <summary>
        /// Update a user
        /// </summary>
        /// <param name="user">User as json</param>
        /// <returns>Updated user</returns>
        [Route("api/User"), HttpPut]
        public User UpdateUser([FromBody]User user)
        {
            return UserDB.UpdateUser(user);
        }

        /// <summary>
        /// Set Active status of user to false
        /// </summary>
        /// <param name="id">User Id</param>
        [Route("api/User/{id}"), HttpDelete]
        public void DeleteUser(int id)
        {
            UserDB.DeleteUser(id);
        }
    }
}
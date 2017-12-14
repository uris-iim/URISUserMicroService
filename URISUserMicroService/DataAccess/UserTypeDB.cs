using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using URISUserMicroService.Models;
using URISUtil.DataAccess;

namespace URISUserMicroService.DataAccess
{    
    public static class UserTypeDB
    {
        private static string AllColumnSelect
        {
            get
            {
                return @"
                    [UserType].[Id],
	                [UserType].[Name],
	                [UserType].[Active]
                ";
            }
        }

        public static UserType ReadRow(SqlDataReader reader)
        {
            UserType userType = new UserType();

            userType.Id = (int)reader["Id"];
            userType.Name = reader["Name"] as string;
            userType.Active = (bool)reader["Active"];

            return userType;
        }

        public static List<UserType> GetUserTypes()
        {
            try
            {
                List<UserType> retVal = new List<UserType>();
                using (SqlConnection connection = new SqlConnection(DBFunctions.ConnectionString))
                {
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = String.Format(@"
                        SELECT
                            {0}
                        FROM
                            [user].[UserType]
                    ", AllColumnSelect);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            retVal.Add(ReadRow(reader));
                        }
                    }
                }
                return retVal;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }
    }
}
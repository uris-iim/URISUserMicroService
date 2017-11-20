using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using URISUserMicroService.Models;
using URISUtil.DataAccess;
using URISUtil.Logging;
using URISUtil.Response;

namespace URISUserMicroService.DataAccess
{
    public static class UserDB
    {
        private static User ReadRow(SqlDataReader reader)
        {
            User retVal = new User();

            retVal.Id = (int)reader["Id"];
            retVal.Name = reader["Name"] as string;
            retVal.Email = reader["Email"] as string;
            retVal.Address = reader["Address"] as string;
            retVal.ZipCode = reader["ZipCode"] as string;
            retVal.CityName = reader["CityName"] as string;
            retVal.CountryName = reader["CountryName"] as string;
            retVal.Phone = reader["Phone"] as string;
            retVal.UserTypeId = (int)reader["UserTypeId"];
            retVal.Active = (bool)reader["Active"];

            return retVal;
        }

        private static int ReadId(SqlDataReader reader)
        {
            return (int)reader["Id"];
        }

        private static string AllColumnSelect
        {
            get
            {
                return @"
                    [User].[Id],
	                [User].[Name],
	                [User].[Email],
                    [User].[Address],
                    [User].[ZipCode],
                    [User].[CityName],
                    [User].[CountryName],
                    [User].[Phone],
                    [User].[UserTypeId],
	                [User].[Active]
                ";
            }
        }

        private static void FillData(SqlCommand command, User user)
        {
            command.AddParameter("@Id", SqlDbType.Int, user.Id);
            command.AddParameter("@Name", SqlDbType.NVarChar, user.Name);
            command.AddParameter("@Email", SqlDbType.NVarChar, user.Email);
            command.AddParameter("@Address", SqlDbType.NVarChar, user.Address);
            command.AddParameter("@ZipCode", SqlDbType.NVarChar, user.ZipCode);
            command.AddParameter("@CityName", SqlDbType.NVarChar, user.CityName);
            command.AddParameter("@CountryName", SqlDbType.NVarChar, user.CountryName);
            command.AddParameter("@Phone", SqlDbType.NVarChar, user.Phone);
            command.AddParameter("@UserTypeId", SqlDbType.Int, user.UserTypeId);
            command.AddParameter("@Password", SqlDbType.NVarChar, user.Password);
            command.AddParameter("@Active", SqlDbType.Bit, user.Active);
        }

        private static object CreateLikeQueryString(string str)
        {
            return str == null ? (object)DBNull.Value : "%" + str + "%";
        }

        public static List<User> GetUsers(string userType, string userName, ActiveStatusEnum active, UserOrderEnum order, OrderEnum orderDirection)
        {
            try
            {
                List<User> retVal = new List<User>();

                using (SqlConnection connection = new SqlConnection(DBFunctions.ConnectionString))
                {
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = String.Format(@"
                        SELECT
                            {0}
                        FROM
                            [user].[User]
                            JOIN [user].[UserType] ON [User].[UserTypeId] = [UserType].[Id]
                        WHERE
                            (@UserType IS NULL OR [user].[UserType].Name LIKE @UserType) AND
                            (@UserName IS NULL OR [user].[User].Name LIKE @UserName) AND
                            (@Active IS NULL OR [user].[User].Active = @Active)
                    ", AllColumnSelect);
                    command.Parameters.Add("@UserType", SqlDbType.NVarChar);
                    command.Parameters.Add("@UserName", SqlDbType.NVarChar);
                    command.Parameters.Add("@Active", SqlDbType.Bit);

                    command.Parameters["@UserType"].Value = CreateLikeQueryString(userType);
                    command.Parameters["@UserName"].Value = CreateLikeQueryString(userName);
                    switch (active)
                    {
                        case ActiveStatusEnum.Active:
                            command.Parameters["@Active"].Value = true;
                            break;
                        case ActiveStatusEnum.Inactive:
                            command.Parameters["@Active"].Value = false;
                            break;
                        case ActiveStatusEnum.All:
                            command.Parameters["@Active"].Value = DBNull.Value;
                            break;
                    }                    

                    System.Diagnostics.Debug.WriteLine(command.CommandText);
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
                Logger.WriteLog(ex);
                throw ErrorResponse.ErrorMessage(HttpStatusCode.BadRequest, ex);
            }
        }

        public static User GetUser(int userId)
        {
            try
            {
                User retVal = new User();

                using (SqlConnection connection = new SqlConnection(DBFunctions.ConnectionString))
                {
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = String.Format(@"
                        SELECT
                            {0}
                        FROM
                            [user].[User]
                        WHERE
                            [Id] = @Id
                    ", AllColumnSelect);

                    command.AddParameter("@Id", SqlDbType.Int, userId);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            retVal = ReadRow(reader);
                            retVal.UserAddresses = UserAddressDB.GetUserAddresses(userId);
                        }
                        else
                        {
                            ErrorResponse.ErrorMessage(HttpStatusCode.NotFound);
                        }
                    }
                }

                return retVal;
            }
            catch (Exception ex)
            {
                Logger.WriteLog(ex);
                throw ErrorResponse.ErrorMessage(HttpStatusCode.BadRequest, ex);
            }
        }

        public static User CreateUser(User user)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(DBFunctions.ConnectionString))
                {
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = @"                     

                        INSERT INTO [user].[User]
                        (
                            [Name],
                            [Email],
                            [Address],
                            [ZipCode],
                            [CityName],
                            [CountryName],
                            [Phone],
                            [UserTypeId],
                            [Password],
                            [Active]                
                        )
                        VALUES
                        (
                            @Name,
                            @Email,
                            @Address,
                            @ZipCode,
                            @CityName,
                            @CountryName,
                            @Phone,
                            @UserTypeId,
                            @Password,
                            @Active 
                        )
                        SET @Id = SCOPE_IDENTITY();
						SELECT @Id as Id  
                    ";
                    FillData(command, user);
                    connection.Open();

                    int id = 0;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            id = ReadId(reader);
                        }
                    }

                    if (user.UserAddresses != null && user.UserAddresses.Count > 0)
                    {
                        foreach (UserAddress userAddress in user.UserAddresses)
                        {
                            userAddress.UserId = id;
                            UserAddressDB.CreateAddress(userAddress);
                        }
                    }

                    return GetUser(id);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(ex);
                throw ErrorResponse.ErrorMessage(HttpStatusCode.BadRequest, ex);
            }
        }

        public static User UpdateUser(User user)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(DBFunctions.ConnectionString))
                {
                    if (user.UserAddresses != null && user.UserAddresses.Count > 0)
                    {
                        UserAddressDB.ClearAddresses(user.UserAddresses, user.Id.Value);
                    }
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = String.Format(@"

                        UPDATE
                            [user].[User]
                        SET
                            [Name] = @Name,
                            [Email] = @Email,
                            [Address] = @Address,
                            [ZipCode] = @ZipCode,
                            [CityName] = @CityName,
                            [CountryName] = @CountryName,
                            [Phone] = @Phone,
                            [UserTypeId] = @UserTypeId,                    
                            [Active] = @Active
                        WHERE
                            [Id] = @Id
                    ");
                    FillData(command, user);
                    connection.Open();
                    command.ExecuteNonQuery();
                    if (user.UserAddresses != null && user.UserAddresses.Count > 0)
                    {
                        foreach (UserAddress userAddress in user.UserAddresses)
                        {
                            UserAddressDB.UpdateUserAddress(userAddress, user.Id.Value);
                        }
                    }
                    return GetUser(user.Id.Value);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(ex);
                throw ErrorResponse.ErrorMessage(HttpStatusCode.BadRequest, ex);
            }
        }

        public static void DeleteUser(int userId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(DBFunctions.ConnectionString))
                {
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = String.Format(@"

                        UPDATE
                            [user].[User]
                        SET
                            [Active] = 'False'
                        WHERE
                            [Id] = @Id     
                    ");

                    command.AddParameter("@Id", SqlDbType.Int, userId);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(ex);
                throw ErrorResponse.ErrorMessage(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}
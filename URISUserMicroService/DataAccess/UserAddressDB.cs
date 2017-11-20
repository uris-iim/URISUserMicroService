using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using URISUserMicroService.Models;
using URISUtil.DataAccess;
using URISUtil.Logging;
using URISUtil.Response;

namespace URISUserMicroService.DataAccess
{
    public static class UserAddressDB
    {
        private static UserAddress ReadUserAddressRow(SqlDataReader reader)
        {
            UserAddress retVal = new UserAddress();

            retVal.Id = (int)reader["Id"];
            retVal.UserId = (int)reader["UserId"]; ;
            retVal.Address = reader["Address"] as string;
            retVal.ZipCode = reader["ZipCode"] as string;
            retVal.CityName = reader["CityName"] as string;
            retVal.CountryName = reader["CountryName"] as string;
            retVal.Latitude = reader["Latitude"] as string;
            retVal.Longitude = reader["Longitude"] as string;
            retVal.Order = (int)reader["Order"];
            retVal.Default = (bool)reader["Default"];
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
                    [UserAddress].[Id],
                    [UserAddress].[UserId],
                    [UserAddress].[Address],
                    [UserAddress].[ZipCode],
                    [UserAddress].[CityName],
                    [UserAddress].[CountryName],
                    [UserAddress].[Latitude],
                    [UserAddress].[Longitude],
                    [UserAddress].[Order],
                    [UserAddress].[Default],
                    [UserAddress].[Active]
                ";
            }
        }

        private static void FillData(SqlCommand command, UserAddress userAddress)
        {
            command.AddParameter("@Id", SqlDbType.Int, userAddress.Id);
            command.AddParameter("@UserId", SqlDbType.NVarChar, userAddress.UserId);
            command.AddParameter("@Address", SqlDbType.NVarChar, userAddress.Address);
            command.AddParameter("@ZipCode", SqlDbType.NVarChar, userAddress.ZipCode);
            command.AddParameter("@CityName", SqlDbType.NVarChar, userAddress.CityName);
            command.AddParameter("@CountryName", SqlDbType.NVarChar, userAddress.CountryName);
            command.AddParameter("@Latitude", SqlDbType.NVarChar, userAddress.Latitude);
            command.AddParameter("@Longitude", SqlDbType.NVarChar, userAddress.Longitude);
            command.AddParameter("@Order", SqlDbType.NVarChar, userAddress.Order);
            command.AddParameter("@Default", SqlDbType.Bit, userAddress.Default);
            command.AddParameter("@Active", SqlDbType.Bit, userAddress.Active);
        }

        public static UserAddress GetAddress(int userId, int userAddressId)
        {
            try
            {
                UserAddress retVal = new UserAddress();

                using (SqlConnection connection = new SqlConnection(DBFunctions.ConnectionString))
                {
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = String.Format(@"
                        SELECT
                            {0}
                        FROM
                            [user].[UserAddress]
                        WHERE
                            [Id] = @Id AND
                            [UserId] = @UserId
                    ", AllColumnSelect);

                    command.AddParameter("@Id", SqlDbType.Int, userAddressId);
                    command.AddParameter("@UserId", SqlDbType.Int, userId);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            retVal = ReadUserAddressRow(reader);
                        }
                    }
                    return retVal;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(ex);
                throw ErrorResponse.ErrorMessage(HttpStatusCode.BadRequest, ex);
            }
        }

        public static List<UserAddress> GetUserAddresses(int userId)
        {
            try
            {
                List<UserAddress> retVal = new List<UserAddress>();

                using (SqlConnection connection = new SqlConnection(DBFunctions.ConnectionString))
                {
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = String.Format(@"
                        SELECT
                            {0}
                        FROM
                            [user].[UserAddress]
                        WHERE
                            [UserId] = @Id AND [Active] = 'True'
                    ", AllColumnSelect);
                    command.AddParameter("@Id", SqlDbType.Int, userId);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            retVal.Add(ReadUserAddressRow(reader));
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

        public static List<UserAddress> GetUserAddresses(ActiveStatusEnum active = ActiveStatusEnum.Active)
        {
            try
            {
                List<UserAddress> retVal = new List<UserAddress>();

                using (SqlConnection connection = new SqlConnection(DBFunctions.ConnectionString))
                {
                    SqlCommand command = connection.CreateCommand();                                        
                    command.CommandText = String.Format(@"
                        SELECT
                            {0}
                        FROM
                            [user].[UserAddress]
                        WHERE
                            [Active] = @Active
                    ", AllColumnSelect);

                    command.AddParameter("@Active", SqlDbType.Bit, active);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            retVal.Add(ReadUserAddressRow(reader));
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

        public static UserAddress CreateAddress(UserAddress userAddress)
        {
            int id = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(DBFunctions.ConnectionString))
                {
                    SqlCommand command = connection.CreateCommand();

                    command.CommandText = String.Format(@"

                        INSERT INTO [user].[UserAddress]
                        (
                            [UserId],
                            [Address],
                            [ZipCode],
                            [CityName],
                            [CountryName],
                            [Latitude],
                            [Longitude],
                            [Order],
                            [Default],
                            [Active]               
                        )
                        VALUES
                        (
                            @UserId,
                            @Address,
                            @ZipCode,
                            @CityName,
                            @CountryName,
                            @Latitude,
                            @Longitude,
                            @Order,
                            @Default,
                            @Active
                        )
                        SET @Id = SCOPE_IDENTITY();
						SELECT @Id as Id  
                    ");
                    FillData(command, userAddress);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            id = ReadId(reader);
                        }
                    }
                    return GetAddress((int)userAddress.UserId, id);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(ex);
                throw ErrorResponse.ErrorMessage(HttpStatusCode.BadRequest, ex);
            }
        }

        public static void UpdateUserAddress(UserAddress userAddress, int userId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(DBFunctions.ConnectionString))
                {
                    SqlCommand command = connection.CreateCommand();
                    if (userAddress.Id == null)
                    {
                        command.CommandText = String.Format(@"
                        INSERT INTO [user].[UserAddress] 
                        (
                            [UserId],
                            [Address],
                            [ZipCode],
                            [CityName],
                            [CountryName],
                            [Latitude],
                            [Longitude],
                            [Order],
                            [Default],
                            [Active]
                         ) 
                         VALUES
                         (
                            @uId,
                            @Address,
                            @ZipCode,
                            @CityName,
                            @CountryName,
                            @Latitude,
                            @Longitude,
                            @Order,
                            @Default,
                            @Active
                         )");
                    }
                    else
                    {
                        command.CommandText = String.Format(@"

                        UPDATE
                            [user].[UserAddress]
                        SET
                            [Address] = @Address,
                            [ZipCode] = @ZipCode,
                            [CityName] = @CityName,
                            [CountryName] = @CountryName,
                            [Latitude] = @Latitude,
                            [Longitude] = @Longitude,
                            [Order] = @Order,
                            [Default] = @Default,
                            [Active] = @Active
                        WHERE
                            [Id] = @Id AND 
                            [UserId] = @uId
                        ");
                    }
                    command.AddParameter("@uId", SqlDbType.Int, userId);
                    FillData(command, userAddress);
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

        public static void ClearAddresses(List<UserAddress> userAddresses, int userId)
        {
            List<UserAddress> addresses = GetUserAddresses(userId);
            foreach (UserAddress address in addresses)
            {
                address.Active = false;
                UpdateUserAddress(address, userId);
            }
        }
    }
}
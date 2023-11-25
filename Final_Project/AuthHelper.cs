using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SQLite;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using Final_Project;

namespace Final_Project
{
    public class AuthHelper
    {
        
        private static readonly AuthHelper instance = new AuthHelper();

        
        public static AuthHelper Instance
        {
            get { return instance; }
        }

        
        private readonly string dbPath = HttpContext.Current.Server.MapPath("~/universityDB.db");
        public string DbPath
        {
            get
            {
                return "Data Source=" +
                dbPath + ";Version=3;";
            }
        }

        
        private AuthHelper() { }

        
        public string EncryptPassword(string password)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    stringBuilder.Append(hashBytes[i].ToString("x2"));
                }

                return stringBuilder.ToString();
            }
        }


        
        public bool IsAuthorized(User user, string role)
        {
            if (user != null && user.RoleName.Equals(role, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        
        public void SaveToSession(string key, object value)
        {
            HttpContext.Current.Session[key] = value;
        }

        
        public T GetFromSession<T>(string key)
        {
            return (T)HttpContext.Current.Session[key];
        }

        
        public void UpdateUserInSession(int userId)
        {
            
            User updatedUser = GetUserById(userId);

            
            SaveToSession("CurrentUser", updatedUser);
        }

        
        public User GetUserById(int userId)
        {
            
            using (SQLiteConnection connection = new SQLiteConnection(DbPath))
            {
                connection.Open();

                string query = @"
            SELECT UserID, Username, RoleName, Name, Surname, DOB, 
                   Nationality, IDNumber, Address
            FROM User
            INNER JOIN Role ON User.RoleID = Role.RoleID
            WHERE UserID = @UserID";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userId);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            
                            User user = new User(
                                userId: Convert.ToInt32(reader["UserID"]),
                                username: Convert.ToString(reader["Username"]),
                                roleName: Convert.ToString(reader["RoleName"]),
                                name: Convert.ToString(reader["Name"]),
                                surname: Convert.ToString(reader["Surname"]),
                                dob: Convert.ToDateTime(reader["DOB"]),
                                nationality: Convert.ToString(reader["Nationality"]),
                                idNumber: Convert.ToString(reader["IDNumber"]),
                                address: Convert.ToString(reader["Address"])
                            );

                            return user;
                        }
                    }
                }
            }

            
            return null;
        }
    }
}
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
        //Singleton Design Pattern in order to only have one instance of this class
        //so that in the future if we have important fields, we dont have different values of data
        //between instances
        private static readonly AuthHelper instance = new AuthHelper();

        
        public static AuthHelper Instance
        {
            get { return instance; }
        }

        //It loads the whole url to the DataBase
        private readonly string dbPath = HttpContext.Current.Server.MapPath("~/universityDB.db");
        
        //Property to get the dbPath, we should have also done the ^httpcontext.... here 
        public string DbPath
        {
            get
            {
                return "Data Source=" +
                dbPath + ";Version=3;";
            }
        }

        //Part of the Singleton Pattern, Private Constructor
        private AuthHelper() { }

        //Takes a password and returns it encrypted using the MD5 Hash Algorithm
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


        //Checks if an instance of User is some role, returns V/F
        public bool IsAuthorized(User user, string role)
        {
            if (user != null && user.RoleName.Equals(role, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        //Method to save to the session, with a key and a value which is an object
        public void SaveToSession(string key, object value)
        {
            HttpContext.Current.Session[key] = value;
        }

        //Method to get the corresponding object saved in the session, maybe user
        public T GetFromSession<T>(string key)
        {
            return (T)HttpContext.Current.Session[key];
        }

        //Updates the user data, only used by Students when updating their information
        public void UpdateUserInSession(int userId)
        {
            
            User updatedUser = GetUserById(userId);

            
            SaveToSession("CurrentUser", updatedUser);
        }

        
        //Given 1 User ID we return the User Instance, using the User.cs class
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
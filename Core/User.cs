using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Core
{
    public class User
    {
        // Properties
        public int UserID { get; set; } // Primary Key
        public string Username { get; set; }
        public string RoleName { get; set; } // New property for RoleName
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime DOB { get; set; }
        public string Nationality { get; set; }
        public string IDNumber { get; set; }
        public string Address { get; set; }

        // Constructor
        public User(int userId, string username, string roleName, string name, string surname,
                    DateTime dob, string nationality, string idNumber, string address)
        {
            UserID = userId;
            Username = username;
            RoleName = roleName;
            Name = name;
            Surname = surname;
            DOB = dob;
            Nationality = nationality;
            IDNumber = idNumber;
            Address = address;
        }
    }
}

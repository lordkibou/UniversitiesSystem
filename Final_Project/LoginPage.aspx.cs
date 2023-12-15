using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Final_Project
{
    public partial class LoginPage : System.Web.UI.Page
    {
        //Field for authhelper
        private AuthHelper authHelper;

        //When page loads we instance the class AuthHelper using Singleton Pattern

        protected void Page_Load(object sender, EventArgs e)
        {
            authHelper = AuthHelper.Instance;
        }

        //Method to perform the login, returns an user if its found with the given credentials
        private User Login(string username, string password)
        {
            try
            {
                //We encrypt the password with MD5 from AuthHelper function
                string encryptedPassword = authHelper.EncryptPassword(password);

                //We do this using in order not to have to open and then close the connection 
                //manually
           
                using (SQLiteConnection connection = new SQLiteConnection(authHelper.DbPath))
                {
                    //We use the path that was establised in AuthHelper
                    connection.Open();

                    //Query to find the user with the credentials
                    string query = @"
                SELECT UserID, Username, RoleName, Name, Surname, DOB, 
                Nationality, IDNumber, Address
                FROM User
                INNER JOIN Role ON User.RoleID = Role.RoleID
                WHERE Username = @Username AND Password = @Password";

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Password", encryptedPassword);
                        //We stablish the command and the reader with the given parameters
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            //If it finds a user we instance the user with his data
                            //So that we can save him into the session and redirect to personal page
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

                //Else we return no user
                return null;
            }
            //We use "try" and "catch" in order to catch exceptions and dont let the website crash
            catch (Exception ex)
            {
                
                throw new Exception("Inicio de sesión fallido. Inténtelo de nuevo.", ex);
            }
        }


        //When click in the login button
        protected void LoginButton_Click(object sender, EventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordTextBox.Text;

            //Perform the Login function with the collected user and password from inputs
            User user = Login(username, password);

            if (user != null)
            {
                //If there is a user, we save it to the session using state management
                authHelper.SaveToSession("CurrentUser", user);

                //We check the user's role, and redirect based on its role
                if (authHelper.IsAuthorized(user, "Administrator"))
                {
                    Response.Redirect("AdminPage.aspx");
                }
                else if (authHelper.IsAuthorized(user, "Professor"))
                {
                    Response.Redirect("ProfessorPage.aspx");
                }
                else if (authHelper.IsAuthorized(user, "Student"))
                {
                    Response.Redirect("StudentPage.aspx");
                }
            }
            else
            {
                //If there is no user we show the error in the credentials to the user
                ErrorMessageLabel.Text = "Invalid username or password. Please try again.";
                ErrorMessageLabel.Visible = true;
            }
        }

    }
}

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
        private AuthHelper authHelper;

        protected void Page_Load(object sender, EventArgs e)
        {
            authHelper = AuthHelper.Instance;
        }

        
        private User Login(string username, string password)
        {
            try
            {
                
                string encryptedPassword = authHelper.EncryptPassword(password);

                using (SQLiteConnection connection = new SQLiteConnection(authHelper.DbPath))
                {
                    connection.Open();

                    
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
            catch (Exception ex)
            {
                
                throw new Exception("Inicio de sesión fallido. Inténtelo de nuevo.", ex);
            }
        }



        protected void LoginButton_Click(object sender, EventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordTextBox.Text;

            
            User user = Login(username, password);

            if (user != null)
            {
                
                authHelper.SaveToSession("CurrentUser", user);

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
                
                ErrorMessageLabel.Text = "Invalid username or password. Please try again.";
                ErrorMessageLabel.Visible = true;
            }
        }

    }
}

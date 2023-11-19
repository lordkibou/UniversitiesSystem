using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System;
using System.Data.SQLite;
using System.Web.UI;

namespace Core
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        private AuthHelper authHelper;

        protected void Page_Load(object sender, EventArgs e)
        {
            authHelper = AuthHelper.Instance;
        }

        // Método para el inicio de sesión del usuario
        private User Login(string username, string password)
        {
            try
            {
                // Cifra la contraseña antes de compararla en la base de datos
                string encryptedPassword = authHelper.EncryptPassword(password);

                using (SQLiteConnection connection = new SQLiteConnection(authHelper.DbPath))
                {
                    connection.Open();

                    // Cambié la consulta para obtener el nombre del rol
                    string query = @"
                    SELECT u.UserID, u.Username, r.RoleName, u.Name, u.Surname, u.DOB, 
                           u.Nationality, u.IDNumber, u.Address
                    FROM User u
                    INNER JOIN Role r ON u.RoleID = r.RoleID
                    WHERE u.Username = @Username AND u.Password = @Password";

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Password", encryptedPassword);

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Crea el objeto User directamente con todas las propiedades
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

                // Si las credenciales son inválidas o el usuario no se encuentra, devuelve null
                return null;
            }
            catch (Exception ex)
            {
                // Registra la excepción o presenta un mensaje de error
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
                // Save user to session
                authHelper.SaveToSession("CurrentUser", user);

                // Redirect based on user role
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
                // Display error message
                ErrorMessageLabel.Text = "Invalid username or password. Please try again.";
            }
        }

        protected void LoginButton1_Click(object sender, EventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordTextBox.Text;

            User user = Login(username, password);

            if (user != null)
            {
                // Save user to session
                authHelper.SaveToSession("CurrentUser", user);

                // Redirect based on user role
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
                // Display error message
                ErrorMessageLabel.Text = "Invalid username or password. Please try again.";
            }
        }
    }
}

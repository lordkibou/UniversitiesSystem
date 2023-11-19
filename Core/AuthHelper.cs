using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SQLite;
using System.Security.Cryptography;
using System.Text;

namespace Core
{
    public class AuthHelper
    {
        // Implementación del patrón Singleton
        private static readonly AuthHelper instance = new AuthHelper();

        // Propiedad pública para acceder a la única instancia
        public static AuthHelper Instance
        {
            get { return instance; }
        }

        private readonly string connectionString = "Data Source=university.db;Version=3;";

        // Constructor privado para evitar la creación de instancias adicionales
        private AuthHelper() { }

        // Método para cifrar la contraseña usando MD5
        private string EncryptPassword(string password)
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

        // Método para el inicio de sesión del usuario
        public User Login(string username, string password)
        {
            try
            {
                // Cifra la contraseña antes de compararla en la base de datos
                string encryptedPassword = EncryptPassword(password);

                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
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

        // Método para verificar la autorización del usuario según el rol
        public bool IsAuthorized(User user, string role)
        {
            if (user != null && user.RoleName.Equals(role, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }


        // Nueva función para guardar un objeto en la sesión
        public void SaveToSession(string key, object value)
        {
            HttpContext.Current.Session[key] = value;
        }

        // Nueva función para obtener un objeto de la sesión
        public T GetFromSession<T>(string key)
        {
            return (T)HttpContext.Current.Session[key];
        }
    }
}

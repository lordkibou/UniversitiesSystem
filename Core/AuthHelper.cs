using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SQLite;
using System.Security.Cryptography;
using System.Text;
using System.IO;

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

        // Field and property for the database path
        private readonly string dbPath = HttpContext.Current.Server.MapPath("~/universityDB.db");
        public string DbPath
        {
            get { return "Data Soue=" +
                dbPath + ";Version=3;"; }
        }

        // Constructor privado para evitar la creación de instancias adicionales
        private AuthHelper() { }

        // Método para cifrar la contraseña usando MD5
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


        // Método para verificar la autorización del usuario según el rol
        public bool IsAuthorized(User user, string role)
        {
            if (user != null && user.RoleName.Equals(role, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }



    }
}

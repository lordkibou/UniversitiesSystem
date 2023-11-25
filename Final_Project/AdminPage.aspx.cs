using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Final_Project;

namespace Final_Project
{
    public partial class AdminPage : System.Web.UI.Page
    {
        private AuthHelper authHelper;
        protected void Page_Load(object sender, EventArgs e)
        {

            authHelper = AuthHelper.Instance;

            
            if (!IsUserAuthenticated() || !IsUserAdmin())
            {
                Response.Redirect("LoginPage.aspx");
            }

            if (!IsPostBack)
            {
                LoadUsersToDelete();
                LoadSubjectsInformation();
            }

            ScriptManager.ScriptResourceMapping.AddDefinition("jquery", new ScriptResourceDefinition
            {
                Path = "~/Scripts/jquery-3.3.1.min.js",
                DebugPath = "~/Scripts/jquery-3.3.1.js",
                CdnPath = "https://code.jquery.com/jquery-3.3.1.min.js",
                CdnDebugPath = "https://code.jquery.com/jquery-3.3.1.js"
            });

        }

        
        private bool IsUserAuthenticated()
        {
            return authHelper.GetFromSession<User>("CurrentUser") != null;
        }

       
        private bool IsUserAdmin()
        {
            User currentUser = authHelper.GetFromSession<User>("CurrentUser");
            return authHelper.IsAuthorized(currentUser, "Administrator");
        }

        protected void InsertUserButton_Click(object sender, EventArgs e)
        {
            try
            {
                
                string newUserName = NewUserNameTextBox.Text;
                string newUserSurname = NewUserSurnameTextBox.Text;
                string newUserPassword = NewUserPasswordTextBox.Text;
                string newUserDOB = NewUserDOBTextBox.Text;
                string newUserNationality = NewUserNationalityTextBox.Text;
                string newUserID = NewUserIDTextBox.Text;
                string newUserAddress = NewUserAddressTextBox.Text;
                string newUserRole = NewUserRoleDropDown.SelectedValue;
                string newUserUsername = NewUserUsernameTextBox.Text;

                
                if (IsValidUserInput(newUserName, newUserSurname, newUserPassword, newUserDOB, newUserNationality, newUserID, newUserAddress, newUserRole, newUserUsername))
                {
                    
                    string encryptedPassword = AuthHelper.Instance.EncryptPassword(newUserPassword);

                    
                    InsertNewUser(newUserName, newUserSurname, encryptedPassword, newUserDOB, newUserNationality, newUserID, newUserAddress, newUserRole, newUserUsername);

                    
                    ClearUserFields();

                    
                    Response.Write("<script>alert('Usuario insertado exitosamente.');</script>");
                }
            }
            catch (Exception ex)
            {
                
                Response.Write("<script>alert('Error al insertar usuario. " + ex.Message + "');</script>");
            }
        }

        
        private bool IsValidUserInput(string userName, string userSurname, string userPassword, string userDOB, string userNationality, string userID, string userAddress, string userRole, string userUsername)
        {
            
            if (!System.Text.RegularExpressions.Regex.IsMatch(userName, "^[a-zA-ZáéíóúüÁÉÍÓÚÜñÑ\\s]+$"))
            {
                return false;
            }

            return true;
        }


        private void ClearUserFields()
        {
            NewUserNameTextBox.Text = string.Empty;
            NewUserSurnameTextBox.Text = string.Empty;
            NewUserPasswordTextBox.Text = string.Empty;
            NewUserDOBTextBox.Text = string.Empty;
            NewUserNationalityTextBox.Text = string.Empty;
            NewUserIDTextBox.Text = string.Empty;
            NewUserAddressTextBox.Text = string.Empty;
            NewUserRoleDropDown.SelectedIndex = 0; 
        }



        private void InsertNewUser(string userName, string userSurname, string userPassword, string userDOB, string userNationality, string userID, string userAddress, string userRole, string userUsername)
        {
            string insertQuery = @"
        INSERT INTO User (Username, Password, Name, Surname, DOB, Nationality, IDNumber, Address, RoleID)
        VALUES (@UserUsername, @Password, @Name, @Surname, @DOB, @Nationality, @IDNumber, @Address, @RoleID);
    ";


            using (SQLiteConnection connection = new SQLiteConnection(authHelper.DbPath))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                {
                    
                    command.Parameters.AddWithValue("@UserUsername", userUsername); 
                    command.Parameters.AddWithValue("@Password", userPassword);
                    command.Parameters.AddWithValue("@Name", userName);
                    command.Parameters.AddWithValue("@Surname", userSurname);
                    command.Parameters.AddWithValue("@DOB", userDOB);
                    command.Parameters.AddWithValue("@Nationality", userNationality);
                    command.Parameters.AddWithValue("@IDNumber", userID);
                    command.Parameters.AddWithValue("@Address", userAddress);
                    command.Parameters.AddWithValue("@RoleID", GetRoleIDByName(userRole));

                    
                    command.ExecuteNonQuery();
                }
            }

            
            Response.Redirect(Request.RawUrl);
        }



        
        private int GetRoleIDByName(string roleName)
        {
            using (SQLiteConnection connection = new SQLiteConnection(authHelper.DbPath))
            {
                connection.Open();
                string query = "SELECT RoleID FROM Role WHERE RoleName = @RoleName;";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RoleName", roleName);

                    
                    object result = command.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : -1; 
                }
            }
        }


        
        private void LoadUsersToDelete()
        {
            DataTable usersTable = GetUsersToDelete();
            UsersToDeleteGridView.DataSource = usersTable;
            UsersToDeleteGridView.DataBind();
        }

        private DataTable GetUsersToDelete()
        {
            using (SQLiteConnection connection = new SQLiteConnection(authHelper.DbPath))
            {
                connection.Open();
                string query = @"
            SELECT UserID, Name, Surname, RoleName
            FROM User
            JOIN Role ON User.RoleID = Role.RoleID
            WHERE RoleName IN ('Student', 'Professor');

        ";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }

        protected void UsersToDeleteGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int userId = Convert.ToInt32(e.Keys["UserID"]);

            DeleteUser(userId);

            Response.Redirect(Request.RawUrl);
        }

        private void DeleteUser(int userId)
                            {
                                string deleteQuery = @"
                        DELETE FROM User
                        WHERE UserID = @UserID;
                    ";

            
                                string deleteTeachingQuery = @"
                        DELETE FROM Teaching
                        WHERE UserID = @UserID;
                    ";

           
                                string deleteEnrollmentQuery = @"
                        DELETE FROM Enrollment
                        WHERE UserID = @UserID;
                    ";

           
            using (SQLiteConnection connection = new SQLiteConnection(authHelper.DbPath))
            {
                connection.Open();

                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (SQLiteCommand command = new SQLiteCommand(deleteQuery, connection, transaction))
                        {
                            
                            command.Parameters.AddWithValue("@UserID", userId);

                            
                            command.ExecuteNonQuery();
                        }

                        
                        using (SQLiteCommand commandTeaching = new SQLiteCommand(deleteTeachingQuery, connection, transaction))
                        {
                            commandTeaching.Parameters.AddWithValue("@UserID", userId);
                            commandTeaching.ExecuteNonQuery();
                        }

                        using (SQLiteCommand commandEnrollment = new SQLiteCommand(deleteEnrollmentQuery, connection, transaction))
                        {
                            commandEnrollment.Parameters.AddWithValue("@UserID", userId);
                            commandEnrollment.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        Response.Write("<script>alert('Error deleting user. " + ex.Message + "');</script>");
                        transaction.Rollback();
                    }
                }
            }

        }


        
        private void LoadSubjectsInformation()
        {
            DataTable subjectsTable = GetSubjectsInformation();
            SubjectsGridView.DataSource = subjectsTable;
            SubjectsGridView.DataBind();
        }

        private DataTable GetSubjectsInformation()
        {
            using (SQLiteConnection connection = new SQLiteConnection(authHelper.DbPath))
            {
                connection.Open();
                string query = @"
            SELECT SubjectID, SubjectName, Credits, Semester
            FROM Subject
        ";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }




        protected void SubjectsGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditUsers")
            {
                editUsersPopup.Style["display"] = "block";
                
                int subjectID = Convert.ToInt32(e.CommandArgument);

                
                LoadUsersIntoListBox(subjectID);

                
            }
        }

        private void LoadUsersIntoListBox(int subjectID)
        {
            ViewState["GlobalSubjectID"] = subjectID;
            
            Dictionary<int, string> allUsersInfo = GetAllUsersInfo();

            
            List<int> enrolledUserIds = GetUsersForSubject(subjectID);

            
            UserCheckBoxList.Items.Clear();

            
            foreach (var userInfo in allUsersInfo)
            {
                ListItem listItem = new ListItem($"{userInfo.Key} - {userInfo.Value}", userInfo.Key.ToString());

                
                if (enrolledUserIds.Contains(userInfo.Key))
                {
                    listItem.Selected = true;
                }

                UserCheckBoxList.Items.Add(listItem);
            }
        }

        private Dictionary<int, string> GetAllUsersInfo()
        {
            Dictionary<int, string> usersInfo = new Dictionary<int, string>();

            using (SQLiteConnection connection = new SQLiteConnection(authHelper.DbPath))
            {
                connection.Open();
                string query = @"
        SELECT UserID, 'Student' as Role
        FROM User
        WHERE RoleID = (SELECT RoleID FROM Role WHERE RoleName = 'Student')
        UNION
        SELECT UserID, 'Professor' as Role
        FROM User
        WHERE RoleID = (SELECT RoleID FROM Role WHERE RoleName = 'Professor');
    ";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int userId = reader.GetInt32(0);
                            string userRole = reader.GetString(1);
                            usersInfo.Add(userId, userRole);
                        }
                    }
                }
            }

            return usersInfo;
        }


        private List<int> GetUsersForSubject(int subjectID)
        {
            List<int> userIds = new List<int>();

            using (SQLiteConnection connection = new SQLiteConnection(authHelper.DbPath))
            {
                connection.Open();
                string query = @"
        SELECT UserID
        FROM Enrollment
        WHERE SubjectID = @SubjectID
        UNION
        SELECT UserID
        FROM Teaching
        WHERE SubjectID = @SubjectID;
    ";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SubjectID", subjectID);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int userId = reader.GetInt32(0);
                            userIds.Add(userId);
                        }
                    }
                }
            }

            return userIds;
        }


        private bool IsRelationExists(int subjectID, int userID)
        {
            string userRole = GetUserRole(userID);
            using (SQLiteConnection connection = new SQLiteConnection(authHelper.DbPath))
            {
                connection.Open();
                string query = "";

                if (userRole == "Student")
                {
                    query = "SELECT COUNT(*) FROM Enrollment WHERE SubjectID = @SubjectID AND UserID = @UserID;";
                }
                else if (userRole == "Professor")
                {
                    query = "SELECT COUNT(*) FROM Teaching WHERE SubjectID = @SubjectID AND UserID = @UserID;";
                }

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SubjectID", subjectID);
                    command.Parameters.AddWithValue("@UserID", userID);

                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        private string GetUserRole(int userID)
        {
            using (SQLiteConnection connection = new SQLiteConnection(authHelper.DbPath))
            {
                connection.Open();
                string query = "SELECT RoleName FROM Role WHERE RoleID = (SELECT RoleID FROM User WHERE UserID = @UserID);";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userID);
                    return Convert.ToString(command.ExecuteScalar());
                }
            }
        }


        protected void ConfirmButton_Click(object sender, EventArgs e)
        {

            
            int subjectID = (int)ViewState["GlobalSubjectID"];

            
            foreach (ListItem listItem in UserCheckBoxList.Items)
            {
                int userId = Convert.ToInt32(listItem.Value);

                if (listItem.Selected)
                {
                    
                    if (!IsRelationExists(subjectID, userId))
                    {
                        
                        string userRole = GetUserRole(userId);
                        if (userRole == "Student")
                        {
                            CreateEnrollment(subjectID, userId);
                        }
                        else if (userRole == "Professor")
                        {
                            CreateTeaching(subjectID, userId);
                        }
                    }
                }
                else
                {
                   
                    string userRole = GetUserRole(userId);
                    if (userRole == "Student")
                    {
                        DeleteEnrollment(subjectID, userId);
                    }
                    else if (userRole == "Professor")
                    {
                        DeleteTeaching(subjectID, userId);
                    }
                }
            }
            editUsersPopup.Style["display"] = "none";
        }


        private void CreateEnrollment(int subjectID, int userID)
        {
            using (SQLiteConnection connection = new SQLiteConnection(authHelper.DbPath))
            {
                connection.Open();
                string query = "INSERT INTO Enrollment (SubjectID, UserID, Year) VALUES (@SubjectID, @UserID, @Year);";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SubjectID", subjectID);
                    command.Parameters.AddWithValue("@UserID", userID);
                    command.Parameters.AddWithValue("@Year", 2023);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void CreateTeaching(int subjectID, int userID)
        {
            using (SQLiteConnection connection = new SQLiteConnection(authHelper.DbPath))
            {
                connection.Open();
                string query = "INSERT INTO Teaching (SubjectID, UserID, Year) VALUES (@SubjectID, @UserID, @Year);";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SubjectID", subjectID);
                    command.Parameters.AddWithValue("@UserID", userID);
                    command.Parameters.AddWithValue("@Year", 2023);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void DeleteEnrollment(int subjectID, int userID)
        {
            using (SQLiteConnection connection = new SQLiteConnection(authHelper.DbPath))
            {
                connection.Open();
                string query = "DELETE FROM Enrollment WHERE SubjectID = @SubjectID AND UserID = @UserID;";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SubjectID", subjectID);
                    command.Parameters.AddWithValue("@UserID", userID);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void DeleteTeaching(int subjectID, int userID)
        {
            using (SQLiteConnection connection = new SQLiteConnection(authHelper.DbPath))
            {
                connection.Open();
                string query = "DELETE FROM Teaching WHERE SubjectID = @SubjectID AND UserID = @UserID;";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SubjectID", subjectID);
                    command.Parameters.AddWithValue("@UserID", userID);
                    command.ExecuteNonQuery();
                }
            }
        }

    }
}
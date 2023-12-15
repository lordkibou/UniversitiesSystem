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
            //Fields
            authHelper = AuthHelper.Instance;

            //If there is no user in the session or the user in session is not admin
            //redirect to LoginPage
            if (!IsUserAuthenticated() || !IsUserAdmin())
            {
                Response.Redirect("LoginPage.aspx");
            }

            //If this is the first time we load the page we load everything by default
            if (!IsPostBack)
            {
                LoadUsersToDelete();
                LoadSubjectsInformation();
            }

            //it loads the required scripts to make the website work properly
            ScriptManager.ScriptResourceMapping.AddDefinition("jquery", new ScriptResourceDefinition
            {
                Path = "~/Scripts/jquery-3.3.1.min.js",
                DebugPath = "~/Scripts/jquery-3.3.1.js",
                CdnPath = "https://code.jquery.com/jquery-3.3.1.min.js",
                CdnDebugPath = "https://code.jquery.com/jquery-3.3.1.js"
            });

        }

        //Checks if there is a user in the session
        private bool IsUserAuthenticated()
        {
            return authHelper.GetFromSession<User>("CurrentUser") != null;
        }


        //Checks if the user from the session is an admin
        private bool IsUserAdmin()
        {
            User currentUser = authHelper.GetFromSession<User>("CurrentUser");
            return authHelper.IsAuthorized(currentUser, "Administrator");
        }
        //method to add functionallyty to the insert user button
        //it adds the new data written by the admin in the database as a new user 
        protected void InsertUserButton_Click(object sender, EventArgs e)
        {
            try
            {
                //retrieves the data from all the textboxes
                string newUserName = NewUserNameTextBox.Text;
                string newUserSurname = NewUserSurnameTextBox.Text;
                string newUserPassword = NewUserPasswordTextBox.Text;
                string newUserDOB = NewUserDOBTextBox.Text;
                string newUserNationality = NewUserNationalityTextBox.Text;
                string newUserID = NewUserIDTextBox.Text;
                string newUserAddress = NewUserAddressTextBox.Text;
                string newUserRole = NewUserRoleDropDown.SelectedValue;
                string newUserUsername = NewUserUsernameTextBox.Text;

                //if the witten data is valid it encrypts the password in MD5 Hash and uploads it to the database
                if (IsValidUserInput(newUserName, newUserSurname, newUserPassword, newUserDOB, newUserNationality, newUserID, newUserAddress, newUserRole, newUserUsername))
                {
                    
                    string encryptedPassword = AuthHelper.Instance.EncryptPassword(newUserPassword);

                    
                    InsertNewUser(newUserName, newUserSurname, encryptedPassword, newUserDOB, newUserNationality, newUserID, newUserAddress, newUserRole, newUserUsername);

                    //clears the inputs to be alble to add many users quickly
                    ClearUserFields();

                    
                    Response.Write("<script>alert('Usuario insertado exitosamente.');</script>");
                }
            }
            //if theres an error uploading a user it catches the error and avoids crashing the website
            catch (Exception ex)
            {
                
                Response.Write("<script>alert('Error al insertar usuario. " + ex.Message + "');</script>");
            }
        }

        //this method checks if the written data in the inputs to create users is valid 
        private bool IsValidUserInput(string userName, string userSurname, string userPassword, string userDOB, string userNationality, string userID, string userAddress, string userRole, string userUsername)
        {
            
            if (!System.Text.RegularExpressions.Regex.IsMatch(userName, "^[a-zA-ZáéíóúüÁÉÍÓÚÜñÑ\\s]+$"))
            {
                return false;
            }

            return true;
        }

        //clears all the input fields about adding users
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


        //method that uploads the new user to the database
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



        //this method retrieves the corresponding role name from a role ID 
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


        //loads the users in the delete table
        private void LoadUsersToDelete()
        {
            DataTable usersTable = GetUsersToDelete();
            UsersToDeleteGridView.DataSource = usersTable;
            UsersToDeleteGridView.DataBind();
        }
        //retrieves all the users to be added in the delete table
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
                        DataTable dataTable = new DataTable(); //fills the actual table with the data
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }
        //method that deletes the selected user by its ID
        protected void UsersToDeleteGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int userId = Convert.ToInt32(e.Keys["UserID"]);

            DeleteUser(userId);

            Response.Redirect(Request.RawUrl);
        }
        //the specific method that goes to the database and deletes the user identifing it by their ID, deleting also all
        //their teching subjects in case they're a professor and enrollments in case they're a student
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


        //Function to load the subjects info into the data, using Bind
        private void LoadSubjectsInformation()
        {
            DataTable subjectsTable = GetSubjectsInformation();
            SubjectsGridView.DataSource = subjectsTable;
            SubjectsGridView.DataBind();
        }

        //Function to get all the subjects information from the Database and fill in the Adapter
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



        //Function to show the popup and load the students in the popup
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
            //WE USE VIEW STATE, state management to save the subject we are dealing with
            // when adding or deleting users from a subject in the popup
            ViewState["GlobalSubjectID"] = subjectID;
            
            //ID from user and Role in the Dictionary in order to show it in the UI
            Dictionary<int, string> allUsersInfo = GetAllUsersInfo();

            
            List<int> enrolledUserIds = GetUsersForSubject(subjectID);

            
            //We clear checkboxes list
            UserCheckBoxList.Items.Clear();

            //For Each User check if they have relation with the subject 
            //fill the checkboxes of the ones that have relation with the subject
            //anyways, we load all the users and show who has relation with the subject
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

        //Load all the users ID and Role for the UI on the popup for each subject
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


        //Get all the users and return a list of IDs from those who have relation with a
        //subject, this is used in the popup for each subject in the admin
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

        //Checks if a given user has relationship with a subject both for professor and student
        //this is called when we confirm changes in the popup for subject in admin
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

        //Given the ID of a User returns only his Role
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

        //When we confirm in the popup, this happens
        protected void ConfirmButton_Click(object sender, EventArgs e)
        {

            //We get the subjectID we had saved in the ViewState, when we chose a subject to edit
            int subjectID = (int)ViewState["GlobalSubjectID"];

            
            //for each checkbox in the list
            foreach (ListItem listItem in UserCheckBoxList.Items)
            {

                //We get the id
                int userId = Convert.ToInt32(listItem.Value);

                //If the checkbox is selected
                if (listItem.Selected)
                {
                    //Checkbox selected and no relation in the database yet, means that we add it
                    if (!IsRelationExists(subjectID, userId))
                    {
                        //We add the relation between the user either student or professor
                        //with the chose subject
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
                   //If the checkbox is not selected we dont care if the user has relation or not
                   //We just delete the relation of that user from the database
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


        //Function that creates the relation of a student with studying a subject, enrolling it 
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

        //Function that creates the relation of a professor to teach a subject
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

        //Function that deletes the relation of a student with studying a subject, deleting its enrollement
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

        //Function that deletes the relation of a professor with teaching a subject
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
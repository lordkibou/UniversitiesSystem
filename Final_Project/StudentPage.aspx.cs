using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Final_Project
{
    public partial class StudentPage : System.Web.UI.Page
    {
        //Creates or loads? an instance of authentication
        private AuthHelper authHelper;


        //When the Student Page loads, before showing any info (white screen) it checks if the user is authenticated
        
        protected void Page_Load(object sender, EventArgs e)
        {
            authHelper = AuthHelper.Instance;
            //if the user is not authenticated or the authenticated user is not a student, it redirects to the login page
            if (!IsUserAuthenticated() || !IsUserStudent())
            {
                Response.Redirect("LoginPage.aspx");
            }
            //if the user authenticated is a student, it loads his/her information and subjects,
            //and has by default the year 2023 as actual
            if (!IsPostBack)
            {
                LoadStudentInfo();
                
                int defaultYear = 2023;

                
                LoadEnrolledSubjects(defaultYear);
            }
            //this only loads the necessary scripts for the website to work properly
            ScriptManager.ScriptResourceMapping.AddDefinition("jquery", new ScriptResourceDefinition
            {
                Path = "~/Scripts/jquery-3.3.1.min.js",
                DebugPath = "~/Scripts/jquery-3.3.1.js",
                CdnPath = "https://code.jquery.com/jquery-3.3.1.min.js",
                CdnDebugPath = "https://code.jquery.com/jquery-3.3.1.js"
            });


        }
        //method to check if the user is authenticated
        private bool IsUserAuthenticated()
        {
            return authHelper.GetFromSession<User>("CurrentUser") != null;
        }

        //method to check if the authenticated user has a student role
        private bool IsUserStudent()
        {
            User currentUser = authHelper.GetFromSession<User>("CurrentUser");
            return authHelper.IsAuthorized(currentUser, "Student");
        }
        //method that loads the student personal info
        private void LoadStudentInfo()
        {
            User currentUser = authHelper.GetFromSession<User>("CurrentUser");
            StudentNameLabel.Text = currentUser.Name + " " + currentUser.Surname;
            StudentDOBLabel.Text = currentUser.DOB.ToShortDateString();
            StudentNationalityLabel.Text = currentUser.Nationality;
            StudentIDLabel.Text = currentUser.IDNumber;
            StudentAddressLabel.Text = currentUser.Address;
        }
        //method that loads the enrolled subjects of the autheticated student on a table
        private void LoadEnrolledSubjects(int year)
        {
            User currentUser = authHelper.GetFromSession<User>("CurrentUser");
            DataTable subjectsTable = GetEnrolledSubjectsWithProfessors(currentUser.UserID, year);
            EnrolledSubjectsGridView.DataSource = subjectsTable;
            EnrolledSubjectsGridView.DataBind();
        }

        //method that actually retrieves the enrolled subjects data of the authenticated student with their professors names
        private DataTable GetEnrolledSubjectsWithProfessors(int studentId, int year)
        {
            using (SQLiteConnection connection = new SQLiteConnection(authHelper.DbPath))
            {
                connection.Open();
                string query = @"
                  SELECT
                      Subject.SubjectName,
                      Subject.Credits,
                      Subject.Semester,
                      GROUP_CONCAT(User.Name || ' ' || User.Surname) AS ProfessorNames
                        FROM
                        Enrollment
                        INNER JOIN
                        Subject ON Enrollment.SubjectID = Subject.SubjectID
                        INNER JOIN
                            Teaching ON Subject.SubjectID = Teaching.SubjectID
                        INNER JOIN
                        User ON Teaching.UserID = User.UserID
                        WHERE
                        Enrollment.UserID = @StudentID AND Enrollment.Year = @Year
                        GROUP BY
                        Subject.SubjectID;";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StudentID", studentId);
                    command.Parameters.AddWithValue("@Year", year);

                    using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }
        //When the student writes the new personal information, this method updates it in the database
        protected void UpdateStudentInformation(int userId, string newName, string newSurname, string newDOB, string newNationality, string newID, string newAddress)
        {            
            string updateQuery = @"
                    UPDATE User
                        SET Name = @NewName, Surname = @NewSurname, DOB = @NewDOB, Nationality = @NewNationality, 
                            IDNumber = @NewID, Address = @NewAddress
                            WHERE UserID = @UserID;
                                                        ";

            
            using (SQLiteConnection connection = new SQLiteConnection(authHelper.DbPath))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                {
                    
                    command.Parameters.AddWithValue("@NewName", newName);
                    command.Parameters.AddWithValue("@NewSurname", newSurname);
                    command.Parameters.AddWithValue("@NewDOB", newDOB);
                    command.Parameters.AddWithValue("@NewNationality", newNationality);
                    command.Parameters.AddWithValue("@NewID", newID);
                    command.Parameters.AddWithValue("@NewAddress", newAddress);
                    command.Parameters.AddWithValue("@UserID", userId);

                    
                    command.ExecuteNonQuery();
                }
            }
            authHelper.UpdateUserInSession(authHelper.GetFromSession<User>("CurrentUser").UserID);
            
            LoadStudentInfo();
        }

        //this method adds the functionallity to the button that enables the personal info of the student to be updated
        //in the database
        protected void UpdateInfoButton_Click(object sender, EventArgs e)
        {
            
            if (Page.IsValid)
            {
                
                string newName = UpdateNameTextBox.Text;
                string newSurname = UpdateSurnameTextBox.Text;
                string newDOB = UpdateDOBTextBox.Text;
                string newNationality = UpdateNationalityTextBox.Text;
                string newID = UpdateIDTextBox.Text;
                string newAddress = UpdateAddressTextBox.Text;

                
                User currentUser = authHelper.GetFromSession<User>("CurrentUser");
                int userId = currentUser.UserID;

                
                UpdateStudentInformation(userId, newName, newSurname, newDOB, newNationality, newID, newAddress);
            }
        }

        //this adds the professor names in the corresponding rows in the table of enrolled subjects
        protected void EnrolledSubjectsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                
                Literal ProfessorsLiteral = (Literal)e.Row.FindControl("ProfessorsLiteral");

                if (ProfessorsLiteral != null)
                {
                    
                    string professors = DataBinder.Eval(e.Row.DataItem, "ProfessorNames").ToString();

                    
                    professors = professors.Replace(",", "<br />");

                    
                    ProfessorsLiteral.Text = professors;
                }
            }
        }
        //this method enables the student to select different years to check for previous or future subject enrollements
        protected void YearSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            int selectedYear = Convert.ToInt32(YearSelector.SelectedValue);

            
            LoadEnrolledSubjects(selectedYear);
        }

    }
}
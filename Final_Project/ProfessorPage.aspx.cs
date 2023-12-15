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
    public partial class ProfessorPage : System.Web.UI.Page
    {

        //Fields
        private AuthHelper authHelper;
        private int selectedYear;

        protected void Page_Load(object sender, EventArgs e)
        {
            authHelper = AuthHelper.Instance;

            //If there is no user in the session or the user in session is not a professor,
            //redirect to LoginPage
            if (!IsUserAuthenticated() || !IsUserProfessor())
            {
                Response.Redirect("LoginPage.aspx");
            }

            //If this is the first time we load the page we load everything by default
            if (!IsPostBack)
            {
                LoadProfessorInfo();
                
                selectedYear = 2023;

                LoadTeachingSubjects(selectedYear);
            }
        }

        //Checks if there is a user in the session
        private bool IsUserAuthenticated()
        {
            return authHelper.GetFromSession<User>("CurrentUser") != null;
        }

        //Checks if the user from the session if a professor
        private bool IsUserProfessor()
        {
            User currentUser = authHelper.GetFromSession<User>("CurrentUser");
            return authHelper.IsAuthorized(currentUser, "Professor");
        }

        //Loads the professor information such as name and surname and the date for courses 
        //into the selector, its a little bit badly crafted
        private void LoadProfessorInfo()
        {
            User currentUser = authHelper.GetFromSession<User>("CurrentUser");
            ProfessorNameLabel.Text = currentUser.Name + " " + currentUser.Surname;

            
            int currentYear = DateTime.Now.Year;

            
            for (int year = currentYear - 1; year <= currentYear + 1; year++)
            {
                YearSelector.Items.Add(new ListItem(year.ToString(), year.ToString()));
            }

            
            YearSelector.SelectedValue = currentYear.ToString();
        }

        //Esta repetida ignorar este metodo, esta abajo otra vez
        private void LoadTeachingSubjects()
        {
            User currentUser = authHelper.GetFromSession<User>("CurrentUser");
            DataTable subjectsTable = GetTeachingSubjectsWithStudents(currentUser.UserID, 2023); 
            TeachingSubjectsGridView.DataSource = subjectsTable;
            TeachingSubjectsGridView.DataBind();
        }

        //Given year and professorID
        private DataTable GetTeachingSubjectsWithStudents(int professorId, int year)
        {
            //We do this using in order not to have to open and then close the connection 
            //manually

            //We forgot to do try and catch, but we do it in order SQL functions
            using (SQLiteConnection connection = new SQLiteConnection(authHelper.DbPath))
            {
                connection.Open();

                //We use a powerful query to get subject info and all the students on that subject
                string query = @"
    SELECT
        Subject.SubjectID,
        Subject.SubjectName,
        Subject.Credits,
        Subject.Semester,
        GROUP_CONCAT(User.Name || ' ' || User.Surname) AS StudentNames
    FROM
        Teaching
    INNER JOIN
        Subject ON Teaching.SubjectID = Subject.SubjectID
    INNER JOIN
        Enrollment ON Subject.SubjectID = Enrollment.SubjectID
    INNER JOIN
        User ON Enrollment.UserID = User.UserID
    WHERE
        Teaching.UserID = @ProfessorID AND Teaching.Year = @Year
    GROUP BY
        Subject.SubjectID;
";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProfessorID", professorId);
                    command.Parameters.AddWithValue("@Year", year);

                    using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
                    {
                        //We fill the datatable and then the TeachingSubjectsGridView is performed
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }

        //Function to bind and adapt data into the grid automatically
        protected void TeachingSubjectsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                
                Literal studentNamesLiteral = (Literal)e.Row.FindControl("StudentNamesLiteral");

                if (studentNamesLiteral != null)
                {
                    
                    string studentNames = DataBinder.Eval(e.Row.DataItem, "StudentNames") as string;

                    if (!string.IsNullOrEmpty(studentNames))
                    {
                        
                        studentNames = studentNames.Replace(",", "<br />");
                    }

                    
                    studentNamesLiteral.Text = studentNames;
                }
            }
        }

        //Every time we change the selector of years we load the subjects for that year
        protected void YearSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            selectedYear = Convert.ToInt32(YearSelector.SelectedValue);

            
            LoadTeachingSubjects(selectedYear);
        }

        //Gets the user from the session and passes the id fom the professor and the year
        //in order to call the function that returns the subjects with students
        //and loads into table
        private void LoadTeachingSubjects(int year)
        {
            User currentUser = authHelper.GetFromSession<User>("CurrentUser");
            DataTable subjectsTable = GetTeachingSubjectsWithStudents(currentUser.UserID, year);
            TeachingSubjectsGridView.DataSource = subjectsTable;
            TeachingSubjectsGridView.DataBind();
        }


    }
}
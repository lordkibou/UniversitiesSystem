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
        private AuthHelper authHelper;
        private int selectedYear;

        protected void Page_Load(object sender, EventArgs e)
        {
            authHelper = AuthHelper.Instance;

            
            if (!IsUserAuthenticated() || !IsUserProfessor())
            {
                Response.Redirect("LoginPage.aspx");
            }

            if (!IsPostBack)
            {
                LoadProfessorInfo();
                
                selectedYear = 2023;

                LoadTeachingSubjects(selectedYear);
            }
        }

        
        private bool IsUserAuthenticated()
        {
            return authHelper.GetFromSession<User>("CurrentUser") != null;
        }

        
        private bool IsUserProfessor()
        {
            User currentUser = authHelper.GetFromSession<User>("CurrentUser");
            return authHelper.IsAuthorized(currentUser, "Professor");
        }

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

        
        private void LoadTeachingSubjects()
        {
            User currentUser = authHelper.GetFromSession<User>("CurrentUser");
            DataTable subjectsTable = GetTeachingSubjectsWithStudents(currentUser.UserID, 2023); 
            TeachingSubjectsGridView.DataSource = subjectsTable;
            TeachingSubjectsGridView.DataBind();
        }

        
        private DataTable GetTeachingSubjectsWithStudents(int professorId, int year)
        {
            using (SQLiteConnection connection = new SQLiteConnection(authHelper.DbPath))
            {
                connection.Open();
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
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }

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

        protected void YearSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            selectedYear = Convert.ToInt32(YearSelector.SelectedValue);

            
            LoadTeachingSubjects(selectedYear);
        }

        private void LoadTeachingSubjects(int year)
        {
            User currentUser = authHelper.GetFromSession<User>("CurrentUser");
            DataTable subjectsTable = GetTeachingSubjectsWithStudents(currentUser.UserID, year);
            TeachingSubjectsGridView.DataSource = subjectsTable;
            TeachingSubjectsGridView.DataBind();
        }


    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Final_Project
{
    public partial class HomePage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        //Redirection to Login when we click in Login button
        protected void btnLogin_Click1(object sender, EventArgs e)
        {
            Response.Redirect("LoginPage.aspx");
        }
    }
}
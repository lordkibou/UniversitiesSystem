<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LoginPage.aspx.cs" Inherits="Final_Project.LoginPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login Page</title>
    <link rel="stylesheet" href="styles.css" />
</head>
<body>
    <form id="form1" runat="server">
            <div class="container">
         <header>
     <div id="logo-container">
         <a href="HomePage.aspx">
             <h1><span id="logo">📖</span>University Carglass</h1>
         </a>
     </div>
 </header>

            <div class="login-container">
                <asp:Label ID="Label1" runat="server" Text="Write your account credentials:"></asp:Label>
                <br />
                <br />
                <div>
                    <asp:Label ID="Label2" runat="server" Text="Username:"></asp:Label>
                    <br />
                </div>
                <asp:TextBox ID="UsernameTextBox" runat="server"></asp:TextBox>
                <br />
                <br />
                <asp:Label ID="Label3" runat="server" Text="Password:"></asp:Label>
                <p>
                    <asp:TextBox ID="PasswordTextBox" runat="server" TextMode="Password"></asp:TextBox>
                </p>
                <asp:Button ID="LoginButton" runat="server" OnClick="LoginButton_Click" CssClass="login-button" Text="Log In" />
                <br />
                <br />
                <asp:Label ID="ErrorMessageLabel" runat="server" Text="ErrorMessage" Visible="False"></asp:Label>
            </div>

            <footer style="background-color: #3498db;">
                <p>&copy; 2023 University Carglass. All rights reserved.</p>
            </footer>
        </div>
    </form>
</body>
</html>

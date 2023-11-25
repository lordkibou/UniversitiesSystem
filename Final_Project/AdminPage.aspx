<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminPage.aspx.cs" Inherits="Final_Project.AdminPage" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Admin Page</title>
    <link rel="stylesheet" href="styles.css" />
    <style>


.popup {
    position: fixed;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    background: #fff;
    padding: 20px;
    border: 1px solid #ccc;
    z-index: 1000;
    display: none;
    box-shadow: 0 0 20px rgba(0, 0, 0, 0.5);
}


#UserCheckBoxList {
    list-style: none;
    padding: 0;
}


#UserCheckBoxList label {
    display: flex;
    align-items: center;
    margin-bottom: 5px;
}
    </style>
</head>
<body>
    <script>
    function showPopup(subjectID) {
        document.getElementById('editUsersPopup').style.display = 'block';
    }

    function hidePopup() {
        document.getElementById('editUsersPopup').style.display = 'none';
    }
</script>
    <form id="form1" runat="server">

                   <div class="container">
        <header>
    <div id="logo-container">
        <a href="HomePage.aspx">
            <h1><span id="logo">📖</span>University Carglass</h1>
        </a>
    </div>
</header>

        <div>
    
    <section id="userspages">
    <h2>Subjects Information</h2>            
    <asp:GridView ID="SubjectsGridView" runat="server" AutoGenerateColumns="False" OnRowCommand="SubjectsGridView_RowCommand">
        <Columns>
            <asp:BoundField DataField="SubjectID" HeaderText="Subject ID" ReadOnly="True" SortExpression="SubjectID" />
            <asp:BoundField DataField="SubjectName" HeaderText="Subject Name" SortExpression="SubjectName" />
            <asp:BoundField DataField="Credits" HeaderText="Credits" SortExpression="Credits" />
            <asp:BoundField DataField="Semester" HeaderText="Semester" SortExpression="Semester" />
            <asp:TemplateField HeaderText="Actions">
                <ItemTemplate>
                    <asp:Button runat="server" Text="Edit Users" CommandArgument='<%# Eval("SubjectID") %>' CommandName="EditUsers"/>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

    

    <div id="editUsersPopup" class="popup" runat="server">
        <div class="popup-content">
            <asp:CheckBoxList ID="UserCheckBoxList" runat="server"></asp:CheckBoxList>
            <asp:Button ID="ConfirmButton" runat="server" Text="Confirmar" OnClick="ConfirmButton_Click"/>
            <asp:Button ID="CancelButton" runat="server" Text="Cancelar"  OnClientClick="hidePopup(); return false;" />
        </div>
    </div>

    <h2>Delete Users</h2>
    <asp:GridView ID="UsersToDeleteGridView" runat="server" AutoGenerateColumns="False" OnRowDeleting="UsersToDeleteGridView_RowDeleting" DataKeyNames="UserID">
        <Columns>
            <asp:BoundField DataField="UserID" HeaderText="User ID" ReadOnly="True" SortExpression="UserID" />
            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
            <asp:BoundField DataField="Surname" HeaderText="Surname" SortExpression="Surname" />
            <asp:BoundField DataField="RoleName" HeaderText="Role" SortExpression="RoleName" />
            <asp:CommandField ShowDeleteButton="True" DeleteText="Delete" />
        </Columns>
    </asp:GridView>


            <h2>Insert Users</h2>
        <div class="column">
            <div class="form-group">
            <asp:TextBox ID="NewUserUsernameTextBox" runat="server" placeholder="Username"></asp:TextBox>
                <asp:RegularExpressionValidator ID="regexNewUserUsername" runat="server" ControlToValidate="NewUserUsernameTextBox"
                    ValidationExpression="^[a-zA-Z0-9_-]{4,16}$" ErrorMessage="Please enter a valid username (4-16 alphanumeric characters, underscores, or hyphens)"
                    Display="Dynamic" ForeColor="Red" />
                </div>

            <div class="form-group">
            <asp:TextBox ID="NewUserNameTextBox" runat="server" placeholder="Name"></asp:TextBox>
                <asp:RegularExpressionValidator ID="regexNewUserName" runat="server" ControlToValidate="NewUserNameTextBox"
                    ValidationExpression="^[a-zA-ZáéíóúüÁÉÍÓÚÜñÑ\s]+$" ErrorMessage="Enter only letters in the name field"
                    Display="Dynamic" ForeColor="Red" />

                </div>

            <div class="form-group">
            <asp:TextBox ID="NewUserSurnameTextBox" runat="server" placeholder="Surname"></asp:TextBox>
                <asp:RegularExpressionValidator ID="regexNewUserSurname" runat="server" ControlToValidate="NewUserSurnameTextBox"
                    ValidationExpression="^[a-zA-ZáéíóúüÁÉÍÓÚÜñÑ\s]+$" ErrorMessage="Enter only letters in the last name field"
                    Display="Dynamic" ForeColor="Red" />

                </div>

            <div class="form-group">
            <asp:TextBox ID="NewUserPasswordTextBox" runat="server" TextMode="Password" placeholder="Password"></asp:TextBox>
                <asp:RegularExpressionValidator ID="regexNewUserPassword" runat="server" ControlToValidate="NewUserPasswordTextBox"
                    ValidationExpression="^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$" ErrorMessage="The password must be at least 8 characters, including at least one letter and one number"
                    Display="Dynamic" ForeColor="Red" />

                </div>

            <div class="form-group">
            <asp:TextBox ID="NewUserDOBTextBox" runat="server" TextMode="Date" placeholder="Date of Birth YYYY-MM-DD"></asp:TextBox>
                <asp:RegularExpressionValidator ID="regexNewUserDOB" runat="server" ControlToValidate="NewUserDOBTextBox"
                    ValidationExpression="\d{4}-\d{2}-\d{2}" ErrorMessage="Enter a valid date in YYYY-MM-DD format"
                    Display="Dynamic" ForeColor="Red" />

                </div>

            <div class="form-group">
            <asp:TextBox ID="NewUserNationalityTextBox" runat="server" placeholder="Nationality"></asp:TextBox>
                <asp:RegularExpressionValidator ID="regexNewUserNationality" runat="server" ControlToValidate="NewUserNationalityTextBox"
                    ValidationExpression="^[a-zA-ZáéíóúüÁÉÍÓÚÜñÑ\s]+$" ErrorMessage="Enter only letters in the nationality field"
                    Display="Dynamic" ForeColor="Red" />

                </div>

            <div class="form-group">
            <asp:TextBox ID="NewUserIDTextBox" runat="server" placeholder="New ID Number"></asp:TextBox>
                <asp:RegularExpressionValidator ID="regexNewUserID" runat="server" ControlToValidate="NewUserIDTextBox"
                    ValidationExpression="^\d{1,9}$" ErrorMessage="Enter only numbers up to 9 digits in the ID field"
                    Display="Dynamic" ForeColor="Red" />

                </div>

            <div class="form-group">
            <asp:TextBox ID="NewUserAddressTextBox" runat="server" placeholder="Address"></asp:TextBox>
                <asp:RegularExpressionValidator ID="regexNewUserAddress" runat="server" ControlToValidate="NewUserAddressTextBox"
                    ValidationExpression="^[a-zA-Z0-9\s\.,#-]+$" ErrorMessage="Please enter a valid address"
                    Display="Dynamic" ForeColor="Red" />
                
            </div>
                
            <div class="form-group">
                    <asp:DropDownList ID="NewUserRoleDropDown" runat="server">
                    <asp:ListItem Text="Select Role" Value="" />
                    <asp:ListItem Text="Student" Value="Student" />
                    <asp:ListItem Text="Professor" Value="Professor" />
                    </asp:DropDownList>

                </div>

            <div class="form-group">
                    <asp:Button ID="InsertUserButton" runat="server" CssClass="login-button" Text="Insert User" OnClick="InsertUserButton_Click" />
                </div>
            
            </div>
                </section>
                   <footer style="background-color: #3498db;">
                <p>&copy; 2023 University Carglass. All rights reserved.</p>
            </footer>
        </div>
      </div>
    </form>
</body>
</html>
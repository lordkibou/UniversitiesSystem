<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StudentPage.aspx.cs" Inherits="Final_Project.StudentPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Student Page</title>
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
            <section id="userspages">
                <div>
                    <h2>Student Information</h2>
                    <ul>
                        <li><strong>Name:</strong> <asp:Label ID="StudentNameLabel" runat="server" /></li>
                        <li><strong>Date of Birth:</strong> <asp:Label ID="StudentDOBLabel" runat="server" /></li>
                        <li><strong>Nationality:</strong> <asp:Label ID="StudentNationalityLabel" runat="server" /></li>
                        <li><strong>ID Number:</strong> <asp:Label ID="StudentIDLabel" runat="server" /></li>
                        <li><strong>Address:</strong> <asp:Label ID="StudentAddressLabel" runat="server" /></li>
                    </ul>

                    <h2>Enrolled Subjects</h2>
                    <asp:DropDownList ID="YearSelector" runat="server" AutoPostBack="true" OnSelectedIndexChanged="YearSelector_SelectedIndexChanged">
                    <asp:ListItem Text="2023" Value="2023" />
                    <asp:ListItem Text="2022" Value="2022" />
                    </asp:DropDownList>

                    <asp:GridView ID="EnrolledSubjectsGridView" runat="server" AutoGenerateColumns="False" OnRowDataBound="EnrolledSubjectsGridView_RowDataBound">
                    <Columns>
                    <asp:BoundField DataField="SubjectName" HeaderText="Subject Name" SortExpression="SubjectName" />
                    <asp:BoundField DataField="Credits" HeaderText="Credits" SortExpression="Credits" />
                    <asp:BoundField DataField="Semester" HeaderText="Semester" SortExpression="Semester" />
                    <asp:TemplateField HeaderText="Professors">
                    <ItemTemplate>
                    <asp:Literal ID="ProfessorsLiteral" runat="server"></asp:Literal>
                    </ItemTemplate>
                    </asp:TemplateField>
                    </Columns>
                    </asp:GridView>


                    <h2>Update Personal Information</h2>

                        <div class="form-group">
                    <asp:TextBox ID="UpdateNameTextBox" runat="server" placeholder="New Name" MaxLength="50" />
                        <asp:RegularExpressionValidator ID="regexNombre" runat="server" ControlToValidate="UpdateNameTextBox"
                        ValidationExpression="^[a-zA-ZáéíóúüÁÉÍÓÚÜñÑ\s]+$" ErrorMessage="Enter only letters in the name field"
                        Display="Dynamic" ForeColor="Red" />
                            
                            </div>

                        <div class="form-group">
                    <asp:TextBox ID="UpdateSurnameTextBox" runat="server" placeholder="New Surname" MaxLength="50" />
                        <asp:RegularExpressionValidator ID="regexApellido" runat="server" ControlToValidate="UpdateSurnameTextBox"
                        ValidationExpression="^[a-zA-ZáéíóúüÁÉÍÓÚÜñÑ\s]+$" ErrorMessage="Enter only letters in the surname field"
                        Display="Dynamic" ForeColor="Red" />
                    
                            </div>

                        <div class="form-group">
                    <asp:TextBox ID="UpdateDOBTextBox" runat="server" TextMode="Date" Pattern="\d{4}-\d{2}-\d{2}" placeholder="Day of Birth YYYY-MM-DD" />
                        <asp:RegularExpressionValidator ID="regexFecha" runat="server" ControlToValidate="UpdateDOBTextBox"
                        ValidationExpression="\d{4}-\d{2}-\d{2}" ErrorMessage="Enter a valid date in YYYY-MM-DD format"
                        Display="Dynamic" ForeColor="Red" />

                            </div>

                        <div class="form-group">
                    <asp:TextBox ID="UpdateNationalityTextBox" runat="server" placeholder="New Nationality" MaxLength="50" />
                        <asp:RegularExpressionValidator ID="regexNacionalidad" runat="server" ControlToValidate="UpdateNationalityTextBox"
                        ValidationExpression="^[a-zA-ZáéíóúüÁÉÍÓÚÜñÑ\s]+$" ErrorMessage="Enter only letters in the nationality field"
                        Display="Dynamic" ForeColor="Red" />
                    
                            </div>

                        <div class="form-group">
                    <asp:TextBox ID="UpdateIDTextBox" runat="server" placeholder="New ID Number" MaxLength="9" />
                        <asp:RegularExpressionValidator ID="regexID" runat="server" ControlToValidate="UpdateIDTextBox"
                        ValidationExpression="^\d{1,9}$" ErrorMessage="Enter only numbers up to 9 digits in the ID field"
                        Display="Dynamic" ForeColor="Red" />
                   
                            </div>

                        <div class="form-group">
                    <asp:TextBox ID="UpdateAddressTextBox" runat="server" placeholder="New Address" MaxLength="100"></asp:TextBox>
                    
                            </div>

                        <div class="form-group">
                    <asp:Button ID="UpdateInfoButton" runat="server" Text="Update Information" CssClass="login-button" OnClick="UpdateInfoButton_Click" />
                
                        </div>

                        </div>
            </section>

            <footer style="background-color: #3498db;">
                <p>&copy; 2023 University Carglass. All rights reserved.</p>
            </footer>
        </div>
    </form>
</body>
</html>

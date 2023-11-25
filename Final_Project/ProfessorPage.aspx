<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProfessorPage.aspx.cs" Inherits="Final_Project.ProfessorPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Professor Page</title>
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
        <div>
            <section id="userspages">
            <h2>Professor Information</h2>
            <ul>
                <li><strong>Name:</strong> <asp:Label ID="ProfessorNameLabel" runat="server" /></li>
                
            </ul>

            <h2>Teaching Subjects</h2>
                <asp:DropDownList ID="YearSelector" runat="server" AutoPostBack="true" OnSelectedIndexChanged="YearSelector_SelectedIndexChanged">
                </asp:DropDownList>
            <asp:GridView ID="TeachingSubjectsGridView" runat="server" AutoGenerateColumns="False" OnRowDataBound="TeachingSubjectsGridView_RowDataBound">
    <Columns>
        <asp:BoundField DataField="SubjectName" HeaderText="Subject Name" SortExpression="SubjectName" />
        <asp:BoundField DataField="Credits" HeaderText="Credits" SortExpression="Credits" />
        <asp:BoundField DataField="Semester" HeaderText="Semester" SortExpression="Semester" />
        <asp:TemplateField HeaderText="Enrolled Students">
            <ItemTemplate>
                <asp:Literal ID="StudentNamesLiteral" runat="server"></asp:Literal>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>


            
                </section>
            <footer style="background-color: #3498db;">
                <p>&copy; 2023 University Carglass. All rights reserved.</p>
            </footer>
        </div>
    </form>
</body>
</html>
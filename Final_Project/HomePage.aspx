<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HomePage.aspx.cs" Inherits="Final_Project.HomePage" %>

<!DOCTYPE html>

<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>University Carglass</title>
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
    <div id="nav-login-container">
        <nav>
            <ul>
                <li><a href="#courses">Courses</a></li>
                <li><a href="#about">About Us</a></li>
            </ul>
        </nav>
        <div id="btnLoginContainer">
            <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="login-button" OnClick="btnLogin_Click1" />
        </div>
    </div>
</header>



            <section id="home">
                <div id="divintro">
                <h2 id="welcome">Welcome to Our University!</h2>
                <h3>Fostering Knowledge, Igniting Curiosity – Your Gateway to Academic Excellence.</h3>

                </div>
                <asp:Image ID="Image1" runat="server" ImageUrl="~/images/image1.png" CssClass="column" AlternateText="University Students" Width="600" Height="400" />
                <p>At University Carglass, we believe in fostering an environment of academic excellence and personal growth. Our commitment to education goes beyond the classroom, providing students with opportunities to explore, innovate, and succeed.</p>

                <h3>Our Mission</h3>
                <p>Our mission is to empower students to become critical thinkers, effective communicators, and responsible global citizens. Through innovative teaching methods and a diverse range of programs, we aim to inspire a passion for lifelong learning.</p>

                <h3>Academic Programs</h3>
                <p>Explore our extensive range of academic programs, designed to meet the evolving needs of today's students. From science and technology to humanities and arts, our courses are led by experienced faculty members dedicated to your success.</p>

                <h3>Campus Life</h3>
                <p>Immerse yourself in a vibrant campus life where you can engage in extracurricular activities, join student organizations, and make lifelong connections. Our state-of-the-art facilities provide a conducive environment for both learning and recreation.</p>

                <h3>Admissions</h3>
                <p>Ready to join the University Carglass community? Discover the admissions process, scholarship opportunities, and everything you need to know to start your educational journey with us.</p>

                <h3>Contact Us</h3>
                <p>Have questions or need more information? Our friendly staff is here to help. Contact us through the provided channels, and we'll be happy to assist you on your path to academic success.</p>

            </section>

            
            <section id="courses">
                <h2 id="coursestitle">Courses at University Carglass</h2>
                <p>Explore our diverse range of academic programs designed to cater to the interests and career goals of our students. At University Carglass, we are committed to providing high-quality education that prepares students for success in their chosen fields.</p>

                
                <div class="course">
                    <h3>Computer Science</h3>
                    <p>Our Computer Science program equips students with the knowledge and skills needed for a successful career in the rapidly evolving field of technology. From programming languages to artificial intelligence, students delve into a comprehensive curriculum guided by experienced faculty.</p>
                </div>

                <div class="course">
                    <h3>Business Administration</h3>
                    <p>Explore the world of business with our Business Administration program. Students gain insights into strategic management, marketing, finance, and entrepreneurship. Practical projects and industry exposure ensure a well-rounded education for future business leaders.</p>
                    <p>
                <asp:Image ID="Image2" runat="server" ImageUrl="~/images/image2.jpg" CssClass="columnImages" AlternateText="University Students" Width="500" Height="300" />
                    </p>
                </div>

                

            </section>

            <section id="about">
                <h2 id="abouttitle">About Us</h2>
                <p>Welcome to University Carglass, a place where academic excellence meets a nurturing environment for personal and intellectual growth. Our institution is dedicated to providing a transformative educational experience that prepares students for success in an ever-changing world.</p>

                <h3>Our Values</h3>
                <p>At University Carglass, we uphold values that guide our mission and vision. Integrity, inclusivity, innovation, and a commitment to excellence are at the core of everything we do. We strive to create an atmosphere where students, faculty, and staff can thrive and contribute to a positive learning community.</p>

                <h3>Faculty and Staff</h3>
                <p>Our team of dedicated faculty members brings a wealth of knowledge and experience to the classrooms. Passionate about teaching and mentorship, they are committed to fostering a dynamic learning environment. Our staff members work diligently to support the overall well-being of our university community.</p>

                <h3>State-of-the-Art Facilities</h3>
                <p>Explore our modern campus equipped with cutting-edge facilities. From advanced laboratories to collaborative spaces, we provide an infrastructure that enhances the overall learning experience. Our commitment to technology ensures that students have the tools they need for academic success.</p>

                <h3>Community Engagement</h3>
                <p>University Carglass believes in giving back to the community. Through various outreach programs, partnerships, and community service initiatives, we aim to make a positive impact beyond the walls of our campus. Join us in creating a brighter future for all.</p>
                            <p>
                <asp:Image ID="Image3" runat="server" ImageUrl="~/images/image3.jpg" CssClass="columnImages" AlternateText="University Students" Width="500" Height="300" />
                            </p>
</section>


            <footer style="background-color: #3498db;">
                <p>&copy; 2023 University Carglass. All rights reserved.</p>
            </footer>
        </div>
    </form>
</body>
</html>

<script src="https://code.jquery.com/jquery-3.6.4.min.js"></script>


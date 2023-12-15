using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Final_Project
{   
    //Its like a POJO in Java
    public class Subject
    {
        //Fields are the variables and the get and set are properties to access the fields
        public int SubjectID { get; set; } 
        public string SubjectName { get; set; }
        public int Credits { get; set; }
        public int Semester { get; set; }

        //class builder
        public Subject(int subjectId, string subjectName, int credits, int semester)
        {
            SubjectID = subjectId;
            SubjectName = subjectName;
            Credits = credits;
            Semester = semester;
        }
    }

}
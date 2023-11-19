using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Core
{
    public class Subject
    {
        public int SubjectID { get; set; } // Primary Key
        public string SubjectName { get; set; }
        public int Credits { get; set; }
        public int Semester { get; set; }

        public Subject(int subjectId, string subjectName, int credits, int semester)
        {
            SubjectID = subjectId;
            SubjectName = subjectName;
            Credits = credits;
            Semester = semester;
        }
    }

}
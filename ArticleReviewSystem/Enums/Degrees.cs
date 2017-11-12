using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArticleReviewSystem.Enums
{
    public class Degrees
    {
        public  Dictionary<string, string> DegreesDictonary; 
        public  Degrees()
        {
            DegreesDictonary = new Dictionary<string, string>()
            {
                {" "," "},
                { "Doctor of Philosophy" ,"PhD"},
                { "Doctor of Engineering", "EngD"},
                { "Master of Arts", "M.A."},
                { "Master of Business Administration", "M.B.A"},
                { "Master of Education", "M.Ed."},
                { "Master of Engineering", "M.Eng."},
                { "Master of Science", "M.Sc."},
                { "Bachelor of Arts", " "},
                { "Bachelor of Education", " "},
                { "Bachelor of Engineering", " "},
                { "Bachelor of Sciene", " "}
            };
        }
    }
}
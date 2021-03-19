using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DB_Project.Models
{
    public class ClassDetails
    {
        public int id {get; set;}
        public string name {get; set;}
        public char section {get; set;}
        public string code { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public string picture { get; set; }
    }
}
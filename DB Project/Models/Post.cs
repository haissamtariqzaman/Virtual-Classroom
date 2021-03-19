using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DB_Project.Models
{
    public class Post
    {
        public int id { get; set; }
        public User user {get; set;}
        public string post {get; set;} 
        public List<Comment> comments{get; set;}
        public List<File> files { get; set; }
        public string datePosted { get; set; }
    }
}
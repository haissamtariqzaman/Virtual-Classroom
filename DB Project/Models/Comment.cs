using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DB_Project.Models
{
    public class Comment
    {
        public string commentContent{ get; set;}
        public User user { get; set; }
        public DateTime datePosted { get; set; }
    }
}
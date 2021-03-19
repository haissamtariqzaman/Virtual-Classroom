using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DB_Project.Models
{
    public class AssignmentSubmitted
    {
        public int submissionId { get; set; }
        public int assignmentId { get; set; }
        public User user { get; set; }
        public string dateSubmitted { get; set; }
        public List<File> filesSubmitted { get; set; }
        public string status { get; set; }
    }
}
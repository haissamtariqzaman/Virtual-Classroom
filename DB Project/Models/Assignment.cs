using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;

namespace DB_Project.Models
{
    public class Assignment
    {
        public int id { get; set; }
        public string title { get; set; }
        public string notes { get; set; }
        public string deadline { get; set; }
        public int noOfResub { get; set; }
        public string dateUploaded { get; set; }
        public string dayDue { get; set; }
        public List<File> files { get; set; }

        public void setDayDue()
        {
            //CultureInfo c = new  CultureInfo("en-US");
            DateTime d = Convert.ToDateTime(deadline);
            //deadline=d.ToString("d", new CultureInfo("en-GB"));
            dayDue = d.DayOfWeek.ToString();
        }

        public void editDate()
        {
            char[] arr=dateUploaded.ToCharArray();
            char[] arr1 = new char[10];

            int x = 0;

            while (arr[x] != ' ')
            {
                arr1[x] = arr[x];
                x++;
            }

            dateUploaded = new string(arr1);
        }
    }
}
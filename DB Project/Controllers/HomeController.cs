using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DB_Project.Models;
using System.Dynamic;
using System.IO;

namespace DB_Project.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Login()
        {
            if (Session["username"] != null)
            {
                return RedirectToAction("ClassSelect");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            int status = CRUD.login(username, password);
            string data = null;

            if (status >= 0)
            {
                User u = CRUD.getUserDetails(status);
                Session["username"] = username;
                Session["id"] = status;
                Session["user"] = u;
                return RedirectToAction("ClassSelect");
            }
            else if (status == -1)
            {
                data = "Incorrect username or password!";
            }
            else
            {
                data = "Sorry we have a connection problem!";
            }
            return View((object)data);
        }

        public ActionResult logout()
        {
            Session["username"] = null;
            Session["id"] = null;
            return RedirectToAction("Login");
        }

        public ActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Signup(string fname, string lname, string dob, string gender, string email, string password)
        {
            int state = CRUD.signup(fname, lname, dob, gender[0], email, password);
            string message = null;

            if (state == 1)
            {
                return RedirectToAction("Login");
            }
            else if (state == 0)
            {
                message = "This email already exists!";
            }
            else
            {
                message = "Sorry we have a connection problem!";
            }
            return View((object)message);
        }

        public ActionResult Stream(string class1)
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login");
            }

            ClassDetails classd = CRUD.getsClassDetail(Int32.Parse(class1));
            List<ClassDetails> temp = new List<ClassDetails>();
            temp.Add(classd);
            Assignment[] a = CRUD.getAssignmentDueSoon(temp, (int)Session["id"]);

            List<Post> posts = CRUD.getPosts(Int32.Parse(class1));
            User thisUser = CRUD.getUserDetails((int)Session["id"]);
            int isTeacher = CRUD.checkTeacher(Int32.Parse(class1), Int32.Parse(Session["id"].ToString()));
            
            dynamic mymodel = new ExpandoObject();

            mymodel.classdetails = classd;
            mymodel.assigndue = a;
            mymodel.posts = posts;
            mymodel.thisUser = thisUser;
            mymodel.isTeacher = isTeacher;
            return View(mymodel);
        }

        [HttpPost]
        public ActionResult Stream(string cid, string post, List<HttpPostedFileBase> fileUploaded)
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login");
            }

            List<DB_Project.Models.File> allFiles = null;

            if (fileUploaded[0] != null)
            {
                allFiles = new List<DB_Project.Models.File>();

                string temp1 = Server.MapPath("~/Files/" + cid);
                if (System.IO.File.Exists(temp1) == false)
                {
                    Directory.CreateDirectory(temp1);
                }
                temp1 = Server.MapPath("~/Files/" + cid + "/posts");
                if (System.IO.File.Exists(temp1) == false)
                {
                    Directory.CreateDirectory(temp1);
                }

                foreach (HttpPostedFileBase f in fileUploaded)
                {
                    string fileName = Path.GetFileName(f.FileName);
                    string savePath = Server.MapPath("~/Files/" + cid + "/posts/");
                    string pathToCheck = savePath + fileName;
                    string temp = null;
                    int count = 1;

                    while (System.IO.File.Exists(pathToCheck))
                    {
                        temp = count.ToString() + fileName;
                        pathToCheck = savePath + temp;
                        count++;
                    }

                    f.SaveAs(pathToCheck);

                    DB_Project.Models.File f1 = new DB_Project.Models.File();
                    f1.filePath = pathToCheck;
                    f1.fileName = fileName;
                    allFiles.Add(f1);
                }
            }

            CRUD.addPost(Int32.Parse(cid), (int)Session["id"], post, allFiles);

            return RedirectToAction("Stream", new { class1 = Int32.Parse(cid) });
        }

        public ActionResult Comment(string postId, string class1, string comment)
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login");
            }

            if (CRUD.checkClass((int)Session["id"], Int32.Parse(class1)) == true)
            {
                CRUD.addComment(Int32.Parse(postId), (int)Session["id"], comment);
            }

            return RedirectToAction("Stream", new { class1 = Int32.Parse(class1) });
        }

        public ActionResult delComment(string pid, string uid, string dt, string class1)
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login");
            }

            DateTime dt1 = new DateTime(long.Parse(dt));

            CRUD.deleteComment(Int32.Parse(pid), Int32.Parse(uid), dt1);

            return RedirectToAction("Stream", new { class1 = class1 });
        }

        public ActionResult DeletePost(string pid, string class1)
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login");
            }
            if (CRUD.checkClass((int)Session["id"], Int32.Parse(class1)) == true)
            {
                CRUD.deletePost(Int32.Parse(pid));
            }

            return RedirectToAction("Stream", new { class1 = class1 } );
        }

        public ActionResult DownloadFile(string path, string name)
        {
            byte[] file_bytes = System.IO.File.ReadAllBytes(path);
            return File(file_bytes, System.Net.Mime.MediaTypeNames.Application.Octet, name);
        }

        public ActionResult Classwork(string class1)
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login");
            }

            Assignment assignment = (Assignment)TempData["assign"];
            Assignment lec = (Assignment)TempData["lec"];

            List<Assignment> assignments = CRUD.getAssignments(Int32.Parse(class1));
            List<Assignment> lectures = CRUD.getLectures(Int32.Parse(class1));
            int isTeacher = CRUD.checkTeacher(Int32.Parse(class1), Int32.Parse(Session["id"].ToString()));
            int editFlag = (assignment == null) ? 0 : 1;
            int editLecFlag = (lec == null) ? 0 : 1;
            dynamic mymodel = new ExpandoObject();
            mymodel.assignments = assignments;
            mymodel.lectures = lectures;
            mymodel.classid = class1;
            mymodel.isTeacher = isTeacher;
            mymodel.editAssign = assignment;
            mymodel.editflag = editFlag;
            mymodel.editLec = lec;
            mymodel.editLecFlag = editLecFlag;

            return View(mymodel);
        }

        public ActionResult editAssign(string class1, string assignId)
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login");
            }

            if (CRUD.checkTeacher(Int32.Parse(class1), (int)Session["id"]) == 0)
            {
                return RedirectToAction("ClassSelect", new { class1 = class1 });
            }

            Assignment a = CRUD.getAssignDetail(Int32.Parse(assignId));
            TempData["assign"] = a;
            return RedirectToAction("Classwork", new { class1 = class1 });
        }

        public ActionResult editLec(string class1, string LecId)
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login");
            }

            if (CRUD.checkTeacher(Int32.Parse(class1), (int)Session["id"]) == 0)
            {
                return RedirectToAction("ClassSelect", new { class1 = class1 });
            }

            Assignment l = CRUD.getLectureDetails(Int32.Parse(LecId));
            TempData["lec"] = l;
            return RedirectToAction("Classwork", new { class1 = class1 });
        }

        public ActionResult AddAssignment(string id, string title, string note, string due, string resub, List<HttpPostedFileBase> files)
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login");
            }

            if (CRUD.checkTeacher(Int32.Parse(id), Int32.Parse(Session["id"].ToString())) != 1)
            {
                return RedirectToAction("Classwork", new { class1 = id });
            }

            List<DB_Project.Models.File> allFiles = null;

            if (files[0] != null)
            {
                allFiles = new List<DB_Project.Models.File>();

                string temp1 = Server.MapPath("~/Files/" + id);
                if (System.IO.File.Exists(temp1) == false)
                {
                    Directory.CreateDirectory(temp1);
                }
                temp1 = Server.MapPath("~/Files/" + id + "/assignments");
                if (System.IO.File.Exists(temp1) == false)
                {
                    Directory.CreateDirectory(temp1);
                }

                foreach (HttpPostedFileBase f in files)
                {
                    string fileName = Path.GetFileName(f.FileName);
                    string savePath = Server.MapPath("~/Files/" + id + "/assignments/");
                    string pathToCheck = savePath + fileName;
                    string temp = null;
                    int count = 1;

                    while (System.IO.File.Exists(pathToCheck))
                    {
                        temp = count.ToString() + fileName;
                        pathToCheck = savePath + temp;
                        count++;
                    }

                    f.SaveAs(pathToCheck);

                    DB_Project.Models.File f1 = new DB_Project.Models.File();
                    f1.filePath = pathToCheck;
                    f1.fileName = fileName;
                    allFiles.Add(f1);
                }
            }

            Assignment a = new Assignment();
            a.files = allFiles;
            a.title = title;
            a.notes = note;
            a.deadline = due;
            a.noOfResub = Int32.Parse(resub);

            CRUD.addAssignmnet(Int32.Parse(id), a);

            return RedirectToAction("Classwork", new { class1 = id });
        }

        public ActionResult AddLecture(string id, string title, string note, List<HttpPostedFileBase> files)
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login");
            }

            if (CRUD.checkTeacher(Int32.Parse(id), Int32.Parse(Session["id"].ToString())) != 1)
            {
                return RedirectToAction("Classwork", new { class1 = id });
            }

            List<DB_Project.Models.File> allFiles = null;

            if (files[0] != null)
            {
                allFiles = new List<DB_Project.Models.File>();

                string temp1 = Server.MapPath("~/Files/" + id);
                if (System.IO.File.Exists(temp1) == false)
                {
                    Directory.CreateDirectory(temp1);
                }
                temp1 = Server.MapPath("~/Files/" + id + "/lectures");
                if (System.IO.File.Exists(temp1) == false)
                {
                    Directory.CreateDirectory(temp1);
                }

                foreach (HttpPostedFileBase f in files)
                {
                    string fileName = Path.GetFileName(f.FileName);
                    string savePath = Server.MapPath("~/Files/" + id + "/lectures/");
                    string pathToCheck = savePath + fileName;
                    string temp = null;
                    int count = 1;

                    while (System.IO.File.Exists(pathToCheck))
                    {
                        temp = count.ToString() + fileName;
                        pathToCheck = savePath + temp;
                        count++;
                    }

                    f.SaveAs(pathToCheck);

                    DB_Project.Models.File f1 = new DB_Project.Models.File();
                    f1.filePath = pathToCheck;
                    f1.fileName = fileName;
                    allFiles.Add(f1);
                }
            }

            Assignment l = new Assignment();
            l.files = allFiles;
            l.title = title;
            l.notes = note;

            CRUD.addLecture(Int32.Parse(id), l);

            return RedirectToAction("Classwork", new { class1 = id });
        }

        public ActionResult People(string class1)
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login");
            }

            List<User> students = CRUD.getStudents(Int32.Parse(class1));
            List<User> teachers = CRUD.getTeachers(Int32.Parse(class1));
            int isTeacher = CRUD.checkTeacher(Int32.Parse(class1), Int32.Parse(Session["id"].ToString()));

            dynamic mymodel = new ExpandoObject();
            mymodel.classId = class1;
            mymodel.students = students;
            mymodel.teachers = teachers;
            mymodel.isTeacher = isTeacher;
            mymodel.userId = Int32.Parse(Session["id"].ToString());

            string temp = (string)TempData["PeopleMessage"];

            if (temp != null)
            {
                ViewBag.Message = temp;
            }

            return View(mymodel);
        }

        [HttpPost]
        public ActionResult People(string email, string id, string add)
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login");
            }

            if (CRUD.checkTeacher(Int32.Parse(id), Int32.Parse(Session["id"].ToString())) == 0)
            {
                return RedirectToAction("People", new { class1 = id });
            }

            int output = 0;

            if (String.Compare(add, "Add Teacher") == 0)
            {
                output = CRUD.addTeacher(Int32.Parse(id), email);
            }
            else if (String.Compare(add, "Add Student") == 0)
            {
                output = CRUD.addStudent(Int32.Parse(id), email);
            }

            if (output == -1)
            {
                TempData["PeopleMessage"] = "User not added!";
            }
            else if (output == 0)
            {
                TempData["PeopleMessage"] = "Cannot find user!";
            }
            else if (output == 2)
            {
                TempData["PeopleMessage"] = "User cannot be teacher and student of same class!";
            }

            return RedirectToAction("People", new { class1 = id });
        }

        public ActionResult removeTeacher(string id, string u)
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login");
            }

            if (CRUD.checkTeacher(Int32.Parse(id), Int32.Parse(Session["id"].ToString())) == 1 && string.Compare(Session["id"].ToString(), u) != 0)
            {
                CRUD.removeTeacher(Int32.Parse(id), Int32.Parse(u));
            }

            return RedirectToAction("People", new { class1 = id });
        }

        public ActionResult removeStudent(string id, string u)
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login");
            }

            if (CRUD.checkTeacher(Int32.Parse(id), Int32.Parse(Session["id"].ToString())) == 1)
            {
                CRUD.removeStudent(Int32.Parse(id), Int32.Parse(u));
            }

            return RedirectToAction("People", new { class1 = id });
        }

        public ActionResult ClassSelect()
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login");
            }

            string message = (string)TempData["classSelectMessage"];

            if (message != null)
            {
                ViewBag.message = message;
            }

            dynamic mymodel = new ExpandoObject();

            List<ClassDetails> cd = CRUD.getClassDetails((int)Session["id"]);
            Assignment[] a = CRUD.getAssignmentDueSoon(cd, (int)Session["id"]);

            mymodel.classDetails = cd;
            mymodel.assignmnetDue = a;

            return View(mymodel);
        }

        public ActionResult AddClass(string name, string section, string subject)
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login");
            }

            int status = CRUD.addClass(name, section, subject, (int)Session["id"]);

            if (status == -1)
            {
                TempData["classSelectMessage"] = "Sorry we have a connection problem!";
            }
            else if (status == 0)
            {
                TempData["classSelectMessage"] = "Class not added!";
            }

            return RedirectToAction("ClassSelect");
        }

        public ActionResult JoinClass(string code)
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login");
            }

            int status = CRUD.joinClass(code, (int)Session["id"]);

            if (status == 0)
            {
                TempData["classSelectMessage"] = "Class not found!";
            }
            else if (status == 2)
            {
                TempData["classSelectMessage"] = "Cannot add to class!";
            }

            return RedirectToAction("ClassSelect");
        }

        public ActionResult ExitClass(string class1)
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login");
            }

            int status = CRUD.exitClass(Int32.Parse(class1), (int)Session["id"]);

            if (status == 0)
            {
                TempData["classSelectMessage"] = "Cannot delete class!";
            }

            return RedirectToAction("ClassSelect");
        }

        public ActionResult AssignSub(string class1, string aid)
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login");
            }

            if (CRUD.checkStudent(Int32.Parse(class1), (int)Session["id"]) == 0)
            {
                return RedirectToAction("ClassSelect");
            }

            Assignment a = CRUD.getAssignDetail(Int32.Parse(aid));
            int subLeft = a.noOfResub + 1 - CRUD.noOfsubmission(Int32.Parse(aid), Int32.Parse(Session["id"].ToString()));
            AssignmentSubmitted asub = CRUD.getUserSubmission((int)Session["id"], Int32.Parse(aid));

            dynamic mymodel = new ExpandoObject();
            mymodel.assignment = a;
            mymodel.subLeft = subLeft;
            mymodel.classId = class1;
            mymodel.assignSub = asub;

            return View(mymodel);
        }

        [HttpPost]
        public ActionResult AssignSub(string class1, string aid, List<HttpPostedFileBase> files)
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login");
            }

            if (CRUD.checkStudent(Int32.Parse(class1), (int)Session["id"]) == 0)
            {
                return RedirectToAction("ClassSelect");
            }

            List<DB_Project.Models.File> allFiles = null;

            if (files[0] != null)
            {
                allFiles = new List<DB_Project.Models.File>();

                string temp1 = Server.MapPath("~/Files/" + class1);
                if (System.IO.File.Exists(temp1) == false)
                {
                    Directory.CreateDirectory(temp1);
                }
                temp1 = Server.MapPath("~/Files/" + class1 + "/assignmentsSub");
                if (System.IO.File.Exists(temp1) == false)
                {
                    Directory.CreateDirectory(temp1);
                }
                temp1 = Server.MapPath("~/Files/" + class1 + "/assignmentsSub" + "/" + aid);
                if (System.IO.File.Exists(temp1) == false)
                {
                    Directory.CreateDirectory(temp1);
                }
                temp1 = Server.MapPath("~/Files/" + class1 + "/assignmentsSub" + "/" + aid + "/" + Session["id"].ToString());
                if (System.IO.File.Exists(temp1) == false)
                {
                    Directory.CreateDirectory(temp1);
                }

                foreach (HttpPostedFileBase f in files)
                {
                    string fileName = Path.GetFileName(f.FileName);
                    string savePath = Server.MapPath("~/Files/" + class1 + "/assignmentsSub" + "/" + aid + "/" + Session["id"].ToString() + "/");
                    string pathToCheck = savePath + fileName;
                    string temp = null;
                    int count = 1;

                    while (System.IO.File.Exists(pathToCheck))
                    {
                        temp = count.ToString() + fileName;
                        pathToCheck = savePath + temp;
                        count++;
                    }

                    f.SaveAs(pathToCheck);

                    DB_Project.Models.File f1 = new DB_Project.Models.File();
                    f1.filePath = pathToCheck;
                    f1.fileName = fileName;
                    allFiles.Add(f1);
                }
            }

            CRUD.submitAssignment(Int32.Parse(aid), (int)Session["id"], allFiles);

            return RedirectToAction("AssignSub", new { class1 = class1, aid = aid });
        }

        public ActionResult StudentsWork(string class1, string aid)
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login");
            }

            if (CRUD.checkTeacher(Int32.Parse(class1), (int)Session["id"]) == 0)
            {
                return RedirectToAction("ClassSelect");
            }

            List<AssignmentSubmitted> onTime = CRUD.getSubmittedAssingment(Int32.Parse(aid));
            List<AssignmentSubmitted> late = CRUD.getLateSubmittedAssingment(Int32.Parse(aid));
            List<User> notSub = CRUD.getUnsubmittedAssingment(Int32.Parse(aid));

            dynamic mymodel = new ExpandoObject();
            mymodel.onTime = onTime;
            mymodel.late = late;
            mymodel.notSub = notSub;
            mymodel.class1 = class1;

            return View(mymodel);
        }

        public ActionResult EditAssignment(string title, string note, string due, string resub, string assignid, string id)
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login");
            }

            Assignment a = new Assignment();
            a.id = Int32.Parse(assignid);
            a.title = title;
            a.notes = note;
            a.deadline = due;
            a.noOfResub = Int32.Parse(resub);

            CRUD.updateAssignemnt(a, Int32.Parse(id));

            return RedirectToAction("classwork", new { class1 = id });
        }

        public ActionResult EditLecture(string title,string note, string lecId, string id)
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login");
            }

            Assignment l = new Assignment();
            l.id = Int32.Parse(lecId);
            l.title = title;
            l.notes = note;

            CRUD.updateLecture(l, Int32.Parse(id));

            return RedirectToAction("classwork", new { class1 = id });
        }

        public ActionResult DeleteAssignment(string assignid, string id)
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login");
            }
            if (CRUD.checkTeacher(Int32.Parse(id), (int)Session["id"]) == 0)
            {
                return RedirectToAction("ClassSelect", new { class1 = id });
            }

            CRUD.deleteAssignment(Int32.Parse(assignid), Int32.Parse(id));

            return RedirectToAction("ClassWork", new { class1 = id });
        }

        public ActionResult DeleteLecture(string Lecid, string id)
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login");
            }
            if (CRUD.checkTeacher(Int32.Parse(id), (int)Session["id"]) == 0)
            {
                return RedirectToAction("ClassSelect", new { class1 = id });
            }

            CRUD.deleteLecture(Int32.Parse(Lecid), Int32.Parse(id));

            return RedirectToAction("ClassWork", new { class1 = id });
        }

        public ActionResult PassC()
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        [HttpPost]
        public ActionResult PassC(string old, string newp)
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login");
            }

            int output = CRUD.updatePassword((int)Session["id"], newp, old);

            if(output==0)
            {
                ViewBag.msg = "Password not changed! Current Password is incorrect";
                return View();
            }

            return RedirectToAction("ClassSelect");
        }

        public ActionResult DisplayC()
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        [HttpPost]
        public ActionResult DisplayC(HttpPostedFileBase picture)
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Login");
            }

            if (picture != null)
            {
                string fileName = Path.GetFileName(picture.FileName);
                string savePath = Server.MapPath("~/Pictures/");
                string pathToCheck = savePath + fileName;
                string temp = fileName;
                int count = 1;

                while (System.IO.File.Exists(pathToCheck))
                {
                    temp = count.ToString() + fileName;
                    pathToCheck = savePath + temp;
                    count++;
                }

                picture.SaveAs(pathToCheck);

                savePath = "/Pictures/" + temp;

                CRUD.updatePicture((int)Session["id"], savePath);
                User u = CRUD.getUserDetails((int)Session["id"]);
                Session["user"] = u;
            }

            return View();
        }
    }
}
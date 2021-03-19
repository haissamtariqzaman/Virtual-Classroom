using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Data;

namespace DB_Project.Models
{
    public class CRUD
    {
        //private static string connString = "Data Source=localhost; Initial Catalog=virtualClassroom; Integrated Security=True";
        private static string connString = "workstation id=VClassroom.mssql.somee.com;packet size=4096;user id=haissamtariq_SQLLogin_1;pwd=t7y49ym1xa;data source=VClassroom.mssql.somee.com;persist security info=False;initial catalog=VClassroom";

        public static int signup(string fname, string lname, string dob, char gender, string email, string password)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd;
            int result = 0;

            try
            {
                conn.Open();
                cmd = new SqlCommand("signup", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@fName", SqlDbType.VarChar, 30).Value = fname;
                cmd.Parameters.Add("@lName", SqlDbType.VarChar, 30).Value = lname;
                cmd.Parameters.Add("@dob", SqlDbType.VarChar, 10).Value = dob;
                cmd.Parameters.Add("@gender", SqlDbType.Char).Value = gender;
                cmd.Parameters.Add("@email", SqlDbType.VarChar, 64).Value = email;
                cmd.Parameters.Add("@password", SqlDbType.VarChar, 64).Value = password;
                cmd.Parameters.Add("@picture", SqlDbType.VarChar, 50).Value = "/Pictures/user-512.png";
                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                result = (int)cmd.Parameters["@output"].Value;
            }

            catch (SqlException excep)
            {
                result = -1;
                Console.WriteLine(excep.Message.ToString());
            }

            finally
            {
                conn.Close();
            }

            return result;
        }

        public static int login(string username, string password)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd;
            int status = -1;

            try
            {
                conn.Open();
                cmd = new SqlCommand("login", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@username", SqlDbType.VarChar, 64).Value = username;
                cmd.Parameters.Add("@password", SqlDbType.VarChar, 64).Value = password;
                cmd.Parameters.Add("@status", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                status = (int)cmd.Parameters["@status"].Value;
            }

            catch (SqlException ex)
            {
                status = -2;
                Console.WriteLine(ex.Message.ToString());
            }

            finally
            {
                conn.Close();
            }

            return status;
        }

        private static string generateCode()
        {
            string domain = "a0bc1de2f3gh4ijk5l6mn7op8qr9stuvwxyz";
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd;
            int status = 0;
            Random rand = new Random();
            string code = null;

            while (status == 0)
            {
                code = null;
                for (int x = 0; x < 6; x++)
                {
                    code = code + domain[rand.Next(36)];
                }
                try
                {
                    conn.Open();
                    cmd = new SqlCommand("checkClassCode", conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@ccode", SqlDbType.VarChar, 6).Value = code;
                    cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.ExecuteNonQuery();
                    status = (int)cmd.Parameters["@output"].Value;
                }
                catch (SqlException s)
                {
                    status = -1;
                    Console.WriteLine(s.Message.ToString());
                }
                finally
                {
                    conn.Close();
                }
            }
            return code;
        }

        public static int addClass(string name, string sec, string subject, int userid)
        {
            string code = generateCode();

            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd;
            int status = 0;

            try
            {
                conn.Open();
                cmd = new SqlCommand("addClass", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@code", SqlDbType.VarChar, 6).Value = code;
                cmd.Parameters.Add("@name", SqlDbType.VarChar, 64).Value = name;
                cmd.Parameters.Add("@section", SqlDbType.Char, 1).Value = sec[0];
                cmd.Parameters.Add("@subject", SqlDbType.VarChar, 30).Value = subject;
                cmd.Parameters.Add("@userId", SqlDbType.Int).Value = userid;
                cmd.ExecuteNonQuery();
                status = 1;
            }
            catch (SqlException s)
            {
                Console.WriteLine(s.Message.ToString());
                status = -1;
            }
            finally
            {
                conn.Close();
            }
            return status;
        }

        public static int joinClass(string code, int userid)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd;
            int status = 0;

            try
            {
                conn.Open();
                cmd = new SqlCommand("joinClass", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@studentId", SqlDbType.Int).Value = userid;
                cmd.Parameters.Add("@code", SqlDbType.VarChar, 6).Value = code;
                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                status = (int)cmd.Parameters["@output"].Value;
            }
            catch(SqlException s)
            {
                status = -1;
                Console.WriteLine(s.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            return status;
        }

        public static int exitClass(int classId, int userid)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd;
            int status = 0;

            try
            {
                conn.Open();
                cmd = new SqlCommand("exitClass", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@userid", SqlDbType.Int).Value = userid;
                cmd.Parameters.Add("@cid", SqlDbType.VarChar, 6).Value = classId;
                cmd.Parameters.Add("@myoutput", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                status = (int)cmd.Parameters["@myoutput"].Value;
            }
            catch (SqlException s)
            {
                status = -1;
                Console.WriteLine(s.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            return status;
        }

        public static List<ClassDetails> getClassDetails(int userid)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd;
            List<ClassDetails> list = null;

            try
            {
                conn.Open();
                cmd = new SqlCommand("Get_Classes_Details", conn);
                cmd.CommandType=System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = userid;
                SqlDataReader data = cmd.ExecuteReader();

                list = new List<ClassDetails>();

                while (data.Read())
                {
                    ClassDetails cd = new ClassDetails();
                    cd.id = (int)data["id"];
                    cd.name = data["name"].ToString();
                    cd.section = (data["section"].ToString())[0];
                    cd.fname = data["fName"].ToString();
                    cd.lname = data["lname"].ToString();
                    cd.picture = data["picture"].ToString();
                    list.Add(cd);
                }

                data.Close();
            }
            catch (SqlException s)
            {
                Console.WriteLine("SQL Error" + s.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            return list;
        }

        public static ClassDetails getsClassDetail(int classid)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd;
            ClassDetails classd = null;

            try
            {
                conn.Open();
                cmd = new SqlCommand("getClassDetail", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@cid", SqlDbType.Int).Value = classid;
                cmd.Parameters.Add("@name",SqlDbType.VarChar,40).Direction=ParameterDirection.Output;
                cmd.Parameters.Add("@section",SqlDbType.Char,1).Direction=ParameterDirection.Output;
                cmd.Parameters.Add("@code",SqlDbType.VarChar,6).Direction=ParameterDirection.Output;
                cmd.ExecuteNonQuery();

                classd=new ClassDetails();
                classd.name = cmd.Parameters["@name"].Value.ToString();
                classd.section = (cmd.Parameters["@section"].Value.ToString())[0];
                classd.code = cmd.Parameters["@code"].Value.ToString();
                classd.id = classid;
            }
            catch (SqlException s)
            {
                Console.WriteLine("SQL Error" + s.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            return classd;
        }

        public static Assignment[] getAssignmentDueSoon(List<ClassDetails> cs, int userid)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = null;
            Assignment[] assign = new Assignment[cs.Count];

            try
            {
                conn.Open();
                cmd = new SqlCommand("assignmnetdue", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@id_student", SqlDbType.Int).Value = userid;
                cmd.Parameters.Add("@id_class", SqlDbType.Int);

                int x = 0;

                foreach (ClassDetails d in cs)
                {
                    cmd.Parameters["@id_class"].Value = d.id;
                    SqlDataReader data = cmd.ExecuteReader();
                    Assignment a = null;
                    if (data.HasRows && data.Read())
                    {
                        a = new Assignment();
                        a.id = (int) data["ID"];
                        a.title = data["Titles"].ToString();
                        a.notes = data["Notes"].ToString();
                        a.deadline = data["Deadline"].ToString();
                        a.noOfResub = (int)data["NoOfResub"];
                        a.dateUploaded = data["dateUploaded"].ToString();
                        a.setDayDue();
                    }
                    data.Close();
                    assign[x++] = a;
                }
            }
            catch (SqlException s)
            {
                Console.WriteLine("SQL Error" + s.Message.ToString());
            }
            finally
            {
                conn.Close();
            }

            return assign;
        }

        public static User getUserDetails(int userId)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = null;
            User u = null;

            try
            {
                conn.Open();
                cmd = new SqlCommand("Get_Details", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = userId;
                cmd.Parameters.Add("@email", SqlDbType.VarChar, 64).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@fname", SqlDbType.VarChar, 30).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@lname", SqlDbType.VarChar, 30).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@dob", SqlDbType.Date, 64).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@gender", SqlDbType.Char, 1).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@picture", SqlDbType.VarChar, 30).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();

                u = new User();

                u.fname = cmd.Parameters["@fname"].Value.ToString();
                u.lname = cmd.Parameters["@lname"].Value.ToString();
                u.pic = cmd.Parameters["@picture"].Value.ToString();
                u.id = userId;
            }
            catch (SqlException s)
            {
                Console.WriteLine("SQL Error" + s.Message.ToString());
            }
            finally
            {
                conn.Close();
            }

            return u;
        }

        private static List<Comment> getComments(int postId)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = null;
            List<Comment> c = null;

            try
            {
                conn.Open();
                cmd = new SqlCommand("comment_posts", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = postId;
                SqlDataReader data=cmd.ExecuteReader();

                c=new List<Comment>();

                while(data.Read())
                {
                    Comment temp=new Comment();
                    temp.user = new User();
                    temp.user.id=(int)data["UserID"];
                    temp.commentContent=data["content"].ToString();
                    temp.datePosted = (DateTime)data["datePosted"];
                    c.Add(temp);
                }

                foreach (Comment cm in c)
                {
                    cm.user = getUserDetails(cm.user.id);
                }
            }
            catch (SqlException s)
            {
                Console.WriteLine("SQL Error" + s.Message.ToString());
            }
            finally
            {
                conn.Close();
            }

            return c;
        }

        private static List<File> getFiles(int postId)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = null;
            List<File> f = null;

            try
            {
                conn.Open();
                cmd = new SqlCommand("file_posts", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@idfile", SqlDbType.Int).Value = postId;
                SqlDataReader data = cmd.ExecuteReader();

                f = new List<File>();

                while (data.Read())
                {
                    File temp = new File();
                    temp.fileName = data["File_Name"].ToString();
                    temp.filePath = data["File_Path"].ToString();
                    f.Add(temp);
                }
            }
            catch (SqlException s)
            {
                Console.WriteLine("SQL Error" + s.Message.ToString());
            }
            finally
            {
                conn.Close();
            }

            return f;
        }
        
        public static List<Post> getPosts(int classId)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = null;
            List<Post> posts = null;
            
            try
            {
                conn.Open();
                cmd = new SqlCommand("getPosts", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = classId;
                SqlDataReader data = cmd.ExecuteReader();

                posts = new List<Post>();

                while (data.Read())
                {
                    Post p = new Post();
                    p.id = (int)data["ID"];
                    p.user=new User();
                    p.user.id = (int)data["userID"];
                    p.post = data["Content"].ToString();
                    p.datePosted = data["datePosted"].ToString();
                    posts.Add(p);
                }

                foreach (Post p in posts)
                {
                    p.user = getUserDetails(p.user.id);
                    p.comments = getComments(p.id);
                    p.files = getFiles(p.id);
                }
            }
            catch (SqlException s)
            {
                Console.WriteLine("SQL Error" + s.Message.ToString());
            }
            finally
            {
                conn.Close();
            }

            return posts;
        }

        public static bool checkClass(int userId, int classId)
        {
            bool found=false;
            List<ClassDetails> c = getClassDetails(userId);
            foreach (ClassDetails cl in c)
            {
                if (cl.id == classId)
                {
                    found = true;
                    break;
                }
            }
            return found;
        }

        public static int addComment(int postId, int userId, string content)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = null;
            int output = 0;
            try
            {
                conn.Open();
                cmd = new SqlCommand("addcomment", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@postId", SqlDbType.Int).Value = postId;
                cmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                cmd.Parameters.Add("@content", SqlDbType.VarChar,510).Value = content;
                cmd.ExecuteNonQuery();
                output = 1;
            }
            catch (SqlException s)
            {
                Console.WriteLine("SQL Error" + s.Message.ToString());
                output = -1;
            }
            finally
            {
                conn.Close();
            }
            return output;
        }

        private static int addPostFiles(int pid,List<File> files)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = null;
            int output = 0;

            try
            {
                conn.Open();
                cmd = new SqlCommand("addPostFiles", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@postID", SqlDbType.Int).Value=pid;
                cmd.Parameters.Add("@FilePath", SqlDbType.VarChar,200);
                cmd.Parameters.Add("@FileName", SqlDbType.VarChar, 50);

                foreach (File f in files)
                {
                    cmd.Parameters["@FilePath"].Value = f.filePath;
                    cmd.Parameters["@FileName"].Value = f.fileName;
                    cmd.ExecuteNonQuery();
                }
                output = 1;
            }
            catch (SqlException s)
            {
                Console.WriteLine("SQL Error" + s.Message.ToString());
                output = -1;
            }
            finally
            {
                conn.Close();
            }
            return output;
        }

        public static int addPost(int cid, int userId, string post, List<File> files )
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = null;
            int output = 0;
            int id = -1;
            try
            {
                conn.Open();
                cmd = new SqlCommand("addPost", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@classId", SqlDbType.Int).Value = cid;
                cmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                cmd.Parameters.Add("@content", SqlDbType.VarChar, 510).Value = post;
                cmd.Parameters.Add("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                id=(int)cmd.Parameters["@Id"].Value;
                output = (files==null)?1:addPostFiles(id, files);
            }
            catch (SqlException s)
            {
                Console.WriteLine("SQL Error" + s.Message.ToString());
                output = -1;
            }
            finally
            {
                conn.Close();
            }
            return output;
        }

        public static List<User> getStudents(int classId)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = null;
            List<User> u = new List<User>();

            try
            {
                conn.Open();
                cmd = new SqlCommand("StudnetsOfClass", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@class_id", SqlDbType.Int).Value = classId;
                SqlDataReader data = cmd.ExecuteReader();

                while (data.Read())
                {
                    User us = new User();
                    us.id = (int)data["id"];
                    us.fname = data["fName"].ToString();
                    us.lname = data["lname"].ToString();
                    us.email = data["email"].ToString();
                    us.pic = data["picture"].ToString();
                    u.Add(us);
                }
            }
            catch (SqlException s)
            {
                Console.WriteLine("SQL Error" + s.Message.ToString());
            }
            finally
            {
                conn.Close();
            }

            return u;
        }

        public static List<User> getTeachers(int classId)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = null;
            List<User> u = new List<User>();

            try
            {
                conn.Open();
                cmd = new SqlCommand("TeachersOfCLass", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@class_id", SqlDbType.Int).Value = classId;
                SqlDataReader data = cmd.ExecuteReader();

                while (data.Read())
                {
                    User us = new User();
                    us.id = (int)data["id"];
                    us.fname = data["fName"].ToString();
                    us.lname = data["lname"].ToString();
                    us.email = data["email"].ToString();
                    us.pic = data["picture"].ToString();
                    u.Add(us);
                }
            }
            catch (SqlException s)
            {
                Console.WriteLine("SQL Error" + s.Message.ToString());
            }
            finally
            {
                conn.Close();
            }

            return u;
        }

        public static int addStudent(int classId, string email)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = null;
            int output = 0;

            try
            {
                conn.Open();
                cmd = new SqlCommand("addStudent", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@classId", SqlDbType.Int).Value = classId;
                cmd.Parameters.Add("@email", SqlDbType.VarChar,64).Value = email;
                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                output = (int)cmd.Parameters["@output"].Value;
            }
            catch(SqlException s)
            {
                Console.WriteLine("SQL Error" + s.Message.ToString());
                output = -1;
            }
            finally
            {
                conn.Close();
            }

            return output;
        }

        public static int addTeacher(int classId, string email)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = null;
            int output = 0;

            try
            {
                conn.Open();
                cmd = new SqlCommand("addTeacher", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@classId", SqlDbType.Int).Value = classId;
                cmd.Parameters.Add("@email", SqlDbType.VarChar, 64).Value = email;
                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                output = (int)cmd.Parameters["@output"].Value;
            }
            catch (SqlException s)
            {
                Console.WriteLine("SQL Error" + s.Message.ToString());
                output = -1;
            }
            finally
            {
                conn.Close();
            }

            return output;
        }

        public static int removeTeacher(int classId, int userid)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = null;
            int output = 0;

            try
            {
                conn.Open();
                cmd = new SqlCommand("removeTeacher", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@classId", SqlDbType.Int).Value = classId;
                cmd.Parameters.Add("@teacherId", SqlDbType.Int).Value = userid;
                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                output = (int)cmd.Parameters["@output"].Value;
            }
            catch (SqlException s)
            {
                Console.WriteLine("SQL Error" + s.Message.ToString());
                output = -1;
            }
            finally
            {
                conn.Close();
            }

            return output;
        }

        public static int removeStudent(int classId, int userid)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = null;
            int output = 0;

            try
            {
                conn.Open();
                cmd = new SqlCommand("removeStudent", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@classId", SqlDbType.Int).Value = classId;
                cmd.Parameters.Add("@studentId", SqlDbType.Int).Value = userid;
                cmd.ExecuteNonQuery();
                output = 1;
            }
            catch (SqlException s)
            {
                Console.WriteLine("SQL Error" + s.Message.ToString());
                output = -1;
            }
            finally
            {
                conn.Close();
            }

            return output;
        }

        public static int checkTeacher(int classId, int userid)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = null;
            int output = 0;

            try
            {
                conn.Open();
                cmd = new SqlCommand("checkTeacher", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@cid", SqlDbType.Int).Value = classId;
                cmd.Parameters.Add("@userid", SqlDbType.Int).Value = userid;
                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                output = (int)cmd.Parameters["@output"].Value;
            }
            catch (SqlException s)
            {
                Console.WriteLine("SQL Error" + s.Message.ToString());
                output = -1;
            }
            finally
            {
                conn.Close();
            }

            return output;
        }

        private static List<File> getFilesAssignment(int assignmentId)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = null;
            List<File> f = null;

            try
            {
                conn.Open();
                cmd = new SqlCommand("assignFiles", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@assignId", SqlDbType.Int).Value = assignmentId;
                SqlDataReader data = cmd.ExecuteReader();

                f = new List<File>();

                while (data.Read())
                {
                    File temp = new File();
                    temp.fileName = data["fileName"].ToString();
                    temp.filePath = data["FilePath"].ToString();
                    f.Add(temp);
                }
            }
            catch (SqlException s)
            {
                Console.WriteLine("SQL Error" + s.Message.ToString());
            }
            finally
            {
                conn.Close();
            }

            return f;
        }

        public static List<Assignment> getAssignments(int classId)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = null;
            List<Assignment> a_list = new List<Assignment>();

            try
            {
                conn.Open();
                cmd = new SqlCommand("assignment_detail", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@class_id", SqlDbType.Int).Value = classId;
                SqlDataReader data = cmd.ExecuteReader();

                while (data.Read())
                {
                    Assignment a = new Assignment();
                    a.id = (int)data["id"];
                    a.title = data["Titles"].ToString();
                    a.notes = data["Notes"].ToString();
                    a.deadline = data["Deadline"].ToString();
                    a.noOfResub = (int)data["NoOfResub"];
                    a.dateUploaded = data["dateUploaded"].ToString();
                    a.editDate();
                    a_list.Add(a);
                }
                foreach (Assignment a in a_list)
                {
                    a.files = getFilesAssignment(a.id);
                }
            }
            catch (SqlException s)
            {
                Console.WriteLine("SQL Error" + s.Message.ToString());
            }
            finally
            {
                conn.Close();
            }

            return a_list;
        }

        private static List<File> getFileslectures(int assignmentId)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = null;
            List<File> f = null;

            try
            {
                conn.Open();
                cmd = new SqlCommand("file_lecture", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@idl", SqlDbType.Int).Value = assignmentId;
                SqlDataReader data = cmd.ExecuteReader();

                f = new List<File>();

                while (data.Read())
                {
                    File temp = new File();
                    temp.fileName = data["fileName"].ToString();
                    temp.filePath = data["FilePath"].ToString();
                    f.Add(temp);
                }
            }
            catch (SqlException s)
            {
                Console.WriteLine("SQL Error" + s.Message.ToString());
            }
            finally
            {
                conn.Close();
            }

            return f;
        }

        public static List<Assignment> getLectures(int classId)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = null;
            List<Assignment> l_list = new List<Assignment>();

            try
            {
                conn.Open();
                cmd = new SqlCommand("detail_lecture", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@idc", SqlDbType.Int).Value = classId;
                SqlDataReader data = cmd.ExecuteReader();

                while (data.Read())
                {
                    Assignment lec = new Assignment();
                    lec.id = (int)data["id"];
                    lec.title = data["Title"].ToString();
                    lec.notes = data["note"].ToString();
                    lec.dateUploaded = data["dateUploaded"].ToString();
                    lec.editDate();
                    l_list.Add(lec);
                }
                foreach (Assignment lec in l_list)
                {
                    lec.files = getFileslectures(lec.id);
                }
            }
            catch (SqlException s)
            {
                Console.WriteLine("SQL Error" + s.Message.ToString());
            }
            finally
            {
                conn.Close();
            }

            return l_list;
        }

        private static int addAssignmentFiles(int Aid,File f)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = null;
            int output = 0;

            try
            {
                conn.Open();
                cmd = new SqlCommand("addAssignmentFiles", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@AID", SqlDbType.Int).Value = Aid;
                cmd.Parameters.Add("@FilePath", SqlDbType.VarChar, 200).Value = f.filePath;
                cmd.Parameters.Add("@FileName", SqlDbType.VarChar, 50).Value = f.fileName;
                cmd.ExecuteNonQuery();
                output = 1;
            }
            catch (SqlException s)
            {
                Console.WriteLine("SQL Error" + s.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            return output;
        }

        public static int addAssignmnet(int classId, Assignment a)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = null;
            int output = 0;

            try
            {
                conn.Open();
                cmd = new SqlCommand("addAssignment", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@classID", SqlDbType.Int).Value = classId;
                cmd.Parameters.Add("@Titles", SqlDbType.VarChar, 50).Value = a.title;
                cmd.Parameters.Add("@Notes", SqlDbType.VarChar, 510).Value = a.notes;
                cmd.Parameters.Add("@Deadline", SqlDbType.DateTime).Value = a.deadline;
                cmd.Parameters.Add("@NoOfResub", SqlDbType.Int).Value = a.noOfResub;
                cmd.Parameters.Add("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                int Aid = (int)cmd.Parameters["@Id"].Value;

                if (a.files != null)
                {
                    foreach (File f in a.files)
                    {
                        addAssignmentFiles(Aid, f);
                    }
                }

                output = 1;
            }
            catch (SqlException s)
            {
                Console.WriteLine("SQL Error" + s.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            return output;
        }

        private static int addLectureFiles(int lid, File f)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = null;
            int output = 0;

            try
            {
                conn.Open();
                cmd = new SqlCommand("addLectureFiles", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@LecId", SqlDbType.Int).Value = lid;
                cmd.Parameters.Add("@filePath", SqlDbType.VarChar, 200).Value = f.filePath;
                cmd.Parameters.Add("@fileName", SqlDbType.VarChar, 50).Value = f.fileName;
                cmd.ExecuteNonQuery();
                output = 1;
            }
            catch (SqlException s)
            {
                Console.WriteLine("SQL Error" + s.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            return output;
        }

        public static int addLecture(int classId, Assignment a)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = null;
            int output = 0;

            try
            {
                conn.Open();
                cmd = new SqlCommand("addLecture", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@classId", SqlDbType.Int).Value = classId;
                cmd.Parameters.Add("@title", SqlDbType.VarChar, 50).Value = a.title;
                cmd.Parameters.Add("@note", SqlDbType.VarChar, 510).Value = a.notes;
                cmd.Parameters.Add("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                int lid = (int)cmd.Parameters["@Id"].Value;

                if (a.files != null)
                {
                    foreach (File f in a.files)
                    {
                        addLectureFiles(lid, f);
                    }
                }

                output = 1;
            }
            catch (SqlException s)
            {
                Console.WriteLine("SQL Error" + s.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            return output;
        }

        public static int checkStudent(int classId, int userid)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = null;
            int output = 0;

            try
            {
                conn.Open();
                cmd = new SqlCommand("checkStudent", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@cid", SqlDbType.Int).Value = classId;
                cmd.Parameters.Add("@userid", SqlDbType.Int).Value = userid;
                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                output = (int)cmd.Parameters["@output"].Value;
            }
            catch (SqlException s)
            {
                Console.WriteLine("SQL Error" + s.Message.ToString());
                output = -1;
            }
            finally
            {
                conn.Close();
            }

            return output;
        }

        public static int noOfsubmission(int assignId, int userid)
        {
            SqlConnection conn  = new SqlConnection(connString);
            SqlCommand cmd = null;
            int subs=-1;

            try
            {
                conn.Open();
                cmd = new SqlCommand("NoOfTimesUserSubmitted",conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@assign_id", SqlDbType.Int).Value = assignId;
                cmd.Parameters.Add("@user_id", SqlDbType.Int).Value = userid;
                cmd.Parameters.Add("@sub", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                subs = (int)cmd.Parameters["@sub"].Value;
            }
            catch (SqlException s)
            {
                Console.WriteLine("SQL error! " + s.Message.ToString());
            }
            finally
            {
                conn.Close();
            }

            return subs;
        }

        public static Assignment getAssignDetail(int assignId)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = null;
            Assignment a=null;

            try
            {
                conn.Open();
                cmd = new SqlCommand("assignDetails", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@assign_id", SqlDbType.Int).Value = assignId;
                SqlDataReader data = cmd.ExecuteReader();
                data.Read();
                a = new Assignment();
                a.id = (int)data["ID"];
                a.title = data["Titles"].ToString();
                a.notes = data["Notes"].ToString();
                a.deadline = data["Deadline"].ToString();
                a.noOfResub = (int)data["NoOfResub"];
                a.dateUploaded = data["dateUploaded"].ToString();
                a.files = getFilesAssignment(assignId);
            }
            catch (SqlException s)
            {
                Console.WriteLine("SQL error! " + s.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            return a;
        }

        private static int assignsubmitFiles(File f, int subId)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = null;
            int output = 0;

            try
            {
                conn.Open();
                cmd = new SqlCommand("addAssignmentSubFiles", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@subID", SqlDbType.Int).Value = subId;
                cmd.Parameters.Add("@FilePath", SqlDbType.VarChar, 200).Value = f.filePath;
                cmd.Parameters.Add("@FileName", SqlDbType.VarChar, 50).Value = f.fileName;
                cmd.ExecuteNonQuery();
                output = 1;
            }
            catch (SqlException s)
            {
                Console.WriteLine("SQL Error" + s.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            return output;
        }

        public static int submitAssignment(int assignId, int userId, List<File> files)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = null;
            int output = 0;

            try
            {
                conn.Open();
                cmd = new SqlCommand("submitAssignment", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@AID", SqlDbType.Int).Value = assignId;
                cmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                cmd.Parameters.Add("@ID", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();

                int id = (int)cmd.Parameters["@ID"].Value;

                if (files != null)
                {
                    foreach (File f in files)
                    {
                        assignsubmitFiles(f, id);
                    }
                }
                output = 1;
            }
            catch (SqlException s)
            {
                Console.WriteLine("SQL error! " + s.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            return output;
        }

        private static List<File> getAssignSubFiles(int subId)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = null;
            List<File> f = null;

            try
            {
                conn.Open();
                cmd = new SqlCommand("assignSubFiles", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@assignSubId", SqlDbType.Int).Value = subId;
                SqlDataReader data = cmd.ExecuteReader();

                f = new List<File>();

                while (data.Read())
                {
                    File temp = new File();
                    temp.fileName = data["FileName"].ToString();
                    temp.filePath = data["FilePath"].ToString();
                    f.Add(temp);
                }
            }
            catch (SqlException s)
            {
                Console.WriteLine("SQL Error" + s.Message.ToString());
            }
            finally
            {
                conn.Close();
            }

            return f;
        }

        public static AssignmentSubmitted getUserSubmission(int userId, int aid)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = null;
            AssignmentSubmitted asub = null;
         
            try
            {
                conn.Open();
                cmd = new SqlCommand("getUserSubmission", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@aid", SqlDbType.Int).Value = aid;
                cmd.Parameters.Add("@userid", SqlDbType.Int).Value = userId;
                cmd.Parameters.Add("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@datesub", SqlDbType.DateTime).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@late", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();

                if ((int)cmd.Parameters["@Id"].Value!=-1)
                {
                    asub = new AssignmentSubmitted();
                    asub.submissionId=(int)cmd.Parameters["@Id"].Value;
                    asub.dateSubmitted = cmd.Parameters["@datesub"].Value.ToString();
                    asub.assignmentId = aid;
                    asub.filesSubmitted = getAssignSubFiles(asub.submissionId);
                    if ((int)cmd.Parameters["@late"].Value == 0)
                    {
                        asub.status = "On Time";
                    }
                    else
                    {
                        asub.status = "Late";
                    }
                }
            }
            catch (SqlException s)
            {
                Console.WriteLine("SQL error! " + s.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            return asub;
        }

        private static List<File> getFilesSubAssignment(int assignmentSubId)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = null;
            List<File> f = null;

            try
            {
                conn.Open();
                cmd = new SqlCommand("assignSubFiles", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@assignSubId", SqlDbType.Int).Value = assignmentSubId;
                SqlDataReader data = cmd.ExecuteReader();

                f = new List<File>();

                while (data.Read())
                {
                    File temp = new File();
                    temp.fileName = data["fileName"].ToString();
                    temp.filePath = data["FilePath"].ToString();
                    f.Add(temp);
                }
            }
            catch (SqlException s)
            {
                Console.WriteLine("SQL Error" + s.Message.ToString());
            }
            finally
            {
                conn.Close();
            }

            return f;
        }

        public static List<AssignmentSubmitted> getSubmittedAssingment(int assignId)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = null;
            List<AssignmentSubmitted> a_list = new List<AssignmentSubmitted>();

            try
            {
                conn.Open();
                cmd = new SqlCommand("assignOnTime", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@assignId", SqlDbType.Int).Value = assignId;
                SqlDataReader data = cmd.ExecuteReader();

                while (data.Read())
                {
                    AssignmentSubmitted a = new AssignmentSubmitted();
                    a.submissionId = (int)data["ID"];
                    a.user = new User();
                    a.user.id = (int)data["userID"];
                    a.dateSubmitted = data["dateSubmitted"].ToString();
                    a_list.Add(a);
                }
                foreach (AssignmentSubmitted a in a_list)
                {
                    a.filesSubmitted = getFilesSubAssignment(a.submissionId);
                    a.user = getUserDetails(a.user.id);
                }
            }
            catch (SqlException s)
            {
                Console.WriteLine("SQL Error" + s.Message.ToString());
            }
            finally
            {
                conn.Close();
            }

            return a_list;
        }

        public static List<AssignmentSubmitted> getLateSubmittedAssingment(int assignId)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = null;
            List<AssignmentSubmitted> a_list = new List<AssignmentSubmitted>();

            try
            {
                conn.Open();
                cmd = new SqlCommand("assignNotOnTime", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@assignId", SqlDbType.Int).Value = assignId;
                SqlDataReader data = cmd.ExecuteReader();

                while (data.Read())
                {
                    AssignmentSubmitted a = new AssignmentSubmitted();
                    a.submissionId = (int)data["ID"];
                    a.user = new User();
                    a.user.id = (int)data["userID"];
                    a.dateSubmitted = data["dateSubmitted"].ToString();
                    a_list.Add(a);
                }
                foreach (AssignmentSubmitted a in a_list)
                {
                    a.filesSubmitted = getFilesSubAssignment(a.submissionId);
                    a.user = getUserDetails(a.user.id);
                }
            }
            catch (SqlException s)
            {
                Console.WriteLine("SQL Error" + s.Message.ToString());
            }
            finally
            {
                conn.Close();
            }

            return a_list;
        }

        public static List<User> getUnsubmittedAssingment(int assignId)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = null;
            List<User> a_list = new List<User>();

            try
            {
                conn.Open();
                cmd = new SqlCommand("studentsNotSubmittedAssign", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@assignId", SqlDbType.Int).Value = assignId;
                SqlDataReader data = cmd.ExecuteReader();

                while (data.Read())
                {
                    User a = new User();
                    a.id = (int)data["studentId"];
                    a = getUserDetails(a.id);
                    a_list.Add(a);
                }
            }
            catch (SqlException s)
            {
                Console.WriteLine("SQL Error" + s.Message.ToString());
            }
            finally
            {
                conn.Close();
            }

            return a_list;
        }

        public static int updateAssignemnt(Assignment a, int classid)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = null;
            int output = 0;

            try
            {
                conn.Open();
                cmd = new SqlCommand("updAssignment", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = a.id;
                cmd.Parameters.Add("@title", SqlDbType.VarChar,50).Value = a.title;
                cmd.Parameters.Add("@notes", SqlDbType.VarChar,510).Value = a.notes;
                cmd.Parameters.Add("@deadline", SqlDbType.DateTime).Value = a.deadline;
                cmd.Parameters.Add("@noofresub", SqlDbType.Int).Value = a.noOfResub;
                cmd.Parameters.Add("@classId", SqlDbType.Int).Value = classid;
                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                output = (int)cmd.Parameters["@output"].Value;
            }
            catch (SqlException s)
            {
                Console.WriteLine("SQL Error" + s.Message.ToString());
                output = -1;
            }
            finally
            {
                conn.Close();
            }

            return output;
        }

        public static Assignment getLectureDetails(int lecId)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = null;
            Assignment l = null;

            try
            {
                conn.Open();
                cmd = new SqlCommand("getLecDetails", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@Lid", SqlDbType.Int).Value = lecId;
                cmd.Parameters.Add("@title", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@note", SqlDbType.VarChar, 510).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@dateUploaded", SqlDbType.Date).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();

                l = new Assignment();
                l.id = lecId;
                l.title = cmd.Parameters["@title"].Value.ToString();
                l.notes = cmd.Parameters["@note"].Value.ToString();
                l.dateUploaded = cmd.Parameters["@dateUploaded"].Value.ToString();
                l.files = getFileslectures(lecId);
            }
            catch (SqlException s)
            {
                Console.WriteLine("SQL error! " + s.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            return l;
        }

        public static int updateLecture(Assignment a, int classid)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = null;
            int output = 0;

            try
            {
                conn.Open();
                cmd = new SqlCommand("updLecture", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = a.id;
                cmd.Parameters.Add("@title", SqlDbType.VarChar, 50).Value = a.title;
                cmd.Parameters.Add("@note", SqlDbType.VarChar, 510).Value = a.notes;
                cmd.Parameters.Add("@classId", SqlDbType.Int).Value = classid;
                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                output = (int)cmd.Parameters["@output"].Value;
            }
            catch (SqlException s)
            {
                Console.WriteLine("SQL Error" + s.Message.ToString());
                output = -1;
            }
            finally
            {
                conn.Close();
            }

            return output;
        }

        public static int deleteAssignment(int aId,int classId)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = null;
            int output = 0;

            try
            {
                conn.Open();
                cmd = new SqlCommand("deleteAssignment", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@assignId", SqlDbType.Int).Value = aId;
                cmd.Parameters.Add("@classId", SqlDbType.Int).Value = classId;
                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                output = (int)cmd.Parameters["@output"].Value;
            }
            catch (SqlException s)
            {
                Console.WriteLine("SQL Error" + s.Message.ToString());
                output = -1;
            }
            finally
            {
                conn.Close();
            }
            return output;
        }

        public static int deleteLecture(int lId, int classId)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = null;
            int output = 0;

            try
            {
                conn.Open();
                cmd = new SqlCommand("deleteLecture", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@lectureId", SqlDbType.Int).Value = lId;
                cmd.Parameters.Add("@classId", SqlDbType.Int).Value = classId;
                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                output = (int)cmd.Parameters["@output"].Value;
            }
            catch (SqlException s)
            {
                Console.WriteLine("SQL Error" + s.Message.ToString());
                output = -1;
            }
            finally
            {
                conn.Close();
            }
            return output;
        }

        public static int updatePassword(int userId, string pass, string old)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = null;
            int output = 0;

            try
            {
                conn.Open();
                cmd = new SqlCommand("updPassword", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = userId;
                cmd.Parameters.Add("@password", SqlDbType.VarChar, 64).Value = pass;
                cmd.Parameters.Add("@oldP", SqlDbType.VarChar, 64).Value = old;
                cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                output = (int)cmd.Parameters["@output"].Value;
            }
            catch (SqlException s)
            {
                Console.WriteLine("SQL Error" + s.Message.ToString());
                output = -1;
            }
            finally
            {
                conn.Close();
            }

            return output;
        }

        public static int updatePicture(int userId, string picture)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = null;
            int output = 0;

            try
            {
                conn.Open();
                cmd = new SqlCommand("updPicture", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = userId;
                cmd.Parameters.Add("@picture", SqlDbType.VarChar, 250).Value = picture;
                cmd.ExecuteNonQuery();
                output = 1;
            }
            catch (SqlException s)
            {
                Console.WriteLine("SQL Error" + s.Message.ToString());
                output = -1;
            }
            finally
            {
                conn.Close();
            }

            return output;
        }

        public static int deletePost(int pId)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = null;
            int output = 0;

            try
            {
                conn.Open();
                cmd = new SqlCommand("deletePost", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@pId", SqlDbType.Int).Value = pId;
                cmd.ExecuteNonQuery();
                output = 1;
            }
            catch (SqlException s)
            {
                Console.WriteLine("SQL Error" + s.Message.ToString());
                output = -1;
            }
            finally
            {
                conn.Close();
            }
            return output;
        }

        public static int deleteComment(int pid,int uid,DateTime date)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = null;
            int output = 0;

            try
            {
                conn.Open();
                cmd = new SqlCommand("deleteComment", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@pId", SqlDbType.Int).Value = pid;
                cmd.Parameters.Add("@userId", SqlDbType.Int).Value = uid;
                cmd.Parameters.Add("@date", SqlDbType.DateTime).Value = date;
                cmd.ExecuteNonQuery();
                output = 1;
            }
            catch (SqlException s)
            {
                Console.WriteLine("SQL Error" + s.Message.ToString());
                output = -1;
            }
            finally
            {
                conn.Close();
            }
            return output;
        }
    }
}
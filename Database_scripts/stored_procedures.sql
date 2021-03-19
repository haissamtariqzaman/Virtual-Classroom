use virtualClassroom
go

/*1*/
Create procedure dbo.[login]
@username varchar(64),
@password varchar(64),
@status int output
As
Begin
	if exists (
		Select *
		From  users 
		where[users].email=@username and [users].password=hashbytes('SHA2_256',@password)
		)
		begin
			select @status=id
			from users
			where users.email=@username
		end
	else
		begin
			set @status=-1
		end
End;
go

/*2*/
create procedure dbo.Get_Password
@username  varchar(64),
@password  varchar(64) OUTPUT
as
begin
select @password= users.password
from [users]
where email = @username
end;
go

/*3*/
create procedure dbo.Get_UserDetails
@email  varchar(64),
@Id int output,
@fname  varchar(30) OUTPUT,
@lname varchar(30) OUTPUT,
@dob  date output,
@gender char output
as
begin
select @id=id, @fname= users.fname , @lname=users.lname, @dob=users.dob, @gender=users.gender
from [users]
where email = @email
end;
go

/*4*/
create procedure dbo.Get_Details
@Id int ,
@email varchar(64) output,
@fname  varchar(30) OUTPUT,
@lname varchar(30) OUTPUT,
@dob  date output,
@gender char output,
@picture varchar(50) output
as
begin
select @email=email, @fname= users.fName , @lname=users.lname, @dob=users.dob, @gender=users.gender, @picture=users.picture
from [users]
where Id = @id
end; 
go

/*5*/
create procedure dbo.Get_Classes_Details
@Id int
as
begin
	select t.classId as id, [name], section, users.fName, users.lname, users.picture
	from(	select classId,min(dateJoined) mdate
			from classTeacher
			group by classId) as t join classTeacher on (classTeacher.classId=t.classId and classTeacher.dateJoined=t.mdate) join class on classTeacher.classId=class.id join [users] on [users].id=classTeacher.teacherId
	where classTeacher.classId in (	select classId
									from classTeacher
									where teacherId=@Id
										union
									select classId
									from classStudent
									where studentId=@Id)
end;
go

/*6*/
create procedure dbo.getPosts
@Id int
as
begin
select *
from class join posts on class.id=posts.classID
where @Id=class.id
order by posts.datePosted desc
end;
go

/*7*/
create procedure dbo.classTeacherr
@classId int
as
begin
select *
from classTeacher join Users on classTeacher.teacherId=Users.ID
where @classId=classTeacher.classId 
end;
go

/*8*/
create procedure dbo.file_posts
@idfile int
as
begin
select [FileName] as [File_Name], FilePath as [File_Path]
from PostFiles
where postID = @idfile
end;
go

/*9*/
create procedure dbo.comment_posts
@id int
as 
begin
select userId as UserID ,content,datePosted
from comments 
where postID = @id
order by datePosted
end;
go

/*10*/
create procedure dbo.assignmnetdue
@id_class int,
@id_student int
as
begin
select a.ID,a.Titles,a.classID,a.Notes,a.Deadline,a.NoOfResub,a.dateUploaded
from Assignments as a 
where classId=@id_class and datediff(day, getdate(),a.Deadline) <= 7 and a.Deadline > getdate() and not exists (select AID
																												from AssignmentSub
																												where AID=a.ID and userID=@id_student )
end;
go

/*11*/
create procedure dbo.detail_lecture
@idc int
as
begin
select *
from lectures
where classId = @idc
end;
go

/*12*/
create procedure dbo.file_lecture
@idl int
as
begin
select [FileName] ,filePath as FilePath
from lectureFiles
where id = @idl
end;
go

/*13*/
create procedure dbo.assignment_detail
@class_id int
as
begin
select *
from Assignments
where classID = @class_id
end;
go

/*14*/
create procedure dbo.assignFiles
@assignId int
as
begin
	select [fileName], FilePath
	from AssignmentFiles
	where AID=@assignId
end;
go

/*15*/
create procedure assignSub
@assignId int
as
begin
	select ID,userId,dateSubmitted
	from AssignmentSub
	where AID=@assignId
end;
go

/*16*/
create procedure assignOnTime
@assignId int
as
begin
	select AssignmentSub.ID,userID,dateSubmitted
	from Assignments join AssignmentSub on Assignments.Id=AID
	where Assignments.ID=@assignId and Deadline>=dateSubmitted
end;
go

/*17*/
create procedure assignNotOnTime
@assignId int
as
begin
	select AssignmentSub.ID,userID,dateSubmitted
	from Assignments join AssignmentSub on Assignments.Id=AID
	where Assignments.ID=@assignId and Deadline<dateSubmitted
end;
go

/*18*/
create procedure studentsNotSubmittedAssign
@assignId int
as
begin
	select studentId
	from (classStudent join class on classStudent.classId=class.id) join Assignments on class.id=Assignments.classID
	where Assignments.id=@assignId
	except
	select userID
	from AssignmentSub
	where AID=@assignId
end;
go

/*19*/
create procedure dbo.assignSubFiles
@assignSubId int
as
begin
	select [FileName],FilePath
	from AssignmentSubFiles
	where subID=@assignSubId
end;
go

/*20*/
CREATE PROCEDURE dbo.assignDetails
@assign_id int
AS
begin
	select *
	from Assignments
	where id=@assign_id
end;
go

/*21*/
CREATE PROCEDURE dbo.NoOfTimesUserSubmitted
@assign_id int,
@user_id int,
@sub int OUTPUT
AS
begin
	select @sub=count(ID)
	from AssignmentSub
	where UserID=@User_ID AND AID=@assign_id
	group by UserID,AID

	if @sub is null
		begin
			set @sub=0
		end
end;
go

/*22*/
CREATE PROCEDURE dbo.TeachersOfCLass
@class_id int
AS
begin
	select id,fName,lname,email,picture
	from classTeacher join users on teacherId=id
	where classid=@class_id
end;
go

/*23*/
CREATE PROCEDURE dbo.StudnetsOfClass
@class_id int
AS
begin
	select id,fName,lname,email,picture
	from users join classStudent on id=studentId
	where classid=@class_id
end;
go

/*24*/
CREATE PROCEDURE dbo.NoOfStudentsSubmittedAssign
@assign_ID int,
@total int OUTPUT
AS
begin
	select count(userID) as totalsub
	from AssignmentSub
	where AID=@assign_ID
	group by AID
end;
go

/*25*/
CREATE PROCEDURE dbo.NoOfStudentsInClass
@class_id int,
@total int OUTPUT
AS
begin
	select count(studentid) as NoOfStudents
	from class join classStudent on id=classid
	where classid=1
	group by classid
end;
go

/*26*/
create procedure dbo.signup
@fName varchar(30),
@lName varchar(30),
@dob date,
@gender char(1),
@email varchar(64),
@password varchar(64),
@picture varchar(50),
@output int output
as
begin
	if exists(	select email
				from users
				where email=@email)
		begin
			set @output=0
		end
	else
		begin
			insert into users(fName,lname,dob,gender,email,[password],picture)
			values(@fName,@lName,@dob,@gender,@email,hashbytes('SHA2_256',@password),@picture)

			set @output=1
		end
end;
go

/*27*/
create procedure dbo.addClass
@code varchar(6),
@name varchar(40),
@section char(1),
@subject varchar(30),
@userId int
as
begin
	begin transaction
		insert into class(code,[name],section,subject,dateCreated)
		values(@code,@name,@section,@subject,GETDATE())

		declare @cid int

		select @cid=id
		from class
		where code=@code

		insert into classTeacher
		values (@cid,@userId,GETDATE(),null)
	commit transaction
end;
go

/*28*/
create procedure joinClass
@studentId int,
@code varchar(6),
@output int output
as
begin
	declare @cid int

	set @cid=null

	select @cid=id
	from class
	where code=@code

	if @cid is not null
		begin
			if not exists(	select teacherId
					from classTeacher
					where teacherId=@studentId and classId=@cid)
				begin
					insert into classStudent
					values(@cid,@studentId,GETDATE())

					set @output=1
				end
			else
				begin
					set @output=2
				end
		end
	else
		begin
			set @output=0
		end
end;
go

/*29*/
create procedure addStudent
@email varchar(64),
@classId int,
@output int output
as
begin
	declare @stdId int

	select @stdId=id
	from users 
	where email=@email

	if @stdId is not null
		begin
			declare @check int
			
			select @check=teacherId
			from classTeacher
			where teacherId=@stdId and classId=@classId

			if @check is null
				begin
					insert into classStudent
					values (@classId,@stdId,GETDATE())
					set @output=1
				end
			else
				begin
					set @output=2
				end
		end
	else
		begin
			set @output=0
		end
end;
go

/*30*/
create procedure addTeacher
@email varchar(64),
@classId int,
@output int output
as
begin
	declare @tId int

	select @tId=id
	from users 
	where email=@email

	if @tId is not null
		begin
			declare @check int
			
			select @check=studentId
			from classStudent
			where studentId=@tId and classId=@classId

			if @check is null
				begin
					insert into classTeacher
					values (@classId,@tId,GETDATE(),null)
					set @output=1
				end
			else
				begin
					set @output=2
				end
		end
	else
		begin
			set @output=0
		end
end;
go

/*31*/
create procedure addPost
@classId int,
@userId int,
@content varchar(510),
@Id int output
as
begin
	insert into Posts (classID,userID,Content,datePosted)
	values (@classId,@userId,@content,GETDATE())

	set @Id=SCOPE_IDENTITY()
end;
go

/*32*/
create procedure addPostFiles
@postID int,
@FilePath varchar(200),
@FileName varchar(50)
as
begin
	insert into PostFiles
	values (@postID,@FilePath,@FileName)
end;
go

/*33*/
create procedure addcomment
@postId int,
@userId int,
@content varchar(510)
as
begin
	insert into comments
	values (@postID,@userId,GETDATE(),@content)
end;
go

/*34*/
create procedure addLecture
@classId int,
@title varchar(50),
@note varchar(255),
@Id int output
as
begin
	insert into lectures (classId,title,note,dateUploaded)
	values (@classId,@title,@note,getdate())

	set @Id=SCOPE_IDENTITY()
end;
go

/*35*/
create procedure addLectureFiles
@LecId int,
@filePath varchar(200),
@fileName varchar(50)
as
begin
	insert into lectureFiles
	values (@LecId,@filePath,@fileName)
end;
go

/*36*/
create procedure addAssignment
@classID int,
@Titles varchar(50),
@Notes varchar(510),
@Deadline datetime,
@NoOfResub int,
@Id int output
as
begin
	insert into Assignments (classID,Titles,Notes,Deadline,NoOfResub,dateUploaded)
	values (@classID,@Titles,@Notes,@Deadline,@NoOfResub,getdate())

	set @Id=SCOPE_IDENTITY()
end;
go

/*37*/
create procedure addAssignmentFiles
@AID int,
@FilePath varchar(200),
@FileName varchar(50)
as
begin
	insert into AssignmentFiles
	values (@AID,@FilePath,@FileName)
end;
go

/*38*/
create procedure submitAssignment
@AID int,
@userId int,
@ID int output
as
begin
	insert into AssignmentSub (AID,userID,dateSubmitted)
	values (@AID,@userId,getdate())

	set @Id=SCOPE_IDENTITY()
end;
go

/*39*/
create procedure addAssignmentSubFiles
@subID int,
@FilePath varchar(200),
@FileName varchar(50)
as
begin
	insert into AssignmentSubFiles
	values (@subID,@FilePath,@FileName)
end;
go

/*40*/
create procedure removeStudent
@classId int,
@studentId int
as 
begin
	delete from classStudent
	where classId=@classId and studentId=@studentId
end;
go

/*41*/
create procedure removeTeacher
@classId int,
@teacherId int,
@output int output
as 
begin
	
	if 1<(	select count(teacherId)
			from classTeacher
			where classId=@classId)
		begin
			delete from classTeacher
			where classId=@classId and teacherId=@teacherId
			set @output=1
		end
	else if not exists(	select*
						from classStudent
						where classId=@classId)
		begin
			begin transaction
				delete from classTeacher
				where classId=@classId and teacherId=@teacherId

				delete from class
				where id=@classId
				set @output=1
			commit transaction
		end
	else
		begin
			set @output=0
		end
end;
go

/*42*/
create procedure removeClass
@classId int
as
begin
	delete from class
	where id=@classId
end;
go

/*43*/
create procedure deletePost
@pId int
as
begin
	delete from Posts
	where ID=@pId
end;
go

/*44*/
create procedure deleteComment
@pId int,
@userId int,
@date datetime
as
begin
	delete from comments
	where postId=@pId and userId=@userId and datePosted=@date 
end;
go

/*45*/
create procedure deleteLecture
@lectureId int,
@classId int,
@output int output
as
begin
	set @output=0
	if exists (	select*
				from lectures
				where ID=@lectureId and classID=@classId )
		begin
			delete from lectures
			where id=@lectureId
			set @output=1
		end
end;
go

/*46*/
create procedure deleteAssignment
@assignId int,
@classId int,
@output int output
as
begin
	set @output=0
	if exists (	select*
				from Assignments
				where ID=@assignId and classID=@classId )
		begin
			delete from Assignments
			where ID=@assignId
			set @output=1
		end
end;
go

/*47*/
create procedure updClass
@id int,
@name varchar(40),
@section char(1),
@subject varchar(30)
as
begin
	update class
	set [name]=@name,
		section=@section,
		[subject]=@subject
	where id=@id
end;
go

/*48*/
create procedure updPost
@id int,
@Content varchar(510)
as
begin
	update Posts
	set Content=@Content
	where ID=@id
end;
go

/*49*/
create procedure updLecture
@id int,
@note varchar(255),
@title varchar(50),
@classId int,
@output int output
as
begin
	set @output=0
	if exists (	select*
				from lectures
				where id=@id and classId=@classId)
		begin
			update lectures
			set note=@note, title=@title
			where id=@id
			
			set @output=1
		end
end;
go

/*50*/
create procedure updteacherOfficeHours
@cid int,
@tid int,
@ohours varchar(20)
as
begin
	update classTeacher
	set officeHours=@ohours
	where classId=@cid and teacherId=@tid
end;
go

/*51*/
create procedure updAssignment
@id int,
@notes varchar(510),
@deadline datetime,
@noofresub int,
@title varchar(50),
@classId int,
@output int output
as
begin
	if exists(	select*
				from Assignments
				where classID=@classId and ID=@id)
		begin
			update Assignments
			set Notes=@notes,
				Deadline=@deadline,
				NoOfResub=@noofresub,
				Titles=@title
			where id=@id
			set @output=1
		end
	else
		begin
			set @output=0
		end
end;
go

/*52*/
create procedure updPassword
@id int,
@password varchar(64),
@oldP varchar(64),
@output int output
as
begin
	set @output=0
	
	if exists (	select*
				from users
				where [password]=hashbytes('SHA2_256',@oldP) and id=@id)
		begin
			update users
			set [password]=hashbytes('SHA2_256',@password)
			where id=@id
			set @output=1
		end
end;
go

/*53*/
create procedure checkClassCode
@ccode varchar(6),
@output int output
as
begin
	if exists (	select id
				from class
				where code=@ccode)
		begin
			set @output=0
		end
	else
		begin
			set @output=1
		end
end;
go

/*54*/
create procedure getClassDetail
@cid int,
@name varchar(40) output,
@section char output,
@code varchar(6) output
as
begin
	select @name=[name], @section=section, @code=code
	from class
	where id=@cid
end;
go

/*55*/
create procedure exitClass
@cid int,
@userid int,
@myoutput int output
as
begin
	if exists(	select*
				from classTeacher
				where classId=@cid and teacherId=@userid)
		begin
			exec dbo.removeTeacher
			@teacherId=@userid,
			@classId=@cid,
			@output=@myoutput output
		end
	else
		begin
			exec removeStudent
			@classId=@cid,
			@studentId=@userid

			set @myoutput=1
		end
end;
go

/*56*/
create procedure checkTeacher
@cid int,
@userid int,
@output int output
as
begin
	if exists(	select*
				from classTeacher
				where teacherId=@userid and classId=@cid)
		begin
			set @output=1
		end
	else
		begin
			set @output=0
		end
end
go

/*57*/
create procedure checkStudent
@cid int,
@userid int,
@output int output
as 
begin
	if exists(	select* 
				from classStudent
				where studentId=@userid and classId=@cid)
		begin
			set @output=1
		end
	else
		begin
			set @output=0
		end
end
go

/*58*/
create procedure getUserSubmission
@aid int,
@userid int,
@Id int output,
@datesub datetime output,
@late int output
as
begin
	set @Id=null
	
	select top 1 @Id=Id, @datesub=dateSubmitted
	from AssignmentSub
	where AID=@aid and userID=@userid
	order by dateSubmitted desc
	
	if @Id is not null
		begin
			if ((select Deadline
				from Assignments
				where ID=@aid) < @datesub)
				begin
					set @late=1
				end
			else
				begin
					set @late=0
				end
		end
	else
		begin
			set @Id=-1
		end
end
go

/*59*/
create procedure getLecDetails
@Lid int,
@title varchar(50) output,
@note varchar(510) output,
@dateUploaded date output
as
begin
	select @title=title, @note=note, @dateUploaded=dateUploaded
	from lectures
	where id=@Lid
end
go

/*60*/
create procedure updPicture
@id int,
@picture varchar(250)
as
begin
	update users
	set picture=@picture
	where id=@id
end;
go
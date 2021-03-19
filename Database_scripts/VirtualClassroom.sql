use master;
create database virtualClassroom;
go

use virtualClassroom;

create table users(
id int identity not null,
fName varchar(30) not null,
lname varchar(30) not null,
dob date not null,
gender char not null,
email varchar(64) not null,
password binary(32) not null,
picture varchar(250)
);

create table class(
id integer identity not null,
code varchar(6) not null,
name varchar(40) not null,
section char not null,
subject varchar(30) not null,
dateCreated date not null
);

create table classStudent(
classId int not null,
studentId int not null,
dateEnrolled date not null
);

create table classTeacher(
classId int not null,
teacherId int not null,
dateJoined datetime not null,
officeHours varchar(20)
);

create table lectures(
id int identity not null,
classId int not null,
title varchar(50) not null,
note varchar(510),
dateUploaded date not null
);

create table lectureFiles(
id int not null,
filePath varchar(200) not null,
[FileName] varchar(50) not null
);

create table comments(
postId int not null,
userId int not null,
datePosted datetime not null,
content varchar(510) not null
);

create table Assignments(
ID int identity not null,
classID int not null,
Titles varchar(50) not null,
Notes varchar(510) not null,
Deadline datetime not null,
NoOfResub int not null,
dateUploaded date not null
);
  
create table AssignmentSub(
ID int identity not null,
AID int not null,
userID int not null,
dateSubmitted datetime not null
);
  
create table Posts(
ID int identity not null,
classID int not null,
userID int not null,
Content varchar(510) not null,
datePosted datetime not null
);
  
create table PostFiles(
postID int not null,
FilePath varchar(200) not null,
[FileName] varchar(50) not null
);
  
create table AssignmentSubFiles(
subID int not null,
FilePath varchar(200) not null,
[FileName] varchar(50) not null
);
  
create table AssignmentFiles(
AID int not null,
FilePath varchar(200) not null,
[FileName] varchar(50) not null
);
  
alter table users add constraint pk_users primary key(id);
alter table class add constraint pk_class primary key(id);
alter table classTeacher add constraint pk_classTeacher primary key(classId,TeacherId);
alter table classStudent add constraint pk_classStudent primary key(classId,studentId);
alter table lectures add constraint pk_lectures primary key(id);
alter table lectureFiles add constraint pk_lectureFiles primary key(filePath);
alter table comments add constraint pk_comments primary key(postId,userId,datePosted);  
Alter Table Assignments add constraint pk_Assigments primary key (ID);
Alter Table AssignmentSub add constraint pk_AssigmentSub primary key (ID);
Alter Table Posts add constraint pk_posts primary key (ID);
Alter Table AssignmentSubFiles add constraint pk_assignemntSubFiles primary key (FilePath);
Alter Table PostFiles add constraint pk_postFiles primary key (FilePath);
Alter Table AssignmentFiles add constraint pk_assignemntFiles primary key (FilePath);

alter table users add constraint uq_email unique(email);
alter table class add constraint uq_code unique(code);

alter table classStudent add constraint fk_classStudent_user foreign key(studentId) references users(id);
alter table classStudent add constraint fk_classStudent_class foreign key(classId) references class(id) on delete cascade on update cascade;
alter table classTeacher add constraint fk_classTeacher_user foreign key(teacherId) references users(id);
alter table classTeacher add constraint fk_classTeacher_class foreign key(classId) references class(id) on delete cascade on update cascade;
alter table lectures add constraint fk_lectures_class foreign key(classId) references class(id) on delete cascade on update cascade;
alter table lectureFiles add constraint fk_lectureFiles_lectures foreign key(id) references lectures(id) on delete cascade on update cascade;
alter table comments add constraint fk_comments_users foreign key(userId) references users(id);
alter table comments add constraint fk_comments_posts foreign key(postId) references posts(ID) on delete cascade on update cascade;
alter table Posts add constraint fk_Posts_class foreign key(classID) references class(id) on delete cascade on update cascade;
alter table Posts add constraint fk_Posts_user foreign key(userID) references users(id);
alter table PostFiles add constraint fk_PostFiles_Posts foreign key(postID) references Posts(ID) on delete cascade on update cascade;
alter table Assignments add constraint fk_Assignments_class foreign key(classID) references class(id) on delete cascade on update cascade;
alter table AssignmentFiles add constraint fk_AssignmentFiles_Assignments foreign key(AID) references Assignments(ID) on delete cascade on update cascade;
alter table AssignmentSub add constraint fk_AssignmentSub_users foreign key(userID) references users(id);
alter table AssignmentSub add constraint fk_AssignmnetSub_Assignments foreign key(AID) references Assignments(ID) on delete cascade on update cascade;
alter table AssignmentSubFiles add constraint fk_AssignmentSubFiles_AssignmentSub foreign key(subID) references AssignmentSub(ID) on delete cascade on update cascade;
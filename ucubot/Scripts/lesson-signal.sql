create table lesson_signal (
  Id int not null auto_increment, 
  DateTime Timestamp not null, 
  SignalType int NOT NULL , 
  UserId varchar(256) not null,
  primary key (id));
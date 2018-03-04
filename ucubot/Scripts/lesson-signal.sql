use ucubot;
create table lesson_signal(Id int auto_increment not null,
 timestamp datetime not null,
 SignalType int not null, 
 UserId varchar(100),
 primary key(id));
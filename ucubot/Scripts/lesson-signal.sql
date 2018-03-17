use ucubot;
create table lesson_signal(id int not null auto_increment,
 timestamp datetime not null,
 signal_type int not null, 
 user_id varchar(100),
 primary key(id));
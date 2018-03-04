use ucubot;
create table lesson_signal (
	id integer not null auto_increment,
	time_stamp datetime default current_timestamp,
	signal_type integer,
	used_id varchar(255) not null,
	primary key(id)
);

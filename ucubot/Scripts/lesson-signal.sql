USE ucubot;
CREATE TABLE lesson_signal(
id int NOT NULL AUTO_INCREMENT,
time_stamp DateTime NOT NULL,
signal_type int NOT NULL,
user_id varchar(255) NOT NULL,
PRIMARY KEY (id)
);
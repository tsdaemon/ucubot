USE ucubot;
CREATE TABLE lesson_signal(
id int NOT NULL,
time_stamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
sygnal_type int NOT NULL,
user_id  varchar(30) NOT NULL
);
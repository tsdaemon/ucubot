USE ucubot;
CREATE TABLE lesson_signal(id int autoincrement NOT NULL , timestamp DateTime, signal_type int, user_id VARCHAR(20), PRIMARY KEY (id));

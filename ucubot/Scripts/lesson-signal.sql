USE ucubot;
CREATE TABLE lesson_signal(id int auto_increment NOT NULL , timestamp DateTime, signal_type int, user_id VARCHAR(260), PRIMARY KEY (id));

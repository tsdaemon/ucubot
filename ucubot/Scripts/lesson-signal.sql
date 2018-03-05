USE ucubot;
CREATE TABLE lesson_signal (id int NOT NULL AUTO_INCREMENT, time_stamp DATETIME DEFAULT CURRENT_TIMESTAMP , signal_type int, user_id VARCHAR(128), PRIMARY KEY (id));
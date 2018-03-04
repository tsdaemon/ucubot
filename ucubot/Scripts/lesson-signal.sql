USE ucubot;
CREATE TABLE lesson_signal (id BIGINT NOT NULL AUTO_INCREMENT, PRIMARY KEY (id), timestamp DATETIME, signal_type INT, user_id VARCHAR(100));

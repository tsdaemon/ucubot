USE ucubot;
CREATE TABLE lesson_signal (id INT NOT NULL AUTO_INCREMENT, timestamp DATETIME, signal_type INT, user_id CHAR(100) NOT NULL, PRIMARY KEY(id));
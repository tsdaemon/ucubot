USE ucubot;
CREATE TABLE lesson_signal (
	id INT NOT NULL AUTO_INCREMENT,
	time_stamp TIMESTAMP,
	signal_type INT,
	user_id VARCHAR(255),
	PRIMARY KEY(id)
);

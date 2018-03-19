USE ucubot;

CREATE TABLE lesson_signal (
     id INT NOT NULL AUTO_INCREMENT,
     time_stamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
     signal_type INT,
     user_id VARCHAR(64),
     PRIMARY KEY (id)
);

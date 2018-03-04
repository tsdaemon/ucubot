use ucubot;
CREATE TABLE lesson_signal(
    id LONG NOT NULL AUTO_INCREMENT,
    time_stamp DATETIME,
    signal_type INT,
    user_id VARCHAR(10)
);
use ucubot;
CREATE TABLE lesson_signal (
    id int not null auto_increment primary key,
    time_stamp DATETIME DEFAULT CURRENT_TIMESTAMP,
    signal_type INT,
    user_id VARCHAR(100)
);
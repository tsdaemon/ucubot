use ucubot;
CREATE TABLE lesson_signal (
    id INT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    time_stamp DATETIME DEFAULT CURRENT_TIMESTAMP,
    signal_type INT,
    user_id VARCHAR(100) NOT NULL 
);

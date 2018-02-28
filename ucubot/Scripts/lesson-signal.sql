CREATE TABLE lesson_signal (
    Id INTEGER primary key NOT NULL AUTO_INCREMENT,
    timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
    signal_type INTEGER,
    user_id varchar(255) NOT NULL
);

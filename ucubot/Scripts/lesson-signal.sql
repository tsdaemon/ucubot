USE ucubot;

CREATE TABLE lesson_signal (
    id BIGINT NOT NULL AUTO_INCREMENT,
    Timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    SignalType INT,
    UserId TEXT,
    primary key (id)
);

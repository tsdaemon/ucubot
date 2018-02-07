USE ucubot;

CREATE TABLE lesson_signal (
    id BIGINT NOT NULL AUTO_INCREMENT,
    Timestamp DATETIME,
    SignalType INT,
    UserId TEXT,
    primary key (id)
);

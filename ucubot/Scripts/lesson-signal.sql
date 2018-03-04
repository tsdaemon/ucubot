USE ucubot;
CREATE TABLE lesson_signal (Id int, Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP, SignalType int, UserId VARCHAR(128));

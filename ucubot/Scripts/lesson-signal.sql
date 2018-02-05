CREATE TABLE LessonSignal (
    Id INTEGER primary key NOT NULL AUTO_INCREMENT,
    Timestamp DATETIME,
    SignalType INTEGER,
    UserId varchar(255)
);

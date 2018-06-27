USE ucubot;

ALTER TABLE lesson_signal DROP COLUMN UserId;
ALTER TABLE lesson_signal ADD COLUMN StudentId VARCHAR(128) NOT NULL;

ALTER TABLE lesson_signal
ADD CONSTRAINT FK_StudentId
    FOREIGN KEY (StudentId)
    REFERENCES student (UserId);
    
TRUNCATE TABLE lesson_signal;
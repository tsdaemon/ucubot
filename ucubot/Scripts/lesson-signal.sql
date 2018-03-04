USE ucubot;
CREATE TABLE lesson_signal (Id Int NOT NULL AUTO_INCREMENT,
                            Timestamp DateTime, 
                            SignalType Int, 
                            UserId VARCHAR(128) NOT NULL);
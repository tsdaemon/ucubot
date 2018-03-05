USE ucubot;

CREATE TABLE lesson_signal(
  Id INT NOT NULL AUTO_INCREMENT,
  Timestamp TIMESTAMP,
  SignalType INT,
  UserId TEXT,
  PRIMARY KEY (Id)
);
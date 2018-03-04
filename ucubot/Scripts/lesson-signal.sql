 USE ucubot;
 CREATE TABLE lesson_signal
  (Id INT UNSIGNED AUTO_INCREMENT,
  Timestamp DATETIME,
  SignalType INT,
  UserId VARCHAR(40),
  primary key(Id));
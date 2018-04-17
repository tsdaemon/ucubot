 USE ucubot;
 CREATE TABLE lesson_signal
  (Id INT UNSIGNED AUTO_INCREMENT,
  Timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  SignalType INT,
  UserId VARCHAR(40),
  primary key(Id));

USE ucubot;
CREATE TABLE lesson_signal(
                  id Int NOT NULL AUTO_INCREMENT,
                  time_stamp Timestamp,
                  signaltype Int,
                  userid VARCHAR(128) NOT NULL,
                  PRIMARY KEY (id));

USE ucubot;
CREATE TABLE student
  (id INT UNSIGNED AUTO_INCREMENT,
  first_name: VARCHAR(60),
  last_name VARCHAR(60),
  user_id VARCHAR(40),
  primary key(id),
  UNIQUE(user_id)
  );

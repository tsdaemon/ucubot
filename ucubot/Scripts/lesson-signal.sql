use ucubot;
Create table lesson_signal(Id INT not null auto_increment primary key, Timestemp DateTime DEFAULT CURRENT_TIMESTAMP, signal_type int, user_id varchar(10));

 CREATE DATABASE ucubot;
 CREATE USER 'anastasia'@'%' IDENTIFIED BY 'daps24';
 GRANT ALL PRIVILEGES ON ucubot.* TO 'anastasia'@'%';
 FLUSH PRIVILEGES;
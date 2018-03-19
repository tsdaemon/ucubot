CREATE DATABASE ucubot;
USE ucubot;
CREATE USER 'ucuuser'@'%' IDENTIFIED BY 'appsucu';
GRANT ALL PRIVILEGES ON ucubot . * TO 'ucuuser'@'%';
FLUSH PRIVILEGES;


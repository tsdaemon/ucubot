CREATE DATABASE ucubot;
USE ucubot;
CREATE USER 'Antonyshyn'@'%' IDENTIFIED BY 'password';
GRANT All PRIVILEGES ON ucubot . * TO 'Antonyshyn'@'%';  
Flush PRIVILEGES;

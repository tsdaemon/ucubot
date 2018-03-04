CREATE DATABASE ucubot;
USE ucubot;
CREATE USER 'user'@'%' IDENTIFIED BY 'password';
GRANT ALL PRIVILEGES ON ucubot.* To 'user'@'%';
FLUSH PRIVILEGES;
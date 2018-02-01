CREATE DATABASE ucubot;
USE ucubot;
CREATE USER 'user'@'%';
GRANT ALL PRIVILEGES ON ucubot.* To 'user'@'%' IDENTIFIED BY 'password';
FLUSH PRIVILEGES;
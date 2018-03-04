CREATE DATABASE ucubot;
CREATE USER 'temary'@'%' identified by '12345678';
GRANT ALL PRIVILEGES ON ucubot.* TO 'temary'@'%';
FLUSH PRIVILEGES;
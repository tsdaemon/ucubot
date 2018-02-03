CREATE DATABASE ucubot;

CREATE USER 'newuser'@'%' IDENTIFIED BY 'password';

GRANT ALL PRIVILEGES ON ucubot.* TO 'newuser'@'%';

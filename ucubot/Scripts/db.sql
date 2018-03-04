CREATE DATABASE ucubot;
CREATE USER 'ucubotuser'@'%' IDENTIFIED BY '1111';
GRANT ALL PRIVILEGES ON ucubot.* TO 'ucubotuser'@'%';

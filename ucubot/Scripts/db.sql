CREATE DATABASE ucubot;
CREATE USER 'newuser'@'%' IDENTIFIED BY 'newuser';
GRANT ALL PRIVILEGES ON ucubot.* TO 'newuser'@'%';
FLUSH PRIVILEGES;

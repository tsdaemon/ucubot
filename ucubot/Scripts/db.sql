CREATE DATABASE ucubot;
CREATE USER 'kosar'@'%' IDENTIFIED BY 'pass'; --create user
GRANT ALL PRIVILEGES ON ucubot.* TO 'kosar'@'%'; --give privileges
FLUSH PRIVILEGES;

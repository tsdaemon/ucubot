CREATE DATABASE ucubot;
CREATE USER 'ucubot_user'@'%' IDENTIFIED BY 'password123';
GRANT ALL PRIVILEGES ON ucubot.* TO 'ucubot_user'@'%';
FLUSH PRIVILEGES;
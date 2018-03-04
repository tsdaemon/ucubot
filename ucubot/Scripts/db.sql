CREATE DATABASE ucubot;
CREATE USER 'Macrosrider'@'%' IDENTIFIED BY 'password';
GRANT ALL PRIVILEGES ON ucubot.* TO 'Macrosrider'@'%';
FLUSH PRIVILEGES;
CREATE DATABASE ucubot;

CREATE USER 'ucubot_user'@'%' IDENTIFIED BY '1234';

GRANT ALL PRIVILEGES ON ucubot.* To 'ucubot_user'@'%';

FLUSH PRIVILEGES;

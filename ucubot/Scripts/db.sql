CREATE DATABASE ucubot;
CREATE USER 'y_stasinchuk'@'%' IDENTIFIED BY 'passwrodium';
GRANT ALL PRIVILEGES ON ucubot.* TO 'y_stasinchuk'@'%';
FLUSH PRIVILEGES 
CREATE DATABASE ucubot;
CREATE USER 'max_voloskiy'@'%' IDENTIFIED BY 'password';
GRANT ALL PRIVILEGES ON * . * TO 'max_voloskiy'@'%';
FLUSH PRIVILEGES;
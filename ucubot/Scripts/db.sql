CREATE DATABASE ucubot;
CREATE USER 'Bohdan'@'%' IDENTIFIED BY 'password';
GRANT ALL PRIVILEGES ON ucubot . * TO 'Bohdan'@'%';
FLUSH PRIVILEGES;
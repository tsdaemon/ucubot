CREATE DATABASE ucubot;
USE ucubot;
CREATE USER 'orenchuk'@'%' IDENTIFIED BY 'orenchuky';
GRANT ALL PRIVILEGES ON ucubot . * TO 'orenchuk'@'%';
FLUSH PRIVILEGES;
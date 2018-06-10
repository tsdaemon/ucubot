CREATE DATABASE ucubot;
CREATE USER 'martalozynska'@'%' IDENTIFIED BY 'bananaelephant';
GRANT ALL PRIVILEGES ON * . * TO 'martalozynska'@'%';
FLUSH PRIVILEGES;

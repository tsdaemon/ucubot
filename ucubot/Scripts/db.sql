create database ucubot;
CREATE USER 'newuser'@'%' IDENTIFIED BY 'pasw%orD1';
GRANT ALL PRIVILEGES ON * . * TO 'newuser'@'%';
flush privileges;

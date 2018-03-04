create database ucubot;
use ucubot;
create user 'ucu_user'@'%' identified by 'rootPW';
GRANT ALL PRIVILEGES ON ucubot.* TO 'ucu_user'@'%';
flush privileges;
create database ucubot;
create user 'svk'@'%' identified by '1234';
grant all privileges on ucubot . * to 'svk'@'%';
flush privileges;
CREATE DATABASE ucubot;
USE ucubot;
Create user 'Anatoliy'@'%' identified by 'password';
grant all privileges on ucubot . * to 'Anatoliy'@'%';	
Flush privileges;

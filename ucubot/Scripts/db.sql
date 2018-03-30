CREATE DATABASE ucubot;
USE ucubot;
Create user 'Anatoliy'@'localhost' identified by 'password';
grant all privileges on ucubot . * to 'Anatoliy'@'localhost';	
Flush privileges;

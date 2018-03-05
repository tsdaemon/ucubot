drop database ucubot;
CREATE DATABASE ucubot;
USE ucubot;
Drop user Anatoliy;
Create user Anatoliy identified by 'password';
grant all privileges on * . * to Anatoliy;	
Flush privileges;
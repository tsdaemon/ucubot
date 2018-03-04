CREATE USER 'testuser '@' %' identified by 'password';
grant all privileges on ucubot.* to 'testuser'@' %';
flush privileges;
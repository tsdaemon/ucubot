CREATE SCHEMA 'ucubot';
CREATE USER 'urswego'@'%' IDENTIFIED BY 'password_1234';
GRANT ALL PRIVILEGES ON ucubot.* To 'urswego'@'%';
FLUSH PRIVILEGES;
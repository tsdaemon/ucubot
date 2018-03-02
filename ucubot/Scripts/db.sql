CREATE DATABASE ucubot;
GRANT ALL PRIVILEGES ON ucubot.* TO 'kosar'@'localhost' IDENTIFIED BY 'bier1664'; --create user
UPDATE mysql.user SET Host='%' WHERE Host='localhost' AND User='kosar'; --set user host to %
CREATE DATABASE ucubot;
GRANT ALL PRIVILEGES ON ucubot.* TO 'me'@'localhost';
UPDATE mysql.user SET Host='%' WHERE Host='localhost' AND User='me';
FLUSH PRIVILEGES;
exit;
@ECHO off
ECHO Stopping RabbitMQ ...
CALL rabbitmqctl stop_app
ECHO Done

ECHO Resetting RabbitMQ ...
CALL rabbitmqctl reset
ECHO Done

ECHO Starting RabbitMQ ...
CALL rabbitmqctl start_app
ECHO Done

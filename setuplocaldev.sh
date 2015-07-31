#!/bin/bash
boot2docker delete
boot2docker init
boot2docker up

boot2docker ssh 'mkdir /var/queue; mkdir /var/logs/; mkdir /var/logs/rook; mkdir /var/data/; mkdir /var/data/rook; exit'

docker create --name=rook-queue-datastore -v=//var/queue:/var/lib/rabbitmq/mnesia debian:wheezy
docker run -d --hostname rook-queue --name rook-queue -p 8080:15672 -p 5672:5672 --volumes-from rook-queue-datastore rabbitmq:3-management
docker run --name rook-logs -v //var/log/rook:/var/log/rook debian:wheezy
docker run -d --hostname rook-sample-db -p 27017:27017 --name rook-sample-db -v //var/data/rook:/data/db  mongo --smallfiles
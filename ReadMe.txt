Summary:

This is a prototype/Sample micro-service implimentation in .NET 4.5.1
This prototype achives the following micro-service design principles

1). In built metrics and logging
	- It is intended that micro-services will output logs to text files that will then be parsed by Splunk
2). Loosly couple contracts
	- Both publishing and subscribing contracts use the C# 4 dynamic type object. This means that if the contract has new
	  properties added to it, the service will ignore then new vproperties and continue to work. It also mean that there
	  is no compile time checking, as these objects are not evaluated untill run-time.
3). Lightweight platform agnostic hosting. 
	- Using Topshelf and keeping .NET framework libraries to a minimum means this service can be hosted with 
	  Mono on Unix and Linux machines (as well as windows). This further means that lighweight virtualisation technologies such as
	  docker can be used to build new environments quickly. 
4). Data repository. 
	- MongoDB is just one type of datastore. By using the repository pattern we ensure we can easily swap the implimentation
	  for other data stores suck as Keep/Cassandra. 
5). Event driven Publisher/Subscriber (observer pattern) message bus. 
	- Using RabbitMQ as a service bus/message queue this service is capable of participating in an event driven system. 


------------------------------------------------------------------------------------
Dependancies:

This sample microservice has the following libraries, frameworks and dependancies

Application Dependancies:

.NET 4.5.1 - A minimal amount of the .NET framwork is required in order to maintain cross platform compatability (mono)
Topshelf - A light weight service hosting framework compatible with mono
RabbitMQ/EasyNetQ - RabbitMQ is a popular message bus, EasyNetQ is a .NET wrapper for the RabbitMQ API
NLog - A popular logging framework
MongoDB gen10 - A popular NoSQL document data store. Note the use of an old version of the c# driver. 
MongoDB.Repository - A robust generic repository pattern for MogoDB. No support yet for mongo c# driver 2.0
Ninject - A popular Dependancy Injection framework
System.Web.Extensions - .NET framwork extention containing needed JSON serialisation method


Enviroment Dependancies:

Docker
RabbitMQ
MongoDB

---------------------------------------------------------------------------------
Usage:

Windows Debug - Manual.

In order to run this service in your development environment (your PC), ensure
you have installed boot2docker. Once you have it installed you will need to get RabbitMQ and MongoDB
running in docker containers, as well as storage containers. 

forst create the following file path on your local machine 

C:\var\log\rook


This path will be used to store the logs when the service is running. When the service runs in a docker container environment, this path will resolve to the 
persistant logging storage container.

Now download and install boot2docker, then launch it from the start menu icon. Text initilise your boot2docker vm

$ boot2docker init

Run any export commands the output tells you to, then start the vm:

$ boot2docker up

It is important in most environments to start up the message bus first. But before we do that, 
we first need ensure we can connect to any containers we create. Get the ip address of your boot2docker VM

$ boot2docker ip

For this sample add the following enteries to your host file, replacing the IP with the IP address reported by the last command

192.168.59.103	rook-queue
192.168.59.103	rook-sample-db

Now run the following comands to set up the boot2docker-vm

$ boot2docker ssh
$ mkdir /var/queue
$ mkdir /var/logs/
$ mkdir /var/logs/rook
$ mkdir /var/data/
$ mkdir /var/data/rook

You are now ready to fire up the RabbitMQ container and its data store

$ docker create --name=rook-queue-datastore -v=//var/queue:/var/lib/rabbitmq/mnesia debian:wheezy

$ docker run -d --hostname rook-queue --name rook-queue -p 8080:15672 -p 5672:5672 --volumes-from rook-queue-datastore rabbitmq:3-management

This will start up RabbitMQ on your local docker host, and expose the managment page on http://rook-queue:8080
the message bus will be avaliable on rook-queue:5672

Next create your logging storage container

$ docker run --name rook-logs -v //var/log/rook:/var/log/rook debian:wheezy

And Fianly MongoDB

$ docker run -d --hostname rook-sample-db -p 27017:27017 --name rook-sample-db -v //var/data/rook:/data/db  mongo --smallfiles


Hitting debug in visual studio will then fire up the service in a console window, and connect to the database and queue you have
in docker containers. 

--------------------------------------------------------------------------------------------






To run this app in a docker container

$ docker run  --name rook-sample --link rook-sample-db:db --link rook-queue:queue  --volumes-from rook-logs

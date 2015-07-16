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
Useage:

In order to run this service in your development environment (your PC), ensure
you have installed boot2docker. Once you have it installed you will need to get RabbitMQ and MongoDB
running in docker containers. 

RabbitMQ command:

docker run -d --hostname my-rabbit --name rook-rabbit -p 8080:15672 -p 5672:5672 rabbitmq:3-management

This will start up RabbitMQ on your local docker host, and expose the managment page on http://<<DockerHostIP>>:8080
the message bus will be avaliable on <<dockerHostIP>>:5672

MongoDB command:

docker run --name rook-mongo -d mongo

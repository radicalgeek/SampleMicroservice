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
Ninject - A popular Dependancy Injection framework NOTE: not using NuGet, but rather the mono build from the projects TeamCity builds (in lib folder)
System.Web.Extensions - .NET framwork extention containing needed JSON serialisation method


Environment Dependancies:

Docker
RabbitMQ
MongoDB

Development dependancies:

boot2docker
visual studio 2013

---------------------------------------------------------------------------------
Local Dev machine setup:

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

You are now ready to fire up the RabbitMQ container and its data store. Ensure you exit the docker vm and 
return to the boot2docker shell

$ docker create --name=rook-queue-datastore -v=//var/queue:/var/lib/rabbitmq/mnesia debian:wheezy

$ docker run -d --hostname rook-queue --name rook-queue -p 8080:15672 -p 5672:5672 --volumes-from rook-queue-datastore rabbitmq:3-management

This will start up RabbitMQ on your local docker host, and expose the managment page on http://rook-queue:8080
the message bus will be avaliable on rook-queue:5672

Next create your logging storage container

$ docker run --name rook-logs -v //var/log/rook:/var/log/rook debian:wheezy

And Fianly MongoDB

$ docker run -d --hostname rook-sample-db -p 27017:27017 --name rook-sample-db -v //var/data/rook:/data/db  mongo --smallfiles

-------------------------------------------------------------------------------------------------------------------------------

Local Debug:


Hitting debug in visual studio will then fire up the service in a console window, and connect to the database and queue you have
in docker containers. Logs will be output to both the console window, and to the logging directory C:\var\log\rook on your machine. 

you will have a working copy of both the message queue and the database, connected just as they would in any other environment. 

You can populate the database with data as you need to for your development. 

You can also write intergration tests that require instances of these services to be avaliable, that are executed by calling public methods
from test projects 

Finaly, the unit testing projects are able to exercise the logic indipendently of any services

--------------------------------------------------------------------------------------------

Local Build:

It is important that you test the application in a docker container. Doing so runs the .net executable using mono. 

This gives you the opertunity to interact with the service just as it would be done in other environments. 
To excercise the service simply place messages on the bus that the service is listening for.  

First build the project in visual studio, then using the same docker shell window you used to start your environment, navigate 
to the build output directory of the service. 

Next build a docker image containing the service with the follow command

$ docker build -t sampleservice .

$ docker run -d --name rook-sample --link rook-sample-db:db --link rook-queue:queue --volumes-from rook-logs sampleservice

You can now intergration test this solution. Note that logs will be stored in /var/log/rook on the boot2docker-vm

-----------------------------------------------------------------------------------------------

Remote Build:


-----------------------------------------------------------------------------------------------

Usage:

To use this sample micro-service take the following steps. Lets say in this example we are creating a widgets service

1. Branch this code base giving the new branch the name widgetservice
2. Replace all occuronces of the words "Prototype" and "Sample" with the word Widget
3. Adjust the rules around whether to pick up a message in ____.cs
4. Adjust the proccessing of messages in ______.cs
5. Configure environment vars such as DB name, queue name etc.
6. Copy the builds in jenkins from this project
7. Deploy
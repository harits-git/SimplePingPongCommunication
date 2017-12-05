# SimplePingPongCommunication
A console app that simulates data communication (e.g. chatting) between two or more clients via Rabbit Message Queue (RMQ).

Requirement:
1. A running RMQ server using default connection parameters (changeable in the codes). 
2. This project needs internet connection to collect all necessary dependencies via NuGet package manager (automatically on build command).

To run the simulation:
1. Make sure the RMQ is running.
2. Build the project once.
3. Find the directory where the .exe file is generated. Usually in <project folder>\bin\Debug directory.
4. Run the .exe file twice or more to create some instances of client.
5. Make sure to provide unique names to the clients.
6. Data transmission is directed to a target, then the target client gets the message. 
7. If there are only two clients, the communication is 'ping-pong'-ed.
8. If more than two clients, they communicate as if repeater system.
  
You are welcome to fork the project to use remote modules: remote RMQ server and remote clients.

Credits to:
http://www.rabbitmq.com
Microsoft Visual Studio 2015

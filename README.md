=> **Mandatory Features**
* Allow registered users to log in and talk with other users in a chatroom. => **Done!**
* Allow users to post messages as commands into the chatroom with the following format "/stock=stock_code". => **Done!**
* Create a decoupled bot that will call an API using the stock_code as a parameter. => **Done!**
  * https://stooq.com/q/l/?s=aapl.us&f=sd2t2ohlcv&h&e=csv, here aapl.us is the stock_code
* The bot should parse the received CSV file and then it should send a message back into the chatroom using a message broker like RabbitMQ. => **Done!**
* The message will be a stock quote using the following format: “APPL.US quote is $93.42 per share”. => **Done!**
* The post owner will be the bot. => **Done!**
* Have the chat messages ordered by their timestamps and show only the last 50 messages. => **Not Done!** :(
* Unit test the functionality you prefer. => **Not Done!** :(

=> **Bonus (Optional)**
* Have more than one chatroom. => **Done!**
* Use .NET identity for users authentication => **Done!**
* Handle messages that are not understood or any exceptions raised within the bot. => **Done!**
* Build an installer. => **Done!** *(used docker compose)*

=> **Libraries and main Tools used on this project**
* **RabbitMQ.Client** = To handle decoupled processing using Message Broker pattern.
* **StooqApi** = To deal with Stooq api and responses
* **SignalR** = To create the chat solution and handle communication between users and different chat groups for conversation
* **Docker** and **Docker-Compose** = To test locally using a RabbitMq server without the need of install all prerequisits

=> **Steps to start the application**
* Make sure to be at the UI project folder, like the below:
	* src\client\Jobsity.CodeChallenge.Chat.UI
	* run the following command: "dotnet build", if everything is OK, go to next step
	* return back in the root folder, where the .sln file is placed
	* run the following command: "docker-compose up --build"
	* in other terminal run the following command to identify the port for the web application
		* docker ps -a
		* in my case, 51977, so open the following url: http://localhost:51977/
	
	* **Create your Identity user in the first time**
		* Click on Register
		* Fulfill the form with required fields (email and user within 6 characters lenght)
		* After click on register button, click on the confirmation for the email like the one: "Click here to confirm your account"
		* In the right upper corner, click on Login menu using the same credential just created in the previous step and perform the login
			* Reproduce the same steps for a second or N users you would like to test the application
	
	* **Using the application**
		* After the login you are redirected to the index page, if not, click on the Home uppper menu
		* Type in the field chatroom name a chat room to start talking with others and press the Join ChatRoom button
		* To retrieve stock information, just put in the Message box the following: "/stock=<stock_code>"
			* Use this value as an example: /stock=aapl.us
		* To talk with other person in the same room, just open other browser tabs on in private (anonnymous tabs) mode.
		* With a second user created in the previous steps, perform the login and join in the same chat room to start talking

	* **Have Fun!** :)


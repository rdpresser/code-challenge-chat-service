=> Mandatory Features
* Allow registered users to log in and talk with other users in a chatroom. => Done!
* Allow users to post messages as commands into the chatroom with the following format "/stock=stock_code". => Done!
* Create a decoupled bot that will call an API using the stock_code as a parameter. => Done!
  * https://stooq.com/q/l/?s=aapl.us&f=sd2t2ohlcv&h&e=csv, here aapl.us is the stock_code
* The bot should parse the received CSV file and then it should send a message back into the chatroom using a message broker like RabbitMQ. => Done!
* The message will be a stock quote using the following format: “APPL.US quote is $93.42 per share”. => Done!
* The post owner will be the bot. => Done!
* Have the chat messages ordered by their timestamps and show only the last 50 messages. => Not Done!
* Unit test the functionality you prefer. => Not Done!

=> Bonus (Optional)
* Have more than one chatroom. => Done!
* Use .NET identity for users authentication => Done!
* Handle messages that are not understood or any exceptions raised within the bot. => Done!
* Build an installer. => Done!

=> Libraries used on this project
* RabbitMQ.Client = To handle decoupled processing using Message Broker pattern.
* StooqApi = To deal with Stooq api and responses
* SignalR = To create the chat solution and handle communication between users and different chat groups for conversation

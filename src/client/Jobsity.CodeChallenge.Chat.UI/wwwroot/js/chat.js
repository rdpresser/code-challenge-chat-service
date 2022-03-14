"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

function setContent(message) {
    var messagesList = document.getElementById("messagesList");
    messagesList.innerHTML = "";

    var li = document.createElement("li");
    messagesList.appendChild(li);
    li.textContent = message;
}

function manageContent(message) {
    
    var messagesList = document.getElementById("messagesList");
    var li = document.createElement("li");
    
    messagesList.appendChild(li);
    li.textContent = message;
}

connection.on("ReceiveMessage", function (user, message) {
    manageContent(`${user} says ${message}`);
});

connection.on("ReceiveUserLeaveMessage", function (user, chatRoom) {
    manageContent(`${user} left ${chatRoom}`);
});

connection.on("ReceiveUserJoinMessage", function (user, chatRoom) {
    manageContent(`${user} joined ${chatRoom}`);
});

connection.start().then(function () {
    //document.getElementById("sendButton").disabled = false;
    document.getElementById("joinButton").disabled = false;
    document.getElementById("chatRoomInput").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

var joinButton = document.getElementById("joinButton");
var sendButton = document.getElementById("sendButton");
joinButton.addEventListener("click", function (event) {

    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    var chatRoom = document.getElementById("chatRoomInput");
    
    var request = {
        user: user,
        message: message,
        chatRoom: chatRoom.value
    };

    var isJoin = joinButton.value == "Join ChatRoom";
    if (isJoin) {

        if (chatRoom.value == "") {
            alert('Chat Room is required');
            chatRoom.focus();
            return;
        }

        joinButton.value = "Leave ChatRoom";
        sendButton.disabled = false;
        chatRoom.disabled = true;

        setContent(`${user}, welcome to Chat Room '${chatRoom.value}'`)

        connection.invoke("JoinGroup", request).catch(function (err) {
            return console.error(err.toString());
        });
    }
    else {
        joinButton.value = "Join ChatRoom";
        sendButton.disabled = true;
        chatRoom.disabled = false;
        chatRoom.focus();

        setContent(`${user}, sorry to see you leave Chat Room '${chatRoom.value}'`)
        chatRoom.value = "";

        connection.invoke("LeaveGroup", request).catch(function (err) {
            return console.error(err.toString());
        });
    }
   
    event.preventDefault();
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    var chatRoom = document.getElementById("chatRoomInput").value;
    
    var request = {
        user: user,
        message: message,
        chatRoom: chatRoom
    };

    connection.invoke("SendMessage", request).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});
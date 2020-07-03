"use strict";
let user = 'Deafault'
let destMessage = null
let destMessageName = null
let myuserLocal = null
var connection = new signalR.HubConnectionBuilder().withUrl("/Chat").build();
var sw=1
//Disable send button until connection is established
document.getElementById("sendButtonMessage").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = user + " says " + msg;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("ulmessages").appendChild(li);
});

connection.start().then(function () {   
    document.getElementById("sendButtonMessage").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());    

});

document.getElementById("sendButtonMessage").addEventListener("click", event => {
    let message = $("#messageInputChat").val()
    if (message.length > 0) {
        var fecha = new Date().toLocaleTimeString()
        
        connection.invoke("SendMessagePrivate", destMessage, destMessageName, message).catch(err => console.error(err.toString()))           
        $(".messages").append(`<div class="message msg_cli"><strong>yo</strong ><div> ${message}</div></div >`);
        $("#messageInputChat").val("")
    }
    event.preventDefault()
})

document.getElementById("messageInputChat").addEventListener("focus", function () {    
    connection.invoke("readmessage", myuserLocal, destMessageName).catch(err => console.error(err.toString()))

})

connection.on("readmessage", (destinatario,query) => {       
    $(".noty").each(function () {
        if ($(this).data("destinatarionombre") == destinatario) {
            let elem = $(this)            
            elem.html("")
        }
    })
})

// #007bff 
// text-primary
connection.on("ReceiveMessaggePrivate", (user, sender, message) => {
    
    document.getElementById("audio").play();
        const fecha = new Date().toLocaleTimeString()
        destMessage = sender;
        destMessageName = user;
        connection.invoke("MessageUpdate", destMessageName).catch(function (err) {
        });
    $("#textziel").html(destMessageName) 
    let id = document.getElementById("min_user")
    
    if (id.innerText == "Open") {
        var usuarionoty = document.getElementById('user_content');
        usuarionoty.style.background = "#FFBD4A"
        var farbe = document.getElementById("farbe")
        farbe.style.color="#000000"
        id.innerText = "Message"
        setTimeout(function () {
            usuarionoty.style.background = "#FFFFFF"
            id.innerText = "Open"
            farbe.style.color = "#007bff"
        }, 8000)
    }
    
})


connection.on("ReceiveMessage", function (user, message) {
    const fecha = new Date().toLocaleTimeString()
    $(".messages").append(`<div class="message msg_ser"><strong>${user}</strong ><div> ${message}</div></div >`);
})


connection.on("ClientConnected", function (data) {   
    console.log("conectado"+data)
    $("#list_clients").html("")
    $.each(data, (ind, elem) => {
        $("#list_clients").append(`<div class="user_chat_list" data-destinatario="${elem.UserID}" data-destinatarioNombre="${elem.userName}"><strong>${elem.UserName}</strong><small>Online </small><div class="noty" data-destinatarionombre="${elem.userName}"></div></br></div>`);
    })
    selectClientMessage()
})

connection.on("ClientUpdate", (data) => {
    //console.log(myuserLocal+" actualizado " + data)
    $("#list_clients").html("")

    $.each(data, (ind, elem) => {
        $("#list_clients").append(`<div class="user_chat_list" data-destinatario="${elem.userID}" data-destinatarioNombre="${elem.userName}"><strong>${elem.userName}</strong><small>Online </small><div class="noty" data-destinatarionombre="${elem.userName}"></div></br></div>`);
    })
    selectClientMessage()
})

connection.on("mensajesnoleidos", (data) => {
    $(".noty").each(function () {
        let elem=$(this)       
        let cont = 0;
        //console.log("# mensajes: " + data.length)
        for (let i = 0; i < data.length; i++) {                                
            if (elem.data("destinatarionombre") == data[i].quellID && data[i].zielID == myuserLocal) {            
                cont++;
            }
        }
        //console.log("mensajes " + myuserLocal +" : " + cont)
        if (cont > 0) {
            elem.html(cont)
        }        
    })
    var chatBox = document.getElementById('scrolldown');
    chatBox.scrollTop = chatBox.scrollHeight;
    
})


document.getElementById("min_message").addEventListener("click", function () {
    document.getElementById("message_content").style.display = "none"
    let id = document.getElementById("min_user")
    id.classList.remove("close_user")
    id.classList.remove("close_user3")
    id.classList.add("close_user2")  
})

document.getElementById("min_user").addEventListener("click", function () {
    if (sw == 0) {
        document.getElementById("list_clients").style.display = "none"
        document.getElementById("user_head").style.height = "auto"        
        let id = document.getElementById("min_user")
        id.innerText="Open"
        id.classList.remove("close_user")
        id.classList.remove("close_user3")
        id.classList.add("close_user2") 
        document.getElementById("message_content").style.display = "none"
        sw = 1;
    }
    else {
        document.getElementById("list_clients").style.display = "block"
        document.getElementById("user_head").style.height = "50%" 
        let id = document.getElementById("min_user")
        id.innerText="X"
        id.classList.remove("close_user2")
        id.classList.add("close_user3")
        sw = 0;
    }

})

connection.on("myUser", (data) => {    
    myuserLocal=data
    //console.log('myUser: ' + myuserLocal)
})


connection.on("MessageUpdate", (data) => {    
    $(".messages").empty()
    for (var i = data.length-1; i >= 0; i--){                  
        if (data[i].quellID == myuserLocal) {
            $(".messages").append(`<div class="message msg_cli"><strong>yo</strong><div> ${data[i].nachricht}</div></div >`)            
        }
        else {
            $(".messages").append(`<div class="message msg_cli"><strong>${data[i].quellID}</strong><div> ${data[i].nachricht}</div></div >`)        
        }
    }  
    var chatBox = document.getElementById('scrolldown');
    chatBox.scrollTop = chatBox.scrollHeight;
})

document.getElementById("sendButtonMessage").addEventListener("click", function (event) {
    let message = $("#messageInputChat").val()
    if (message.length > 1) {
        connection.invoke("SendMessage", message).catch(function (err) {
            $(".messages").append(`<div class="message msg_cli"><strong>${user}</strong ><div> ${message}</div></div >`)            
        });
    }
    $("#messageInputChat").val("")
    event.preventDefault()
    })


function selectClientMessage() {
    $(".user_chat_list").each(function (index) {
        $(this).on('click', () => {            
            destMessage = $(this).data('destinatario')
            destMessageName = $(this).data('destinatarionombre')
            $("#textziel").html(destMessageName)  
            $("#message_content").slideDown()
            connection.invoke("MessageUpdate", destMessageName).catch(function (err) {
            });
            let id = document.getElementById("min_user")
            id.classList.remove("close_user2")
            id.classList.remove("close_user3")
            id.classList.add("close_user")  
        })
    });
    var chatBox = document.getElementById('scrolldown');
    chatBox.scrollTop = chatBox.scrollHeight;
}
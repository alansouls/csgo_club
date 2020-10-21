"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/matchhub").build();

connection.on("UpdateLobby", function (matchId) {
    if (matchId == currentMatchId) {
        window.location.reload();
    }
});


document.getElementsByClassName("update-lobby-state").addEventListener("click", function (event) {
    connection.invoke("SendUpdateLobby", currentMatchId).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});
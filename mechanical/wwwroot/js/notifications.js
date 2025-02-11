const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .build();

connection.start().catch(err => console.error(err));

connection.on("ReceiveNotification", message => {
    const notificationList = document.querySelector("ul");
    const newNotification = document.createElement("li");
    newNotification.textContent = message;
    notificationList.prepend(newNotification);
    alert(message);
});
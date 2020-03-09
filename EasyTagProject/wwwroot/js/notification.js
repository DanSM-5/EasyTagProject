const deleteNotification = async (id) => {
    
    const result = await fetch('/api/notification/unset', {
        method: 'post',
        body: id,
        headers: {
            'Content-Type': 'application/json'
        }
    });
    return 'You are being redirected to a different page';
}

const setNotification = async (roomId, date) => {
    const result = await fetch(`/api/notification/set`, {
        method: 'post',
        body: JSON.stringify({ roomId: roomId, date: date }),
        headers: {
            'Content-Type': 'application/json'
        }
    });
    return (await result.json()).id;
}

const keepNotification = async (id) => {
    const result = await fetch('/api/notification/keep', {
        method: 'post',
        body: id,
        headers: {
            'Content-Type': 'application/json'
        }
    });
};

const notificationExist = async (roomId, date) => {
    const result = await fetch(`/api/notification/${roomId}/${date}`);
    return result.json();
};

const checkAgain = async (func, clearNotification) => {
    const resultObject = await func();
    if (!resultObject.existNotification) {
        $('#notification').fadeOut();
        clearInterval(clearNotification);
        loadNofication();
    }
};

const loadNofication = async() =>{
    const roomId = parseInt($('#RoomId')[0].value);
    const date = getDateString(new Date($('#End')[0].value));

    const verifyNotification = async () => await notificationExist(roomId, date);
    const resultObject = await verifyNotification();

    if (resultObject.existNotification) {
        $('#notification')[0].textContent = 'Someone else is editing this day, please wait or your information may be lost';
        $('#notification').fadeIn("slow");
        const clearNotification = setInterval(async () => await checkAgain(verifyNotification, clearNotification), 2 * 60 * 1000);
    } else {
        const responseId = await setNotification(roomId, date);
        setInterval(async() => await keepNotification(responseId), 5 * 60 * 1000);
        window.onbeforeunload = () => deleteNotification(responseId);
    }
}

const getDateString = date => {
    const month = (date.getMonth() + 1).toString();
    const day = date.getDate().toString();
    const year = date.getFullYear().toString();
    return `${year}-${month.length == 1 ? "0" + month : month}-${day.length == 1 ? "0" + day : day}`;
};

window.addEventListener("load", loadNofication, false);
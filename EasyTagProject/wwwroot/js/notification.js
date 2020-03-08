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

const loadNofication = async() =>{
    const roomId = parseInt($('#RoomId')[0].value);
    const date = getDateString(new Date($('#End')[0].value));

    const result = await fetch(`/api/notification/${roomId}/${date}`);
    const resultObject = await result.json();

    if (resultObject.existNotification) {
        $('#notification')[0].textContent = 'Someone else is editing this day, please wait or your information may be lost';
        $('#notification').fadeIn("slow");
    } else {
        const responseId = await setNotification(roomId, date);
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
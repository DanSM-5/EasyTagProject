let deleteMessageConfirmation = (e) => {
    e.preventDefault();
    if (confirm('Are you sure you want to delete this appointment? This action cannot be undone')) {
        $(e.target).parent().submit();
    }
}

(() => {
    const buttons = document.getElementsByClassName('delete');
    Array.prototype.forEach.call(buttons, b => {
        b.addEventListener('click', deleteMessageConfirmation, false);
    });
})();
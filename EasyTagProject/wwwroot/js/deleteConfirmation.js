let deleteMessageConfirmation = (e) => {
    e.preventDefault();
    if (confirm('Are you sure you want to delete this appointment? This action cannot be undone')) {
        let form = $(e.target).parent();
        form.submit();
    }
}

(() => {
    let buttons = document.getElementsByClassName('delete');
    Array.prototype.forEach.call(buttons, b => {
        console.log(b.textContent);
        b.addEventListener('click', deleteMessageConfirmation, false);
    });
})();
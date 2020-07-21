
// Function to fade in and out the description
const fadeDescription = () => {
    const displayed = 'displayed';
    const appointments = Array.from(document.getElementsByClassName('appointment'));
    appointments.map(a => a.addEventListener('click', () => {
        const description = a.nextElementSibling;
        if (a.classList.contains(displayed)) {
            description.style.display = "none";
        } else {
            description.style.display = "table-row";
        }
        a.classList.toggle(displayed);
    }, false));
}

// add events after the page is loaded
window.addEventListener('load', fadeDescription, false);

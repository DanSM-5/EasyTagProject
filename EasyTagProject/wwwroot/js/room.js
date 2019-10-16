
// Function to fade in and out the description
let fadeAppointmentFunction = (e) => {
    if ($(e.target).is('td')) {
        let displayClass = 'displayed';
        let idNum = $(e.target).parent().attr('id').split('-')[0];
        let tr = $('#' + idNum + '-header');
        let hiddenId = '#' + idNum + "-description";

        if ($(tr).hasClass(displayClass)) {
            $(tr).removeClass(displayClass);
            $(hiddenId).fadeOut();
        } else {
            $(tr).addClass(displayClass);
            $(hiddenId).fadeIn();
        }
    }
};

// load event listeners
let addEventsListeners = function () {
    $(document).click(fadeAppointmentFunction);
};

// add events after the page is loaded
$(document).ready(addEventsListeners);


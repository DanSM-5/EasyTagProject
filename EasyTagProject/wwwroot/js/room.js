
// Function to fade in and out the description
let fadeAppointmentFunction = (e) => {
    if ($(e.target).is('td')) {
        let displayClass = 'displayed';
        let idNum = $(e.target)
            .parent()
            .attr('id')
            .split('-')[0];

        let tr = $('#' + idNum + '-header');
        let hiddenTd = '#' + idNum + "-description";

        if ($(tr).hasClass(displayClass)) {
            $(tr).removeClass(displayClass);
            $(hiddenTd).fadeOut();
        } else {
            $(tr).addClass(displayClass);
            $(hiddenTd).fadeIn();
        }
    }
};

// add events after the page is loaded
$(document).click(fadeAppointmentFunction);


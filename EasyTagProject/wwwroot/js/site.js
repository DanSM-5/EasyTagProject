$(document).ready(addEventListener(
    'click',
    (e) => {
        console.log('run')
        if ($(e.target).hasClass('d-table-cell')) {
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
    }
));


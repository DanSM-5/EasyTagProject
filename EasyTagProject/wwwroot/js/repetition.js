
let selectPeriod = () => {
    let periodSelected = $('#PeriodType')[0];
    let question = $('#question');
    let period = $('#period')[0];
    let includeWeekends = $('#IncludeWeekends')[0];
    let repeatNumber = $('#RepeatNumber')[0];
    let target = $('#TargetDay')[0];
    let once = $('#once');
    let another = $('#another');

    if (periodSelected.value === 'Daily') {
        question.fadeIn();
        period.textContent = 'days';
        includeWeekends.checked = true;
        once.fadeOut();
        another.fadeIn();
        target.value = Date.now;
    } else if (periodSelected.value === 'Once') {
        question.fadeOut();
        another.fadeOut();
        once.fadeIn();
        repeatNumber.value = 1;
        includeWeekends.value = true;
    } else if (periodSelected.value === 'Weekly') {
        question.fadeOut();
        period.textContent = 'weeks';
        includeWeekends.checked = true;
        once.fadeOut();
        another.fadeIn();
        target.value = Date.now;
    } else if (periodSelected.value === 'Monthly') {
        question.fadeOut();
        period.textContent = 'months';
        includeWeekends.checked = true;
        once.fadeOut();
        another.fadeIn();
        target.value = Date.now;
    } else if (periodSelected.value === 'Unselect') {
        once.hide();
        another.fadeIn();
    }
}

$('#PeriodType').change(selectPeriod);

(selectPeriod)();
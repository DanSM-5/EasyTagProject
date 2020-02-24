
let selectPeriod = () => {
    let periodSelected = $('#PeriodType')[0];
    let question = $('#question');
    let period = $('#period')[0];
    let includeWeekends = $('#IncludeWeekends')[0];

    if (periodSelected.value === 'Daily') {
        question.fadeIn();
        period.textContent = 'days';
        includeWeekends.checked = true;
    } else if (periodSelected.value === 'Weekly') {
        question.fadeOut();
        period.textContent = 'weeks';
        includeWeekends.checked = true;
    } else if (periodSelected.value === 'Monthly') {
        question.fadeOut();
        period.textContent = 'months';
        includeWeekends.checked = true;
    }
}

$('#PeriodType').change(selectPeriod);
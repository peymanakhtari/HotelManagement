new Vue({
    el: '#datepicker_range_reportReserve',
    components: {
        datePicker
    },
    methods: {
        select(date) {

        },
    }
});
new Vue({
    el: '#datepicker_range_submitDate',
    components: {
        datePicker
    },
    methods: {
        select(date) {

        },
    }
});

$(document).ready(function () {
    $('.pdp-input').eq(0).attr('name', 'dateRange');
    $('.pdp-input').eq(1).attr('name', 'SubmitDate');
    $('.pdp-input').eq(1).val($('#reserveSubmitDate').val());

    $('#filterTable').on('keyup', function () {
        var value = $(this).val();
        $('tbody tr').filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        })
    })

});

function ExportToExcel(type, fn, dl) {
    var elt = document.getElementById('tableReportReserve');
    var wb = XLSX.utils.table_to_book(elt, { sheet: "sheet1" });
    return dl ?
        XLSX.write(wb, { bookType: type, bookSST: true, type: 'base64' }) :
        XLSX.writeFile(wb, fn || ('MySheetName.' + (type || 'xlsx')));
}

$(document).on('keydown', function (event) {
    if (event.which === 13) {
        $('.btnSearch').click();
    }
});
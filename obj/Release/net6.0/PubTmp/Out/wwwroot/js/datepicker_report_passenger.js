new Vue({
    el: '#datepicker_range_reportPassenger',
    components: {
        datePicker
    },
    methods: {
        select(date) {

        },
    }
});

$(document).ready(function () {
    $('.pdp-input').attr('name', 'dateRange');

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
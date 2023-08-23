
new Vue({
    el: '#datepicker_range',
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

    var previousDate = null;

    $('tbody tr').each(function () {
        var currentDate = $(this).find('td.Date').text();

        if (currentDate !== previousDate) {
            $(this).addClass('seprator');
            previousDate = currentDate;
        }
    });

    $('tbody tr:nth-child(1)').removeClass('seprator');

    if ($('#roomNumber').val() == 0) {
        $('#RoomId').val(0);
    }
    if ($('#_websiteId').val() == 0) {
        $('#websiteId').val(0);
    }
    if ($('#_bedCount').val() == 0) {
        $('#bedCount').val(0);
    }
    if ($('#_submitter').val() == 0) {
        $('#submitter').val(0);
    }

    $('input[type=radio][name=filter]')[0].checked = true;

    $('.deleteReserve').click(function () {
        var id = $(this).attr('id').split('-')[1];
        if (confirm(' این رزرو حذف شود؟')) {
            $.ajax({
                url: '/Reserve/DeleteReserve',
                type: 'post',
                data: { id: id },
                success: function (res) {
                    if (res) {
                        location.reload();
                    }
                },
                error: function () {
                    alert('server error')
                }
            })
        }
    })

    $('#folioBedehkar').val($('#folioBedehkar').is(':checked'));
    $('#folioBedehkar').change(function () {
        $(this).val($(this).is(':checked'))
    })
    $('#hafDay').val($('#hafDay').is(':checked'));
    $('#hafDay').change(function () {
        $(this).val($(this).is(':checked'))
    })

    var filter = $('#reserveFiler').val();
    $(`input[name="filter"][value="${filter}"]`).prop('checked', true);

    $('#btnShowFilterDiv').click(function () {
        $('#filterDiv').toggleClass('d-none')
    })

})

$(document).on('keydown', function (event) {
    if (event.which === 13) {
        location.reload();
    }
});

function ExportToExcel(type, fn, dl) {
    var elt = document.getElementById('tableReserve');
    var wb = XLSX.utils.table_to_book(elt, { sheet: "sheet1" });
    return dl ?
        XLSX.write(wb, { bookType: type, bookSST: true, type: 'base64' }) :
        XLSX.writeFile(wb, fn || ('MySheetName.' + (type || 'xlsx')));
}




new Vue({
    el: '#datepicker_range_folio',
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
    $('.btnDeleteReserves').click(function () {
        var id = $(this).attr('id');
        if (confirm('میخواهید تمامی رزرو های مربوط به این فولیو پاک گردد؟')) {
            $.ajax({
                type: 'post',
                url: '/Reserve/DeleteAllReservesByFolioId',
                data: { folioId: id },
                success: function () {
                    location.reload();
                },
                error: function () {
                    alert('server error')
                }
            })
        }
    })

    $('.btnDeleteFolio').click(function () {
        var id = $(this).attr('id').split('-')[1];
        if (confirm(`فولیو ${id} پاک شود؟`)) {
            $.ajax({
                type: 'post',
                url: '/Folio/DeleteFolio',
                data: { folioId: id },
                success: function (res) {
                    if (res) {
                        location.reload();
                    }
                    else {
                        alert('از این فولیو رزرو هایی ثبت شده است. ابتدا باید تمامی رزرو ها را حذف کنید');
                    }
                },
                error: function () {
                    alert('server error')
                }
            })
        }

    })

    $('.changeRoom').click(function () {
        var id = $(this).attr('id').split('-')[1];
        var returnUrl = $('#returnurl').val();
        console.log(returnUrl);
        $.ajax({
            type: 'post',
            url: '/Reserve/CheckIfReservesHaveSameRoom',
            data: { id: id },
            success: function (res) {
                if (res) {
                    location.href = `/Folio/ChangeRoom/${id}?returnUrl=${returnUrl}`;
                }
                else {
                    alert('باید تمام رزرو های ثبت شده اتاق مشابه داشته باشند');
                }
            },
            error: function () {
                alert('server error')
            }
        })

    })


    $('#folioBedehkar').val($('#folioBedehkar').is(':checked'));
    $('#folioBedehkar').change(function () {
        $(this).val($(this).is(':checked'))
    })

    $('#noReserve').val($('#noReserve').is(':checked'));
    $('#noReserve').change(function () {
        $(this).val($(this).is(':checked'))
    })

    $('#preReserve').val($('#preReserve').is(':checked'));
    $('#preReserve').change(function () {
        $(this).val($(this).is(':checked'))
    })

    $('#filterFolio').on('keyup', function () {
        var value = $(this).val();
        $('tbody tr').filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        })
    })

})

$(document).on('keydown', function (event) {
    if (event.which === 13) {
        $('.btnSearch').click();
    }
});

function ExportToExcel(type, fn, dl) {
    var elt = document.getElementById('tableFolio');
    var wb = XLSX.utils.table_to_book(elt, { sheet: "sheet1" });
    return dl ?
        XLSX.write(wb, { bookType: type, bookSST: true, type: 'base64' }) :
        XLSX.writeFile(wb, fn || ('MySheetName.' + (type || 'xlsx')));
}
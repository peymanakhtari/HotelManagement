new Vue({
    el: '#datepicker_range_reserveTable',
    components: {
        datePicker
    },
    methods: {
        select(date) {

        },
    }
});

window.addEventListener("load", async (event) => {

    function isMobile() {
        const regex = /Mobi|Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i;

        return regex.test(navigator.userAgent);
    }
    if (isMobile()) {
        var val = localStorage.getItem('useDesktopAnounsed');
        if (val === null) {
            $('#exampleModalCenter').modal('show');
            localStorage.setItem('useDesktopAnounsed', true)
        }

    }
});




$(document).ready(function () {
    $('.pdp-input').attr('name', 'dateRange');


    var url = window.location.href;
    var returnUrlReserveTable = url.substring(url.indexOf('/reserve/ReservesTable'));
    $('td a').each(function () {
        if (!$(this).hasClass('showText')) {
            var href = $(this).attr('href');
            $(this).attr('href', href + `&returnUrl=${returnUrlReserveTable}`);
        }
    });


    $('a.preventDefault').on('click', function (event) {
        event.preventDefault();
    });
    $('.alertCheck').click(function () {
        $(this).toggleClass('isChecked');
        $(this).toggleClass('notchecked');

        var roomNumber = $(this).data('number');
        var date = $(this).data('date');
        $.ajax({
            type: 'post',
            url: '/Reserve/alertCheck',
            data: {
                roomNumber: roomNumber,
                date: date
            }, success: function () {

            },
            error: function () {
                alert('server error')
            }
        })

    })

    $('#showFolioCode').change(function () {
        $('.past').toggleClass('showText')
    })

    $('#btnCloseAlert').click(function () {
        $('#exampleModalCenter').modal('hide');
    })

})

window.addEventListener('DOMContentLoaded', () => {
    const table = document.getElementById('myTable');
    const cells = table.getElementsByTagName('td');

    Array.from(cells).forEach(cell => {
        const anchor = cell.querySelector('a');

        anchor.addEventListener('mouseenter', () => {
            const columnIndex = cell.cellIndex;
            const rowIndex = cell.parentNode.rowIndex;

            const columns = table.getElementsByTagName('td');
            const rows = table.getElementsByTagName('tr');

            Array.from(columns).forEach(columnCell => {
                if (columnCell.cellIndex === columnIndex) {
                    columnCell.classList.add('hover-column');
                }
            });

            Array.from(rows).forEach(row => {
                if (row.rowIndex === rowIndex) {
                    Array.from(row.cells).forEach(rowCell => {
                        rowCell.classList.add('hover-row');
                    });
                }
            });

            cell.classList.add('hover-cell');
        });

        anchor.addEventListener('mouseleave', () => {
            const columnIndex = cell.cellIndex;
            const rowIndex = cell.parentNode.rowIndex;

            const columns = table.getElementsByTagName('td');
            const rows = table.getElementsByTagName('tr');

            Array.from(columns).forEach(columnCell => {
                if (columnCell.cellIndex === columnIndex) {
                    columnCell.classList.remove('hover-column');
                }
            });

            Array.from(rows).forEach(row => {
                if (row.rowIndex === rowIndex) {
                    Array.from(row.cells).forEach(rowCell => {
                        rowCell.classList.remove('hover-row');
                    });
                }
            });

            cell.classList.remove('hover-cell');
        });
    });
    $('#calculateIncome').click(function () {
        $(this).prop('disabled', true)
        var dateRange = $('.pdp-input').val();
        $.ajax({
            url: '/Reserve/CalculateIncomeInDateRange',
            type: 'post',
            data: { dateRange: dateRange },
            success: function (res) {
                alert(res);
                $('#calculateIncome').prop('disabled', false)
            },
            error: function () {
                alert('server error')
            }
        })
    })
});

$(document).on('keydown', function (event) {
    if (event.which === 13) {
        location.reload();
    }
});


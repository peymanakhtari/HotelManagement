new Vue({
    el: '#datepicker',
    components: {
        datePicker
    },
    methods: {
        select(date) {
            $('#reserveDate').val(date.toString());
        },
    }
});

$(document).ready(function () {
    console.log($('#roomIdValue').val());
    if ($('#roomIdValue').val() == 0) {
        $('#RoomId').val(0);
    }
})

$(document).on('keydown', function (event) {
    if (event.which === 13) {
        $('#btnSearchFolioByCode').click();
    }
});
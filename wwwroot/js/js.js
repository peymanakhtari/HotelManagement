
$(document).ready(function () {

    function adjustElements() {

        var menuWidth;
        var tableWidth = $('.tableData').width() + 32;
        $('.tableData').parent().css('padding', '0 16px 0 16px')
        if ($(window).width() < 991) {
            menuWidth = tableWidth;
            $('nav').attr('dir', 'ltr')
        }
        else {
            menuWidth = '100%';
            $('nav').attr('dir', 'rtl');
            var style = getComputedStyle(document.body)
            tableWidth = style.getPropertyValue('--mainMargin')
        }
        $('.menu').css('width', menuWidth);
        $('h3').css('width', menuWidth);
        $('#filterBox').css('width', tableWidth);
        $('#filterContainer').css('width', tableWidth);
        $('.emptyRoomAlert').css('width', tableWidth);
    }

    $(window).on('resize', adjustElements);
    adjustElements();


    $("input[required], select[required]").attr("oninvalid", "this.setCustomValidity('پر کردن این فیلد الزامیست!')");
    $("input[required], select[required]").attr("oninput", "setCustomValidity('')");

    $('input').attr('autocomplete', 'off')
    $('#folioCode').attr('dir', 'ltr')

    if ($('#reservePrice').val() != undefined) {
        $('#reservePrice').val($('#reservePrice').val().replace(/,/g, ''));
        $('#reservePrice').digits();
    }


    resetValidateReserve();
    resetValidateHafReserve();

    $('.pdp-input').eq(0).val($('#reserveDate').val());

    $('#filterReserve').on('keyup', function () {
        var value = $(this).val();
        $('tbody tr').filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        })
    })

    $('.Bedehkar').change(function () {
        if ($(this).is(":checked")) {
            $(this).val(true)
        }
        else {
            $(this).val(false)
        }
    })

    $('.Bedehkar').val($('.Bedehkar').is(":checked"))




    //========================================add or edit folio==================================
    var preReserve = $('#preReserve').attr('checked');

    if (preReserve === "checked") {
        $('#frmNew').css('display', 'none')
        $('#frmPre').css('display', 'block')
    }

    $('#preReserve').change(function () {
        if (this.checked) {
            $('#frmNew').css('display', 'none')
            $('#frmPre').css('display', 'block')
            return
        }
        $('#frmNew').css('display', 'block')
        $('#frmPre').css('display', 'none')
    });



    $('#addPassenger').click(function () {
        var val = $('#PassengerContainer').children().last().attr('id');
        if (val === undefined) {
            showTilePassengers(true)
            $('#PassengerContainer').append(`
            <div id="passenger-1">
                    <div class="inputDiv">
                        <label for="" class="form-lable d-block">1- نام </label>
                        <input required name="[0].Name" type="text" class="form-control d-inline-block">
                    </div>
                    <div class="inputDiv">
                        <label for="" class="form-lable d-block">نام خانوادگی </label>
                        <input required name="[0].Family" type="text" class="form-control d-inline-block">
                    </div>
                    <div class="inputDiv">
                        <label for="" class="form-lable d-block">نام پدر </label>
                        <input required name="[0].FatherName" type="text" class="form-control d-inline-block">
                    </div>
                    <div class="inputDiv">
                        <label for="" class="form-lable d-block"> کد ملی </label>
                        <input dir="ltr" min="999999999" max="10000000000" required name="[0].NationalCode" type="number" class="form-control d-inline-block">
                    </div>
                    <span id="delete-1" class="deletePassenger"><i class="bi bi-trash"></i></span>
                </div>
           `)
        }
        else {
            var IdOfLastChild = val.split('-')[1];
            var IdOfnewChild = Number(IdOfLastChild) + 1;
            var indexOfEntity = IdOfnewChild - 1;
            $('#PassengerContainer').append(`
                <div id="passenger-${IdOfnewChild}">
                    <div class="inputDiv">
                        <label for="" class="form-lable d-block">${Number(IdOfLastChild) + 1}- نام </label>
                        <input required name="[${indexOfEntity}].Name" type="text" class="form-control d-inline-block">
                    </div>
                    <div class="inputDiv">
                        <label for="" class="form-lable d-block">نام خانوادگی </label>
                        <input required name="[${indexOfEntity}].Family" type="text" class="form-control d-inline-block">
                    </div>
                    <div class="inputDiv">
                        <label for="" class="form-lable d-block">نام پدر </label>
                        <input required name="[${indexOfEntity}].FatherName" type="text" class="form-control d-inline-block">
                    </div>
                    <div class="inputDiv">
                        <label for="" class="form-lable d-block"> کد ملی </label>
                        <input dir="ltr" min="999999999" max="10000000000" required name="[${indexOfEntity}].NationalCode" type="number" class="form-control d-inline-block">
                    </div>
                    <span id="delete-${IdOfnewChild}" class="deletePassenger"><i class="bi bi-trash"></i></span>
                </div>  
           `)

        }
        $('input').attr('autocomplete', 'off')
    })

    $('#PassengerContainer').on('click', '.deletePassenger', function () {
        if ($('#PassengerContainer >div').length == 1) {
            showTilePassengers(false);
        }
        $('#passenger-' + $(this).attr('id').split('-')[1]).remove();

        $('#PassengerContainer > div').each(function (index) {
            $(this).find('input').each(function () {
                var prop = $(this).attr('name').split('.')[1];
                var newName = '[' + index + '].' + prop;
                $(this).attr('name', newName);
            })
        });
    });

    if ($('#PassengerContainer > div').length > 0) {
        $('#titleHasPassengers').css('display', 'block')
    }
    else {
        $('#titleNoPassengers').css('display', 'block')
    }
    function showTilePassengers(val) {
        if (val) {
            $('#titleHasPassengers').css('display', 'block')
            $('#titleNoPassengers').css('display', 'none')
        }
        else {
            $('#titleHasPassengers').css('display', 'none')
            $('#titleNoPassengers').css('display', 'block')
        }
    }

    $('#frmNew').submit(function (e) {
        $('.btnSubmitFolio').prop('disabled', true);
    });

    $('#frmPre').submit(function (e) {
        $('.btnSubmitFolio').prop('disabled', true);
    });

    //=====================================Reservation=======================================
    $('#selectFolio').change(function () {
        var name = $(this).find('option:selected').html();
        var code = $(this).val();
        $('#PassengerName').val(code != 0 ? name : '');
        $('#folioCode').val(code != 0 ? code : '');
    })

    $('#btnSearchFolioByCode').click(function () {

        replaceBtnWithSpinner($('#btnSearchFolioByCode'))
        var data = $('#folioCode').val();
        $.ajax({
            type: 'POST',
            url: '/Folio/GetFolioInReserve',
            data: { id: data },
            success: function (res) {
                var folio = JSON.parse(res)
                if (res !== 'null') {
                    var name = '';
                    if (folio.Name != null) {
                        name = folio.Name
                    }
                    $('#PassengerName').val(name + ' ' + folio.Family)
                }
                else {
                    alert('فولیو پیدا نشد')
                }
                ActiveBtnAgain($('#btnSearchFolioByCode'), 'جستجو')

            },
            error: function (jqXHR, textStatus, errorThrown) {
                ActiveBtnAgain($('#btnSearchFolioByCode'), 'جستجو')
                alert('خطای سرور')
            }

        })
        $('#selectFolio').val(0);
    })

    $('#btnSubmitReserve').click(function () {
        if (validateReserve()) {
            replaceBtnWithSpinner($('#btnSubmitReserve'))

            var formData = $('#frmReserve').serialize();
            $.ajax({
                url: '/Reserve/AddOrEdit',
                type: 'POST',
                data: formData,
                success: function (response) {
                    const result = response.result;
                    const message = response.message;
                    if (result) {
                        location.href = '/Home/Index?folioId=' + message;
                    }
                    else {
                        if (message === 'NumberOfDaysError') {
                            alert('شما میتوانید حداکثر 10 روز را ثبت کنید')
                            ActiveBtnAgain($('#btnSubmitReserve'), 'ثبت')
                        }
                        else if (message === 'priceError') {
                            alert('مبلغ باید بین 200000 و 4000000 باشد')
                            ActiveBtnAgain($('#btnSubmitReserve'), 'ثبت')
                        }
                        else {
                            alert(`با رزرو   ${message}  تداخل دارد`)
                            ActiveBtnAgain($('#btnSubmitReserve'), 'ثبت')
                        }
                    }

                },
                error: function () {
                    ActiveBtnAgain($('#btnSubmitReserve'), 'ثبت')
                    alert('خطای سرور')
                }
            });
        }

    })

    $('#btnSubmitHafReserve').click(function () {
        if (validateHafReserve()) {
            $('#btnSubmitHafReserve').prop('disabled', true);
            $('#frmReserveHafDay').submit()
        }
    })

    $('#reservePrice').on('input', function () {
        $(this).val($(this).val().replace(/,/g, ''));
        $(this).digits();
    })

    $('.btnRefresh').click(function () {
        location.reload();
    })
}
)

//================================================  input validation  ===================================================

function replaceBtnWithSpinner(elem) {
    elem.prop('disabled', true);
    elem.html(`<div  class="spinner-border text-light spinner-border-sm " role="status">
<span class="visually-hidden">Loading...</span>
</div>`)
}

function ActiveBtnAgain(elem, text) {
    elem.html(text);
    elem.prop('disabled', false);
}

$.fn.digits = function () {
    return this.each(function () {
        $(this).val($(this).val().replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,"));
    })
}



function validateReserve() {
    resetValidateReserve();
    if ($('.pdp-input').val().length == '') {
        $('#dateVld').show();
        return false
    }
    if ($('#RoomId').val() == 0) {
        $('#roomVld').show();
        return false
    }
    if ($('#folioCode').val() == 0) {
        $('#folioVld').show();
        return false
    }
    if ($('#reservePrice').val() == 0) {
        $('#priceVld').show();
        return false
    }
    return true
}
function resetValidateReserve() {
    $('.frmReserveValidation span').hide();
}



function validateHafReserve() {
    resetValidateHafReserve()
    if ($('#folioCode').val() == 0) {
        $('#folioVld').show();
        return false
    }
    var price = $('#reservePrice').val().replaceAll(',', '');
    let priceisNum = /^\d+$/.test(price);

    if (!priceisNum) {
        $('#priceVld').show();
        return false;
    }
    if (price < 100000 || price > 4000000) {
        alert('مبلغ باید بین 200000 و 4000000 باشد')
        return false;
    }
    return true
}


function resetValidateHafReserve() {
    $('.frmReserveValidationHafDay span').hide();
}


//================================================  filter  ===================================================



//================================================  filter  ===================================================



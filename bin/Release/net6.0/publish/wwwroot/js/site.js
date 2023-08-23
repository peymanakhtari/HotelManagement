$.urlParam = function (name) {
    var results = new RegExp('[\?&]' + name + '=([^&#]*)')
        .exec(window.location.search);

    return (results !== null) ? results[1] || 0 : false;
}

function addCommasToNumberInTable() {
    var numberCells = document.querySelectorAll('#tableContainer .priceFormat');
    numberCells.forEach(function (cell) {
        var number = parseInt(cell.textContent, 10);
        if (!isNaN(number)) {
            var formattedNumber = number.toLocaleString();
            cell.textContent = formattedNumber;
        } else {
            cell.textContent = '';
        }
    });
}

addCommasToNumberInTable();

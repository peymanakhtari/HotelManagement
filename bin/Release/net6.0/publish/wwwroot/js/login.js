

window.addEventListener("load", async (event) => {


    const element = document.querySelector('#validation');

    var elem = document.querySelector('.loginBox')
    if (element) {
        elem.style.transition = 'box-shadow .4s, opacity 0s'
        elem.style.opacity = 1;
    } else {

        await delay(500);
        elem.style.opacity = 1;

    }

});

function delay(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

document.getElementById("frmLogin").addEventListener("submit", function (event) {
    // Prevent the form from submitting immediately

    // Disable the submit button
    document.getElementById("btnsubmit").disabled = true;

    // Perform other actions (e.g., AJAX request, form validation)
    // Once your actions are complete, you can re-enable the submit button if needed
});



// Category Drop Down
$(document).ready(function () {

    // project master
    $('#ddProjectMaster').select2({
        placeholder: 'Select here.....',
        allowClear: false,
    });
    $('#ddProjectMaster').on('select2:select', function (e) {
        __doPostBack('<%= ddProjectMaster.ClientID %>', '');
    });

    // work order
    $('#ddWorkOrder').select2({
        placeholder: 'Select here.....',
        allowClear: false,
    });
    $('#ddWorkOrder').on('select2:select', function (e) {
        __doPostBack('<%= ddWorkOrder.ClientID %>', '');
    });

    // vendor name
    $('#ddVender').select2({
        placeholder: 'Select here.....',
        allowClear: false,
    });
    $('#ddVender').on('select2:select', function (e) {
        __doPostBack('<%= ddVender.ClientID %>', '');
    });

    // abstract no
    $('#ddAbstractNo').select2({
        placeholder: 'Select here.....',
        allowClear: false,
    });
    $('#ddAbstractNo').on('select2:select', function (e) {
        __doPostBack('<%= ddAbstractNo.ClientID %>', '');
    });





    // doc type
    $('#ddDocType').select2({
        placeholder: 'Select here.....',
        allowClear: false,
    });

    // stage level
    $('#ddStage').select2({
        placeholder: 'Select here.....',
        allowClear: false,
    });


    // Reinitialize Select2 after UpdatePanel partial postback
    var prm = Sys.WebForms.PageRequestManager.getInstance();

    // Reinitialize Select2 for all dropdowns
    prm.add_endRequest(function () {

        setTimeout(function () {

        }, 0);

        // project
        $('#ddProjectMaster').select2({
            placeholder: 'Select here.....',
            allowClear: false,
        });

        // work order
        $('#ddWorkOrder').select2({
            placeholder: 'Select here.....',
            allowClear: false,
        });
        $('#ddWorkOrder').on('select2:select', function (e) {
            __doPostBack('<%= ddWorkOrder.ClientID %>', '');
        });

        // vendor name
        $('#ddVender').select2({
            placeholder: 'Select here.....',
            allowClear: false,
        });

        // abstract no
        $('#ddAbstractNo').select2({
            placeholder: 'Select here.....',
            allowClear: false,
        });




        // doc type
        $('#ddDocType').select2({
            placeholder: 'Select here.....',
            allowClear: false,
        });

        // stage level
        $('#ddStage').select2({
            placeholder: 'Select here.....',
            allowClear: false,
        });
    });
});












// Pace.js configuration (optional)
Pace.options = {
    restartOnRequestAfter: false,
    restartOnPushState: false
};

// Use JavaScript or jQuery to hide the loading animation once the page is fully loaded
Pace.on('done', function () {
    $('.pace').addClass('pace-inactive');
});
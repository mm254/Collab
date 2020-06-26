// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function showPackageForm() {
    ShowError("Text", "Wurst");
    var x = document.getElementById("newPackageForm");
    if (x.style.display === "none") {
        x.style.display = "block";
    } else {
        x.style.display = "none";
    }
}

function showMemberForm() {
    ShowError("Text", "Wurst");
    var x = document.getElementById("addMemberForm");
    if (x.style.display === "none") {
        x.style.display = "block";
    } else {
        x.style.display = "none";
    }
}

/**
 * Shows an error message
 * @param string msg Main error text
 * @param string innerMsg Detailed error text
 */
function ShowError(msg, innerMsg) {
    try {
        CreateBasePopup("Error");
        var divError;
        var errorText = msg;
        if (innerMsg !== "" && innerMsg !== undefined && innerMsg !== null) {
            errorText += innerMsg;
        }
        if ($('#divPopup_Body').length) {
            if ($('#divPopup_Body').length) {
                divError = $('#divPopupError').empty();
            } else {
                var body = $('#divPopup_Body');
                divError = $('<div/>').attr("id", "divPopupError").addClass("divPopupError").appendTo(body);
            }
            $('<label/>').text(errorText).css({ "width": "100%", "color": "red" }).appendTo(divError);
        } else {
            alert(errorText);
        }
    } catch (e) {
        alert(e.message);
    }

/**
 * Creates a basic popup window
 * @param headerText Text for the popup header
 */
function CreatePopup(headerText) {
    try {
        if ($('#divPopup').length) {
            return;
        }
        if (headerText === undefined || headerText === "") {
            headerText = "Popup";
        }
        $('main').addClass("diabled");
        var popup = $('<div/>').attr({ "id": "divPopup" }).addClass("divPopup").appendTo('body');
        var header = $('<div/>').addClass("divPopup_Header").appendTo(popup);
        $('<label/>').text(headerText).appendTo(header);
        $('<button/>').text("X").attr({ "id": "btnClosePopup" }).addClass("btnClosePopup").appendTo(header);
        $('<div/>').attr({ "id": "divPopup_Body" }).addClass("divPopup_Body").appendTo(popup);
        var footer = $('<div/>').addClass("divPopup_Footer").appendTo(popup);
        $('<button/>').text("Okay").attr({ "id": "btnPopupSave" }).addClass("btnPopupSave btnPopup").appendTo(footer);

        $('btnClosePopup').click(function () {
            $('main').removeClass("disabled");
            $('#divPopup').remove();
        })
    }
    catch (e) {
        ShowError(e);
    }
}
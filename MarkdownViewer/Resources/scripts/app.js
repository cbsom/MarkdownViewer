/// <reference path="jquery.js" />

const State_Previewing = 0;
const State_Editing = 1;

$(function () {
    /*******************************************************************************************************
    The controller class comes directly from the ChromiumWebBrowser object.
    It was registered to this browser instance by the main form constructor
    as an instance of the MarkdownViewer.PageController class.
    *******************************************************************************************************/

    $('#btnEditMarkdown').on('click', editMarkdown);
    $('#btnOpenMarkdown').on('click', openFile);
    $('#btnSaveAsHtml').on('click', saveAsHtml);
    $('#btnSaveAsMarkdown').on('click', saveAsMarkdown);
    $('#btnSaveAsPDF').on('click', printToPdf);
    $('#btnShowDevTools').on('click', showDevTools);
    $('#btnCloseAlert').on('click', function () { $('#divAlert').fadeOut(); });

    window.alert = function (msg, type, selfCloseSeconds) {
        $('#divAlertMessage').html(msg);
        $('#divAlert')
            .removeClass('alert-success alert-info alert-warning alert-danger')
            .addClass(type && ['success', 'info', 'warning', 'danger'].indexOf(type.toString().toLowerCase()) > -1 ?
                'alert-' + type : 'alert-info')
            .css('left', parseInt(($('body').innerWidth() / 2) - ($('#divAlert').width() / 2)).toString() + 'px')
            .fadeIn();
        if (selfCloseSeconds) {
            window.setTimeout(function () {
                $('#divAlert').fadeOut();
            }, selfCloseSeconds * 1000);
        }
    }

    initialize();
});

function initialize() {
    $('#h4MdPath').html(controller.markdownFileName);
    //Load the html converted from the markdown contained in Program.MarkdownText.
    $('#divContent').html(controller.getFormatted(controller.markdownText));

    //The html has loaded onto the page. We can now format the code sections.
    window.loadBrushes();
    SyntaxHighlighter.all();
}

function editMarkdown() {
    $('#divContent').html('<textarea id="txtEditor"></textarea>');

    window.simplemde = new SimpleMDE({
        element: $("#txtEditor")[0],
        autoDownloadFontAwesome: false,
        initialValue: controller.markdownText,
        forceSync: true
    });
    window.simplemde.codemirror.on("change", function () {
        controller.markdownText = window.simplemde.value();
    });
    $('#btnEditMarkdown').addClass('active').off('click');
    $('#btnShowFormatted').removeClass('active').on('click', showFormatted);
    controller.setBrowserState(State_Editing);
}

function showFormatted() {
    if (window.simplemde) {
        controller.markdownText = window.simplemde.value();
    }
    initialize();
    controller.setBrowserState(State_Previewing);
    $('#btnShowFormatted').addClass('active').off('click');
    $('#btnEditMarkdown').removeClass('active').on('click', editMarkdown);
}

function openFile() {
    controller.openMarkdown();
}

function saveAsHtml() {
    var html = `<!DOCTYPE html>
        <html>
            ${document.head.outerHTML}
            ${document.body.outerHTML}
        </html>`.replace(/src *= *"scripts\\/gi, 'src="file://' + controller.resourcesDirectory + '\\scripts\\')
                .replace(/href *= *"styles\\/gi, 'href="file://' + controller.resourcesDirectory + '\\styles\\')
                .replace(/src *= *"images\\/gi, 'src="file://' + controller.resourcesDirectory + '\\images\\')
                .replace(/<!--<NAV_BAR>-->[^]+<!--<\/NAV_BAR>-->/, '');
    controller.saveHtml(html);
}

function saveAsMarkdown() {
    controller.saveAsMarkdown();
}

function printToPdf() {
    controller.printToPdf();
}


function showDevTools() {
    controller.showDevTools();
}

function fileWasChanged() {
    alert('file was changed!', 'warning', 5);
}

function fileWasDeleted() {
    alert('file was deleted!', 'warning', 5);
}
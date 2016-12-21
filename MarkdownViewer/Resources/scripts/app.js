/// <reference path="jquery.js" />
/// <reference path="simplemde.min.js" />

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
    $('#btnAbout').on('click', showAbout);
    $('#btnCloseAlert').on('click', function () { $('#divAlert').fadeOut(); });

    window.alert = function (msg, type, selfCloseSeconds) {
        $('#divAlertMessage').html(msg);
        $('#divAlert')
            .removeClass('alert-success alert-info alert-warning alert-danger')
            .addClass(type && ['success', 'info', 'warning', 'danger'].indexOf(type.toString().toLowerCase()) > -1 ?
                'alert-' + type : 'alert-info')
            .css({
                'left': parseInt(($('body').innerWidth() / 2) - ($('#divAlert').width() / 2)).toString() + 'px',
                'zIndex': 1000
            })
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
    $('#h4MdPath').html(controller.markdownPath);
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
        toolbar: [{
            name: "save",
            action: saveChanges,
            className: "fa fa-floppy-o",
            title: "Save Changes"
        }, "|", "bold", "italic", "strikethrough", "heading", "|", "quote", "unordered-list", "ordered-list", "|", "link", "image", "table", "|", "side-by-side"],
        shortcuts: {
            "saveChanges": "Ctrl-S"
        }
    });
    window.simplemde.codemirror.on("change", function () {
        controller.markdownText = window.simplemde.value();
    });

    $('#btnEditMarkdown').addClass('active').off('click');
    $('#btnShowFormatted').removeClass('active').on('click', showFormatted);
    $('#divMain').removeClass('container');
    controller.setBrowserState(State_Editing);
}

function showFormatted() {
    if (window.simplemde) {
        controller.markdownText = window.simplemde.value();
    }
    initialize();
    $('#btnShowFormatted').addClass('active').off('click');
    $('#btnEditMarkdown').removeClass('active').on('click', editMarkdown);
    $('#divMain').addClass('container');

    controller.setBrowserState(State_Previewing);
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

function getHtml(md) {
    return controller.getHtml(md);
}

function saveAsMarkdown() {
    controller.saveAsMarkdown();
}


function saveChanges() {
    if (controller.saveMarkdown()) {
        alert('The file has been saved.', 'info', 2);
    }
    else {
        alert('The file could not be saved at this time.', 'warning');
    }
}

function printToPdf() {
    controller.printToPdf();
}


function showDevTools() {
    controller.showDevTools();
}

function showAbout() {
    alert(`<img src="images\\Icon-sm.png" />&nbsp; &nbsp;
          <strong style="color:#000;font-size:1.4em;">Markdown Viewer</strong>
          <hr />
          A clean and quick Markdown file viewer for Windows.
          <br />
          <hr />
          <small style="font-size:7pt;">Created by Compute Software Solutions<sup>&copy; </sup></small>`, 'info', 5);
}

function fileWasDeleted() {
    alert('The Markdown file has been deleted!', 'warning', 5);
}
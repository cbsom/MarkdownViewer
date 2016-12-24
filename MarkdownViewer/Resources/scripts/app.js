/*******************************************************************************************************
 NOTE: The controller class comes directly from the ChromiumWebBrowser object.
 It was registered to this browser instance by the main form constructor
 as an instance of the MarkdownViewer.PageController class.
*******************************************************************************************************/
'use strict';
const State_Previewing = 0;
const State_Editing = 1;

class MarkdownViewer {
    constructor() {
        $('#btnEditMarkdown').on('click', () => this.editMarkdown());
        $('#btnOpenMarkdown').on('click', () => this.openFile());
        $('#btnSaveAsHtml').on('click', () => this.saveAsHtml());
        $('#btnSaveAsMarkdown').on('click', () => this.saveAsMarkdown());
        $('#btnSaveAsPDF').on('click', () => this.printToPdf());
        $('#btnShowDevTools').on('click', () => this.showDevTools());
        $('#btnAbout').on('click', () => this.showAbout());
        $('#btnCloseAlert').on('click', () =>  $('#divAlert').fadeOut());

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

        this.initialize();
    }

    initialize() {
        $('#h4MdPath').html(controller.markdownFileName);
        //Load the html converted from the markdown contained in Program.MarkdownText.
        $('#divContent').html(controller.getFormatted(controller.markdownText));

        //The html has loaded onto the page. We can now format the code sections.
        window.loadBrushes();
        SyntaxHighlighter.all();
    }

    editMarkdown() {
        $('#divContent').html('<textarea id="txtEditor"></textarea>');

        this.createEditor();

        $('#btnEditMarkdown').addClass('active').off('click');
        $('#btnShowFormatted').removeClass('active').on('click', () => this.showFormatted());
        $('#divMain').removeClass('container');
        controller.setBrowserState(State_Editing);

        if (this.showingSideBySide === true) {
            this.showSideBySide(true);
        }
    }

    showFormatted() {
        if (this.simplemde) {
            controller.markdownText = this.simplemde.value();
        }
        initialize();
        $('#btnShowFormatted').addClass('active').off('click');
        $('#btnEditMarkdown').removeClass('active').on('click', () => this.editMarkdown());
        $('#divMain').addClass('container');

        controller.setBrowserState(State_Previewing);
    }

    openFile() {
        controller.openMarkdown();
    }

    saveAsHtml() {
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

    getHtml(md) {
        return controller.getHtml(md);
    }

    saveAsMarkdown() {
        controller.saveAsMarkdown();
    }

    printToPdf() {
        controller.printToPdf();
    }

    showDevTools() {
        controller.showDevTools();
    }

    showAbout() {
        alert(`<img src="images\\Icon-sm.png" />&nbsp; &nbsp;
          <strong style="color:#000;font-size:1.4em;">Markdown Viewer</strong>
          <hr />
          A clean and quick Markdown file viewer for Windows.
          <br />
          <hr />
          <small style="font-size:7pt;">Created by Compute Software Solutions<sup>&copy; </sup></small>`, 'info', 5);
    }

    fileWasDeleted() {
        alert('The Markdown file has been deleted!', 'warning', 5);
    }

    createEditor() {
        this.simplemde = new SimpleMDE({
            element: $("#txtEditor")[0],
            autoDownloadFontAwesome: false,
            initialValue: controller.markdownText,
            toolbar: [{
                name: "save",
                action: function () {
                    if (controller.saveMarkdown()) {
                        alert('The file has been saved.', 'info', 2);
                    }
                    else {
                        alert('The file could not be saved at this time.', 'warning');
                    }
                },
                className: "fa fa-floppy-o",
                title: "Save Changes"
            },
            "|", "bold", "italic", "strikethrough", "heading", "|", "quote", "unordered-list", "ordered-list", "|", "link", "image", "table", "|",
            {
                name: "side-by-side",
                action: (() => this.showSideBySide()),
                className: "fa fa-columns",
                title: "Side by Side Editing"
            }, "|"]
        });
        this.simplemde.codemirror.on("change", function () {
            controller.markdownText = this.simplemde.value();
        });
    }

    showSideBySide(show) {
        if (show === true || !this.simplemde.isSideBySideActive()) {
            this.simplemde.toggleSideBySide();
            /*The inbuilt SimpleMDE side-by-side button was having issues with something in our css
             * so we need to do horrible, unsightly and unseemly hacks ;) */
            if (!this.simplemde.isSideBySideActive())
                this.simplemde.toggleSideBySide();

            this.showingSideBySide = true;
        }
        else {
            this.simplemde.toggleSideBySide();
            this.showingSideBySide = false;
        }
    }   
}

$(function () {
    window.markdownViewer = new MarkdownViewer();
});
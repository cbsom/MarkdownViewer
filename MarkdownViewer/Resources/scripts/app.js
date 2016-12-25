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
        this.initialize();
    }

    initialize() {
        if (!!controller.markdownFileName) {
            $('#h4MdPath').html(controller.markdownFileName);
            //Load the html converted from the markdown contained in Program.MarkdownText.
            $('#divContent').html(controller.getFormatted(controller.markdownText));
            $('#btnEditMarkdown').on('click', () => this.editMarkdown());

            //The html has loaded onto the page. We can now format the code sections.
            window.loadBrushes();
            SyntaxHighlighter.all();
        }
    }

    editMarkdown() {
        //The SimpleMDE component loves textareas
        $('#divContent').html('<textarea id="txtEditor"></textarea>');

        //Now that we have a textarea for it to eat, we can create the SimpleMDE component
        this.createEditor();

        $('#btnEditMarkdown').addClass('active').off('click');
        $('#btnShowFormatted').removeClass('active').one('click', () => this.showFormatted());
        $('#divMain').removeClass('container');

        //The MenuHandler class customizes the right-click menu according to the current state.
        controller.setBrowserState(State_Editing);

        //If the user was previously editing side-by-side before previewing, they probably want to go back to that.
        if (this.showingSideBySide === true) {
            this.showSideBySide(true);
        }
    }

    showFormatted() {
        if (this.simplemde) {
            //We are changing from editing to viewing: copy the contents of the editor back into the controller.
            //Note, this does not update the changes back to the physical file. Only controller.saveMarkdown() does that.
            controller.markdownText = this.simplemde.value();
        }
        //We need to convert the markdown to html and load it into the page content.
        this.initialize();

        $('#btnShowFormatted').addClass('active').off('click');
        $('#btnEditMarkdown').removeClass('active').one('click', () => this.editMarkdown());
        $('#divMain').addClass('container');

        //The MenuHandler class customizes the right-click menu according to the current state.
        controller.setBrowserState(State_Previewing);
    }

    //Open a different Markdown file
    openFile() {
        controller.openMarkdown();
    }

    //Save the current Markdown as html
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

    //Save the current Markdown file as...
    saveAsMarkdown() {
        controller.saveAsMarkdown();
    }

    //Calls the browser components PrintToPdfAsync function
    printToPdf() {
        controller.printToPdf();
    }

    //Show Chromes DevTools
    showDevTools() {
        controller.showDevTools();
    }

    //You always need (at least) one of these
    showAbout() {
        alert(`<img src="images\\Icon-sm.png" />&nbsp; &nbsp;
          <strong style="color:#000;font-size:1.4em;">Markdown Viewer</strong>
          <hr />
          A clean and quick Markdown file viewer for Windows.
          <br />
          <hr />
          <small style="font-size:7pt;">Created by Compute Software Solutions<sup>&copy; </sup></small>`, 'info', 5);
    }

    //The PageController calls this function if the file we are viewing gets itself deleted.
    fileWasDeleted() {
        alert('The Markdown file has been deleted!', 'warning', 5);
    }

    //Create a SimpleMDEeditor component - cooked the way we like it
    createEditor() {
        this.simplemde = new SimpleMDE({
            element: $("#txtEditor")[0],
            autoDownloadFontAwesome: false,
            initialValue: controller.markdownText,
            toolbar: [{
                name: "save",
                action: (() => this.saveChanges()),
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

    saveChanges() {
        if (controller.saveMarkdown()) {
            alert('The file has been saved.', 'info', 2);
        }
        else {
            alert('The file could not be saved at this time.', 'warning');
        }
    }

    //The inbuilt SimpleMDE side-by-side button was having issues with something in our css
    //so we need to do horrible, unsightly and unseemly hacks
    showSideBySide(show) {
        if (show === true || !this.simplemde.isSideBySideActive()) {
            this.simplemde.toggleSideBySide();
            //De-ja-vu. Didn't we just do this? Yup. ugly hacks - such as this one.
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

//Replace alert with a custom bootstrap styled one.
window.alert = function (msg, type, selfCloseSeconds) {
    $('#divAlertMessage').html(msg);
    $('#divAlert')
        .removeClass('alert-success alert-info alert-warning alert-danger')
        .addClass(type && ['success', 'info', 'warning', 'danger'].indexOf(type.toString().toLowerCase()) > -1 ?
            'alert-' + type.toLowerCase() : 'alert-info')
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

$(function () {
    var mv = window.markdownViewer = new MarkdownViewer();

    $('#btnOpenMarkdown').on('click', () => mv.openFile());
    $('#btnOpenMarkdown2').on('click', () => mv.openFile());
    $('#btnSaveAsHtml').on('click', () => mv.saveAsHtml());
    $('#btnSaveAsMarkdown').on('click', () => mv.saveAsMarkdown());
    $('#btnSaveAsPDF').on('click', () => mv.printToPdf());
    $('#btnShowDevTools').on('click', () => mv.showDevTools());
    $('#btnAbout').on('click', () => mv.showAbout());
    $('#btnCloseAlert').on('click', () =>  $('#divAlert').fadeOut());
});
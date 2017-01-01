/*******************************************************************************************************
 NOTE: The controller class comes directly from the ChromiumWebBrowser object.
 It was registered to this browser instance by the main form constructor
 as an instance of the MarkdownViewer.PageController class.
*******************************************************************************************************/
'use strict';
const global = this;
const State_Previewing = 0;
const State_Editing = 1;

class MarkdownViewer {
    constructor(global) {
        this.global = global;
        this.showingSideBySide = false;
        this.state = State_Previewing;
        this.initialize();
    }

    initialize() {
        if (!!controller.markdownFileName) {
            $('#h4MdPath').html(controller.markdownFileName);
            //Load the html converted from the markdown contained in Program.MarkdownText.
            $('#divContent').html(controller.getFormatted(controller.markdownText));
            $('#btnEditMarkdown').on('click', () => this.editMarkdown());

            //The html has loaded onto the page. We can now format the code sections.
            this.global.loadBrushes();
            SyntaxHighlighter.all();

            //If this function is being called by the controller, for example if a new file is being opened, 
            //we want to make sure that we show the html preview - even if the user was in middle of editing the original file.
            if (this.state !== State_Previewing) {
                this.showFormatted();
            }
        }
    }

    editMarkdown() {
        //The MenuHandler class customizes the right-click menu according to the current state.
        controller.setBrowserState(State_Editing);
        this.state = State_Editing;

        //The SimpleMDE component loves textareas
        $('#divContent').html('<textarea id="txtEditor"></textarea>');

        //Now that we have a textarea for it to eat, we can create the SimpleMDE component
        this.createEditor();

        $('#btnEditMarkdown').addClass('active').off('click');
        $('#btnShowFormatted').removeClass('active').one('click', () => this.showFormatted());
        $('#divMain').removeClass('container');


        //If the user was previously editing side-by-side before previewing, they probably want to go back to that.
        if (this.showingSideBySide === true) {
            this.showSideBySide(true);
        }
    }

    showFormatted() {
        //The MenuHandler class customizes the right-click menu according to the current state.
        controller.setBrowserState(State_Previewing);
        this.state = State_Previewing;

        if (this.simplemde) {
            //We are changing from editing to viewing: copy the contents of the editor back into the controller.
            //Note, this does not update the changes back to the physical file. Only controller.saveMarkdown() does that.
            controller.markdownText = this.simplemde.value();

            if (this.simplemde.isFullscreenActive()) {
                this.simplemde.toggleFullScreen();
            }
        }
        //We need to convert the markdown to html and load it into the page content.
        this.initialize();

        $('#btnShowFormatted').addClass('active').off('click');
        $('#btnEditMarkdown').removeClass('active').one('click', () => this.editMarkdown());
        $('#divMain').addClass('container');

        //Due to an issue with the simplemde side-by-side functionality, the body sometimes stays overflow:hidden even after the side-by-side is closed
        $('body').css('overflow', 'auto');
    }

    //Open a different Markdown file
    openFile() {
        if (controller.hasUnsavedChanges()) {
            confirm('There are unsaved changes.<br /> Do you wish to save these changes?', 'Save Changes?',
                (/*onYes*/) => {
                    this.saveChanges();
                    alert('The changes have been saved.');
                    controller.openMarkdown();
                },
                (/*onNo*/) => controller.openMarkdown());
        }
        else {
            controller.openMarkdown();
        }
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
            //The fullscreen button will be hidden by the css. It was only added here to prevent errors in the side-by-side button
            "|", "bold", "italic", "strikethrough", "heading", "|", "quote", "unordered-list", "ordered-list", "|", "link", "image", "table", "fullscreen", "|",
            {
                name: "side-by-side",
                action: (() => this.showSideBySide()),
                className: "fa fa-columns",
                title: "Side by Side Editing"
            }, "|",
            {
                name: "guide",
                action: (() => this.showGuide()),
                className: "fa fa-question-circle",
                title: "Markdown Guide"
            }]
        });
        this.simplemde.codemirror.on("change",
            () => controller.markdownText = this.simplemde.value());
    }

    saveChanges() {
        if (controller.saveMarkdown()) {
            alert('The file has been saved.', 'info', 2);
        }
        else {
            alert('The file could not be saved at this time.', 'warning');
        }
    }

    showSideBySide(show) {
        this.showingSideBySide = show || !this.simplemde.isSideBySideActive();
        if (typeof show === 'undefined' || show !== this.simplemde.isSideBySideActive()) {
            this.simplemde.toggleSideBySide();
        }

        //There are some issues with the simplemde side-by-side code.
        if (this.showingSideBySide && (!this.simplemde.isSideBySideActive())) {
            this.simplemde.toggleSideBySide();
        }
    }

    showGuide() {
        controller.showGuide();
    }

    find() {
        controller.toggleFind();
    }

    doKeyUp(evt) {
        var keyCode = evt.keyCode,
            ctrlKey = evt.ctrlKey,
            shiftKey = evt.shiftKey,
            altKey = evt.altKey,
            selectedText = this.global.getSelection().toString();
        this.global.setTimeout(function () {
            controller.doKeyUp(keyCode, ctrlKey, shiftKey, altKey, selectedText);
        }, 5);
    }
}

//Replace alert with a custom bootstrap styled one.
global.alert = function (msg, type, selfCloseSeconds) {
    var hasValidType = type && ['success', 'info', 'warning', 'danger'].includes(type.toLowerCase());
    $('#divAlertMessage').html(msg);
    $('#divAlert')
        .removeClass('alert-success alert-info alert-warning alert-danger')
        .addClass(hasValidType ? 'alert-' + type.toLowerCase() : 'alert-info')
        .css({
            'left': parseInt(($('body').innerWidth() / 2) - ($('#divAlert').width() / 2)).toString() + 'px',
            'zIndex': 1000
        })
        .fadeIn();
    if (selfCloseSeconds) {
        global.setTimeout(function () {
            $('#divAlert').fadeOut();
        }, selfCloseSeconds * 1000);
    }
}

//Replace confirm with a custom bootstrap styled one that has 3 events.
global.confirm = function (msg, title, onYes, onNo, OnCancel) {
    $('#divConfirmTitle').html(title || 'Please confirm...');
    $('#pConfirmBody').html(msg || 'Are you sure?');
    $('#btnConfirmCancel').off('click').one('click', function () {
        if (OnCancel) { OnCancel(); }
    });
    $('#btnConfirmYes').off('click').one('click', function () {
        if (onYes) { onYes(); }
    });
    $('#btnConfirmNo').off('click').one('click', function () {
        if (onNo) { onNo(); }
    });
    $('#divConfirm').modal('show');
}

$(function () {
    var mv = global.markdownViewer = new MarkdownViewer(global);

    $('.btnOpenMarkdown').on('click', () => mv.openFile());
    $('#btnSaveAsHtml').on('click', () => mv.saveAsHtml());
    $('#btnSaveAsMarkdown').on('click', () => mv.saveAsMarkdown());
    $('#btnSaveAsPDF').on('click', () => mv.printToPdf());
    $('#btnShowDevTools').on('click', () => mv.showDevTools());
    $('#btnAbout').on('click', () => mv.showAbout());
    $('#btnCloseAlert').on('click', () =>  $('#divAlert').fadeOut());
    $('#btnFind').on('click', () =>  mv.find());
    $('body').on('keyup', function (e) { mv.doKeyUp(e); });
});
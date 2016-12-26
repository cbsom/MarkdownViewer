![](https://bitbucket-assetroot.s3.amazonaws.com/c/photos/2016/Dec/20/161221880-5-markdownviewer-logo_avatar.png) 
# Markdown Viewer

**A clean, quick and simple Markdown viewer for Windows.**

Preview and Edit local Markdown files.

The client is a very simple Windows Forms container for a [CEFSharp](https://github.com/cefsharp/CefSharp) browser component which is based on the [Chromium Embedded Framework](https://bitbucket.org/chromiumembedded/cef).

To format the Markdown for display, it is converted to HTML with [MarkdownSharp](https://code.google.com/archive/p/markdownsharp/), customized to properly format github style code blocks.

The HTML is styled with [Bootstrap](http://getbootstrap.com/), and the code blocks are syntax highlighted with [SyntaxHighlighter](http://alexgorbatchev.com/SyntaxHighlighter/).

Markdown file editing is done using the wonderful [SimpleMDE](https://simplemde.com/).

### Customize Application

The application is actually a web site and it allows full customization of the GUI - including the option to change the entire page. 

To customize the GUI you can either:
1. Edit the /Resources/HTML_TEMPLATE.html file. The styles are contained in /Resources/styles/app.css and the code is in /Resources/scripts/app.js
2. Create a new template. To configure the application to use your new template, open the MarkdownViewer.exe.config file and edit the configuration/userSettings/MarkdownViewer.Properties.Settings/setting element to point to your template.

#### The Client Side Javascript

We have that wonderfuly rare situation where we know exactly which browser the user will be using as we have the browser embedding in the application. 

The Chromium Embedded Framework fully supports ECMAScript 2015 (ES6). No need for Babel.


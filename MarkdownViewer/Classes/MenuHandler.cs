using CefSharp;
namespace MarkdownViewer
{
    internal class MenuHandler : IContextMenuHandler
    {
        private const CefMenuCommand EditMarkdown = (CefMenuCommand)26501;
        private const CefMenuCommand ViewFormatted = (CefMenuCommand)26502;
        private const CefMenuCommand SaveChanges = (CefMenuCommand)26503;
        private const CefMenuCommand ViewSideBySide = (CefMenuCommand)26504;

        void IContextMenuHandler.OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
        {
            model.Clear();

            if (Program.BrowserState == BrowserStates.Previewing)
            {
                model.AddItem((CefMenuCommand)EditMarkdown, "Edit Markdown");
            }
            else if (Program.BrowserState == BrowserStates.Editing)
            {
                model.AddItem((CefMenuCommand)SaveChanges, "Save Changes");
                model.AddSeparator();
                model.AddItem((CefMenuCommand)ViewFormatted, "View Formatted");
                model.AddItem((CefMenuCommand)ViewSideBySide, "Edit and View Side-By-Side");
            }
            model.AddSeparator();
            model.AddItem(CefMenuCommand.Print, "Print");
            model.AddSeparator();
            model.AddItem(CefMenuCommand.Copy, "Copy");
            if (Program.BrowserState == BrowserStates.Editing)
            {
                model.AddItem(CefMenuCommand.Cut, "Cut");
                model.AddItem(CefMenuCommand.Paste, "Paste");
            }
            model.AddSeparator();
            model.AddItem(CefMenuCommand.Reload, "Reload File");

        }

        public bool OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
        {
            switch (commandId)
            {
                case EditMarkdown:
                    browser.MainFrame.ExecuteJavaScriptAsync("window.markdownViewer.editMarkdown();");
                    break;
                case ViewFormatted:
                    browser.MainFrame.ExecuteJavaScriptAsync("window.markdownViewer.showFormatted();");
                    break;
                case SaveChanges:
                    browser.MainFrame.ExecuteJavaScriptAsync("window.markdownViewer.saveChanges();");
                    break;
                case ViewSideBySide:
                    browser.MainFrame.ExecuteJavaScriptAsync("window.markdownViewer.showSideBySide(true);");
                    break;
            }

            return false;
        }

        public void OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame)
        {
            //throw new NotImplementedException();
        }

        public bool RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
        {
            return false;
            //throw new NotImplementedException();
        }
    }
}

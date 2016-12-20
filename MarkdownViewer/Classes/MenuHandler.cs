using CefSharp;
namespace MarkdownViewer
{
    internal class MenuHandler : IContextMenuHandler
    {
        private const int EditMarkdown = 26501;
        private const int ViewFormatted = 26502;

        void IContextMenuHandler.OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
        {
            model.Clear();

            model.AddItem(CefMenuCommand.Print, "Print"); // Remove "View Source" option                
            model.AddSeparator();
            if (Program.BrowserState == BrowserStates.Previewing)
            {
                model.AddItem((CefMenuCommand)EditMarkdown, "Edit Markdown");
            }
            else if (Program.BrowserState == BrowserStates.Editing)
            {
                model.AddItem((CefMenuCommand)ViewFormatted, "View Formatted");
            }
        }

        public bool OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
        {
            if (commandId == (CefMenuCommand)EditMarkdown)
            {
                browser.MainFrame.ExecuteJavaScriptAsync("editMarkdown();");
            }
            else if (commandId == (CefMenuCommand)ViewFormatted)
            {
                browser.MainFrame.ExecuteJavaScriptAsync("showFormatted();");
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

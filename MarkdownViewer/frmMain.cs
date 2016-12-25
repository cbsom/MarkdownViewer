using System;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;

namespace MarkdownViewer
{
    public partial class frmMain : Form
    {
        private readonly ChromiumWebBrowser _browser;
        private string _baseUrl = "file://" + Program.ResourcesDirectory + "\\";

        internal frmMain()
        {
            InitializeComponent();
            this._browser = new ChromiumWebBrowser("about:blank")
            {
                Dock = DockStyle.Fill,
                RequestHandler = new RequestHandler(),
                MenuHandler = new MenuHandler()
            };
            this._browser.RegisterJsObject("controller", new PageController());
            this.Controls.Add(this._browser);
            this._browser.LoadHtml(Program.GetHtmlTemplate(), this._baseUrl);
        }

        internal void RunJavscript(string script)
        {
            if (this._browser != null)
            {
                this._browser.GetMainFrame().ExecuteJavaScriptAsync(script);
            }
        }

        internal void ShowDevTools()
        {
            if (this._browser != null)
            {
                this._browser.ShowDevTools();
            }
        }

        internal void PrintToPdf(string path)
        {
            var settings = new PdfPrintSettings()
            {
                BackgroundsEnabled = true,
                HeaderFooterEnabled = true
            };
            var doIt = new Action(() =>
            {
                this._browser.PrintToPdfAsync(path, settings);
            });

            if (this._browser != null)
            {
                if (this._browser.InvokeRequired)
                {
                    this._browser.Invoke(doIt);
                }
                else
                {
                    doIt();
                }
            }
        }
    }
}

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

        internal void ToggleFind()
        {
            if (this._browser != null)
            {
                if (this.pnlFind.Visible)
                {
                    this.pnlFind.Visible = false;
                    this._browser.StopFinding(true);
                }
                else
                {
                    this.pnlFind.Visible = true;
                    this.pnlFind.Focus();
                    this.txtFind.Focus();

                    if (!string.IsNullOrEmpty(this.txtFind.Text))
                    {
                        this._browser.Invoke(new Action(() =>
                            this._browser.Find(0, this.txtFind.Text, true, false, false)));
                        this.txtFind.SelectAll();
                    }
                }
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

        private void txtFind_TextChanged(object sender, EventArgs e)
        {
            if (this._browser != null)
            {
                this._browser.Invoke(new Action(() =>
                {
                    this._browser.StopFinding(false);
                    this._browser.Find(0, this.txtFind.Text, true, false, false);
                }));
            }
        }

        private void btnNextSearch_Click(object sender, EventArgs e)
        {
            if (!this.pnlFind.Visible)
            {
                this.ToggleFind();
            }
            this._browser.Invoke(new Action(() =>
            {
                this._browser.Find(0, this.txtFind.Text, true, false, true);
            }));
        }

        private void frmMain_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F3:
                    this.btnNextSearch.PerformClick();
                    break;
                case Keys.Escape:
                    if (this._browser != null)
                    {
                        this._browser.Invoke(new Action(() => this._browser.StopFinding(true)));
                    }
                    this.pnlFind.Visible = false;
                    break;
            }
        }
    }
}

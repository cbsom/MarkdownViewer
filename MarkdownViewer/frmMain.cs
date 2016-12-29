using System;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using System.Linq;

namespace MarkdownViewer
{
    public partial class frmMain : Form
    {
        #region private variables
        private readonly ChromiumWebBrowser _browser;
        private readonly string _baseUrl = "file://" + Program.ResourcesDirectory + "\\";
        private readonly Keys[] _keysWeHandle = new Keys[] { Keys.Escape, Keys.F3, Keys.F };
        #endregion

        #region constructors
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
            this._browser.LoadingStateChanged += _browser_LoadingStateChanged;
        }
        #endregion

        #region event handlers       
        private void txtFind_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab || e.KeyCode == Keys.Enter && !string.IsNullOrEmpty(this.txtFind.Text))
            {
                this.NextMatch();
                e.SuppressKeyPress = true;
                return;
            }
            //Filter out all key strokes we don't need to handle.
            else if ((!_keysWeHandle.Contains(e.KeyCode)) || (e.KeyCode == Keys.F && !e.Control))
            {
                return;
            }
            else
            {
                //try to get currently selected text in the browser window
                string searchText = null;
                var response = this.EvaluateScriptAsync(@"(function(){ return window.getSelection().toString(); })();");
                if (response.Success)
                {
                    searchText = Convert.ToString(response.Result);
                }

                this.DoKeyUp(e.KeyCode, e.Control, e.Shift, e.Alt, searchText);
            }
        }

        private void _browser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (!e.IsLoading)
            {
                //After the browser has finished loading, we want it to have focus to be able to catch keyboard events.
                this.FocusOnBrowser();
            }
        }

        private void txtFind_TextChanged(object sender, EventArgs e)
        {
            if (this._browser != null)
            {
                this._browser.Invoke(new Action(() =>
                {
                    this.StopFinding(false);
                    this._browser.Find(0, this.txtFind.Text, true, false, false);
                }));
            }
        }

        private void btnNextSearch_Click(object sender, EventArgs e)
        {
            this.NextMatch();
        }
        #endregion

        #region private functions
        private void FocusOnBrowser()
        {
            if (this._browser != null)
                this._browser.Invoke(new Action(() => this._browser.Focus()));
        }

        private void CloseFindingPanel()
        {
            if (this._browser != null)
            {
                this.StopFinding(true);
            }
            this.pnlFind.Visible = false;
            this.FocusOnBrowser();
        }

        private void StopFinding(bool clearSelection)
        {
            this._browser.Invoke(new Action(() => this._browser.StopFinding(clearSelection)));
        }

        private void NextMatch()
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

        #endregion

        #region public/internal functions
        internal void RunJavscript(string script)
        {
            if (this._browser != null)
            {
                if (this._browser.InvokeRequired)
                {
                    this._browser.Invoke(new Action(() =>
                        this._browser.GetMainFrame().ExecuteJavaScriptAsync(script)));
                }
                else
                {
                    this._browser.GetMainFrame().ExecuteJavaScriptAsync(script);
                }
            }
        }

        internal JavascriptResponse EvaluateScriptAsync(string script)
        {
            JavascriptResponse response = null;
            if (this._browser != null)
            {
                if (this._browser.InvokeRequired)
                {
                    this._browser.Invoke(new Action(() =>
                        response = this._browser.GetMainFrame().EvaluateScriptAsync(script).Result));
                }
                else
                {
                    response = this._browser.GetMainFrame().EvaluateScriptAsync(script).Result;
                }
            }
            return response;
        }

        internal void ToggleFind()
        {
            if (this._browser != null)
            {
                if (this.pnlFind.Visible)
                {
                    this.CloseFindingPanel();
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

        internal void DoKeyUp(Keys keyCode, bool control, bool shift, bool alt, string selectedText)
        {
            //We only handle a few keys
            if ((!_keysWeHandle.Contains(keyCode)))
            {
                return;
            }
            switch (keyCode)
            {
                case Keys.F3:
                    this.btnNextSearch.PerformClick();
                    break;
                case Keys.Escape:
                    this.CloseFindingPanel();
                    break;
                case Keys.F:
                    if (control)
                    {
                        if (!string.IsNullOrEmpty(selectedText))
                        {
                            this.txtFind.Text = selectedText;
                        }

                        if (this.pnlFind.Visible)
                        {
                            this.txtFind.Focus();
                        }
                        else
                        {
                            this.ToggleFind();
                        }
                    }
                    break;
            }
        }
        #endregion       
    }
}

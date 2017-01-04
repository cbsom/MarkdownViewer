using MarkdownSharp;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace MarkdownViewer
{
    /// <summary>
    /// This class is accessed from the "client-side" javascript code as the "controller" class.
    /// </summary>
    public class PageController
    {
        public string MarkdownPath { get { return Program.MarkdownPath; } }

        public string MarkdownFileName { get { return Path.GetFileName(Program.MarkdownPath); } }

        public string ResourcesDirectory { get { return Program.ResourcesDirectory; } }

        public string MarkdownText
        {
            get { return Program.MarkdownText; }
            set { Program.MarkdownText = value; }
        }

        public string GetFormatted(string markdownText)
        {
            Markdown md = new Markdown();
            string html = null;
            try
            {
                html = md.Transform(markdownText);
            }
            catch (Exception ex)
            {
                html = string.Format(@"<h2>Oops!</h2>We are terribly sorry, an error has occurred.<p>&nbsp;</p>
                    <em>{0}</em><hr /><pre>{1}</pre>", ex.Message, ex.StackTrace);
                Program.Log(this.MarkdownPath, "Exception converting Markdown to HTML\n\t{0}\n\t\t{1}",
                    ex.Message, ex.StackTrace);
            }

            return html;
        }

        public bool SaveMarkdown()
        {
            return Program.SaveMarkdown();
        }

        public void SaveHtml(string html)
        {
            using (SaveFileDialog d = new SaveFileDialog()
            {
                AddExtension = true,
                DefaultExt = "html",
                FileName = Program.MarkdownPath + ".html",
                OverwritePrompt = true,
                InitialDirectory = Path.GetDirectoryName(Program.MarkdownPath),
                Filter = "HTML files (*.html)|*.html|All files (*.*)|*.*",
                Title = "Save " + Path.GetFileName(Program.MarkdownPath) + " as..."
            })
            {
                //As this will be called from within the browser, it will be on another UI thread
                Program.MainForm.Invoke(new Action(() =>
                {
                    if (d.ShowDialog(Program.MainForm) == DialogResult.OK)
                    {
                        File.WriteAllText(d.FileName, html);
                        Program.MainForm.RunJavscript("alert('" +
                            d.FileName.Replace(@"\", @"\\") +
                            @"\nhas been successfully created.');");
                    }
                }));
            }
        }

        public bool SaveAsMarkdown()
        {
            bool success = false;
            using (SaveFileDialog d = new SaveFileDialog()
            {
                AddExtension = true,
                DefaultExt = "md",
                FileName = Program.MarkdownPath,
                OverwritePrompt = true,
                InitialDirectory = Path.GetDirectoryName(Program.MarkdownPath),
                Filter = "Markdown files (*.md)|*.md|All files (*.*)|*.*",
                Title = "Save " + Path.GetFileName(Program.MarkdownPath) + " as..."
            })
            {
                //As this will be called from within the browser, it will be on another UI thread
                Program.MainForm.Invoke(new Action(() =>
                {
                    if (d.ShowDialog(Program.MainForm) == DialogResult.OK)
                    {
                        File.WriteAllText(d.FileName, this.MarkdownText);
                        success = true;
                        Program.MainForm.RunJavscript("alert('" +
                            d.FileName.Replace(@"\", @"\\") +
                            @"\nhas been successfully created.');");
                    }
                }));
            }
            return success;
        }

        public void OpenMarkdown()
        {
            if (!this.CheckForChanges())
            {
                return;
            }
            using (OpenFileDialog d = new OpenFileDialog()
            {
                DefaultExt = "md",
                FileName = Program.MarkdownPath + ".html",
                InitialDirectory = Path.GetDirectoryName(Program.MarkdownPath),
                Filter = "Markdown files (*.md)|*.md|All files (*.*)|*.*",
                Title = "Open Markdown File"
            })
            {
                //As this will be called from within the browser, it will be on another UI thread
                Program.MainForm.Invoke(new Action(() =>
                {
                    if (d.ShowDialog() == DialogResult.OK)
                    {
                        Program.ChangeFile(d.FileName);
                    }
                }));
            }
        }

        public void ShowDevTools()
        {
            Program.MainForm.ShowDevTools();
        }

        public void ShowGuide()
        {
            var mdPath = Properties.Settings.Default.MarkdownGuideFile.Replace(
                "{{{RESOURCES_DIRECTORY}}}", Program.ResourcesDirectory);
            if (!File.Exists(mdPath))
            {
                mdPath = Properties.Settings.Default.MarkDownGuideURL;
            }
            Process.Start(mdPath);
        }

        public void PrintToPdf()
        {
            using (SaveFileDialog d = new SaveFileDialog()
            {
                AddExtension = true,
                DefaultExt = "pdf",
                FileName = Program.MarkdownPath + ".pdf",
                OverwritePrompt = true,
                InitialDirectory = Path.GetDirectoryName(Program.MarkdownPath),
                Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*",
                Title = "Print to PDF  - " + Path.GetFileName(Program.MarkdownPath)
            })
            {
                //As this will be called from within the browser, it will be on another UI thread
                Program.MainForm.Invoke(new Action(() =>
                {
                    if (d.ShowDialog(Program.MainForm) == DialogResult.OK)
                    {
                        Program.MainForm.PrintToPdf(d.FileName);
                        Program.MainForm.RunJavscript("alert('" +
                            d.FileName.Replace(@"\", @"\\") +
                            @"\nhas been successfully created.');");
                    }
                }));
            }
        }

        public void SetBrowserState(int state)
        {
            Program.BrowserState = (BrowserStates)state;
        }

        public void ToggleFind()
        {
            //As this will be called from within the browser, it will be on another UI thread
            Program.MainForm.Invoke(new Action(() =>
            {
                Program.MainForm.ToggleFind();
            }));
        }

        public string Version()
        {
            return System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();
        }

        public void DoKeyUp(int keyCode, bool control, bool shift, bool alt, string selectedText)
        {
            //As this will be called from within the browser, it will be on another UI thread
            Program.MainForm.Invoke(new Action(() =>
            {
                Program.MainForm.DoKeyUp((Keys)keyCode, control, shift, alt, selectedText);
            }));
        }

        /// <summary>
        /// Checks for changes to the markdown text. Returns false if the user chose "Cancel" otherwise returns true.
        /// </summary>
        /// <returns></returns>
        internal bool CheckForChanges()
        {
            bool situationDealtWith = true;
            if (this.HasUnsavedChanges())
            {
                var result = MessageBox.Show("There are unsaved changes. Do you wish to save your changes?", "MarkdownViewer", 
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                switch (result)
                {
                    case DialogResult.Cancel:
                        situationDealtWith = false;
                        break;
                    case DialogResult.Yes:                        
                        if(this.SaveMarkdown() || this.SaveAsMarkdown())
                        {
                            Program.MainForm.RunJavscript("alert('Your changes have been successfully saved.', 'success', 2);");
                        }
                        else
                        {
                            situationDealtWith = false;
                        }
                        break;
                }
            }

            return situationDealtWith;
        }

        private bool HasUnsavedChanges()
        {
            //If there is no file open, we will return false.
            bool hasChanges = false;
            if (!string.IsNullOrEmpty(this.MarkdownPath) && File.Exists(this.MarkdownPath))
            {
                hasChanges = (File.ReadAllText(this.MarkdownPath) != this.MarkdownText);                
            }
            return hasChanges;
        }
    }
}

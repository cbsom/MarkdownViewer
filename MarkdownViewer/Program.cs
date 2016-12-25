using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace MarkdownViewer
{
    enum BrowserStates
    {
        Previewing = 0,
        Editing = 1
    }

    class Program
    {
        internal static string ResourcesDirectory { get; private set; }

        internal static string MarkdownPath { get; private set; }

        internal static string MarkdownText { get; set; }

        internal static frmMain MainForm { get; private set; }

        internal static BrowserStates BrowserState { get; set; }

        [STAThread]
        static void Main(string[] args)
        {
            Program.Log(null, "Starting MarkdownViewer.exe " + string.Join(" ", args));
            if (args.Length > 0)
            {
                MarkdownPath = args[0];
            }
            ResourcesDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Resources");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            MainForm = new frmMain();
            BrowserState = BrowserStates.Previewing;
            if (!string.IsNullOrEmpty(MarkdownPath))
            {
                Initialize(MarkdownPath);
            }

            Application.Run(MainForm);
        }

        internal static string GetHtmlTemplate()
        {
            string templateFilePath = Properties.Settings.Default.HtmlTemplateFilePath.Replace(
                "{{{RESOURCES_DIRECTORY}}}", Program.ResourcesDirectory);
            string template;

            if (File.Exists(templateFilePath))
            {
                template = File.ReadAllText(templateFilePath);
            }
            else
            {
                template = Properties.Resources.HTML_TEMPLATE;
            }

            return template
                .Replace("'{{{SYNTAX_AUTOLOADER}}}'", SyntaxBrush.GetAutoLoaderDeclarationList());
        }

        internal static bool SaveMarkdown()
        {
            bool success = false;
            try
            {
                File.WriteAllText(MarkdownPath, MarkdownText);
                success = true;
            }
            catch (Exception ex)
            {
                Log(MarkdownPath, "Exception saving markdown file:\n\t{0}\n\t\t{1}", ex.Message, ex.StackTrace);
            }
            return success;
        }

        internal static void Log(string mdPath, string message, params object[] args)
        {
            Console.WriteLine("{0}\t[{1}]\t{2}",
                DateTime.Now.TimeOfDay.ToString(),
                (string.IsNullOrEmpty(mdPath) ? " ? " : Path.GetFileName(mdPath)),
                string.Format(message, args));
        }

        internal static void Initialize(string path)
        {
            MarkdownPath = path;
            try
            {
                Log(MarkdownPath, "Reading markdown file: {0}", MarkdownPath);
                MarkdownText = File.ReadAllText(MarkdownPath);
                MainForm.Text = MarkdownPath + " - Markdown Viewer";
            }
            catch (Exception ex)
            {
                Log(MarkdownPath, "Exception reading markdown file:\n\t{0}\n\t\t{1}", ex.Message, ex.StackTrace);
                return;
            }

            if (string.IsNullOrWhiteSpace(MarkdownText))
            {
                Program.Log(MarkdownPath, "Markdown file is empty");
                return;
            }
        }
    }
}

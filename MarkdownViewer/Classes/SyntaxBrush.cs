using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarkdownViewer
{
    internal class SyntaxBrush
    {
        private static List<SyntaxBrush> _syntaxBrushList = new List<MarkdownViewer.SyntaxBrush>(new SyntaxBrush[] {
                new SyntaxBrush(new string[] { "applescript" }, "shBrushAppleScript.js"),
                new SyntaxBrush(new string[] { "as3", "actionscript3" }, "shBrushAS3.js"),
                new SyntaxBrush(new string[] { "bash", "shell" }, "shBrushBash.js"),
                new SyntaxBrush(new string[] { "cf", "coldfusion" }, "shBrushColdFusion.js"),
                new SyntaxBrush(new string[] { "c-sharp", "csharp", "cs" }, "shBrushCSharp.js"),
                new SyntaxBrush(new string[] { "cpp", "c", "h" }, "shBrushCpp.js"),
                new SyntaxBrush(new string[] { "css" }, "shBrushCss.js"),
                new SyntaxBrush(new string[] { "delphi", "pas", "pascal" }, "shBrushDelphi.js"),
                new SyntaxBrush(new string[] { "diff", "patch" }, "shBrushDiff.js"),
                new SyntaxBrush(new string[] { "erl", "erlang" }, "shBrushErlang.js"),
                new SyntaxBrush(new string[] { "groovy" }, "shBrushGroovy.js"),
                new SyntaxBrush(new string[] { "js", "jscript", "javascript", "json" }, "shBrushJScript.js"),
                new SyntaxBrush(new string[] { "java" }, "shBrushJava.js"),
                new SyntaxBrush(new string[] { "jfx", "javafx" }, "shBrushJavaFX.js"),
                new SyntaxBrush(new string[] { "perl", "pl" }, "shBrushPerl.js"),
                new SyntaxBrush(new string[] { "php" }, "shBrushPhp.js"),
                new SyntaxBrush(new string[] { "plain", "text", "txt" }, "shBrushPlain.js"),
                new SyntaxBrush(new string[] { "ps", "powershell" }, "shBrushPowerShell.js"),
                new SyntaxBrush(new string[] { "py", "python" }, "shBrushPython.js"),
                new SyntaxBrush(new string[] { "rails", "ror", "ruby" }, "shBrushRuby.js"),
                new SyntaxBrush(new string[] { "scala" }, "shBrushScala.js"),
                new SyntaxBrush(new string[] { "sass", "scss" }, "shBrushSass.js"),
                new SyntaxBrush(new string[] { "sql" }, "shBrushSql.js"),
                new SyntaxBrush(new string[] { "vb", "vbnet" }, "shBrushVb.js"),
                new SyntaxBrush(new string[] { "xml", "xslt", "html", "xhtml", "htm", "asp", "aspx" }, "shBrushXml.js")
            });

        internal string[] FileAliases { get; private set; }
        internal string BrushFilePath { get; private set; }
        internal SyntaxBrush(string[] aliases, string fileName)
        {
            this.FileAliases = aliases;
            this.BrushFilePath = "file://" + System.IO.Path.Combine(Program.ResourcesDirectory, "shBrushes", fileName);
        }

        internal string GetSyntaxHighlighterAutoLoaderDeclaration()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\n\t\t\t[");
            foreach (string str in this.FileAliases)
            {
                sb.AppendFormat("'{0}',", str);
            }
            sb.AppendFormat("'{0}']", this.GetEscapedPath());
            return sb.ToString();
        }

        internal static bool DoesAliasHaveBrush(string alias)
        {
            return _syntaxBrushList.Any(sb => sb.FileAliases.Contains(alias));
        }

        internal static string GetAutoLoaderDeclarationList()
        {
            string autoLoader = "";
            foreach (var sh in _syntaxBrushList)
            {
                if (!string.IsNullOrEmpty(autoLoader))
                {
                    autoLoader += ",";
                }
                autoLoader += sh.GetSyntaxHighlighterAutoLoaderDeclaration();
            }
            return autoLoader;
        }

        /// <summary>
        /// Javascript strings don't have a way to prevent escaping a backslash
        /// </summary>
        /// <returns></returns>
        private string GetEscapedPath()
        {
            return this.BrushFilePath.Replace(@"\", @"\\").Replace("\"", "\\\"").Replace("'", "\'");
        }
    }
}

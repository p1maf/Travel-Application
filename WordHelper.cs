using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Word = Microsoft.Office.Interop.Word;
using System.IO;
namespace travel
{
    class WordHelper
    {
        private readonly FileInfo _fileInfo;

        public WordHelper(string fileName)
        {
            if (File.Exists(fileName))
            {
                _fileInfo = new FileInfo(fileName);
            }
            else
            {
                throw new ArgumentException("File not found");
            }
        }
        internal bool Process(Dictionary<string, string> items, string numberDogovor, string numberVosvrat, bool pechat)
        {
            Word.Application app = null;
            try
            {
                app = new Word.Application();
                Object file = _fileInfo.FullName;
                Object missing = Type.Missing;
                app.Documents.Open(file);

                foreach (var item in items)
                {
                    Word.Find find = app.Selection.Find;
                    find.Text = item.Key;
                    find.Replacement.Text = item.Value;

                    Object wrap = Word.WdFindWrap.wdFindContinue;
                    Object replace = Word.WdReplace.wdReplaceAll;

                    find.Execute(
                        FindText: Type.Missing,
                        MatchCase: false,
                        MatchWholeWord: false,
                        MatchWildcards: false,
                        MatchSoundsLike: false,
                        MatchAllWordForms: false,
                        Format: true,
                        Wrap: wrap,
                        ReplaceWith: missing,
                        Replace: replace
                        );
                }

                if(numberDogovor != "")
                {
                    if (!Directory.Exists(_fileInfo.DirectoryName + "\\Договоры"))
                    {
                        Directory.CreateDirectory(_fileInfo.DirectoryName + "\\Договоры");
                    }

                    Object newFileName = Path.Combine(_fileInfo.DirectoryName + "\\Договоры", $"Номер договора - {numberDogovor}");
                    app.ActiveDocument.SaveAs(newFileName);
                    if(pechat == true)
                    {
                        app.Visible = true;
                    }
                    else
                    {
                        app.ActiveDocument.Close();
                        app.Quit();
                    }
                }
                else
                {
                    if (!Directory.Exists(_fileInfo.DirectoryName + "\\Возврат"))
                    {
                        Directory.CreateDirectory(_fileInfo.DirectoryName + "\\Возврат");
                    }

                    Object newFileName = Path.Combine(_fileInfo.DirectoryName + "\\Возврат", $"Номер тура - {numberVosvrat}");
                    app.ActiveDocument.SaveAs(newFileName);
                    app.ActiveDocument.Close();
                    app.Quit();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }
    }
}

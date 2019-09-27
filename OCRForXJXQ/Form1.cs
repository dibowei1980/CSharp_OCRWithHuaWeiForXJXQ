using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OCRForXJXQ
{
    using System.Net;
    using System.IO;
    using System.Xml;
    using Newtonsoft.Json.Serialization;
    using Newtonsoft.Json;

    using NPOI;
    using NPOI.HWPF.UserModel;
    using NPOI.HWPF.Model;
    using NPOI.HWPF;
    using NPOI.HWPF.Extractor;


    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void process_Click(object sender, EventArgs e)
        {
            var pdfPath = txt_PdfFolder.Text;
            if (!Directory.Exists(pdfPath))
            {
                MessageBox.Show(string.Format("目录{0}不存在", pdfPath));
                return;
            }
            var regextString = File.ReadAllLines(Environment.s_filterFileName, Encoding.GetEncoding("GBK"))[0];
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(regextString);
            string dh;
            var dict = getFileDict(Path.Combine(pdfPath, "目录.doc"), out dh);
            var pdfFiles = Directory.GetFiles(pdfPath, "*.pdf", SearchOption.AllDirectories);
            foreach (var pdf in pdfFiles)
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pdf);
                string tableName = fileNameWithoutExtension;
                if (dict.ContainsKey(tableName))
                    tableName = dict[tableName];
                if (regex.IsMatch(tableName))
                {
                    var imgs = ConvertPdf2Image.Convert(pdf);
                    var index = 1;
                    foreach (var img in imgs)
                    {
                        using (var stream = new MemoryStream())
                        {
                            img.Save(stream, img.RawFormat);
                            var bytes = new Byte[stream.Length];
                            stream.Read(bytes, 0, bytes.Length);
                            var imgBase64 = Convert.ToBase64String(bytes);
                            var jsonString = OCRParser.GetTableJsonStringByBase64(imgBase64);
                            string jsonFileName = Path.Combine(pdfPath, string.Format("{0}_{1}", dh, tableName));
                            if (imgs.Count > 1)
                                jsonFileName += "_" + index++;
                            jsonFileName += "_json.txt";
                        }
                        img.Dispose();
                    }
                }
            }
        }


        private Dictionary<string, string> getFileDict(string docFileName, out string dh)
        {
            dh = "";
            var dict = new Dictionary<string, string>();
            if (File.Exists(docFileName))
                using (Stream stream = new FileStream(docFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    HWPFDocument hd = new HWPFDocument(stream);
                    var paraTable = hd.ParagraphTable;
                    Range rang = hd.GetRange();
                    int paragraphCount = rang.NumParagraphs;
                    for (int i = 0; i < paragraphCount; i++)
                    {
                        var pph = rang.GetParagraph(i);
                        var text = pph.Text.Replace(":", "：").Replace(" ", "").Trim();
                        if (text.StartsWith("档号："))
                        {
                            dh = text.Replace("档号：", "").Trim();
                            break;
                        }
                    }
                    rang = hd.GetRange();
                    TableIterator it = new TableIterator(rang);
                    while (it.HasNext())
                    {
                        NPOI.HWPF.UserModel.Table tb = (NPOI.HWPF.UserModel.Table)it.Next();
                        for (int i = 0; i < tb.NumRows; i++)
                        {
                            var row = tb.GetRow(i);
                            var cellCount = row.numCells();
                            if (cellCount > 1)
                            {
                                var key = row.GetCell(0).Text.Trim().Replace("\a", "");
                                var value = row.GetCell(1).Text.Trim().Replace("\a", "");
                                if (!dict.ContainsKey(key))
                                    dict.Add(key, value);
                            }
                        }
                    }
                }
            return dict;
        }



        private void btn_ParseJson_Click(object sender, EventArgs e)
        {
            string jsonString = File.ReadAllText("c:\\test_json.txt", Encoding.UTF8);
            OTable table = JsonConvert.DeserializeObject<OTable>(jsonString);
            OCRTable oCRTable = new OCRTable(table);
            var json = JsonConvert.SerializeObject(oCRTable);
        }

        private void btn_PDF2IMG_Click(object sender, EventArgs e)
        {
            string pdfFileName = @"C:\Users\Administrator\Desktop\test\OCRTestData\0001(1)\0001\0001.pdf";
            ConvertPdf2Image.Convert(pdfFileName, definition: ConvertPdf2Image.Definition.Four);
        }


        private void tbn_SelectFolder_Click(object sender, EventArgs e)
        {
            var dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            txt_PdfFolder.Text = dlg.SelectedPath;
        }
    }
}

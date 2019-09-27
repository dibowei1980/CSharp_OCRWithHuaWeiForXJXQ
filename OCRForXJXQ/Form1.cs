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
            //读取system目录下filter.txt文件中定义的PDF文件转换过滤正则表达式，只有与之匹配的文件才会被转换
            var regextString = File.ReadAllLines(Environment.s_filterFileName, Encoding.GetEncoding("GBK"))[0];
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(regextString);
            //读取system目录下的replaceString.txt文件中定义的校正文本，形成字典对解析的文本进行校正
            var tmpStr = File.ReadAllLines(Environment.s_replaceFileName);
            var adjustStrDict = new Dictionary<string, string>();
            foreach (var strLine in tmpStr)
            {
                var ary = strLine.Split(',');
                if (ary.Length > 1)
                {
                    var key = ary[0].Trim();
                    var value = ary[1].Trim();
                    if (!adjustStrDict.ContainsKey(key))
                        adjustStrDict.Add(key, value);
                }
            }
            string dh;
            var dict = getFileDict(Path.Combine(pdfPath, "目录.doc"), out dh);
            var pdfFiles = Directory.GetFiles(pdfPath, "*.pdf", SearchOption.AllDirectories);
            foreach (var pdf in pdfFiles)
            {
                string originName = Path.GetFileNameWithoutExtension(pdf);
                string tableName = originName;
                if (dict.ContainsKey(tableName))
                    tableName = dict[tableName];
                if (regex.IsMatch(tableName))
                {
                    //将PDF转为图片列表
                    var imgs = ConvertPdf2Image.Convert(pdf, definition: ConvertPdf2Image.Definition.Four);
                    var index = 1;
                    foreach (var img in imgs)
                    {
                        var imgBase64 = "";
                        //将图片转为base64字串，以便传递给华为文字识别API
                        using (var stream = new MemoryStream())
                        {
                            img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                            var bytes = stream.ToArray();
                            imgBase64 = Convert.ToBase64String(bytes);
                        }
                        img.Dispose();
                        //调用华为OCR接口返回JSON解析串
                        var jsonString = OCRParser.GetTableJsonStringByBase64(imgBase64);
                        //反序列化为OTable对象
                        OTable table = JsonConvert.DeserializeObject<OTable>(jsonString);
                        OCRTable oCRTable = new OCRTable(table);
                        //进行解析文本校正
                        oCRTable.AdjustStringByDict(adjustStrDict);
                        //var json = JsonConvert.SerializeObject(oCRTable);
                        var ocrTableString = oCRTable.ToString();
                        //构造文本文件名
                        string txtFileName = Path.Combine(pdfPath, string.Format("{0}_{1}{2}", dh, originName, tableName));
                        if (imgs.Count > 1)
                            txtFileName += "_" + index++;
                        txtFileName += ".txt";
                        File.WriteAllText(txtFileName, ocrTableString);
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


        private void tbn_SelectFolder_Click(object sender, EventArgs e)
        {
            var dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            txt_PdfFolder.Text = dlg.SelectedPath;
        }
    }
}

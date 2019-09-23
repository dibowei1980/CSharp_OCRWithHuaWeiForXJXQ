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
    public partial class Form1 : Form
    {
        string m_requestTokenFileName = "";
        string m_systemPath = "";
        string m_tokenFileName = "";
        public Form1()
        {
            InitializeComponent();
            m_systemPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "..\\system");
            m_requestTokenFileName = Path.Combine(m_systemPath, "requestTokenString.txt");
            m_tokenFileName = Path.Combine(m_systemPath, "token.txt");
        }

        private void process_Click(object sender, EventArgs e)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            var str = "";
            if (File.Exists(m_tokenFileName))
                str = File.ReadAllText(m_tokenFileName);
            var ary = str.Split(':');
            string token = "";
            if (ary[0] == string.Format("{0:yyyyMMdd}", DateTime.Now) && ary.Length > 1)
                token = ary[1];
            else
            {
                string token_jsonParam = File.ReadAllText(m_requestTokenFileName, Encoding.UTF8);
                var token_url = "https://iam.cn-north-4.myhuaweicloud.com/v3/auth/tokens";
                var token_request = (HttpWebRequest)WebRequest.Create(token_url);
                token_request.Method = "POST";
                token_request.ContentType = "application/json;charset=UTF-8";
                byte[] byteData_tokenrequest = Encoding.UTF8.GetBytes(token_jsonParam);
                int tlength = byteData_tokenrequest.Length;
                token_request.ContentLength = tlength;
                var tWriter = token_request.GetRequestStream();
                tWriter.Write(byteData_tokenrequest, 0, tlength);
                tWriter.Close();
                using (var tResponse = (HttpWebResponse)token_request.GetResponse())
                {
                    var tHeader = tResponse.Headers;
                    for (int i = 0; i < tHeader.Keys.Count; i++)
                    {
                        if (tHeader.Keys[i] == "X-Subject-Token")
                        {
                            token = tHeader["X-Subject-Token"];
                            break;
                        }
                    }
                    File.WriteAllText(m_tokenFileName, string.Format("{0:yyyyMMdd}:{1}", DateTime.Now, token), Encoding.UTF8);
                }
            }
            if (token == "")
            {
                MessageBox.Show("token获取失败.");
                return;
            }
            string _url = "https://ocr.cn-north-4.myhuaweicloud.com/v1.0/ocr/general-table";
            var request = (HttpWebRequest)WebRequest.Create(_url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers.Add("X-Auth-Token", token.Replace("\n", "").Replace("\r", ""));
            string imgFileName = @"C:\Users\dibowei1980\Desktop\新建文件夹\0001\0001.jpg";
            var base64 = Convert.ToBase64String(System.IO.File.ReadAllBytes(imgFileName));
            var jsonParam = string.Format("{1}\"image\" : \"{0}\",\n\"return_confidence\" : false{2}", base64, "{", "}");
            byte[] byteData = Encoding.UTF8.GetBytes(jsonParam);
            int length = byteData.Length;
            request.ContentLength = length;
            Stream writer = request.GetRequestStream();
            writer.Write(byteData, 0, length);
            writer.Close();
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                var responseString = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8")).ReadToEnd();
                MessageBox.Show(responseString.ToString());
            }
        }
    }
}

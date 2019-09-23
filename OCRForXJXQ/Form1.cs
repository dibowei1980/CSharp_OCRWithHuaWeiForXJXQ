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
    public partial class Form1 : Form
    {
        string m_requestTokenFileName = "";
        string m_systemPath = "";
        string m_tokenFileName = "";
        public Form1()
        {
            InitializeComponent();
            m_systemPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "..\\..\\system");
            m_requestTokenFileName = Path.Combine(m_systemPath, "requestTokenString.txt");
            m_tokenFileName = Path.Combine(m_systemPath, "token.txt");
        }

        private void process_Click(object sender, EventArgs e)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            var str = "";
            if (File.Exists(m_tokenFileName))
                str = File.ReadAllText(m_tokenFileName);
            var ary = str.Split(';');
            string token = "";
            DateTime dt;
            if (DateTime.TryParse(ary[0], out dt) && dt > DateTime.Now)
                token = ary[1];
            else
            {
                string token_jsonParam = File.ReadAllText(m_requestTokenFileName, Encoding.UTF8);
                var token_url = "https://iam.myhuaweicloud.com/v3/auth/tokens";
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
                    using (var responseStream = tResponse.GetResponseStream())
                    using (var reader = new StreamReader(responseStream))
                    {
                        string report = reader.ReadToEnd();
                        var tree = JsonConvert.DeserializeXmlNode(report).ChildNodes;

                        File.WriteAllText(m_tokenFileName, tree[0].ChildNodes[0].ChildNodes[0].Value + ";" + token, Encoding.UTF8);
                    }
                }
            }
            if (token == "")
            {
                MessageBox.Show("token获取失败.");
                return;
            }
            string _url = "https://ocr.cn-north-1.myhuaweicloud.com/v1.0/ocr/general-table";
            var request = (HttpWebRequest)WebRequest.Create(_url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers.Add("X-Auth-Token", token);
            string imgFileName = Path.Combine(m_systemPath, "test\\0001.png");
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

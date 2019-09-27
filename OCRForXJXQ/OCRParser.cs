using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCRForXJXQ
{
    using System.IO;
    using System.Net;
    using Newtonsoft.Json.Serialization;
    using Newtonsoft.Json;
    public static class OCRParser
    {
        public static string GetToken()
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            var str = "";
            if (File.Exists(Environment.s_tokenFileName))
                str = File.ReadAllText(Environment.s_tokenFileName);
            var ary = str.Split(';');
            string token = "";
            DateTime dt;
            DateTime.TryParse(ary[0], out dt);
            if (dt > DateTime.Now)
                token = ary[1];
            else
            {
                string token_jsonParam = File.ReadAllText(Environment.s_requestTokenFileName, Encoding.UTF8);
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

                        File.WriteAllText(Environment.s_tokenFileName, tree[0].ChildNodes[0].ChildNodes[0].Value + ";" + token, Encoding.UTF8);
                    }
                }
            }
            if (token == "")
            {
                throw new Exception("token获取失败.");
            }
            return token;
        }


        public static string GetTableJsonStringByFile(string image)
        {
            var base64 = Convert.ToBase64String(System.IO.File.ReadAllBytes(image));
            return GetTableJsonStringByBase64(base64);
        }

        public static string GetTableJsonStringByBase64(string imgBase64)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            var token = GetToken();
            string _url = "https://ocr.cn-north-1.myhuaweicloud.com/v1.0/ocr/general-table";
            var request = (HttpWebRequest)WebRequest.Create(_url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers.Add("X-Auth-Token", token);
            var jsonParam = string.Format("{1}\"image\" : \"{0}\",\n\"return_confidence\" : false{2}", imgBase64, "{", "}");
            byte[] byteData = Encoding.UTF8.GetBytes(jsonParam);
            int length = byteData.Length;
            request.ContentLength = length;
            Stream writer = request.GetRequestStream();
            writer.Write(byteData, 0, length);
            writer.Close();
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                var responseString = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8")).ReadToEnd();
                return responseString;
            }
        }
    }
}

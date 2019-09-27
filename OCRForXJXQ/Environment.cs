using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCRForXJXQ
{
    using System.IO;
    public static class Environment
    {
        public static  string s_requestTokenFileName = "";
        public static string s_systemPath = "";
        public static string s_tokenFileName = "";
        public static string s_filterFileName = "";
        public static string s_replaceFileName = "";
        static Environment()
        {
            s_systemPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "..\\..\\system");
            s_requestTokenFileName = Path.Combine(s_systemPath, "requestTokenString.txt");
            s_tokenFileName = Path.Combine(s_systemPath, "token.txt");
            s_filterFileName = Path.Combine(s_systemPath, "filter.txt");
            s_replaceFileName = Path.Combine(s_systemPath, "replaceString.txt");
            
        }

    }
}

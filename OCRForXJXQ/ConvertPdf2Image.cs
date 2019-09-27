using System;
using System.Configuration;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace OCRForXJXQ
{
    using O2S.Components.PDFRender4NET;
    public class ConvertPdf2Image
    {
        public enum Definition
        {
            One = 1, Two = 2, Three = 3, Four = 4, Five = 5, Six = 6, Seven = 7, Eight = 8, Nine = 9, Ten = 10
        }

        /// <summary>
        /// 将PDF文档转换为图片的方法
        /// </summary>
        /// <param name="pdfInputPath">PDF文件路径</param>
        /// <param name="startPageNum">从PDF文档的第几页开始转换</param>
        /// <param name="endPageNum">从PDF文档的第几页开始停止转换</param>
        /// <param name="definition">设置图片的清晰度，数字越大越清晰</param>
        public static List<Bitmap> Convert(string pdfInputPath, int startPageNum = -1, int endPageNum = -1, Definition definition = Definition.Ten)
        {
            using (PDFFile pdfFile = PDFFile.Open(pdfInputPath))
            {
                if (startPageNum <= 1)
                    startPageNum = 1;
                // validate pageNum
                if (startPageNum <= 0)
                {
                    startPageNum = 1;
                }

                if (endPageNum > pdfFile.PageCount || endPageNum < 1)
                    endPageNum = pdfFile.PageCount;

                if (startPageNum > endPageNum)
                {
                    int tempPageNum = startPageNum;
                    startPageNum = endPageNum;
                    endPageNum = startPageNum;
                }

                List<Bitmap> bmps = new List<Bitmap>();
                // start to convert each page
                for (int i = startPageNum; i <= endPageNum; i++)
                {
                    Bitmap pageImage = pdfFile.GetPageImage(i - 1, 56 * (int)definition);
                    bmps.Add(pageImage);
                }
                return bmps;
            }
        }
    }
}

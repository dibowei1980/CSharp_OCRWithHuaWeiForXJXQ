using System;
using System.Configuration;
using System.Drawing;
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
        /// <param name="imageOutputPath">图片输出路径</param>
        /// <param name="imageName">生成图片的名字</param>
        /// <param name="startPageNum">从PDF文档的第几页开始转换</param>
        /// <param name="endPageNum">从PDF文档的第几页开始停止转换</param>
        /// <param name="imageFormat">设置所需图片格式</param>
        /// <param name="definition">设置图片的清晰度，数字越大越清晰</param>
        public static void Convert(string pdfInputPath, string imageOutputPath = "",
            string imageName = "", int startPageNum = -1, int endPageNum = -1, ImageFormat imageFormat = null, Definition definition = Definition.Ten)
        {
            PDFFile pdfFile = PDFFile.Open(pdfInputPath);
            if (imageOutputPath == "")
                imageOutputPath = Path.GetDirectoryName(pdfInputPath);
            if (imageName == "")
                imageName = Path.GetFileNameWithoutExtension(pdfInputPath);
            if (!Directory.Exists(imageOutputPath))
                Directory.CreateDirectory(imageOutputPath);
            if (imageFormat == null)
                imageFormat = ImageFormat.Png;
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

            // start to convert each page
            for (int i = startPageNum; i <= endPageNum; i++)
            {
                Bitmap pageImage = pdfFile.GetPageImage(i - 1, 56 * (int)definition);
                pageImage.Save(Path.Combine(imageOutputPath, imageName + "_" + i.ToString() + "." + imageFormat.ToString()), imageFormat);
                pageImage.Dispose();
            }
            pdfFile.Dispose();
        }
    }
}

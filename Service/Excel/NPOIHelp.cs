using Domain;
using Infrastructure;
using NPOI.HSSF.Record.Crypto;
using NPOI.HSSF.UserModel;
using NPOI.OpenXml4Net.OPC;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.POIFS.Crypt;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.Util;
using NPOI.XSSF.Streaming;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Service
{
    public class NPOIHelp
    {
        public static void BeginGenerate<T>(string filepath, IQueryable<T> query, int totalNumber, List<Diction> dictions, string sheetName, Action<List<T>> action = null)
        {
            SXSSFWorkbook workbook = new SXSSFWorkbook();
            ISheet sheet = workbook.CreateSheet(sheetName);
            //Biff8EncryptionKey.CurrentUserPassword = "111111";
            //sheet.ProtectSheet("password");
            IRow rows = null;
            ICell cell = null;
            List<T> record2 = new List<T>();
            object propertyInfo = null;
            Type type;
            int columnWidth = 0;
            int length = 0;
            //2.默认样式
            XSSFCellStyle style = (XSSFCellStyle)workbook.CreateCellStyle();//创建样式对象
            XSSFFont font = (XSSFFont)workbook.CreateFont(); //创建一个字体样式对象
            //字体默认双居中
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            //字体默认宋体，大小11，
            font.FontName = "宋体"; //和excel里面的字体对应
            font.FontHeightInPoints = 11;//字体大小
            style.SetFont(font);
            if (dictions.Any())//判断是否有表头
            {
                //设置第二行表头
                rows = sheet.CreateRow(0);
                for (int i = 0; i < dictions.Count; i++)
                {
                    cell = rows.CreateCell(i);
                    cell.SetCellValue(dictions[i].Value);
                    cell.CellStyle = style;
                    columnWidth = sheet.GetColumnWidth(i) / 256;
                    length = Encoding.Default.GetBytes(cell.ToString()).Length;
                    if (columnWidth < length + 1)
                    {
                        columnWidth = length + 1;
                    }
                    sheet.SetColumnWidth(i, columnWidth * 256);
                }
            }
            for (int j = 1; j < 2 + totalNumber / 2000; j++)
            {
                record2 = query.Skip((j - 1) * 1 * 2000).Take(2000).ToList();
                action?.Invoke(record2);
                for (int i = 0; i < record2.Count; i++)
                {
                    type = record2[i].GetType();
                    rows = sheet.CreateRow(sheet.LastRowNum + 1);
                    for (int k = 0; k < dictions.Count; k++)
                    {
                        cell = rows.CreateCell(k);
                        if (!string.IsNullOrEmpty(dictions[k].Key))
                        {
                            propertyInfo = type.GetProperty(dictions[k].Key).GetValue(record2[i]);
                            cell.SetCellValue(propertyInfo == null ? "" : propertyInfo.ToString());
                        }
                        else
                        {
                            cell.SetCellValue("");
                        }
                        cell.CellStyle = style;
                    }
                    //每一行最后一个元素的高度
                    length = Encoding.UTF8.GetBytes(cell.ToString()).Length;
                    rows.HeightInPoints = 20 * (length / 60 + 1);
                }
            }

            using (var os = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                workbook.Write(os);
                workbook.Dispose();
                //POIFSFileSystem fs = new POIFSFileSystem();
                //EncryptionInfo info = new EncryptionInfo(EncryptionMode.Agile);
                //info.Encryptor.ConfirmPassword("123456");
                //OPCPackage opc = OPCPackage.Open(filepath, PackageAccess.READ_WRITE);
                //OutputStream abc = info.Encryptor.GetDataStream(fs);
                //opc.Save(abc);
                //fs.WriteFileSystem(os);
                os.Close();
            }
        }

        /// <summary>
        /// 需要查询的语句，查询语句总条数，标题，表名，
        /// </summary>
        /// <param name="query"></param>
        /// <param name="totalNumber"></param>
        /// <param name="dictions"></param>
        /// <param name="sheetName"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static byte[] OutputExcelNew2<T>(IQueryable<T> query, int totalNumber, List<Diction> dictions, string sheetName, Action<List<T>> action = null)
        {
            //1.创建相关对象（数据）
            SXSSFWorkbook workbook = new SXSSFWorkbook();
            ISheet sheet = workbook.CreateSheet(sheetName);
            IRow rows = null;
            ICell cell = null;
            List<T> record2 = new List<T>();
            object propertyInfo = null;
            Type type;
            int columnWidth = 0;
            int length = 0;
            //2.默认样式
            XSSFCellStyle style = (XSSFCellStyle)workbook.CreateCellStyle();//创建样式对象
            XSSFFont font = (XSSFFont)workbook.CreateFont(); //创建一个字体样式对象
            //字体默认双居中
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            //字体默认宋体，大小11，
            font.FontName = "宋体"; //和excel里面的字体对应
            font.FontHeightInPoints = 11;//字体大小
            style.SetFont(font);
            if (dictions.Any())//判断是否有表头
            {
                //设置第二行表头
                rows = sheet.CreateRow(0);
                for (int i = 0; i < dictions.Count; i++)
                {
                    cell = rows.CreateCell(i);
                    cell.SetCellValue(dictions[i].Value);
                    cell.CellStyle = style;
                    columnWidth = sheet.GetColumnWidth(i) / 256;
                    length = Encoding.Default.GetBytes(cell.ToString()).Length;
                    if (columnWidth < length + 1)
                    {
                        columnWidth = length + 1;
                    }
                    sheet.SetColumnWidth(i, columnWidth * 256);
                }
            }
            for (int j = 1; j < 2 + totalNumber / 2000; j++)
            {
                record2 = query.Skip((j - 1) * 1 * 2000).Take(2000).ToList();
                action?.Invoke(record2);
                for (int i = 0; i < record2.Count; i++)
                {
                    type = record2[i].GetType();
                    rows = sheet.CreateRow(sheet.LastRowNum + 1);
                    for (int k = 0; k < dictions.Count; k++)
                    {
                        cell = rows.CreateCell(k);
                        if (!string.IsNullOrEmpty(dictions[k].Key))
                        {
                            propertyInfo = type.GetProperty(dictions[k].Key).GetValue(record2[i]);
                            cell.SetCellValue(propertyInfo == null ? "" : propertyInfo.ToString());
                        }
                        else
                        {
                            cell.SetCellValue("");
                        }
                        cell.CellStyle = style;
                    }
                    //每一行最后一个元素的高度
                    length = Encoding.UTF8.GetBytes(cell.ToString()).Length;
                    rows.HeightInPoints = 20 * (length / 60 + 1);
                }
            }
            byte[] buffer = new byte[1024 * 2];
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                buffer = ms.GetBuffer();
                ms.Close();
                workbook.Dispose();
            }
            return buffer;
        }
    }
}
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.Streaming;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Service
{
    public class NPOIHelpNew
    {
        public const string excleContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        #region 私有方法
        private static XSSFCellStyle GetDefaultStyle(SXSSFWorkbook workbook)
        {
            XSSFCellStyle style = (XSSFCellStyle)workbook.CreateCellStyle();//创建样式对象
            XSSFFont font = (XSSFFont)workbook.CreateFont(); //创建一个字体样式对象
            //字体默认双居中
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            //字体默认宋体，大小11，
            font.FontName = "宋体"; //和excel里面的字体对应
            font.FontHeightInPoints = 11;//字体大小
            style.SetFont(font);
            return style;
        }
        private static int SetDefultHeader(ISheet sheet, IRow rows, ICell cell, List<TitleTree> header, XSSFCellStyle style, int nowRow)
        {
            return SetDefultFooter(sheet, rows, cell, header, style, nowRow);
        }
        private static int SetDefultFooter(ISheet sheet, IRow rows, ICell cell, List<TitleTree> footer, XSSFCellStyle style, int nowRow)
        {
            if (footer.Any())
            {
                int max = 0;
                nowRow = nowRow + 1;
                foreach (var i in footer)
                {
                    try
                    {
                        rows = sheet.CreateRow(nowRow + i.RowStart);
                    }
                    catch (Exception)
                    {
                        rows = sheet.GetRow(nowRow + i.RowStart);
                    }
                    try
                    {
                        cell = rows.CreateCell(i.ColumnStart);
                    }
                    catch (Exception)
                    {
                        cell = rows.GetCell(i.ColumnStart);
                    }
                    cell.SetCellValue(i.Value);
                    cell.CellStyle = style;
                    sheet.AddMergedRegion(new CellRangeAddress(nowRow + i.RowStart, nowRow + i.RowEnd, i.ColumnStart, i.ColumnEnd));
                    if (i.RowEnd > max)
                    {
                        max = i.RowEnd;
                    }
                }
                return max + nowRow;
            }
            return nowRow;
        }
        private static int SetDefultTitle(ISheet sheet, IRow rows, ICell cell, List<string> titleName, XSSFCellStyle style, int nowRow)
        {
            int columnWidth = 0, length = 0;
            nowRow++;
            rows = sheet.CreateRow(nowRow);
            for (int i = 0; i < titleName.Count; i++)
            {
                cell = rows.CreateCell(i);
                cell.SetCellValue(titleName[i]);
                cell.CellStyle = style;
                columnWidth = sheet.GetColumnWidth(i) / 256;
                length = Encoding.Default.GetBytes(cell.ToString()).Length;
                if (columnWidth < length + 1)
                {
                    columnWidth = length + 1;
                }
                sheet.SetColumnWidth(i, columnWidth * 256);
            }
            length = Encoding.UTF8.GetBytes(cell.ToString()).Length;
            rows.HeightInPoints = 20 * (length / 60 + 1);
            return nowRow;
        }
        private static int SetDefultData<T>(ISheet sheet, IRow rows, ICell cell, List<string> titleValue, XSSFCellStyle style, List<T> Data, int nowRow)
        {
            object propertyInfo = null;
            Type type;
            int length = 0;
            for (int i = 0; i < Data.Count; i++)
            {
                type = Data[i].GetType();
                nowRow++;
                rows = sheet.CreateRow(nowRow);
                for (int k = 0; k < titleValue.Count; k++)
                {
                    cell = rows.CreateCell(k);
                    if (!string.IsNullOrEmpty(titleValue[k]))
                    {
                        propertyInfo = type.GetProperty(titleValue[k]).GetValue(Data[i]);
                        cell.SetCellValue(propertyInfo == null ? "" : propertyInfo.ToString());
                    }
                    else
                    {
                        cell.SetCellValue("");
                    }
                    cell.CellStyle = style;
                }
                length = Encoding.UTF8.GetBytes(cell.ToString()).Length;
                rows.HeightInPoints = 20 * (length / 60 + 1);
            }
            return nowRow;
        }
        private static void SetDefultSheet<T>(SXSSFWorkbook workbook, string sheetName, List<TitleTree> header, List<string> titleName, List<string> titleValue, IQueryable<T> query, List<TitleTree> footer, Action<List<T>> action, XSSFCellStyle style)
        {
            var listCount = query.Count();
            if (listCount <= 5000)
            {
                SetDefultSheet(workbook, sheetName, header, titleName, titleValue, query.ToList(), footer, action, style);
            }
            else
            {
                ISheet sheet = workbook.CreateSheet(sheetName);
                IRow rows = null;
                ICell cell = null;
                List<T> list = new List<T>();
                int nowRow = -1;
                if (header.Any())
                {
                    nowRow = SetDefultHeader(sheet, rows, cell, footer, style, nowRow);
                }
                if (titleName.Any())
                {
                    nowRow = SetDefultTitle(sheet, rows, cell, titleName, style, nowRow);
                }
                if (query.Any() && titleValue.Any())
                {
                    for (int j = 1; j < 2 + listCount / 3000; j++)
                    {
                        list = query.Skip((j - 1) * 1 * 3000).Take(3000).ToList();
                        action?.Invoke(list);
                        nowRow = SetDefultData(sheet, rows, cell, titleValue, style, list, nowRow);
                    }
                }
                if (footer.Any())
                {
                    nowRow = SetDefultFooter(sheet, rows, cell, header, style, nowRow);
                }
            }
        }
        private static void SetDefultSheet<T>(SXSSFWorkbook workbook, string sheetName, List<TitleTree> header, List<string> titleName, List<string> titleValue, List<T> query, List<TitleTree> footer, Action<List<T>> action, XSSFCellStyle style)
        {
            action?.Invoke(query);
            ISheet sheet = workbook.CreateSheet(sheetName);
            IRow rows = null;
            ICell cell = null;
            int nowRow = -1;
            if (header.Any())
            {
                nowRow = SetDefultHeader(sheet, rows, cell, footer, style, nowRow);
            }
            if (titleName.Any())
            {
                nowRow = SetDefultTitle(sheet, rows, cell, titleName, style, nowRow);
            }
            if (query.Any() && titleValue.Any())
            {
                nowRow = SetDefultData(sheet, rows, cell, titleValue, style, query, nowRow);
            }
            if (footer.Any())
            {
                nowRow = SetDefultFooter(sheet, rows, cell, header, style, nowRow);
            }
        }
        private static void SetFile(SXSSFWorkbook workbook, string filePath)
        {
            string path = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(path))
            {
                if (path != null)
                {
                    Directory.CreateDirectory(path);
                }
            }
            using (var os = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
            {
                workbook.Write(os);
                os.Close();
                workbook.Dispose();
            }
        }
        #endregion
        /// <summary>
        /// 同一个查询分多表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filepath"></param>
        /// <param name="sheetList"></param>
        public static void OutputExcel<T>(string filepath, List<Sheet<T>> sheetQuery = null, List<SheetList<T>> sheetList = null, string password = null)
        {
            //1.创建相关对象（数据）
            SXSSFWorkbook workbook = new SXSSFWorkbook();
            //2.默认样式
            XSSFCellStyle style = GetDefaultStyle(workbook);
            foreach (var i in sheetQuery)
            {
                SetDefultSheet(workbook, i.SheetName, i.Header, i.TitleName, i.TitleValue, i.Query, i.Footer, i.Action, style);
            }
            foreach (var i in sheetList)
            {
                SetDefultSheet(workbook, i.SheetName, i.Header, i.TitleName, i.TitleValue, i.Query, i.Footer, i.Action, style);
            }
            SetFile(workbook, filepath);
        }

        /// <summary>
        /// 大数据量文件导出
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filepath"></param>
        /// <param name="sheetName"></param>
        /// <param name="header"></param>
        /// <param name="titleName"></param>
        /// <param name="titleValue"></param>
        /// <param name="query"></param>
        /// <param name="footer"></param>
        /// <param name="action"></param>
        public static void OutputExcel<T>(string filepath, string sheetName, List<TitleTree> header, List<string> titleName, List<string> titleValue, IQueryable<T> query, List<TitleTree> footer, Action<List<T>> action)
        {
            //1.创建相关对象（数据）
            SXSSFWorkbook workbook = new SXSSFWorkbook();
            //2.默认样式
            XSSFCellStyle style = GetDefaultStyle(workbook);
            SetDefultSheet(workbook, sheetName, header, titleName, titleValue, query, footer, action, style);
            SetFile(workbook, filepath);
        }
        /// <summary>
        /// 小数据量文件导出
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filepath"></param>
        /// <param name="sheetName"></param>
        /// <param name="header"></param>
        /// <param name="titleName"></param>
        /// <param name="titleValue"></param>
        /// <param name="query"></param>
        /// <param name="footer"></param>
        /// <param name="action"></param>
        public static void OutputExcel<T>(string filepath, string sheetName, List<TitleTree> header, List<string> titleName, List<string> titleValue, List<T> query, List<TitleTree> footer, Action<List<T>> action)
        {
            //1.创建相关对象（数据）
            SXSSFWorkbook workbook = new SXSSFWorkbook();
            //2.默认样式
            XSSFCellStyle style = GetDefaultStyle(workbook);
            SetDefultSheet(workbook, sheetName, header, titleName, titleValue, query, footer, action, style);
            SetFile(workbook, filepath);
        }
        #region 文件导出重载

        public static void OutputExcel<T>(string filepath, string sheetName, List<string> titleName, List<string> titleValue, List<T> query, List<TitleTree> footer, Action<List<T>> action) => OutputExcel(filepath, sheetName, new List<TitleTree>(), titleName, titleValue, query, footer, action);
        public static void OutputExcel<T>(string filepath, string sheetName, List<string> titleName, List<string> titleValue, List<T> query, Action<List<T>> action) => OutputExcel(filepath, sheetName, new List<TitleTree>(), titleName, titleValue, query, new List<TitleTree>(), action);
        public static void OutputExcel<T>(string filepath, string sheetName, List<string> titleName, List<string> titleValue, List<T> query) => OutputExcel(filepath, sheetName, new List<TitleTree>(), titleName, titleValue, query, new List<TitleTree>(), null);
        public static void OutputExcel<T>(string filepath, string sheetName, List<string> titleValue, List<T> query) => OutputExcel(filepath, sheetName, new List<TitleTree>(), new List<string>(), titleValue, query, new List<TitleTree>(), null);


        public static void OutputExcel<T>(string filepath, string sheetName, List<string> titleName, List<string> titleValue, IQueryable<T> query, List<TitleTree> footer, Action<List<T>> action) => OutputExcel(filepath, sheetName, new List<TitleTree>(), titleName, titleValue, query, footer, action);
        public static void OutputExcel<T>(string filepath, string sheetName, List<string> titleName, List<string> titleValue, IQueryable<T> query, Action<List<T>> action) => OutputExcel(filepath, sheetName, new List<TitleTree>(), titleName, titleValue, query, new List<TitleTree>(), action);
        public static void OutputExcel<T>(string filepath, string sheetName, List<string> titleName, List<string> titleValue, IQueryable<T> query) => OutputExcel(filepath, sheetName, new List<TitleTree>(), titleName, titleValue, query, new List<TitleTree>(), null);
        public static void OutputExcel<T>(string filepath, string sheetName, List<string> titleValue, IQueryable<T> query) => OutputExcel(filepath, sheetName, new List<TitleTree>(), new List<string>(), titleValue, query, new List<TitleTree>(), null);
        #endregion

        /// <summary>
        /// 小数据量内存导出
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sheetName"></param>
        /// <param name="header"></param>
        /// <param name="titleName"></param>
        /// <param name="titleValue"></param>
        /// <param name="query"></param>
        /// <param name="footer"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static byte[] OutputExcelMemory<T>(string sheetName, List<TitleTree> header, List<string> titleName, List<string> titleValue, List<T> query, List<TitleTree> footer, Action<List<T>> action)
        {
            action?.Invoke(query);
            //1.创建相关对象（数据）
            SXSSFWorkbook workbook = new SXSSFWorkbook();
            //2.默认样式
            XSSFCellStyle style = GetDefaultStyle(workbook);
            SetDefultSheet(workbook, sheetName, header, titleName, titleValue, query, footer, action, style);
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

        #region 内存导出重载
        public static byte[] OutputExcelMemory<T>(string sheetName, List<string> titleName, List<string> titleValue, List<T> query, List<TitleTree> footer, Action<List<T>> action) => OutputExcelMemory(sheetName, new List<TitleTree>(), titleName, titleValue, query, footer, action);
        public static byte[] OutputExcelMemory<T>(string sheetName, List<string> titleName, List<string> titleValue, List<T> query, Action<List<T>> action) => OutputExcelMemory(sheetName, new List<TitleTree>(), titleName, titleValue, query, new List<TitleTree>(), action);
        public static byte[] OutputExcelMemory<T>(string sheetName, List<string> titleName, List<string> titleValue, List<T> query) => OutputExcelMemory(sheetName, new List<TitleTree>(), titleName, titleValue, query, new List<TitleTree>(), null);
        public static byte[] OutputExcelMemory<T>(string sheetName, List<string> titleValue, List<T> query) => OutputExcelMemory(sheetName, new List<TitleTree>(), null, titleValue, query, new List<TitleTree>(), null);
        #endregion
        /// <summary>
        /// 导出模板方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sheetName"></param>
        /// <param name="titleName"></param>
        /// <param name="header"></param>
        /// <param name="footer"></param>
        /// <returns></returns>
        public static byte[] OutputExcelMemory<T>(string sheetName, List<string> titleName, List<TitleTree> header = null, List<TitleTree> footer = null)
        {
            //1.创建相关对象（数据）
            SXSSFWorkbook workbook = new SXSSFWorkbook();
            //2.默认样式
            XSSFCellStyle style = GetDefaultStyle(workbook);
            ISheet sheet = workbook.CreateSheet(sheetName);
            IRow rows = null;
            ICell cell = null;
            int nowRow = 0;
            if (header.Any())
            {
                nowRow = SetDefultHeader(sheet, rows, cell, footer, style, nowRow);
            }
            nowRow = SetDefultTitle(sheet, rows, cell, titleName, style, nowRow);
            if (footer.Any())
            {
                nowRow = SetDefultFooter(sheet, rows, cell, header, style, nowRow);
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
    public class TitleTree
    {
        public TitleTree(string value, int rowStart, int rowEnd, int columnStart, int columnEnd)
        {
            Value = value;
            RowStart = rowStart;
            RowEnd = rowEnd;
            ColumnStart = columnStart;
            ColumnEnd = columnEnd;
        }
        /// <summary>
        /// 表头
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 开始行
        /// </summary>
        public int RowStart { get; set; }
        /// <summary>
        /// 结束行
        /// </summary>
        public int RowEnd { get; set; }
        /// <summary>
        /// 开始列
        /// </summary>
        public int ColumnStart { get; set; }
        /// <summary>
        /// 结束列
        /// </summary>
        public int ColumnEnd { get; set; }
    }
    public class Sheet<T>
    {
        public string SheetName { get; set; }
        public List<TitleTree> Header { get; set; }
        public List<string> TitleName { get; set; }
        public List<string> TitleValue { get; set; }
        public IQueryable<T> Query { get; set; }
        public List<TitleTree> Footer { get; set; }
        public Action<List<T>> Action { get; set; }
    }
    public class SheetList<T>
    {
        public string SheetName { get; set; }
        public List<TitleTree> Header { get; set; }
        public List<string> TitleName { get; set; }
        public List<string> TitleValue { get; set; }
        public List<T> Query { get; set; }
        public List<TitleTree> Footer { get; set; }
        public Action<List<T>> Action { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Domain;

namespace ManageApi
{
    /// <summary>
    /// excle相关帮助方法
    /// </summary>
    public class NPOIHelp
    {
        /// <summary>
        /// 导出EXCEL
        /// </summary>
        /// <param name="data">数据集合</param>
        /// <param name="titles">表头集合</param>
        /// <param name="name">sheet名称</param>
        /// <returns></returns>
        public static byte[] OutputExcel<T>(List<T> data, string name, List<string> titles =null)
        {

            //1.创建相关对象
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet(name);
            IRow titleRow = null;
            IRow rows = null;

            //2.设置表头
            if (titles != null && titles.Count > 0)
            {
                titleRow = sheet.CreateRow(0);
                for (int i = 0; i < titles.Count; i++)
                {
                    ICell cell = titleRow.CreateCell(i);
                    cell.SetCellValue(titles[i]);
                    //设置样式(可自行调整)
                    SetCellStyle(workbook, cell);
                }
            }
            //3.设置数据
            if (data != null && data.Count > 0)
            {
                foreach (var item in data)
                {
                    rows = sheet.CreateRow(data.IndexOf(item) + 1);
                    var j = 0;
                    foreach (var p in item.GetType().GetProperties())
                    {
                        rows.CreateCell(j).SetCellValue(p == null ? "" : p.GetValue(item).ToString());
                        j++;
                    }
                }
            }
            //1.设置宽度
            SetColumnWidth(sheet, titles.Count);

            byte[] buffer = new byte[1024 * 2];
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                buffer = ms.GetBuffer();
                ms.Close();
            }
            return buffer;
        }

        /// <summary>
        /// 设置字体样式
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="cell"></param>
        public static void SetCellStyle(IWorkbook workbook, ICell cell)
        {
            ICellStyle style = workbook.CreateCellStyle();//创建样式对象
            IFont font = workbook.CreateFont(); //创建一个字体样式对象
            //font.FontName = "方正舒体"; //和excel里面的字体对应
            font.FontHeightInPoints = 12;//字体大小
            style.SetFont(font); //将字体样式赋给样式对象
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            cell.CellStyle = style; //把样式赋给单元格
        }

        /// <summary>
        /// 设置自动宽度
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="count">总列</param>
        public static void SetColumnWidth(ISheet sheet, int count)
        {
            for (int i = 0; i <= count - 1; i++)
            {
                sheet.AutoSizeColumn(i);
            }
            for (int columnNum = 0; columnNum <= count - 1; columnNum++)
            {
                //当前列宽
                int columnWidth = sheet.GetColumnWidth(columnNum) / 256;
                //遍历每一行
                for (int rowNum = 0; rowNum <= sheet.LastRowNum; rowNum++)
                {
                    //获取当前行
                    IRow currentRow = sheet.GetRow(rowNum);
                    if (currentRow.GetCell(columnNum) != null)
                    {
                        ICell currentCell = currentRow.GetCell(columnNum);
                        int length = Encoding.Default.GetBytes(currentCell.ToString()).Length;
                        if (columnWidth < length)
                        {
                            columnWidth = length;
                        }
                    }
                }
                //设置当前列宽
                sheet.SetColumnWidth(columnNum, columnWidth * 256);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream1"></param>
        /// <returns></returns>
        public static List<ManageItem> Import(Stream stream1)
        {
            return null;
        }
        ///// <summary>
        ///// 解析excel=>SalaryData
        ///// </summary>
        ///// <param name="stream"></param>
        /////  <param name="projList"></param>
        ///// <returns></returns>
        //public static Tuple<bool, List<SalaryCalculateData>> ResolveTaskDataExcel(Stream stream, List<SOBSalaryProject> projList)
        //{
        //    var nameList = projList.Where(a => String.IsNullOrWhiteSpace(a.Formula)).OrderBy(a => a.SortNumber).Select(a => a.Name).ToList();
        //    XSSFWorkbook workBook = new XSSFWorkbook(stream);
        //    ISheet sheet = workBook.GetSheetAt(0);
        //    IRow row2 = sheet.GetRow(0);
        //    var res2 = row2.Select(a => a.ToString().Trim()).ToList();
        //    if (res2.Count != nameList.Count) return new Tuple<bool, List<SalaryCalculateData>>(false, null);
        //    for (int i = 0; i < res2.Count; i++)
        //    {
        //        if (res2[i] != nameList[i]) new Tuple<bool, List<SalaryCalculateData>>(false, null);
        //    }
        //    bool isFirstRow = true;
        //    int lastRowNum = sheet.LastRowNum;
        //    int idCardIndex = -1;
        //    List<SalaryCalculateData> res = new List<SalaryCalculateData>();
        //    Dictionary<int, int> mapper = new Dictionary<int, int>();
        //    for (int i = 0; i <= lastRowNum; i++)
        //    {
        //        SalaryCalculateData data = new SalaryCalculateData();
        //        if (sheet.GetRow(i) == null) continue;
        //        if (isFirstRow)
        //        {
        //            var idCardCol = sheet.GetRow(i).Where(a => a.StringCellValue == "身份证").FirstOrDefault();
        //            if (idCardCol == null) return new Tuple<bool, List<SalaryCalculateData>>(false, null);
        //            idCardIndex = idCardCol.ColumnIndex;
        //            for (int q = 0; q < sheet.GetRow(i).LastCellNum; q++)
        //            {
        //                var name = sheet.GetRow(i).GetCell(q).ToString().Trim();
        //                var value = projList.Where(a => a.Name == name).FirstOrDefault().SortNumber;
        //                mapper.Add(q, value);
        //            }
        //            isFirstRow = false;
        //        }
        //        else
        //        {
        //            IRow row = sheet.GetRow(i);
        //            if (row == null) continue;
        //            for (int j = 0; j < mapper.Count; j++)
        //            {
        //                if (j == idCardIndex)
        //                {
        //                    string str = row.GetCell(j) != null ? row.GetCell(j).ToString() : "";
        //                    str = str.Trim().ToLower();
        //                    data.IDCard = str;
        //                    data.IDCardSortNumber = mapper[j];
        //                    SetValueToData(data, mapper[j], str);
        //                }
        //                else
        //                {
        //                    var cell = row.GetCell(j);
        //                    if (cell == null) continue;
        //                    string str = cell != null ? cell.ToString() : "";
        //                    SetValueToData(data, mapper[j], str);
        //                }
        //            }
        //            if (!string.IsNullOrEmpty(data.IDCard)) res.Add(data);
        //        }
        //    }
        //    return new Tuple<bool, List<SalaryCalculateData>>(true, res); ;
        //}
    }
}

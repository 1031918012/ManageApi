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

        #region 考勤导入各种记录
        /// <summary>
        /// 导入加班记录
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static (List<ImportClockRecord>, string) ResolveOvertimeData(Stream stream)
        {
            List<ImportClockRecord> importClockRecords = new List<ImportClockRecord>();
            XSSFWorkbook workBook = new XSSFWorkbook(stream);
            ISheet sheet = workBook[0];//取第一张表
            IRow title = sheet.GetRow(0);
            StringBuilder titlestr = new StringBuilder();
            for (int i = 0; i < title.LastCellNum; i++)
            {
                titlestr.Append(title.Cells[i].StringCellValue);
            }
            if (titlestr.ToString() != "")
            {
                return (importClockRecords, "请导入正确的模板表");
            }
            for (int i = 1; i < sheet.LastRowNum; i++)
            {
                ImportClockRecord record = new ImportClockRecord();
                IRow data = sheet.GetRow(i);
                record.Name = data.Cells[0].StringCellValue;
                record.IdCard = data.Cells[1].StringCellValue;
                record.Clocktime = data.Cells[2].DateCellValue;
                record.Site = data.Cells[3].StringCellValue;
                importClockRecords.Add(record);
            }
            if (!importClockRecords.Any())
            {
                return (importClockRecords, "模板表中不存在有效数据");
            }
            return (importClockRecords, "");
        }
        /// <summary>
        /// 导入补卡记录
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static (List<ImportClockRecord>, string, List<ImportClockRecord>) ResolveRepairData(Stream stream)
        {
            List<ImportClockRecord> importClockRecords = new List<ImportClockRecord>();
            List<ImportClockRecord> errimportClockRecords = new List<ImportClockRecord>();
            XSSFWorkbook workBook = new XSSFWorkbook(stream);
            ISheet sheet = workBook[0];//取第一张表
            IRow title = sheet.GetRow(0);
            StringBuilder titlestr = new StringBuilder();
            for (int i = 0; i < title.LastCellNum; i++)
            {
                titlestr.Append(title.Cells[i].StringCellValue);
            }
            if (titlestr.ToString() != "姓名证照号码补卡时间补卡地址")
            {
                return (importClockRecords, "请导入正确的模板表", errimportClockRecords);
            }
            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                ImportClockRecord record = new ImportClockRecord();
                IRow data = sheet.GetRow(i);
                try
                {
                    if (string.IsNullOrEmpty(data.Cells[0].StringCellValue) || string.IsNullOrEmpty(data.Cells[1].StringCellValue.Removewhite()) || string.IsNullOrEmpty(data.Cells[3].StringCellValue.Removewhite()) || data.Cells[2].DateCellValue == new DateTime())
                    {
                        record.Name = data.Cells[0].StringCellValue;
                        record.IdCard = data.Cells[1].StringCellValue;
                        record.Clocktime = data.Cells[2].DateCellValue.Date;
                        record.Site = data.Cells[3].StringCellValue;
                        errimportClockRecords.Add(record);
                    }
                    else
                    {
                        record.Name = data.Cells[0].StringCellValue;
                        record.IdCard = data.Cells[1].StringCellValue.Removewhite().ToLower();
                        if (data.Cells[1].StringCellValue.Length < 18)
                        {
                            record.IdCard = data.Cells[1].StringCellValue.Removewhite();
                        }
                        record.Clocktime = data.Cells[2].DateCellValue;
                        record.Site = data.Cells[3].StringCellValue.Removewhite();
                        importClockRecords.Add(record);
                    }
                }
                catch (Exception)
                {
                    errimportClockRecords.Add(record);
                }
            }
            if (!importClockRecords.Any())
            {
                return (importClockRecords, "模板表中不存在有效数据", errimportClockRecords);
            }
            return (importClockRecords, "", errimportClockRecords);
        }
        /// <summary>
        /// 导入请假记录
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static (List<ImportLeaveRecord>, string, List<ImportLeaveRecord>) ResolveLeaveData(int type, Stream stream)
        {
            List<ImportLeaveRecord> importClockRecords = new List<ImportLeaveRecord>();
            List<ImportLeaveRecord> errimportClockRecords = new List<ImportLeaveRecord>();
            XSSFWorkbook workBook = new XSSFWorkbook(stream);
            ISheet sheet = workBook[0];//取第一张表
            IRow title = sheet.GetRow(0);
            StringBuilder titlestr = new StringBuilder();
            for (int i = 0; i < title.LastCellNum; i++)
            {
                titlestr.Append(title.Cells[i].StringCellValue);
            }
            if (type == 3 && titlestr.ToString() != "姓名证照号码开始时间结束时间请假类型")
            {
                return (importClockRecords, "请导入正确的模板表", errimportClockRecords);
            }
            if ((type == 2 || type == 1) && titlestr.ToString() != "姓名证照号码开始时间结束时间")
            {
                return (importClockRecords, "请导入正确的模板表", errimportClockRecords);
            }
            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                ImportLeaveRecord record = new ImportLeaveRecord();
                IRow data = sheet.GetRow(i);
                try
                {
                    if (string.IsNullOrEmpty(data.Cells[0].StringCellValue) || string.IsNullOrEmpty(data.Cells[1].StringCellValue) || data.Cells[2].DateCellValue == new DateTime() || data.Cells[3].DateCellValue == new DateTime())
                    {
                        record.Name = data.Cells[0].StringCellValue;
                        record.IdCard = data.Cells[1].StringCellValue.Removewhite().ToLower();
                        record.StartTime = data.Cells[2].DateCellValue;
                        record.EndTime = data.Cells[3].DateCellValue;
                        if (type == 3)
                        {
                            record.LeaveType = data.Cells[4].StringCellValue.Removewhite().ToLower();
                        }
                        if (type == 1)
                        {
                            record.LeaveType = "外出";
                        }
                        if (type == 2)
                        {
                            record.LeaveType = "出差";
                        }
                        errimportClockRecords.Add(record);
                    }
                    else
                    {
                        record.Name = data.Cells[0].StringCellValue;
                        record.IdCard = data.Cells[1].StringCellValue.Removewhite().ToLower();
                        record.StartTime = data.Cells[2].DateCellValue;
                        record.EndTime = data.Cells[3].DateCellValue;
                        if (type == 3)
                        {
                            record.LeaveType = data.Cells[4].StringCellValue.Removewhite().ToLower();
                        }
                        if (type == 1)
                        {
                            record.LeaveType = "外出";
                        }
                        if (type == 2)
                        {
                            record.LeaveType = "出差";
                        }
                        importClockRecords.Add(record);
                    }
                }
                catch (Exception)
                {
                    errimportClockRecords.Add(record);
                }
            }
            if (!importClockRecords.Any())
            {
                return (importClockRecords, "模板表中不存在有效数据", errimportClockRecords);
            }
            return (importClockRecords, "", errimportClockRecords);
        }
        #endregion

        #region XSSF方法(.xlsx文件)
        /// <summary>
        /// 导出   模板方法
        /// </summary>
        /// <param name="data">数据集合</param>
        /// <param name="titles">表头集合</param>
        /// <param name="name">sheet名称</param>
        /// <returns></returns>
        public static byte[] OutputExcel<T1, T2>(List<T1> data, List<T2> titles, string name, List<Diction> titleExtention = null)
        {
            titles = titles.OrderBy(a => a.GetType().GetProperty("SortNumber").GetValue(a)).ToList();
            //1.创建相关对象
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet(name);
            IRow titleRow = null;
            IRow rows = null;
            StringBuilder stringBuilder = new StringBuilder();
            //2.设置表头
            if (titles != null && titles.Count > 0)
            {
                titleRow = sheet.CreateRow(0);
                for (int i = 0; i < titles.Count; i++)
                {
                    ICell cell = titleRow.CreateCell(i);
                    var propertyInfo = titles[i].GetType().GetProperty("Name").GetValue(titles[i]);
                    cell.SetCellValue(propertyInfo == null ? "" : propertyInfo.ToString());
                    SetCellStyleXSSF(workbook, cell);
                    if (propertyInfo.ToString() == "姓名" || propertyInfo.ToString() == "身份证" || propertyInfo.ToString() == "证照号码")
                    {
                        stringBuilder.Append(titles[i].GetType().GetProperty("SortNumber").GetValue(titles[i]).ToString() + "|");
                    }
                }
                if (titleExtention != null && titleExtention.Count > 0)
                {
                    for (int i = 0; i < titleExtention.Count; i++)
                    {
                        ICell cell = titleRow.CreateCell(titles.Count + i);
                        cell.SetCellValue(titleExtention[i].Value);
                        SetCellStyleXSSF(workbook, cell);
                    }
                }
            }
            //3.设置数据
            if (data != null && data.Count > 0)
            {
                string a = stringBuilder.ToString().TrimEnd('|');
                List<string> strs = a.Split("|").ToList();
                for (int i = 0; i < data.Count; i++)
                {
                    Type type = data[i].GetType();
                    //1.创建行
                    rows = sheet.CreateRow(i + 1);
                    //3.根据序号获取值
                    int index = 0;
                    for (int j = 0; j < strs.Count; j++)
                    {
                        var propertyInfo = type.GetProperty(("Project" + strs[j]).ToString()).GetValue(data[i]);
                        rows.CreateCell(Convert.ToInt32(strs[j]) - 1).SetCellValue(propertyInfo == null ? "" : propertyInfo.ToString());
                        index++;
                    }
                    if (titleExtention != null && titleExtention.Count > 0)
                    {
                        for (int k = 0; k < titleExtention.Count; k++)
                        {
                            var propertyInfo = type.GetProperty(titleExtention[k].Key).GetValue(data[i]);
                            rows.CreateCell(k + index).SetCellValue(propertyInfo == null ? "" : propertyInfo.ToString());
                        }
                    }
                }
            }
            //1.设置宽度
            SetColumnWidth(sheet, titles.Count, 0);

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
        /// 导出数据工资表方法
        /// </summary>
        /// <param name="data">数据集合</param>
        /// <param name="titles">表头集合</param>
        /// <param name="name">sheet名称</param>
        /// <returns></returns>
        public static byte[] OutputExcelData<T1, T2>(List<T1> data, List<T2> titles, string name, List<Diction> titleExtention = null)
        {
            titles = titles.OrderBy(a => a.GetType().GetProperty("SortNumber").GetValue(a)).ToList();
            //1.创建相关对象
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet(name);
            IRow titleRow = null;
            IRow rows = null;
            StringBuilder stringBuilder = new StringBuilder();
            //2.设置表头
            if (titles != null && titles.Count > 0)
            {
                titleRow = sheet.CreateRow(0);
                for (int i = 0; i < titles.Count; i++)
                {
                    ICell cell = titleRow.CreateCell(i);
                    var propertyInfo = titles[i].GetType().GetProperty("Name").GetValue(titles[i]);
                    cell.SetCellValue(propertyInfo == null ? "" : propertyInfo.ToString());
                    SetCellStyleXSSF(workbook, cell);
                    stringBuilder.Append(titles[i].GetType().GetProperty("SortNumber").GetValue(titles[i]).ToString() + "|");
                }
                if (titleExtention != null && titleExtention.Count > 0)
                {
                    for (int i = 0; i < titleExtention.Count; i++)
                    {
                        ICell cell = titleRow.CreateCell(titles.Count + i);
                        cell.SetCellValue(titleExtention[i].Value);
                        SetCellStyleXSSF(workbook, cell);
                    }
                }
            }
            //3.设置数据
            if (data != null && data.Count > 0)
            {
                string a = stringBuilder.ToString().TrimEnd('|');
                List<string> strs = a.Split("|").ToList();
                for (int i = 0; i < data.Count; i++)
                {
                    Type type = data[i].GetType();
                    //1.创建行
                    rows = sheet.CreateRow(i + 1);
                    //3.根据序号获取值
                    int index = 0;
                    for (int j = 0; j < strs.Count; j++)
                    {
                        var propertyInfo = type.GetProperty(("Project" + strs[j]).ToString()).GetValue(data[i]);
                        rows.CreateCell(Convert.ToInt32(strs[j]) - 1).SetCellValue(propertyInfo == null ? "" : propertyInfo.ToString());
                        index++;
                    }
                    if (titleExtention != null && titleExtention.Count > 0)
                    {
                        for (int k = 0; k < titleExtention.Count; k++)
                        {
                            var propertyInfo = type.GetProperty(titleExtention[k].Key).GetValue(data[i]);
                            rows.CreateCell(k + index).SetCellValue(propertyInfo == null ? "" : propertyInfo.ToString());
                        }
                    }
                }
            }
            //1.设置宽度
            SetColumnWidth(sheet, titles.Count, 0);

            byte[] buffer = new byte[1024 * 2];
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                buffer = ms.GetBuffer();
                ms.Close();
            }
            return buffer;
        }

        public static byte[] StOutputExcelXSSF<T>(NPIOExtend<T> NPIOextend, FyuUser user)
        {
            //1.创建相关对象
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet(NPIOextend.SheetName);
            IDrawing patr = sheet.CreateDrawingPatriarch();
            IRow titleRow = null;
            IRow rows = null;
            int nowrows = 0;
            int rowNum = 0;
            //2.设置标题
            if (!string.IsNullOrEmpty(NPIOextend.TitleFont))//判断是否有标题
            {
                titleRow = sheet.CreateRow(nowrows);
                ICell sheetcell = titleRow.CreateCell(0);
                sheetcell.SetCellValue(NPIOextend.TitleFont);
                sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, NPIOextend.Titles == null ? 0 : NPIOextend.Titles.Count - 1));
                SetCellStyleXSSF(workbook, sheetcell, NPIOextend.TitleFontColor ?? null);
                nowrows++;
                rowNum = 1;
            }
            //3.设置表头
            if (NPIOextend.Titles != null && NPIOextend.Titles.Count > 0)//判断是否有表头
            {
                //设置第二行表头
                titleRow = sheet.CreateRow(nowrows);
                nowrows++;
                int a = 0;
                foreach (var i in NPIOextend.Titles)
                {
                    ICell cell = titleRow.CreateCell(a);
                    cell.SetCellValue(i.Value);
                    SetCellStyleXSSF(workbook, cell, i.TitleColor ?? null);
                    if (i.Postil != null)
                    {
                        SetCellCommentXSSF(patr, cell, i.Postil);
                    }
                    if (i.Select != null)
                    {
                        CellRangeAddressList regions = new CellRangeAddressList(1, 65535, a, a);
                        XSSFDataValidationConstraint constraint = new XSSFDataValidationConstraint(i.Select);
                        XSSFDataValidation dataValidate = new XSSFDataValidation(constraint, regions, new CT_DataValidation())
                        {
                            EmptyCellAllowed = true,
                            SuppressDropDownArrow = false,
                            ShowPromptBox = true,
                        };
                        sheet.AddValidationData(dataValidate);
                    }
                    a++;
                }
            }
            //4.设置数据
            if (NPIOextend.Data != null && NPIOextend.Data.Count > 0)
            {
                XSSFCellStyle style = (XSSFCellStyle)workbook.CreateCellStyle();//创建样式对象
                XSSFFont font = (XSSFFont)workbook.CreateFont(); //创建一个字体样式对象
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                //字体默认宋体，大小11，
                font.FontName = "宋体"; //和excel里面的字体对应
                font.FontHeightInPoints = 11;//字体大小
                for (int i = 0; i < NPIOextend.Data.Count; i++)
                {
                    Type type = NPIOextend.Data[i].GetType();
                    //1.创建行
                    rows = sheet.CreateRow(i + nowrows);
                    int a = 0;
                    //3.根据序号获取值
                    foreach (var item in NPIOextend.Titles)
                    {
                        ICell cell = rows.CreateCell(a);
                        cell.CellStyle = style; //把样式赋给单元格
                        if (item.Key != "")
                        {
                            var propertyInfo = type.GetProperty(item.Key).GetValue(NPIOextend.Data[i]);
                            cell.SetCellValue(propertyInfo == null ? "" : propertyInfo.ToString());
                        }
                        else
                        {
                            cell.SetCellValue("");
                        }
                        a++;
                    }
                }
                nowrows = NPIOextend.Data.Count + nowrows;
                rows = sheet.CreateRow(nowrows);
                for (int i = 0; i < NPIOextend.Titles.Count; i++)
                {
                    ICell cell = rows.CreateCell(i);
                    cell.CellStyle = style; //把样式赋给单元格
                    cell.SetCellValue("");
                }
                rows = sheet.CreateRow(1 + nowrows);
                for (int i = 0; i < NPIOextend.Titles.Count; i++)
                {
                    ICell cell = rows.CreateCell(i);
                    cell.CellStyle = style; //把样式赋给单元格
                    if (i == 0)
                    {
                        cell.SetCellValue("制表人：");
                        continue;
                    }
                    if (i == 1)
                    {
                        cell.SetCellValue(user.realName);
                        continue;
                    }
                    if (i == 2)
                    {
                        cell.SetCellValue("制表日期：");
                        continue;
                    }
                    if (i == 3)
                    {
                        cell.SetCellValue(DateTime.Now.ToString("yyyy-MM-dd"));
                        continue;
                    }
                    if (i == 5)
                    {
                        cell.SetCellValue("审批人：");
                        continue;
                    }
                    if (i == 8)
                    {
                        cell.SetCellValue("审批日期：");
                        continue;
                    }
                    else
                    {
                        cell.SetCellValue("");
                        continue;
                    }
                }
            }
            //5.设置自动宽度
            SetColumnWidth(sheet, NPIOextend.Titles.Count, rowNum);
            //6.设置自动高度
            SetColumnHeightHSSF(sheet, NPIOextend.Titles.Count, NPIOextend.TitleFont == "" ? 0 : 1);
            return ConvertByteArray(workbook);
        }


        /// <summary>
        /// 数据导出通用方法(XSSF方法)
        /// </summary>
        /// <typeparam name="T">任意模型类型</typeparam>
        /// <param name="data">数据来源</param>
        /// <param name="titles">字典,键名和中文名称对应</param>
        /// <param name="sheetName">表格名称</param>
        /// <param name="sheetTitle">表格标题</param>
        /// <param name="sheetColor">标题颜色</param>
        /// <param name="titleColor">表头颜色</param>
        /// <param name="dataColor">数据颜色</param>
        /// <returns></returns>

        public static byte[] OutputExcelXSSF<T>(NPIOExtend<T> NPIOextend)
        {
            //1.创建相关对象
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet(NPIOextend.SheetName);
            IDrawing patr = sheet.CreateDrawingPatriarch();
            IRow titleRow = null;
            IRow rows = null;
            int nowrows = 0;
            int rowNum = 0;
            //2.设置标题
            if (!string.IsNullOrEmpty(NPIOextend.TitleFont))//判断是否有标题
            {
                titleRow = sheet.CreateRow(nowrows);
                ICell sheetcell = titleRow.CreateCell(0);
                sheetcell.SetCellValue(NPIOextend.TitleFont);
                sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, NPIOextend.Titles == null ? 0 : NPIOextend.Titles.Count - 1));
                SetCellStyleXSSF(workbook, sheetcell, NPIOextend.TitleFontColor ?? null);
                nowrows++;
                rowNum = 1;
            }
            //3.设置表头
            if (NPIOextend.Titles != null && NPIOextend.Titles.Count > 0)//判断是否有表头
            {
                //设置第二行表头
                titleRow = sheet.CreateRow(nowrows);
                nowrows++;
                int a = 0;
                foreach (var i in NPIOextend.Titles)
                {
                    ICell cell = titleRow.CreateCell(a);
                    cell.SetCellValue(i.Value);
                    SetCellStyleXSSF(workbook, cell, i.TitleColor ?? null);
                    if (i.Postil != null)
                    {
                        SetCellCommentXSSF(patr, cell, i.Postil);
                    }
                    if (i.Select != null)
                    {
                        CellRangeAddressList regions = new CellRangeAddressList(1, 65535, a, a);
                        XSSFDataValidationConstraint constraint = new XSSFDataValidationConstraint(i.Select);
                        XSSFDataValidation dataValidate = new XSSFDataValidation(constraint, regions, new CT_DataValidation())
                        {
                            EmptyCellAllowed = true,
                            SuppressDropDownArrow = false,
                            ShowPromptBox = true,
                        };
                        sheet.AddValidationData(dataValidate);
                    }
                    a++;
                }
            }
            //4.设置数据
            if (NPIOextend.Data != null && NPIOextend.Data.Count > 0)
            {
                XSSFCellStyle style = (XSSFCellStyle)workbook.CreateCellStyle();//创建样式对象
                XSSFFont font = (XSSFFont)workbook.CreateFont(); //创建一个字体样式对象
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                //字体默认宋体，大小11，
                font.FontName = "宋体"; //和excel里面的字体对应
                font.FontHeightInPoints = 11;//字体大小
                for (int i = 0; i < NPIOextend.Data.Count; i++)
                {
                    Type type = NPIOextend.Data[i].GetType();
                    //1.创建行
                    rows = sheet.CreateRow(i + nowrows);
                    int a = 0;
                    //3.根据序号获取值
                    foreach (var item in NPIOextend.Titles)
                    {
                        ICell cell = rows.CreateCell(a);
                        cell.CellStyle = style; //把样式赋给单元格
                        if (item.Key != "")
                        {
                            var propertyInfo = type.GetProperty(item.Key).GetValue(NPIOextend.Data[i]);
                            cell.SetCellValue(propertyInfo == null ? "" : propertyInfo.ToString());
                        }
                        else
                        {
                            cell.SetCellValue("");
                        }
                        a++;
                    }
                }
                nowrows = NPIOextend.Data.Count + nowrows;
            }
            //5.设置自动宽度
            SetColumnWidth(sheet, NPIOextend.Titles.Count, rowNum);
            //6.设置自动高度
            SetColumnHeightHSSF(sheet, NPIOextend.Titles.Count, NPIOextend.TitleFont == "" ? 0 : 1);
            return ConvertByteArray(workbook);
        }
        /// <summary>
        /// 数据导出通用方法(XSSF方法)
        /// </summary>
        /// <typeparam name="T">任意模型类型</typeparam>
        /// <param name="data">数据来源</param>
        /// <param name="titles">字典,键名和中文名称对应</param>
        /// <param name="sheetName">表格名称</param>
        /// <param name="sheetTitle">表格标题</param>
        /// <param name="sheetColor">标题颜色</param>
        /// <param name="titleColor">表头颜色</param>
        /// <param name="dataColor">数据颜色</param>
        /// <returns></returns>
        public static byte[] OutputMonthlyRecordExcel(NPIOExtend<AttendanceMonthlyRecord> NPIOextend)
        {
            //1.创建相关对象
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet(NPIOextend.SheetName);
            IDrawing patr = sheet.CreateDrawingPatriarch();
            IRow titleRow = null;
            IRow rows = null;
            int nowrows = 0;
            int rowNum = 0;
            //2.设置标题
            if (!string.IsNullOrEmpty(NPIOextend.TitleFont))//判断是否有标题
            {
                titleRow = sheet.CreateRow(nowrows);
                ICell sheetcell = titleRow.CreateCell(0);
                sheetcell.SetCellValue(NPIOextend.TitleFont);
                sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, NPIOextend.Titles == null ? 0 : NPIOextend.Titles.Count - 1));
                SetCellStyleXSSF(workbook, sheetcell, NPIOextend.TitleFontColor ?? null);
                nowrows++;
                rowNum = 1;
            }
            //3.设置表头
            if (NPIOextend.Titles != null && NPIOextend.Titles.Count > 0)//判断是否有表头
            {
                //设置第二行表头
                titleRow = sheet.CreateRow(nowrows);
                nowrows++;
                int a = 0;
                foreach (var i in NPIOextend.Titles)
                {
                    ICell cell = titleRow.CreateCell(a);
                    cell.SetCellValue(i.Value);
                    SetCellStyleXSSF(workbook, cell, i.TitleColor ?? null);
                    if (i.Postil != null)
                    {
                        SetCellCommentXSSF(patr, cell, i.Postil);
                    }
                    if (i.Select != null)
                    {
                        CellRangeAddressList regions = new CellRangeAddressList(1, 65535, a, a);
                        XSSFDataValidationConstraint constraint = new XSSFDataValidationConstraint(i.Select);
                        XSSFDataValidation dataValidate = new XSSFDataValidation(constraint, regions, new CT_DataValidation())
                        {
                            EmptyCellAllowed = true,
                            SuppressDropDownArrow = false,
                            ShowPromptBox = true,
                        };
                        sheet.AddValidationData(dataValidate);
                    }
                    a++;
                }
            }
            //4.设置数据
            if (NPIOextend.Data != null && NPIOextend.Data.Count > 0)
            {
                for (int i = 0; i < NPIOextend.Data.Count; i++)
                {
                    Type type = NPIOextend.Data[i].GetType();
                    //1.创建行
                    rows = sheet.CreateRow(i + nowrows);
                    int a = 0;
                    int enterPriseIndex = 0;
                    //3.根据序号获取值
                    foreach (var item in NPIOextend.Titles)
                    {
                        if (item.Key != "")
                        {
                            var propertyInfo = type.GetProperty(item.Key).GetValue(NPIOextend.Data[i]);
                            ICell cell = rows.CreateCell(a);
                            cell.SetCellValue(propertyInfo == null ? "" : propertyInfo.ToString());
                            SetCellStyleXSSF(workbook, cell, item.DataColor ?? null);
                        }
                        else
                        {
                            var enterPriseSeting = NPIOextend.Data[i].AttendanceProjects[enterPriseIndex];
                            ICell cell = rows.CreateCell(a);
                            cell.SetCellValue(Convert.ToDouble(Math.Round(enterPriseSeting.AttendanceItemValue, 2)));
                            SetCellStyleXSSF(workbook, cell, item.DataColor ?? null);
                            enterPriseIndex++;
                        }
                        a++;
                    }
                }
                nowrows = NPIOextend.Data.Count + nowrows;
            }
            //5.设置自动宽度
            SetColumnWidth(sheet, NPIOextend.Titles.Count, rowNum);
            //6.设置自动高度
            SetColumnHeightHSSF(sheet, NPIOextend.Titles.Count, NPIOextend.TitleFont == "" ? 0 : 1);
            return ConvertByteArray(workbook);
        }
        /// <summary>
        /// 添加批注(XSSF方法)
        /// </summary>
        /// <param name="patr"></param>
        /// <param name="cell"></param>
        /// <param name="text"></param>
        public static void SetCellCommentXSSF(IDrawing patr, ICell cell, string text)
        {
            IComment comment12 = patr.CreateCellComment(new XSSFClientAnchor(1, 1, 1, 1, 1, 1, 1, 1));//批注显示定位
            comment12.String = new XSSFRichTextString(text);
            cell.CellComment = comment12;
        }
        /// <summary>
        /// 设置样式，默认宋体，11，加入边框颜色自动加入实线边框（XSSF相关,用于兼容2007版本的excle）
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="cell"></param>
        /// <param name="color"></param>
        public static void SetCellStyleXSSF(IWorkbook workbook, ICell cell, AllColor color = null)
        {
            XSSFCellStyle style = (XSSFCellStyle)workbook.CreateCellStyle();//创建样式对象
            XSSFFont font = (XSSFFont)workbook.CreateFont(); //创建一个字体样式对象
            //字体默认双居中
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            //字体默认宋体，大小11，
            font.FontName = "宋体"; //和excel里面的字体对应
            font.FontHeightInPoints = 11;//字体大小
            if (color == null)
            {
                cell.CellStyle = style; //把样式赋给单元格
                return;
            }
            if (color.FontColor != null)
            {
                var a = Color.FromArgb(color.FontColor.Red, color.FontColor.Green, color.FontColor.Blue);
                font.SetColor(new XSSFColor(a));
            }
            style.SetFont(font); //将字体样式赋给样式对象
            if (color.CellColor != null)
            {
                var a = Color.FromArgb(color.CellColor.Red, color.CellColor.Green, color.CellColor.Blue);
                style.SetFillForegroundColor(new XSSFColor(a));
                style.FillPattern = FillPattern.SolidForeground;
            }
            if (color.BoderColor != null)
            {
                //设置了边框颜色，自动加上实线边框
                style.BorderBottom = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Thin;
                var a = Color.FromArgb(color.BoderColor.Red, color.BoderColor.Green, color.BoderColor.Blue);
                style.SetBottomBorderColor(new XSSFColor(a));
            }
            cell.CellStyle = style; //把样式赋给单元格
        }
        #endregion

        #region 通用设置宽高，读出字节，数字转ABCD
        /// <summary>
        /// 设置自动宽度(通用)
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="count">总列</param>
        public static void SetColumnWidth(ISheet sheet, int count, int rowNum)
        {
            for (int i = 0; i <= count - 1; i++)
            {
                sheet.AutoSizeColumn(i);
            }

            for (int columnNum = 0; columnNum <= count - 1; columnNum++)
            {
                //当前列宽
                int columnWidth = sheet.GetColumnWidth(columnNum) / 256;
                //获取当前行
                IRow currentRow = sheet.GetRow(rowNum);
                if (currentRow.GetCell(columnNum) != null)
                {
                    ICell currentCell = currentRow.GetCell(columnNum);
                    int length = Encoding.Default.GetBytes(currentCell.ToString()).Length;
                    if (columnWidth < length + 1)
                    {
                        columnWidth = length + 1;
                    }
                }
                //设置当前列宽
                sheet.SetColumnWidth(columnNum, columnWidth * 256);
            }
        }

        /// <summary>
        /// 设置自动高度(通用)
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="count"></param>
        /// <param name="isSheetTitle">是否存在标题</param>
        public static void SetColumnHeightHSSF(ISheet sheet, int count, int isSheetTitle = 0)
        {
            if (isSheetTitle == 1)
            {
                IRow currentRow = sheet.GetRow(0);
                ICell currentCell = currentRow.GetCell(0);
                int length = Encoding.UTF8.GetBytes(currentCell.ToString()).Length;
                currentRow.HeightInPoints = 20 * (length / 60 + 1);
            }
            for (int rowNum = isSheetTitle; rowNum <= sheet.LastRowNum; rowNum++)
            {
                IRow currentRow = sheet.GetRow(rowNum);
                ICell currentCell = currentRow.GetCell(count - 1);
                int length = Encoding.UTF8.GetBytes(currentCell.ToString()).Length;
                currentRow.HeightInPoints = 20 * (length / 60 + 1);
            }
        }
        /// <summary>
        /// 从内存读出字节（通用）
        /// </summary>
        /// <param name="workbook"></param>
        /// <returns></returns>
        public static byte[] ConvertByteArray(IWorkbook workbook)
        {
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
        /// 1-26转为ABCDE按Excle的格式转换（通用）
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string NunberToChar(int number)
        {
            StringBuilder firstString = new StringBuilder();
            var chushu = number / 26;
            for (int i = 0; i < (int)Math.Log(number, 26) - 1; i++)
            {
                firstString.Append(CharToNunber(26));
                chushu = chushu / 26;
            }
            firstString.Append(CharToNunber(chushu));
            int yushu = number % 26;
            string resstr = CharToNunber(yushu);
            firstString.Append(resstr);
            return firstString.ToString();
        }
        /// <summary>
        /// 数字转字符，最多0-36(通用)
        /// </summary>
        /// <param name="yushu"></param>
        /// <returns></returns>
        public static string CharToNunber(int yushu)
        {
            int num = yushu + 64;
            ASCIIEncoding asciiEncoding = new ASCIIEncoding();
            byte[] btNumber = new byte[] { (byte)num };
            return asciiEncoding.GetString(btNumber);
        }
        /// <summary>
        /// 匹配删除任何空白字符，包括空格，制表符，换页符等，与[\f\n\t\r\v]等效(通用)
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static string Removewhite(string a)
        {
            return Regex.Replace(a, @"\s", "");
        }
        #endregion

        #region HSSF.(xls文件，暂不使用)

        /// <summary>
        /// 模板导出通用方法（未更新）
        /// </summary>
        /// <param name="titles"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static byte[] OutputTitle(Dictionary<string, string> titles, string name)
        {
            //1.创建相关对象
            IWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet(name);
            IRow titleRow = null;
            //2.设置表头
            if (titles != null && titles.Count > 0)
            {
                titleRow = sheet.CreateRow(0);
                var a = 0;
                foreach (var i in titles)
                {
                    ICell cell = titleRow.CreateCell(a);
                    cell.SetCellValue(i.Value);
                    SetCellStyle(workbook, cell);
                    a++;
                }
            }
            //1.设置宽度
            SetColumnWidth(sheet, titles.Count, 0);
            SetColumnHeightHSSF(sheet, titles.Count);

            return ConvertByteArray(workbook);
        }
        /// <summary>
        /// 数据导出通用方法(HSSF方法)
        /// </summary>
        /// <typeparam name="T">任意模型类型</typeparam>
        /// <param name="data">数据来源</param>
        /// <param name="titles">字典,键名和中文名称对应</param>
        /// <param name="sheetName">表格名称</param>
        /// <param name="sheetTitle">表格标题</param>
        /// <param name="sheetColor">标题颜色</param>
        /// <param name="titleColor">表头颜色</param>
        /// <param name="dataColor">数据颜色</param>
        /// <returns></returns>
        public static byte[] OutputExcel<T>(NPIOExtend<T> NPIOextend)
        {
            //1.创建相关对象
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet(NPIOextend.SheetName);
            IDrawing patr = sheet.CreateDrawingPatriarch();
            IRow titleRow = null;
            IRow rows = null;
            int nowrows = 0;
            int rowNum = 0;
            //2.设置标题
            if (!string.IsNullOrEmpty(NPIOextend.TitleFont))//判断是否有标题
            {
                titleRow = sheet.CreateRow(nowrows);
                ICell sheetcell = titleRow.CreateCell(0);
                sheetcell.SetCellValue(NPIOextend.TitleFont);
                sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, NPIOextend.Titles == null ? 0 : NPIOextend.Titles.Count - 1));
                SetCellStyle(workbook, sheetcell, NPIOextend.TitleFontColor ?? null);
                nowrows++;
                rowNum = 1;
            }
            //3.设置表头
            if (NPIOextend.Titles != null && NPIOextend.Titles.Count > 0)//判断是否有表头
            {
                //设置第二行表头
                titleRow = sheet.CreateRow(nowrows);
                nowrows++;
                int a = 0;
                foreach (var i in NPIOextend.Titles)
                {
                    ICell cell = titleRow.CreateCell(a);
                    cell.SetCellValue(i.Value);
                    SetCellStyle(workbook, cell, i.TitleColor ?? null);
                    if (i.Postil != null)
                    {
                        SetCellComment(patr, cell, i.Postil);
                    }
                    if (i.Select != null)
                    {
                        CellRangeAddressList regions = new CellRangeAddressList(1, 65535, a, a);
                        DVConstraint constraint = DVConstraint.CreateExplicitListConstraint(i.Select);
                        HSSFDataValidation dataValidate = new HSSFDataValidation(regions, constraint)
                        {
                            EmptyCellAllowed = true,
                            SuppressDropDownArrow = false,
                            ShowPromptBox = true,
                        };
                        sheet.AddValidationData(dataValidate);
                    }
                    a++;
                }
            }
            //4.设置数据
            if (NPIOextend.Data != null && NPIOextend.Data.Count > 0)
            {
                for (int i = 0; i < NPIOextend.Data.Count; i++)
                {
                    Type type = NPIOextend.Data[i].GetType();
                    //1.创建行
                    rows = sheet.CreateRow(i + nowrows);
                    int a = 0;
                    //3.根据序号获取值
                    foreach (var item in NPIOextend.Titles)
                    {
                        if (item.Key != "")
                        {
                            var propertyInfo = type.GetProperty(item.Key).GetValue(NPIOextend.Data[i]);
                            ICell cell = rows.CreateCell(a);
                            cell.SetCellValue(propertyInfo == null ? "" : propertyInfo.ToString());
                            SetCellStyle(workbook, cell, item.DataColor ?? null);
                        }
                        else
                        {
                            ICell cell = rows.CreateCell(a);
                            cell.SetCellValue("");
                            SetCellStyle(workbook, cell, item.DataColor ?? null);
                        }
                        a++;
                    }
                }
                nowrows = NPIOextend.Data.Count + nowrows;
            }
            //5.设置自动宽度
            SetColumnWidth(sheet, NPIOextend.Titles.Count, rowNum);
            //6.设置自动高度
            SetColumnHeightHSSF(sheet, NPIOextend.Titles.Count, NPIOextend.TitleFont == "" ? 0 : 1);
            return ConvertByteArray(workbook);
        }
        /// <summary>
        /// 添加批注（HSSF相关，用于兼容2007版本的excle）
        /// </summary>
        /// <param name="patr"></param>
        /// <param name="cell"></param>
        /// <param name="text"></param>
        public static void SetCellComment(IDrawing patr, ICell cell, string text)
        {
            IComment comment12 = patr.CreateCellComment(new HSSFClientAnchor(1, 1, 1, 1, 1, 1, 1, 1));//批注显示定位
            comment12.String = new HSSFRichTextString(text);
            cell.CellComment = comment12;
        }
        /// <summary>
        /// 设置样式，默认宋体，11，加入边框颜色自动加入实线边框 （HSSF相关,用于兼容2003版本的excle）
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="cell"></param>
        public static void SetCellStyle(IWorkbook workbook, ICell cell, AllColor color = null)
        {
            //调色板解析颜色
            HSSFPalette palette = ((HSSFWorkbook)workbook).GetCustomPalette();

            ICellStyle style = workbook.CreateCellStyle();//创建样式对象
            IFont font = workbook.CreateFont(); //创建一个字体样式对象
                                                //字体默认双居中
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            //字体默认宋体，大小11，
            font.FontName = "宋体"; //和excel里面的字体对应
            font.FontHeightInPoints = 12;//字体大小
            if (color == null)
            {
                style.SetFont(font);
                cell.CellStyle = style; //把样式赋给单元格
                return;
            }
            if (color.FontColor != null)
            {
                palette.SetColorAtIndex(color.FontColor.Index, color.FontColor.Red, color.FontColor.Green, color.FontColor.Blue);
                var fontcolor = palette.FindColor(color.FontColor.Red, color.FontColor.Green, color.FontColor.Blue);
                font.Color = fontcolor.Indexed;
            }
            style.SetFont(font); //将字体样式赋给样式对象
            if (color.CellColor != null)
            {
                palette.SetColorAtIndex(color.CellColor.Index, color.CellColor.Red, color.CellColor.Green, color.CellColor.Blue);
                var cellcolor = palette.FindColor(color.CellColor.Red, color.CellColor.Green, color.CellColor.Blue);
                style.FillForegroundColor = cellcolor.Indexed;
                style.FillPattern = FillPattern.SolidForeground;
            }
            if (color.BoderColor != null)
            {
                //设置了边框颜色，自动加上实线边框
                style.BorderBottom = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Thin;

                palette.SetColorAtIndex(color.BoderColor.Index, color.BoderColor.Red, color.BoderColor.Green, color.BoderColor.Blue);
                var fontcolor = palette.FindColor(color.BoderColor.Red, color.BoderColor.Green, color.BoderColor.Blue);
                style.BottomBorderColor = fontcolor.Indexed;
            }
            cell.CellStyle = style; //把样式赋给单元格
        }
        #endregion
    }
    public class ImportClockRecord
    {
        public string Name { get; set; }
        public string IdCard { get; set; }
        public DateTime Clocktime { get; set; }
        public string Site { get; set; }
    }
    public class ImportLeaveRecord
    {
        public string Name { get; set; }
        public string IdCard { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string LeaveType { get; set; }
    }


}
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;

namespace Infrastructure
{
    public class ExcelHelper
    {
        /// <summary>
        /// 导出列表到excel文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">需要导出的列表数据</param>
        /// <param name="headers">需要自定义的字段和表头值</param>
        /// <returns></returns>
        public static MemoryStream ExportListToExcel<T>(List<T> data,string filename, Dictionary<string, string> headers = null)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("sheet1");
                worksheet.Cells.LoadFromCollection(data, true);

                if (headers != null)
                {
                    for (int i = 0; i < worksheet.Dimension.End.Column; i++)
                    {
                        var name = worksheet.Cells[1, i + 1]?.Value?.ToString();
                        if (string.IsNullOrEmpty(name) == false && headers.ContainsKey(name))
                        {
                            worksheet.Cells[1, i + 1].Value = headers[name];
                        }
                    }
                }

                return new MemoryStream(package.GetAsByteArray());
            }
        }
    }
}
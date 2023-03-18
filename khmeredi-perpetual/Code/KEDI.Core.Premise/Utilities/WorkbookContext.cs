using System;
using System.Collections.Generic;
using System.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using Newtonsoft.Json;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.RegularExpressions;

namespace KEDI.Core.Premise.Utilities
{
    public class WorkbookContext
    {
        public WorkbookContext() { }
        public IDictionary<string, string> ContentType
        {
            get
            {
                return new Dictionary<string, string>
                {
                    { ".xls", "application/vnd.ms-excel" },
                    { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" }
                };
            }
        }

        public Dictionary<string, List<T>> ToDictionary<T>(IWorkbook workbook) where T : class, new()
        {
            var dictionary = new Dictionary<string, List<T>>();
            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {
                ISheet sheet = workbook.GetSheetAt(i);
                var list = ToList<T>(sheet);
                dictionary.TryAdd(sheet.SheetName, list);
            }
            return dictionary;
        }

        public List<T> ToList<T>(ISheet sheet) where T : class, new()
        {
            List<T> list = new();
            IRow headerRow = sheet.GetRow(0); //Get Header Row       
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
            {
                if (headerRow == null) continue;
                var desObj = new T();
                IRow row = sheet.GetRow(i);
                if (row == null) continue;
                if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                for (int j = row.FirstCellNum; j < row.LastCellNum; j++)
                {
                    ICell hcell = headerRow.Cells[j];
                    ICell cell = row.Cells[j];
                    hcell.SetCellType(CellType.String);
                    cell.SetCellType(CellType.String);
                    string propName = Regex.Replace(hcell.StringCellValue, "\\s+", string.Empty);
                    PropertyInfo propInfo = desObj.GetType().GetProperty(propName);
                    if (propInfo != null)
                    {
                        object propValue = ConvertFrom(propInfo, cell.StringCellValue);
                        propInfo.SetValue(desObj, propValue, null);
                    }
                }
                list.Add(desObj);
            }

            return list;
        }

        public static object ConvertFrom(PropertyInfo propInfo, object value)
        {
            try
            {
                Type propType = Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType;
                TypeConverter typeConverter = TypeDescriptor.GetConverter(propType);
                object propValue = typeConverter.ConvertFrom(value);
                return propValue;
            }
            catch
            {
                return value;
            }
        }

        public List<Dictionary<string, string>> ParseKeyValue(ISheet sheet)
        {
            List<Dictionary<string, string>> list = new ();
            try
            {
                IRow headerRow = sheet.GetRow(0); //Get Header Row       
                for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++) //Read Excel File
                {
                    if (headerRow == null) continue;
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;
                    if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                    Dictionary<string, string> props = new();
                    for (int j = row.FirstCellNum; j < row.LastCellNum; j++)
                    {
                        ICell hcell = headerRow.Cells[j];
                        ICell cell = row.Cells[j];
                        hcell.SetCellType(CellType.String);
                        cell.SetCellType(CellType.String);
                        props.Add(hcell.StringCellValue.Replace(" ", string.Empty), cell.StringCellValue);
                    }
                    list.Add(props);
                }
            }
            catch (Exception) { return list; }
            return list;
        }

        public string Serialize(ISheet sheet, JsonSerializerSettings settings = null)
        {
            return JsonConvert.SerializeObject(ParseKeyValue(sheet), settings);
        }

        public string Serialize(IEnumerable<ISheet> sheets, bool asArray = false, JsonSerializerSettings settings = null)
        {
            if (asArray)
            {
                IList<IEnumerable<Dictionary<string, string>>> nestedArray = new List<IEnumerable<Dictionary<string, string>>>();
                foreach (ISheet sh in sheets)
                {
                    nestedArray.Add(ParseKeyValue(sh));
                }
                return JsonConvert.SerializeObject(nestedArray, settings);
            }

            IDictionary<string, IEnumerable<Dictionary<string, string>>> keyValues = new Dictionary<string, IEnumerable<Dictionary<string, string>>>();
            foreach (ISheet sh in sheets)
            {
                keyValues.Add(sh.SheetName, ParseKeyValue(sh));
            }
            return JsonConvert.SerializeObject(keyValues, settings);
        }

        public string Serialize(IWorkbook workbook, string sheetName, JsonSerializerSettings settings = null)
        {
            return Serialize(workbook.GetSheet(sheetName), settings);
        }

        public string Serialize(IWorkbook workbook, int sheetIndex, JsonSerializerSettings settings = null)
        {
            return Serialize(workbook.GetSheetAt(sheetIndex), settings);
        }

        public string Serialize(IWorkbook workbook, bool asArray = false, JsonSerializerSettings settings = null)
        {
            List<ISheet> sheets = new();
            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {
                sheets.Add(workbook.GetSheetAt(i));
            }

            if (asArray)
            {
                return Serialize(sheets, true, settings);
            }

            return Serialize(sheets, false, settings);
        }

        //Deserialize
        public T Deserialize<T>(ISheet sheet, JsonSerializerSettings settings = null)
        {
            return JsonConvert.DeserializeObject<T>(Serialize(sheet, settings), settings);
        }

        public T Deserialize<T>(string value, JsonSerializerSettings settings = null)
        {
            return JsonConvert.DeserializeObject<T>(value, settings);
        }

        //Parse list of specified objects to datatable.
        public DataTable ParseDataTable<T>(IEnumerable<T> data)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(
                    prop.PropertyType) ?? prop.PropertyType);
            }

            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }

        public DataTable ParseDataTable<T>(string jsonArray, JsonSerializerSettings settings = null)
        {
            return ParseDataTable(Deserialize<IList<T>>(jsonArray, settings));
        }

        public DataTable ParseDataTable<T>(ISheet sheet, JsonSerializerSettings settings = null)
        {
            _ = new DataTable(sheet.SheetName);
            DataTable table = ParseDataTable(Deserialize<IList<T>>(sheet, settings));
            return table;
        }

        public DataTable ParseDataTable<T>(IWorkbook workbook, int sheetIndex, JsonSerializerSettings settings = null)
        {
            ISheet sheet = workbook.GetSheetAt(sheetIndex);
            _ = new DataTable(sheet.SheetName);
            DataTable table = ParseDataTable<T>(sheet, settings);
            return table;
        }

        public DataTable ParseDataTable<T>(IWorkbook workbook, string sheetName, JsonSerializerSettings settings = null)
        {
            ISheet sheet = workbook.GetSheet(sheetName);
            _ = new DataTable(sheet.SheetName);
            DataTable table = ParseDataTable<T>(sheet, settings);
            return table;
        }

        public DataTable ParseDataTable<T>(string fullName, int sheetIndex, JsonSerializerSettings settings = null)
        {
            IWorkbook workbook = WriteWorkbook(fullName);
            ISheet sheet = workbook.GetSheetAt(sheetIndex);
            _ = new DataTable(sheet.SheetName);
            DataTable table = ParseDataTable<T>(sheet, settings);
            return table;
        }

        public DataTable ParseDataTable<T>(string fullName, string sheetName, JsonSerializerSettings settings = null)
        {
            IWorkbook workbook = WriteWorkbook(fullName);
            ISheet sheet = workbook.GetSheet(sheetName);
            _ = new DataTable(sheet.SheetName);
            DataTable table = ParseDataTable<T>(sheet, settings);
            return table;
        }

        public DataSet ParseDataSet<TEntity>(Stream stream)
        {
            try
            {
                using FileStream fs = stream as FileStream;
                IWorkbook workbook = null;
                DataSet ds = new(fs.Name);
                string extension = Path.GetExtension(fs.Name);
                if (stream.CanRead)
                {
                    workbook = WorkbookFactory.Create(fs);
                }

                for (int i = 0; i < workbook.NumberOfSheets; i++)
                {
                    ds.Tables.Add(ParseDataTable<TEntity>(workbook.GetSheetAt(i)));
                }
                return ds;
            }
            catch (Exception) { return new DataSet(); };
        }

        public IWorkbook ReadWorkbook(Stream stream)
        {
            IWorkbook workbook = null;
            try
            {
                using (stream)
                {
                    if (stream.CanRead)
                    {
                        stream.Position = 0;
                        workbook = WorkbookFactory.Create(stream);
                    }
                }
            }
            catch
            {
                return null;
            }
            return workbook;
        }

        public IWorkbook WriteWorkbook(Stream stream)
        {
            using FileStream fs = stream as FileStream;
            IWorkbook workbook = null;
            if (fs.CanRead)
            {
                workbook = WriteWorkbook(fs.Name);
            }
            return workbook;
        }

        public IWorkbook WriteWorkbook(string path)
        {
            IWorkbook workbook = null;
            string extension = Path.GetExtension(path).ToLower();
            switch (extension)
            {
                case ".xlsx":
                    workbook = new XSSFWorkbook();
                    break;
                case ".xls":
                    workbook = new HSSFWorkbook();
                    break;
            }

            return workbook;
        }

        public Stream SaveWorkbook(IWorkbook workbook)
        {
            try
            {
                using Stream stream = new MemoryStream();
                workbook.Write(stream);
                workbook.Close();
                return stream;
            }
            catch (Exception) { return null; }
        }

        public Stream SaveWorkbook(DataTable[] tables, string path)
        {
            try
            {
                using Stream fs = File.Create(path);
                IWorkbook workbook = WriteWorkbook(path);
                for (int i = 0; i < tables.Length; i++)
                {
                    ISheet sheet = workbook.CreateSheet(string.Format("Sheet{0}", i + 1));
                    IRow hrow = sheet.CreateRow(0);
                    for (int h = 0; h < tables[i].Columns.Count; h++)
                    {
                        ICell hcell = hrow.CreateCell(h);
                        hcell.SetCellValue(tables[i].Columns[h].ToString());
                    }

                    for (int r = 0; r < tables[i].Rows.Count; r++)
                    {
                        IRow row = sheet.CreateRow(r + 1);
                        for (int c = 0; c < tables[i].Columns.Count; c++)
                        {
                            ICell cell = row.CreateCell(c);
                            cell.SetCellValue(tables[i].Rows[r][c].ToString());
                        }
                    }
                }
                workbook.Write(fs);
                return fs;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Stream GetWorkbook(DataTable table, string filePath)
        {
            return SaveWorkbook(new DataTable[] { table }, filePath);
        }

        public Stream GetWorkbook<T>(IList<T> data, string filePath)
        {
            return SaveWorkbook(new DataTable[] { ParseDataTable(data) }, filePath);
        }

    }

    public class FileFilter : ValidationAttribute
    {
        public string Allow;
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string extension = (value as IFormFile).FileName.Split('.')[1];
                if (Allow.Contains(extension))
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult(ErrorMessage);
                }
            }
            else
            {
                return ValidationResult.Success;
            }

        }
    }
}
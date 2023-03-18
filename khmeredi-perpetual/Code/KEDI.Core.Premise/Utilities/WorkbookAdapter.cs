using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace KEDI.Core.Premise.Utilities
{
    public interface IWorkbookAdapter
    {
        string[] ContentTypes { get; }
        IWorkbook AddSheet<TEntity>(IEnumerable<TEntity> entities, string sheetName = "");
        IWorkbook AddSheet<TEntity>(IQueryable<TEntity> entities, string sheetName = "");
        IWorkbook AddSheet<TEntity>(TEntity[] entities, string sheetName = "");
        void Write(Stream stream);
    }

    public class WorkbookAdapter : IWorkbookAdapter
    {
        public IWorkbook _workbook = null;
        public string[] ContentTypes
        {
            get
            {
                return new string[]
                {
                    "application/octet-stream",
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "application/vnd.ms-excel"
                };
            }
        }

        public IWorkbook CreateHSSF()
        {
            _workbook = new HSSFWorkbook();
            return _workbook;
        }

        public IWorkbook CreateXSSF()
        {
            _workbook = new XSSFWorkbook();
            return _workbook;
        }

        public IWorkbook AddSheet<TEntity>(IEnumerable<TEntity> entities, string sheetName = "")
        {
            TEntity[] _entities = entities.ToArray();
            return AddSheet(_entities, sheetName);
        }

        public IWorkbook AddSheet<TEntity>(IQueryable<TEntity> entities, string sheetName = "")
        {
            TEntity[] _entities = entities.ToArray();
            return AddSheet(_entities, sheetName);
        }

        public IWorkbook AddSheet<TEntity>(TEntity[] entities, string sheetName = "")
        {
            if (_workbook == null) { _workbook = CreateHSSF(); }
            if (entities == null) { return _workbook; }
            if (entities.Length <= 0) { return _workbook; }
            if (string.IsNullOrWhiteSpace(sheetName))
            {
                sheetName = entities[0].GetType().Name;
            }
            ISheet sheet = _workbook.CreateSheet(sheetName);
            PropertyInfo[] props = typeof(TEntity).GetProperties();
            IRow headerRow = sheet.CreateRow(0);
            for (int i = 0; i < props.Length; i++)
            {
                ICell hcell = headerRow.CreateCell(i);
                hcell.SetCellValue(props[i].Name);
                if (props[i].Name == "No") { SetCellStyle(sheet, _workbook, hcell, i, 5 * 256); }
                else
                {
                    var width = props[i].Name.ToString().Length;
                    SetCellStyle(sheet, _workbook, hcell, i, (width + 20) * 256);
                }
            }

            for (int i = 0; i < entities.Length; i++)
            {
                IRow row = sheet.CreateRow(i + 1);
                TEntity ent = entities[i];
                if (ent == null) { continue; }
                for (int j = 0; j < props.Length; j++)
                {
                    ICell cell = row.CreateCell(j);
                    if (props[j] == null) { continue; }
                    if (props[j].Name.ToString() == "No") { cell.SetCellValue(i + 1); }
                    else
                    {
                        string propVal = GetValue(ent, props[j].Name)?.ToString();
                        var width = props[j].Name.ToString().Length;
                        cell.SetCellValue(propVal);
                    }
                }
            }
            return _workbook;
        }

        public void Write(Stream stream)
        {
            if (_workbook == null) { return; }
            _workbook.Write(stream);
            _workbook.Close();
        }

        public List<T> ToList<T>(ISheet sheet) where T : new()
        {
            return ToArray<T>(sheet).ToList();
        }

        public IDictionary<string, object>[] ToDictionaries(ISheet sheet)
        {
            IDictionary<string, object>[] itemCollection = Array.Empty<Dictionary<string, object>>();
            IRow headerRow = sheet.GetRow(0); //Get Header Row       
            for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++) //Read Excel File
            {
                if (headerRow == null) continue;
                IRow row = sheet.GetRow(i);
                if (row == null) continue;
                if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                IDictionary<string, object> item = new Dictionary<string, object>();
                for (int j = row.FirstCellNum; j < row.LastCellNum; j++)
                {
                    ICell hcell = headerRow.Cells[j];
                    string colName = NoSpace(hcell.StringCellValue);
                    if (hcell == null) { continue; }
                    ICell cell = row.GetCell(j, MissingCellPolicy.CREATE_NULL_AS_BLANK); //Get Empty Cell
                    if (cell.CellType == CellType.Blank)
                    {
                        cell = row.CreateCell(j);
                        object propVal = GetProperty(item, colName);
                        cell.SetCellValue(propVal?.ToString());
                    }
                    hcell.SetCellType(CellType.String);
                    cell.SetCellType(CellType.String);

                    KeyValuePair<string, object> props = new KeyValuePair<string, object>(
                        colName,
                        cell.StringCellValue
                    );
                    item.Add(props);
                }
                itemCollection[i] = item;
            }
            return itemCollection;
        }

        public T[] ToArray<T>(ISheet sheet) where T : new()
        {
            T[] itemCollection = new T[sheet.LastRowNum + 1];
            IRow headerRow = sheet.GetRow(0); //Get Header Row   
            for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++) //Read Excel File
            {
                if (headerRow == null) continue;
                IRow row = sheet.GetRow(i);
                if (row == null) continue;
                if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                T item = new T();
                for (int j = row.FirstCellNum; j < row.LastCellNum; j++)
                {
                    ICell hcell = headerRow.Cells[j];
                    string colName = NoSpace(hcell.StringCellValue);
                    if (hcell == null) { continue; }
                    ICell cell = row.GetCell(j, MissingCellPolicy.CREATE_NULL_AS_BLANK); //Get Empty Cell
                    if (cell.CellType == CellType.Blank)
                    {
                        cell = row.CreateCell(j);
                        object propVal = GetProperty(item, colName);
                        cell.SetCellValue(propVal?.ToString());
                    }
                    hcell.SetCellType(CellType.String);
                    cell.SetCellType(CellType.String);
                    SetProperty(
                        item,
                        colName,
                        cell.StringCellValue
                    );

                }
                itemCollection[i] = item;
            }
            return itemCollection;
        }

        private void SetCellStyle(ISheet sheet, IWorkbook workbook, ICell cell, int Column, int width)
        {
            //Declare Style
            ICellStyle style = workbook.CreateCellStyle();

            //Set Background Color
            style.FillPattern = FillPattern.SolidForeground;
            style.FillForegroundColor = IndexedColors.Yellow.Index;

            //SetFont
            IFont ffont = workbook.CreateFont();
            ffont.FontHeightInPoints = 15;
            style.SetFont(ffont);

            style.BorderLeft = BorderStyle.Medium;
            style.BorderBottom = BorderStyle.Medium;
            style.BorderRight = BorderStyle.Medium;
            style.BorderTop = BorderStyle.Medium;

            //Size of Cell
            sheet.SetColumnWidth(Column, width);
            //Apply Style
            cell.CellStyle = style;
        }


        private object GetValue<TEntity>(TEntity entity, string propName)
        {
            var value = typeof(TEntity).GetProperty(propName)?.GetValue(entity, null);
            return value;
        }
        private void SetValue<TEntity>(TEntity entity, string propName, object value)
        {
            entity.GetType().GetProperty(propName).SetValue(entity, value);

        }
        private void SetProperty<T>(T obj, string key, object value)
        {
            PropertyInfo propInfo = typeof(T).GetProperty(key);
            if (propInfo == null) { return; }
            Type propType = Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType;
            TypeConverter typeConverter = TypeDescriptor.GetConverter(propType);

            object propValue = typeConverter.ConvertFrom(value);
            propInfo.SetValue(obj, propValue, null);
        }

        private object GetProperty<T>(T obj, string key)
        {
            PropertyInfo propInfo = typeof(T).GetProperty(key);
            object value = propInfo.GetValue(obj, null);
            return value;
        }

        private string NoSpace(string value)
        {
            return Regex.Replace(value, "\\s+", string.Empty);
        }
    }
}
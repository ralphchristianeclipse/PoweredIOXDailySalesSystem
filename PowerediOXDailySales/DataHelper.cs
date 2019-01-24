using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.IO;
using DevComponents.DotNetBar.Controls;

namespace PowerediOXDailySales
{
    public static class DataHelper
    {
        public static void ResetDataGridView(this DataGridViewX dataGrid)
        {
            dataGrid.DataSource = null;
            dataGrid.DataMember = "";
            dataGrid.Columns.Clear();

        }
        public static IEnumerable<int> To(this int startValue, int endValue)
        {
            for (int i = startValue; i < endValue; i++)
                yield return i;
        }
        public static void ForEach<T>(this IEnumerable<T> source,Action<T> action)
        {
            foreach (T element in source)
                action(element);
        }
        public static void SetPosition(this DataRow dataRow, int newIndex)
        {
            var newRow = dataRow.Table.NewRow();
            newRow.ItemArray = dataRow.ItemArray;
            dataRow.Table.Rows.Remove(dataRow);
            dataRow.Table.Rows.InsertAt(newRow, newIndex);
        }
        public static void AddColumns<T>(this DataTable dataTable,T defaultValue,params string[] dataColumns)
        {
            dataColumns.ForEach(col => {
                if (!dataTable.Columns.Contains(col))
                    dataTable.Columns.Add(new DataColumn(col, typeof(T), "", MappingType.Attribute) { DefaultValue = defaultValue });
            });
        }
        public static void DeleteColumns(this DataTable dataTable, params string[] dataColumns)
        {
            dataColumns.ForEach(x => dataTable.Columns.Remove(x));
        }
        public static void LoadXML(this DataSet dataSet)
        {
            try
            {
                dataSet.Tables.Clear();
                dataSet.ReadXml($"Database\\{dataSet.DataSetName}.xml", XmlReadMode.Auto);
            }
            catch { }
        }
        public static void SaveXML(this DataSet dataSet)
        {
            if (!Directory.Exists("Database"))
                Directory.CreateDirectory("Database");
            dataSet.WriteXml($"Database\\{dataSet.DataSetName}.xml", XmlWriteMode.WriteSchema);
        }

        public static void SetKey(this DataTable dataTable, params string[] columnNames)
        {
            if (dataTable == null) return;
            var dataColumn = new List<DataColumn>();
            foreach(string col in columnNames)
            {
                var colValue = dataTable.Columns[col];
                colValue.Unique = true;
                dataColumn.Add(colValue);
            }
            dataTable.PrimaryKey = dataColumn.ToArray();
        }

        public static bool IsPrimaryKey(this DataTable dataTable,string columnName)
        {
            return dataTable.PrimaryKey[0].ColumnName == columnName;
        }
        public static void SetProductValue<T>(this DataTable dataTable, T defaultValue, string primaryKey = "Agent")
        {
            dataTable.Columns.Cast<DataColumn>().ToList().ForEach(col =>
            {
                if(col.ColumnName != primaryKey)
                    col.DefaultValue = defaultValue;
            });
        }
        public static DataTable MergeRows(this DataTable dataTable, string primaryKey = "Agent")
        {
            string[] agentValues = dataTable.DefaultView.ToTable(true, primaryKey).Rows.Cast<DataRow>()
                                 .Select(row => row[primaryKey].ToString())
                                 .ToArray();
            var dataTableMerged = dataTable.Clone();

            foreach (string agent in agentValues)
            {
                var tableQuery = $"{primaryKey}='{agent}'";
                var rows = dataTable.Select(tableQuery);
                if (rows.Length > 1)
                {
                    var finalValue = 0;

                    foreach (DataColumn col in dataTable.Columns)
                    {
                        var colName = col.ColumnName;
                        if (colName != primaryKey)
                        {
                            foreach (DataRow row in rows)
                            {
                                finalValue = row[colName].ToInt();
                                var dtFinalrows = dataTableMerged.Select(tableQuery);
                                if (dtFinalrows.Length == 1)
                                {
                                    finalValue += dtFinalrows[0][colName].ToInt();
                                    dtFinalrows[0][colName] = finalValue;
                                }
                                else
                                {
                                    var mergeRow = dataTableMerged.NewRow();
                                    mergeRow[colName] = finalValue;
                                    mergeRow[primaryKey] = agent;
                                    dataTableMerged.Rows.Add(mergeRow);
                                }
                            }
                        }
                    }
                }
                else if (rows.Length == 1)
                {
                    var mergedRow = dataTableMerged.NewRow();
                    dataTable.Columns.Cast<DataColumn>().ToList().ForEach(col => mergedRow[col.ColumnName] = rows[0][col.ColumnName]);
                    dataTableMerged.Rows.Add(mergedRow);
                }
            }
            return dataTableMerged;
        }

        public static int ToInt(this object intString)
        {
            try
            {
                return Convert.ToInt32(intString);
            }
            catch { return 0; }
        }
    }
}


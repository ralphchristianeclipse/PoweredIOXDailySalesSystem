using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using DevComponents.DotNetBar.Metro;
using DevComponents.DotNetBar.Controls;
using DevComponents.Editors.DateTimeAdv;
using DevComponents.DotNetBar;

namespace PowerediOXDailySales
{
    public static class SalesData
    {
        public static DataSet SalesDataSet;
        public static DataSet TempDataSet;
        public static DataGridViewX SalesView;
        public static MonthCalendarAdv SalesCalendar;
        public static List<DateTime> TableDates;
        public static MetroForm SelectedForm;
        public static DataTable CurrentTable;
        public static string CurrentUser = "";
        public static bool NotSaved;
        public static DataGridViewRow TotalSalesGridRow;
        public static DataRow TotalSalesRow;
        public static string MultipleDateSalesChange = "";
        public static string CurrentAdmin = "";

        public static void ResetSalesData()
        {
            SalesDataSet = null;
            TempDataSet = null;
            if (SalesCalendar != null)
            {
                SalesCalendar.MarkedDates = null;
                SalesCalendar.UpdateMarkedDates();
                SalesCalendar.MouseClick -= SalesDelete;
                SalesCalendar.DateSelected -= DateSelect;
            }
            SalesCalendar = null;
            CurrentUser = "";
            NotSaved = false;
            TotalSalesGridRow = null;
            CurrentTable = null;
            if (SalesView != null)
            {
                SalesView.ResetDataGridView();
                SalesView = null;
            }
        }
        public static void AdminSales()
        {
            var AdminDataSet = new DataSet(CurrentUser);
            var table = AdminDataSet.Tables.Add("Managers");
            table.AddColumns("", "Managers", "Total Team Sales");
            foreach (var account in Accounts.AccountsList)
            {
                if (Accounts.IsAdmin(account.Key)) continue;
                var ManagerDataSet = new DataSet(account.Key);
                ManagerDataSet.LoadXML();
                if (ManagerDataSet.Tables.Count < 1)
                {
                    table.Rows.Add(account.Key, 0);
                    continue;
                }
                table.Rows.Add(account.Key,ManagerDataSet.GetAllSales());
            }
            var total = 0;
            foreach(DataRow row in table.Rows)
            {
                total += row["Total Team Sales"].ToInt();
            }
            SelectedForm.GetControl<LabelX>("StatusLabel").Text = $"Total Managers Sales: {total}";
            SalesView = SelectedForm.GetControl<DataGridViewX>("SalesView");
            SalesView.ReadOnly = true;
            SalesView.AllowUserToDeleteRows = false;
            SalesView.DataSource = AdminDataSet;
            SalesView.DataMember = table.TableName;
        }
        public static void SetSalesDataSet(string currentUser, MetroForm tempForm)
        {
            ResetSalesData(); 
            SelectedForm = tempForm;
            CurrentUser = currentUser;
            if (Accounts.IsAdmin(CurrentUser) || currentUser == "")
            {
                CurrentAdmin = CurrentUser;
                AdminSales();
                return;
            }

            SalesView = SelectedForm.GetControl<DataGridViewX>("SalesView");
            SalesView.AllowUserToDeleteRows = true;
            SalesCalendar = SelectedForm.GetControl<MonthCalendarAdv>("SalesCalendar");
            SalesCalendar.SelectionStart = DateTime.Now;
            SalesCalendar.SelectionEnd = DateTime.Now;
            SalesCalendar.MarkedDates = null;
            SalesCalendar.UpdateMarkedDates();
            InitializeSalesData();

            SelectedForm.GetControl<LabelX>("TotalTeamSales").Text = $"Total Team Sales: {SalesDataSet.GetAllSales()}";

            InitializeEvents();
        }
        public static void InitializeSalesData()
        {
            SalesDataSet = new DataSet(CurrentUser);
            SalesDataSet.LoadXML();
            TempDataSet = SalesDataSet;
            SetSalesView();
            TableDates = new List<DateTime>();
            GetAllAvailableTables();
            MultipleDateSalesChange = "";
        }
        public static void DatabaseSave()
        {
            if (!NotSaved)
            {
                ToastNotification.Show(SelectedForm, "Changes has been made");
                return;
            }
            if (CurrentTable != null)
            {
                CurrentTable.SetAgentTotalField(true);
                CurrentTable.GetTotalSalesRow(true);
            }
            SalesDataSet.SaveXML();
            NotSaved = false;
            ToastNotification.Show(SelectedForm, $"Saved Data from {CurrentUser} Sales");
            SetSalesView();
        }
        public static void DateSelect(object sender,DateRangeEventArgs e)
        {
            TempDataSet = SalesDataSet;
            SelectedForm.GetControl<LabelX>("StatusLabel").Text = SetSalesView(e.Start.ToShortDateString());
        }
        public static void SalesDelete(object sender, MouseEventArgs e)
        {
            var selectDate = SalesCalendar.SelectionStart.ToShortDateString();
            if (e.Button == MouseButtons.Right)
            {
                if (SalesDataSet.Tables.Contains(selectDate))
                {
                    if (MessageBoxEx.Show($"Do you want to delete this sales {selectDate} ?", "Delete Sales", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        NotSaved = true;
                        TableDates.Remove(SalesCalendar.SelectionStart);
                        SalesCalendar.MarkedDates = TableDates.ToArray();
                        SalesCalendar.UpdateMarkedDates();
                        SalesDataSet.Tables.Remove(selectDate);
                        ToastNotification.Show(SelectedForm, $"Deleted {selectDate} from the database of {CurrentUser}");
                        if (SelectedForm.GetControl<SwitchButton>("AutoSave").Value)
                            MainForm.Instance.ButtonCodesReturn("Save Changes");
                    }
                }
            }
        }
        public static void InitializeEvents()
        {
            SalesCalendar.MouseClick += SalesDelete;

            SalesCalendar.DateSelected += DateSelect;

            SalesView.UserAddedRow += (_, e) =>
            {
                NotSaved = true;
                if (SelectedForm.GetControl<SwitchButton>("AutoSave").Value)
                    DatabaseSave();
            };
            SalesView.UserDeletedRow += (_, e) =>
            {
                NotSaved = true;
                if (SelectedForm.GetControl<SwitchButton>("AutoSave").Value)
                    DatabaseSave();
            };
            SalesView.DataError += (_, e) =>
            {
                e.ThrowException = false;
            };
            SalesView.CellEndEdit += (_, e) =>
            {
                NotSaved = true;
                if (SelectedForm.GetControl<SwitchButton>("AutoSave").Value) DatabaseSave();
                ShowSales();
                SelectedForm.GetControl<LabelX>("TotalTeamSales").Text = $"All Sales Total: {SalesDataSet.GetAllSales()}";
            };
            SalesView.SelectionChanged += (_, e) =>
            {
                if (SalesView.SelectedCells.Count < 1) return;
                var button = SelectedForm.GetControl<ButtonX>("SelectedProduct");
                button.Text = SalesView.ReadOnly ? "Read Only" : GetSelectedColumn().ColumnName == "Agent" ? "Rename Agent" : GetSelectedColumn().ColumnName;
                SelectedForm.GetControl<LabelX>("SelectedSales").Text = $"Selected Team Sales: {SelectedSales().ToString()}";
                button.ShowSubItems = button.Text != "Rename Agent";
            };
        }
        public static DataColumn GetSelectedColumn()
        {
            return CurrentTable.Columns[SalesView.SelectedCells[0].OwningColumn.Name];
        }

        public static void GetAllAvailableTables()
        {
            SalesDataSet.Tables.Cast<DataTable>().ToList().ForEach(table => AddBoldedDate(table.TableName));
        }
        public static void AddBoldedDate(string dateName)
        {
            var dateValue = DateTime.Parse(dateName);
            if (!TableDates.Contains(dateValue))
                TableDates.Add(dateValue);
            SalesCalendar.Colors.DayMarker.BackColor = Color.GreenYellow;
            SalesCalendar.MarkedDates = TableDates.ToArray();
            SalesCalendar.UpdateMarkedDates();
        }
        public static int SelectedSales()
        {
            var selectedValue = 0;
            foreach (DataGridViewCell cell in SalesView.SelectedCells)
            {
                if(cell.OwningColumn.ValueType == typeof(int))
                selectedValue += cell.Value.ToInt();
            }
            return selectedValue;
        }

        public static DataTable CreateNewSales(string tableName)
        {
            TempDataSet = SalesDataSet;
            if (SelectedForm.GetControl<SwitchButton>("AutoCreate").Value)
            {
                if (TempDataSet.Tables.Contains(tableName)) return TempDataSet.Tables[tableName];
                var lastDate = MultipleDateSalesChange == "" ? CurrentTable == null ? SalesCalendar.SelectionStart.ToShortDateString() : CurrentTable.TableName : MultipleDateSalesChange;
                if(TempDataSet.Tables.Count != 0)
                lastDate = TempDataSet.Tables.Contains(lastDate) ? lastDate : TempDataSet.Tables[0].TableName;
                var table = TempDataSet.Tables.Count != 0
                    ? MessageBoxEx.Show($"Do you want to copy the agents from the last selected date {lastDate}?", "Create New Sales", MessageBoxButtons.YesNo) == DialogResult.Yes
                    ? TempDataSet.Tables[lastDate].Copy() : TempDataSet.Tables[lastDate].Clone()
                    : new DataTable();
                MultipleDateSalesChange = "";
                table.TableName = tableName;
                TempDataSet.Tables.Add(table);
                table.AddColumns("", "Agent");
                table.AddColumns(0, "Product1", "Product2");
                ResetRows(table);
                AddBoldedDate(tableName);
                if (SelectedForm.GetControl<SwitchButton>("AutoSave").Value)
                    DatabaseSave();
                return TempDataSet.Tables[tableName];
            }
            return null;
        }

        public static string GetSalesDateRange()
        {

            var startDate = SalesCalendar.SelectionStart;
            var endDate = SalesCalendar.SelectionEnd;
            if(startDate.ToShortDateString() == endDate.ToShortDateString())
            {
                ToastNotification.Show(SalesView, $"Cannot show sales from {startDate.ToShortDateString()} to {endDate.ToShortDateString()} it's redundant");
                return SetSalesView(startDate.ToShortDateString());
            }
            var dateRange = $"{startDate.ToShortDateString()} - { endDate.ToShortDateString()}";
            var tempDataSet = new DataSet(SalesDataSet.DataSetName);
            var tempTable = new DataTable(dateRange);
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                var table = SalesDataSet.Tables[date.ToShortDateString()];
                if (table == null) continue;
                table.SetAgentTotalField(true);
                table.GetTotalSalesRow(true);
                table.PrimaryKey = null;
                tempTable.Merge(table, false, MissingSchemaAction.Add);
            }
            tempTable = tempTable.MergeRows();
            tempTable.SetKey("Agent");
            tempDataSet.Tables.Add(tempTable);
            TempDataSet = tempDataSet;
            MultipleDateSalesChange = endDate.ToShortDateString();
            SetSalesView(dateRange);
            SalesView.ReadOnly = true;
            SalesView.AllowUserToDeleteRows = false;
            return $"{dateRange}";
        }

        public static void GetTotalSalesRow(this DataTable dataTable, bool remove = false)
        {
            dataTable.SetKey("Agent");
            TotalSalesRow = dataTable.Rows.Find("Total Sales");
            if (TotalSalesRow != null)
                dataTable.Rows.Remove(TotalSalesRow);
            if (!remove)
            {
                TotalSalesRow = dataTable.Rows.Add("Total Sales");
                TotalSalesGridRow = SalesView.Rows.Cast<DataGridViewRow>().Where(row => row?.Cells[0].Value?.ToString() == "Total Sales").FirstOrDefault();
            }
        }

        public static void SetAgentTotalField(this DataTable dataTable, bool remove = false)
        {
            if (dataTable.Columns.Contains("Agent Total"))
                dataTable.Columns.Remove("Agent Total");
            if (!remove)
                dataTable.Columns.Add("Agent Total");
        }
        public static int GetAllSales(this DataSet dataSet)
        {
            var teamSales = 0;
            foreach (DataTable table in dataSet.Tables)
            {
                foreach (DataRow row in table.Rows)
                {
                    if (row["Agent"].ToString() == "Total Sales") continue;
                    foreach (DataColumn col in table.Columns)
                    {
                        if (col.DataType == typeof(int))
                        teamSales += row[col].ToInt();
                    }

                }
            }
            return teamSales;
        }
        public static void AgentTotalSales()
        {
            TotalSalesGridRow.Cells["Agent Total"].Value = 0;
            foreach (DataGridViewRow row in SalesView.Rows)
            {
                if (row == SalesView.Rows[SalesView.Rows.Count - 1]) continue;
                var targetRow = row.Cells["Agent Total"];
                var total = 0;
                foreach (DataGridViewColumn col in SalesView.Columns)
                {
                    if (col.Name == "Agent Total" || col.ValueType != typeof(int)) continue;
                    total += row.Cells[col.Name].Value.ToInt();
                }
                targetRow.Value = total;
                targetRow.Style.BackColor = Color.YellowGreen;
                row.Cells["Agent"].Style.BackColor = Color.DarkCyan;
            }
        }

        public static void TotalSales()
        {
            foreach (DataGridViewColumn col in SalesView.Columns)
            {
                if (col.Name == "Agent") continue;
                var totalValue = 0;
                var tempCell = TotalSalesGridRow.Cells[col.Name];
                foreach (DataGridViewRow row in SalesView.Rows)
                {
                    if (row != TotalSalesGridRow)
                        totalValue += row.Cells[col.Name].Value.ToInt();
                }
                tempCell.Value = totalValue;
                tempCell.Style.BackColor = Color.DarkRed;
            }
        }
        public static void ShowSales()
        {
            CurrentTable.SetAgentTotalField();
            CurrentTable.GetTotalSalesRow();
            if (TotalSalesGridRow == null) return;
            AgentTotalSales();
            TotalSales();
            TotalSalesGridRow.Cells["Agent Total"].Style.BackColor = Color.DarkGreen;
            TotalSalesGridRow.Cells["Agent"].Style.BackColor = Color.DarkBlue;
            TotalSalesGridRow.ReadOnly = true;
            SalesView.Columns["Agent Total"].ReadOnly = true;
        }


        public static string SetSalesView(string tableName = "")
        {
            var tableDataName = tableName == "" ? SalesCalendar.SelectionStart.ToShortDateString() : tableName;
            CurrentTable = TempDataSet.Tables[tableDataName] ?? CreateNewSales(tableDataName);
            if (CurrentTable == null)
            {
                SalesView.ResetDataGridView();
                return "Turn on Create Sales";
            }
            SalesView.ResetDataGridView();
            CurrentTable.SetKey("Agent");
            CurrentTable.SetProductValue(0);
            SalesView.AllowUserToDeleteRows = true;
            SalesView.DataSource = CurrentTable.DataSet;
            SalesView.DataMember = CurrentTable.TableName;
            ShowSales();
            NoSort();
            SalesView.ReadOnly = false;
            SalesView.Columns["Agent"].ReadOnly = true;
            return $"Now Editing: {SalesView.DataMember}";
        }
        public static void NoSort()
        {
            foreach (DataGridViewColumn col in SalesView.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        public static void ResetRows(DataTable table)
        {
            foreach (DataColumn col in table.Columns)
                foreach (DataRow row in table.Rows)
                    if (col.DataType == typeof(int))
                        row[col] = 0;
        }
    }
}

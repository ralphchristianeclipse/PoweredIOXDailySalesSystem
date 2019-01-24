using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Controls;
using DevComponents.DotNetBar.Metro;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace PowerediOXDailySales
{
    using System.ComponentModel;
    using System.Drawing.Printing;
    using System.Text.RegularExpressions;
    using static Accounts;
    using static SalesData;
    public partial class MainForm : MetroForm
    {
        public static List<string> SecretQuestionList = new List<string>() { "Who is your first crush?", "What is your ambition?", "Where do you live?", "When did you graduate highschool?", "Why do you do this?" };
        public static int RetryCount = 3;
        public static List<Control> CachedControlList = new List<Control>();
        public static MainForm Instance = new MainForm();
        public static int RetryTimes = 1;
        public static Timer RetryTimer;
        public static List<Control> AccountControls = new List<Control>();

        public MainForm()
        {
            Instance = this;
            InitializeComponent();
        }
        public void InitializeMainForm()
        {
            foreach (var c in this.GetAllControls())
            {
                if (c is ButtonX)
                {
                    c.Click += ButtonCodes;
                }
            }

            this.Load += (_, e) =>
            {
                this.WindowState = FormWindowState.Maximized;
                this.MinimumSize = this.MaximumSize = this.Size;
            };
            LoginRegisterSlider.IsOpen = false;
            MainStatusPanel.Visible = false;
            RefreshRetryTimer();
            KeyEvents();
            StatusLabel.Text = DateTime.Now.ToLongDateString();
            var timer = new Timer();
            timer.Start();
            timer.Tick += (_, e) =>
            {
                DateTimeLabel.Text = DateTime.Now.ToLongTimeString();
            };
            InitializeMainFormEvents();
        }
        public void InitializeMainFormEvents()
        {
            this.FormClosing += (_, e) =>
            {
                LogOut();
                SplashScreen.Instance.Close();
            };
            MainSlidePanel.IsOpen = false;
            EditorMode.Visible = false;
            ManagerList.Visible = false;
            AccountsView.Visible = false;
            LoginRegisterSlider.Visible = false;
            LoginRegisterSlider.IsOpen = false;
            MainSlidePanel.Visible = false;
            LoginSplitter.ExpandedChanged += (_, e) =>
            {
                AccountStatus.Visible = AccountStatusLabel.Visible = !LoginSplitter.Expanded;
                if (LoginSplitter.Expanded)
                {
                    AccountPanel.VisibleDock(true, RegisterPanel);
                    RegisterButton.Text = "Register";
                }
            };
            EditorMode.ValueChanging += (_, e) =>
            {
                ManagerList.Text = "";
                ManagerList.Items.Clear();
                foreach (var account in AccountsList.Keys)
                {
                    if (!IsAdmin(account))
                        ManagerList.Items.Add(account);
                }
                MainStatusPanel.Visible = !EditorMode.Value;
                SwitchEditorPanels();
                if (AccountsView.Visible)
                {
                    ResetSalesData();
                    ShowAccounts();
                }
            };
            RenameProduct.Click += (_, e) =>
            {
                ButtonCodesReturn(RenameProduct.Text);
            };
            DeleteProduct.Click += (_, e) =>
            {
                ButtonCodesReturn(DeleteProduct.Text);
            };
        }
        public void SwitchEditorPanels()
        {
            SalesView.Visible = ManagerList.Visible = SalesPanel.VisibleDock(!EditorMode.Value, ToolsPanel);
            AccountsView.Visible = AccountPanel.VisibleDock(EditorMode.Value, ToolsPanel);
            AccountPanel.AnimationTime = SalesPanel.AnimationTime = 0;
            AccountPanel.Expanded = SalesPanel.Expanded = false;
            AccountPanel.AnimationTime = SalesPanel.AnimationTime = 250;
            AccountPanel.Expanded = SalesPanel.Expanded = true;
        }
        public int CheckPenalty()
        {
            var fileName = "Penalty.txt";
            var result = 0;
            if (File.Exists(fileName))
            {
                var timeNow = DateTime.Now;
                var penaltyTime = DateTime.Parse(File.ReadAllText(fileName));
                TimeSpan span = penaltyTime - timeNow;
                result = span.TotalSeconds > 1 ? span.TotalSeconds.ToInt() : 0;
            }
            return result;
        }
        public int RefreshRetryTimer()
        {
            RetryTimer?.Dispose();
            RetryTimer = new Timer { Interval = 1000 };
            var penaltyTime = CheckPenalty();
            RetryTimer.Enabled = true;
            if (penaltyTime <= 0)
            {
                RetryTimer.Enabled = false;
                RetryTimer.Stop();
                return 0;
            }
            RetryTimer.Start();
            RetryTimer.Tick += (sender, e) =>
            {
                StatusLabel.Text = penaltyTime > 0 ? penaltyTime == 1 ? $"Wait for {--penaltyTime} second to login again" : $"Wait for {--penaltyTime} seconds to login again" : "Login Here";
                LoginButton.Enabled = penaltyTime <= 0;
                if (penaltyTime <= 0)
                    ResetRetryTimer();
            };

            return penaltyTime;
        }
        public void ResetRetryTimer()
        {
            LoginButton.Enabled = true;
            if (RetryTimer.Enabled)
            {
                RetryCount = 3;
                ToastNotification.Show(this, "Penalty timer has been removed");
            }
            File.Delete("Penalty.txt");
            StatusLabel.Text = "Login Again";
            RetryTimer.Dispose();
        }
        public void ButtonCodes(object sender, EventArgs e)
        {
            ButtonCodesReturn(((dynamic)sender).Text);
        }
        public void ButtonCodesReturn(string buttonCode)
        {
            switch (buttonCode)
            {
                case "Enter Here":
                    LoginRegisterSlider.Visible = true;
                    EnterHere.Visible = false;
                    LoginRegisterSlider.OpenBounds = new Rectangle(this.Width - (LoginRegisterSlider.Width + 15), 3, LoginRegisterSlider.Width, this.DisplayRectangle.Height - StatusPanel.Height);
                    LoginRegisterSlider.IsOpen = true;
                    break;
                case "Exit Login":
                    LoginRegisterSlider.IsOpen = false;
                    EnterHere.Visible = true;
                    break;
                case "Log Out":
                    LogOut();
                    ButtonCodesReturn("Exit Login");
                    MainSlidePanel.IsOpen = MainStatusPanel.Visible = false;
                    StatusPanel.Parent = this;
                    DateTimeLabel.Visible = true;
                    break;
                case "Login":
                    if (Username.Text != "" && Password.Text != "")
                    {
                        var user = Username.Text;
                        var pass = Password.Text;

                        GetAllAccounts();
                        if (IsAccountExist(user))
                        {
                            if (RetryCount > 0 && RefreshRetryTimer() <= 0)
                            {
                                if (AccountsList[user.ToLower()] == pass)
                                {

                                    if (CheckUserStatus(user) || CheckUserStatus(user, "admin"))
                                    {
                                        ShowManagersSales.Visible = DateTimeLabel.Visible = LoginRegisterSlider.IsOpen = ManagerList.Visible = AccountsView.Visible = EditorMode.VisibleDock(false, ToolsPanel);
                                        MainStatusPanel.Visible = true;
                                        CurrentAdmin = "";
                                        AccountPanel.VisibleDock(false, ToolsPanel);
                                        if (IsAdmin(user))
                                        {
                                            CurrentAdmin = user;
                                            EditorMode.VisibleDock(true, ToolsPanel, DockStyle.Top);
                                            EditorMode.SetValueAndAnimate(true, eEventSource.Code);
                                        }
                                        else
                                        {
                                            SalesView.Visible = SalesPanel.VisibleDock(true, ToolsPanel);
                                            EditorMode.VisibleDock(false, ToolsPanel);
                                            SetUserStatus(user, "Online");
                                            SetSalesDataSet(user, this);
                                        }

                                        MainSlidePanel.OpenBounds = new Rectangle(0, 0, this.DisplayRectangle.Width, this.DisplayRectangle.Height);
                                        MainSlidePanel.IsOpen = true;
                                        StatusPanel.Parent = MainSlidePanel;
                                        ManagerNameLabel.Text = $"{user} | {AccountTable.Rows.Find(user)["ManagerName"].ToString()}";
                                        foreach (Control c in this.GetAllControls())
                                        {
                                            if (c is TextBoxX)
                                                c.Text = "";
                                        }
                                    }
                                    else
                                        ToastNotification.Show(LoginPanel, $"{user} has already logged in");
                                }
                                else
                                {
                                    ToastNotification.Show(LoginPanel, $"Invalid Password: Retries Left {RetryCount--}");
                                }
                            }
                            else
                            {
                                if (!RetryTimer.Enabled)
                                {
                                    File.WriteAllText("Penalty.txt", DateTime.Now.AddMinutes(RetryTimes++).ToString());
                                    RefreshRetryTimer();
                                }
                            }
                        }
                        else
                        {
                            ToastNotification.Show(LoginPanel, $"{user} not registered");
                        }
                    }
                    else
                        ToastNotification.Show(LoginPanel, "Input all fields");
                    break;
                case "Register":
                    GetAllAccounts();
                    if (!IsAccountExist(AccountUsername.Text))
                    {
                        if (CheckAllFields())
                        {
                            ToastNotification.Show(this, $"Registered: {AccountUsername.Text}");

                            AccountTable.Rows.Add(AccountUsername.Text, AccountPassword.Text, AccountManagerName.Text, AccountSecretQuestion.Text, AccountSecretAnswer.Text, "Offline");
                            GetAllAccounts();
                            foreach (Control c in this.GetAllControls())
                            {
                                if (c is TextBoxX)
                                    c.Text = "";
                            }
                            if (AccountStatus.Visible)
                            {
                                ShowAccounts();
                                ToastNotification.Show(this, "Reverting back to edit mode");
                            }
                        }
                        else
                            ToastNotification.Show(this, "Input all Fields");
                    }
                    else
                        ToastNotification.Show(this, $"Account Already Registered: {AccountUsername.Text}");
                    break;
                case "Create Account Mode":
                    AccountControls.ClearBindings();
                    AccountUsername.Enabled = AccountStatus.Enabled = true;
                    foreach (Control c in AccountPanel.GetAllControls())
                    {
                        if (c is TextBoxX)
                            c.Text = "";
                    }
                    RegisterButton.Text = "Register";
                    ToastNotification.Show(this, "Create Account Mode activated please enter credentials on the blank fields then press register");
                    break;
                case "Forgot Password":
                    if (Username.Text == "")
                    {
                        ToastNotification.Show(LoginPanel, "Enter a registered account then click this again");
                        return;
                    }
                    var rows = AccountTable.Select($"Username='{Username.Text}'");
                    if (rows.Length != 0)
                    {
                        var row = rows[0];
                        var renameVal = "";
                        var RenameInputBox = new MetroForm();
                        RenameInputBox.Height = 150;
                        RenameInputBox.Width = 250;
                        RenameInputBox.StartPosition = FormStartPosition.CenterScreen;
                        RenameInputBox.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        RenameInputBox.MaximizeBox = false;
                        RenameInputBox.Controls.Add(new LabelX { AutoSize = true, Name = "ForgotPass", Text = row["SecretQuestion"].ToString(), Location = new Point(10, 10) });
                        RenameInputBox.Controls.Add(new TextBoxX { Size = new Size(200, 100), PasswordChar = '*', AutoSize = true, Name = "InputField", Location = new Point(10, 30) });
                        RenameInputBox.Controls.Add(new ButtonX { AutoSize = true, Name = "Submit", Text = "Submit", Location = new Point(10, 60) });
                        RenameInputBox.Text = "Forgot Password";
                        RenameInputBox.GetControl<ButtonX>("Submit").Click += (_, ee) =>
                        {
                            renameVal = RenameInputBox.GetControl<TextBoxX>("InputField").Text;
                            RenameInputBox.Close();
                        };
                        RenameInputBox.ShowDialog();
                        var answerCheck = renameVal == row["SecretAnswer"].ToString();
                        ToastNotification.Show(LoginPanel, answerCheck ? $"Your password is: {row["Password"].ToString()}" : RetryCount > 0 ? $"Invalid Secret Answer: Retries Left {RetryCount--}" : "Invalid Secret Answer: PENALTY!");

                        if (RetryCount <= 0 && !answerCheck && !RetryTimer.Enabled)
                        {
                            File.WriteAllText("Penalty.txt", DateTime.Now.AddMinutes(RetryTimes++).ToString());
                            RefreshRetryTimer();
                        }
                        if(answerCheck)
                            Password.Text = row["Password"].ToString();
                    }
                    else
                        ToastNotification.Show(LoginPanel, $"{Username.Text} doesn't exist");
                    break;
                case "Save Changes":
                    if (SalesView.Visible)
                        DatabaseSave();
                    if (AccountsView.Visible)
                    {
                        GetAllAccounts();
                        ToastNotification.Show(AccountsView, "Saved changes to accounts");
                    }
                    break;
                case "Rename Product":
                    if (SalesView.ReadOnly)
                    {
                        ToastNotification.Show(this, ($"Can't rename the product because you are on viewing mode only"));
                        return;
                    }
                    DataColumn col = CurrentTable.Columns[SelectedProduct.Text];
                    if (col == null)
                    {
                        ToastNotification.Show(this, "No selected product");
                        return;
                    }

                    if (SelectedProduct.Text == "Rename Agent")
                    {
                        ToastNotification.Show(this, ($"Can't rename the agent fields: {SelectedProduct.Text}"));
                        return;
                    }
                    var newName = "";
                    var InputBox = new MetroForm();
                    InputBox.Height = 150;
                    InputBox.Width = 250;
                    InputBox.StartPosition = FormStartPosition.CenterScreen;
                    InputBox.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    InputBox.MaximizeBox = false;
                    InputBox.Controls.Add(new LabelX { AutoSize = true, Name = "ForgotPass", Text = $"Enter New Name for {SelectedProduct.Text}", Location = new Point(10, 10) });
                    InputBox.Controls.Add(new TextBoxX { Size = new Size(200, 100), AutoSize = true, Name = "InputField", Location = new Point(10, 30) });
                    InputBox.Controls.Add(new ButtonX { AutoSize = true, Name = "Submit", Text = "Submit", Location = new Point(10, 60) });
                    InputBox.Text = "Rename Product";
                    InputBox.GetControl<ButtonX>("Submit").Click += (_, ee) =>
                    {
                        newName = InputBox.GetControl<TextBoxX>("InputField").Text;
                        InputBox.Close();
                    };
                    InputBox.ShowDialog(); if (CurrentTable.Columns.Contains(newName))
                    {
                        ToastNotification.Show(this, $"{newName} already exist in the sales");
                        return;
                    }
                    if (newName == "")
                    {
                        ToastNotification.Show(this, "No value has been set");
                        return;
                    }
                    col.ColumnName = newName;
                    SetSalesView();
                    NotSaved = true;
                    break;
                case "Delete Product":
                    if (SalesView.ReadOnly)
                    {
                        ToastNotification.Show(this, ($"Can't delete the product because you are on viewing mode only"));
                        return;
                    }
                    if (SelectedProduct.Text == "Rename Agent")
                    {
                        ToastNotification.Show(this, ($"Can't delete the agent field: {SelectedProduct.Text}"));
                        return;
                    }
                    if (!CurrentTable.Columns.Contains(SelectedProduct.Text))
                    {
                        ToastNotification.Show(this, ($"Sales doesn't contain this product: {SelectedProduct.Text}"));
                        return;
                    }
                    if (MessageBoxEx.Show($"Are you sure you want to delete: {SelectedProduct.Text} ?", "Delete Column", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        CurrentTable?.DeleteColumns(SelectedProduct.Text);
                        SetSalesView();
                        NotSaved = true;
                    }
                    break;
                case "Show Sales Range":
                    if (CurrentTable == null)
                    {
                        ToastNotification.Show(this, "You have not selected a manager");
                        return;
                    }
                    StatusLabel.Text = $"{GetSalesDateRange()}";
                    break;
                case "Rename Agent":
                    if (SalesView.CurrentCell == null) return;
                    var selectedAgent = SalesView.CurrentCell.Value.ToString();
                    if (selectedAgent == "Total Sales") return;
                    if (MessageBoxEx.Show($"Do you want to rename the selected agent : {selectedAgent}", "Rename Agent", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        var newAgentName = "";
                        var AgentBox = new MetroForm();
                        AgentBox.Height = 150;
                        AgentBox.Width = 250;
                        AgentBox.StartPosition = FormStartPosition.CenterScreen;
                        AgentBox.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        AgentBox.MaximizeBox = false;
                        AgentBox.Controls.Add(new LabelX { AutoSize = true, Name = "ForgotPass", Text = $"Enter New Name for {SelectedProduct.Text} : {selectedAgent}", Location = new Point(10, 10) });
                        AgentBox.Controls.Add(new TextBoxX { Size = new Size(200, 100), AutoSize = true, Name = "InputField", Location = new Point(10, 30) });
                        AgentBox.Controls.Add(new ButtonX { AutoSize = true, Name = "Submit", Text = "Submit", Location = new Point(10, 60) });
                        AgentBox.Text = "Rename Agent";
                        AgentBox.GetControl<ButtonX>("Submit").Click += (_, ee) =>
                        {
                            newAgentName = AgentBox.GetControl<TextBoxX>("InputField").Text;
                            AgentBox.Close();
                        };
                        AgentBox.ShowDialog();
                        if (CurrentTable.Rows.Contains(newAgentName))
                        {
                            ToastNotification.Show(this, $"{newAgentName} already exist in the sales agent list");
                            return;
                        }
                        if (newAgentName == "")
                        {
                            ToastNotification.Show(this, "No value has been set");
                            return;
                        }
                        SalesView.CurrentCell.Value = newAgentName;
                        ToastNotification.Show(this, $"Renamed {selectedAgent} to {newAgentName}");
                        SetSalesView();
                        NotSaved = true;
                    }
                    break;
                case "Show Managers Sales":
                    if (CurrentAdmin == "") return;
                    SetSalesDataSet(CurrentAdmin, this);
                    MainStatusPanel.Visible = false;
                    break;
                    /*  case "Print":
                          var salesReport = new CrystalSalesReport();
                          var salesReportViewer = new CrystalReportViewer();
                          salesReport.Database.Tables[0].SetDataSource();
                          salesReport.Refresh();
                          salesReportViewer.Parent = this;
                          salesReportViewer.Dock = DockStyle.Fill;
                          salesReportViewer.ReportSource = salesReport;
                          salesReportViewer.Refresh();
                          salesReportViewer.RefreshReport();
                          salesReportViewer.Show();
                          break; */
            }
        }
        public void ShowAccounts()
        {
            AccountsView.Visible = true;
            AccountsView.ResetDataGridView();
            AccountsView.DataSource = AccountsDataSet;
            AccountsView.DataMember = AccountTable.TableName;
            RegisterButton.Text = "Create Account Mode";
            StatusLabel.Text = "Account Editing Mode";
            BindFields(AccountsView.Rows[0]);
        }

        public void LogOut()
        {
            if (!IsAdmin(CurrentUser))
                SetUserStatus(CurrentUser, "Offline");
            if (NotSaved)
            {
                if (MessageBoxEx.Show("Save your work or not?", "Save", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    DatabaseSave();
                }
            }
            AccountsView.ResetDataGridView();
            SalesView.ResetDataGridView();
            ManagerNameLabel.Text = "JJETSystems";
            StatusLabel.Text = DateTime.Now.ToLongDateString();
            AccountPanel.VisibleDock(true, RegisterPanel);
            RegisterButton.Text = "Register";
        }
        public bool CheckAllFields()
        {
            foreach (Control ctrl in AccountPanel.GetAllControls())
            {
                if (ctrl is TextBoxX && (ctrl.Text == "" || ctrl.Text == ctrl.Name))
                    return false;
            }
            return true;
        }

        public void KeyEvents()
        {
            Username.KeyDown += (_, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (IsAdmin(Username.Text) && AccountsList[Username.Text] == Password.Text)
                        ResetRetryTimer();
                    ButtonCodesReturn("Login");
                }
            };
            Password.KeyDown += (_, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (IsAdmin(Username.Text) && AccountsList[Username.Text] == Password.Text)
                        ResetRetryTimer();
                    ButtonCodesReturn("Login");
                }
            };
            AddProduct.KeyDown += (_, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (AddProduct.Text == "")
                    {
                        ToastNotification.Show(this, "Input product name first");
                        return;
                    }
                    if (SalesView.ReadOnly)
                    {
                        ToastNotification.Show(this, ($"Can't add the product because you are on viewing mode only"));
                        return;
                    }
                    if (CurrentTable == null)
                    {
                        ToastNotification.Show(SalesView, "No Available Sales or Selected Sales");
                        return;
                    }
                    if (CurrentTable.Columns.Contains(AddProduct.Text))
                    {
                        ToastNotification.Show(SalesView, $"{AddProduct.Text} already existing");
                        return;
                    }
                    if (MessageBoxEx.Show($"Do you want to add this product: {AddProduct.Text}", "Add Product", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        CurrentTable.AddColumns(0, AddProduct.Text);
                        SetSalesView();
                        NotSaved = true;
                    }
                }
            };
            AddAgent.KeyDown += (_, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (AddAgent.Text == "")
                    {
                        ToastNotification.Show(this, "Input agent name first");
                        return;
                    }
                    if (SalesView.ReadOnly)
                    {
                        ToastNotification.Show(this, ($"Can't add the agent because you are on viewing mode only"));
                        return;
                    }
                    if (Regex.IsMatch(AddAgent.Text, "[0-9]"))
                    {
                        ToastNotification.Show(SalesView, "Numeric characters are invalid");
                        return;
                    }
                    if (CurrentTable == null)
                    {
                        ToastNotification.Show(SalesView, "No Available Sales or Selected Sales");
                        return;
                    }
                    if (CurrentTable.Rows.Contains(AddAgent.Text))
                    {
                        ToastNotification.Show(SalesView, $"{AddAgent.Text} is already existing");
                        return;
                    }
                    if (MessageBoxEx.Show($"Do you want to add this agent: {AddAgent.Text}", "Add Agent", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        CurrentTable.Rows.Add(AddAgent.Text);
                        SetSalesView();
                        NotSaved = true;
                    }
                }
            };
            AccountControls.AddRange(new Control[] { AccountUsername, AccountPassword, AccountManagerName, AccountSecretAnswer, AccountSecretQuestion, AccountStatus });
            ManagerList.SelectedValueChanged += (_, ee) =>
            {
                AutoCreate.Value = false;
                AutoCreate.Refresh();
                SalesCalendar.SelectionStart = DateTime.Now;
                SalesCalendar.SelectionEnd = DateTime.Now;
                SetSalesDataSet(ManagerList.SelectedItem.ToString(), this);
                StatusLabel.Text = CurrentUser;
                MainStatusPanel.Visible = true;
            };
            AccountSecretQuestion.Items.AddRange(SecretQuestionList.ToArray());
            AccountStatus.Items.AddRange(new string[] { "admin", "Online", "Offline" });
            AccountsView.ReadOnly = true;
            AccountsView.CellClick += (_, ee) =>
            {
                if (ee.RowIndex < 0) return;
                if (RegisterButton.Text == "Register")
                {
                    ToastNotification.Show(this, "Can't edit data for now because you're in Create Account Mode");
                    return;
                }
                BindFields(AccountsView.Rows[ee.RowIndex]);

            };
            AutoSave.ValueChanged += (_, e) =>
            {
                SaveButton.Enabled = !AutoSave.Value;
            };
            AccountsView.UserDeletedRow += (_, e) =>
            {
                if (AutoSave.Value) ButtonCodesReturn("Save Changes");
            };
            AccountsView.UserAddedRow += (_, e) =>
            {
                if (AutoSave.Value) ButtonCodesReturn("Save Changes");
            };
            AccountsView.CellValueChanged += (_, e) =>
            {
                if (AutoSave.Value) ButtonCodesReturn("Save Changes");
            };
            AccountsView.DataError += (_, e) =>
            {
                e.ThrowException = false;
            };
        }

        public void BindFields(DataGridViewRow row)
        {
            var cells = row.Cells;
            AccountControls.ClearBindings();
            AccountUsername.Enabled = AccountStatus.Enabled = true;
            if (cells["Username"].Value.ToString() == "admin")
            {
                AccountUsername.Enabled = AccountStatus.Enabled = false;
                ToastNotification.Show(this, $"Status and Username editing is disabled for the default admin");
            }
            StatusLabel.Text = $"Selected Account: {cells["Username"].Value.ToString()} | Status: {cells["Status"].Value.ToString()}";
            AccountStatus.DataBindings.Add("SelectedItem", cells["Status"], "Value", true, DataSourceUpdateMode.OnValidation);
            AccountSecretQuestion.DataBindings.Add("SelectedItem", cells["SecretQuestion"], "Value", true, DataSourceUpdateMode.OnValidation);
            AccountUsername.DataBindings.Add("Text", cells["Username"], "Value", true, DataSourceUpdateMode.OnValidation);
            AccountPassword.DataBindings.Add("Text", cells["Password"], "Value", true, DataSourceUpdateMode.OnValidation);
            AccountManagerName.DataBindings.Add("Text", cells["ManagerName"], "Value", true, DataSourceUpdateMode.OnValidation);
            AccountSecretAnswer.DataBindings.Add("Text", cells["SecretAnswer"], "Value", true, DataSourceUpdateMode.OnValidation);
        }
        private void LoginPanel_Click(object sender, EventArgs e)
        {

        }
    }
}
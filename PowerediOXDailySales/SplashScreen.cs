using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using System.Threading;
using System.Threading.Tasks;

namespace PowerediOXDailySales
{
    public partial class SplashScreen : OfficeForm
    {
        public static BackgroundWorker ProgressWorker = new BackgroundWorker();
        public static SplashScreen Instance;
        public SplashScreen()
        {
            Instance = this;
            InitializeComponent();
            this.Load += (s, a) =>
            {
                List<bool> bools = new List<bool>();
                bools.AddRange(new bool[] { false, false, false });
                ProgressWorker.WorkerReportsProgress = true;
                ProgressWorker.DoWork += (_, e) =>
                {
                    for (int i = 0; i < 103; i++)
                    {
                        Thread.Sleep(50);
                        ProgressWorker.ReportProgress(i);
                        if(i < 30)
                            ProgressLabel.Invoke((MethodInvoker)delegate {
                                ProgressLabel.Text = $"Loading Sales {i}%";
                            });
                        if (i > 30 && i < 60)
                        {
                            ProgressLabel.Invoke((MethodInvoker)delegate {
                                ProgressLabel.Text = $"Initializing Accounts {i}%";
                                });
                            if (!bools[0])
                            {
                                bools[0] = true;
                                Accounts.InitializeDatabase();
                            }
                        }
                        if (i > 60 && i < 90)
                        {
                            ProgressLabel.Invoke((MethodInvoker)delegate {
                                ProgressLabel.Text = $"Getting Accounts {i}%";
                            });
                            if (!bools[1])
                            {
                                bools[1] = true;
                                Accounts.GetAllAccounts();
                            }
                        }
                        if (i > 90 && i < 101)
                        {
                            Thread.Sleep(250);
                            ProgressLabel.Invoke((MethodInvoker)delegate
                            {
                                ProgressLabel.Text = $"Loading Main Form {i}%";
                            });
                            if (!bools[2])
                            {
                                bools[2] = true;
                                this.Invoke((MethodInvoker)delegate
                               {
                                   MainForm.Instance.InitializeMainForm();
                               });
                            }
                        }
                        if (i >= 101)
                        {
                            ProgressLabel.Invoke((MethodInvoker)delegate {
                                ProgressLabel.Text = $"Welcome to Windows 11";
                            });
                            Thread.Sleep(1000);
                        }
                    }
                };
                ProgressWorker.ProgressChanged += (_, e) =>
                {
                    ProgressLoader.Value = e.ProgressPercentage;
                    DummyProgress.Value = e.ProgressPercentage;
                };
                ProgressWorker.RunWorkerCompleted += (_, e) =>
                {
                    this.Hide();
                    MainForm.Instance.Show();
                };
                ProgressWorker.RunWorkerAsync();
            };
        }
    }
}
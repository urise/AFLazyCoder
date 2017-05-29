using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace AFLazyCoder
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void ConfigureNLog()
        {
            // this is because RichTextBox target from config file opens new form
            // so it should be configured from the code
            var target = new RichTextBoxTarget()
            {
                Name = "Rich",
                ControlName = "txtLog",
                FormName = "MainForm",
                UseDefaultRowColoringRules = true,
                AutoScroll = true,
                Layout = "${message}"
            };

            var xlc = new XmlLoggingConfiguration("NLog.config");
            xlc.AddTarget("Rich", target);
            xlc.LoggingRules.Add(new LoggingRule("*", LogLevel.Info, target));
            LogManager.Configuration = xlc;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            ConfigureNLog();
        }
    }
}

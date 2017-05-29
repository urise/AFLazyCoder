using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using BusinessLayer.Helpers;
using BusinessLayer.Managers;
using NLog;

namespace AFLazyCoder.UserControls
{
    public partial class DbRepositoryControl : UserControl
    {
        #region Properties and Variables

        private static Logger _logger = LogManager.GetLogger("DbRepositoryControl");

        private IManager _manager;
        public IManager Manager
        {
            get
            {
                if (_manager == null)
                {
                    _manager = new CloudServiceManager();
                    _manager.Init(AppConfiguration.AlphaFrontierSourceFolder);
                }
                return _manager;
            }
        }

        private bool ClearInputOnFocus { get; set; }

        private enum WorkingMode
        {
            DbRepository = 0,
            CloudService
        }

        #endregion

        #region Constructors and Initializers

        public DbRepositoryControl()
        {
            InitializeComponent();
        }

        private void Init()
        {
            if (String.IsNullOrEmpty(AppConfiguration.AlphaFrontierSourceFolder))
                return;
            _manager = GetManagerByMode((WorkingMode) cmbMode.SelectedIndex);
            _manager.Init(AppConfiguration.AlphaFrontierSourceFolder);

            LoadClassList();
            if (_manager is CloudServiceManager)
            {
                btnAddClass.Text = @"Add Services";
            }
            else if (_manager is DbRepositoryManager)
            {
                btnAddClass.Text = @"Add DbRepository";
            }
        }

        private void DbRepositoryControl_Load(object sender, EventArgs e)
        {
            cmbMode.SelectedIndex = 0;
            Init();
        }

        #endregion

        #region Auxilliary Methods

        private void SetContolsAvailability(bool isEnabled)
        {
            btnAddClass.Enabled = isEnabled;
            btnAddMethods.Enabled = isEnabled;
            txtInput.Enabled = isEnabled;
        }

        public void LoadClassList()
        {
            lbClassesList.Items.Clear();
            List<string> list = Manager.GetClassList();
            if (list == null)
            {
                SetContolsAvailability(false);
                MessageBox.Show(@"It seems that source folder is not found. Please check folder in app config");
            }
            else
            {
                lbClassesList.Items.AddRange(list.ToArray());
            }
        }

        private IManager GetManagerByMode(WorkingMode mode)
        {
            switch (mode)
            {
                case WorkingMode.DbRepository:
                    return new DbRepositoryManager();
                case WorkingMode.CloudService:
                    return new CloudServiceManager();
            }
            throw new Exception("Unknown working mode");
        }

        #endregion

        #region Event Handlers

        private void btnAddMethods_Click(object sender, EventArgs e)
        {
            if (lbClassesList.SelectedItem == null)
            {
                _logger.Error(@"Choose class first");
                return;
            }
            Manager.AddMethods(lbClassesList.SelectedItem.ToString(), txtInput.Lines);
        }

        private bool ClassExists(string className)
        {
            return lbClassesList.Items.Contains(className);
        }

        private void btnAddClass_Click(object sender, EventArgs e)
        {
            foreach(string serviceName in txtInput.Lines.Select(r => r.Trim()))
            {
                if (String.IsNullOrEmpty(serviceName)) continue;

                if (ClassExists(serviceName))
                {
                    _logger.Error(serviceName + " already exists");
                    continue;
                }

                Manager.AddClass(serviceName);
            }
            LoadClassList();
        }

        private void cmbMode_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cmbMode.SelectedIndex == (int)WorkingMode.CloudService && ! (_manager is CloudServiceManager))
            {
                ClearInputOnFocus = true;
                txtInput.Text = File.ReadAllText(Path.Combine("HelpFiles", "CloudServiceHelp.txt"));
            }
            Init();
        }

        private void txtInput_Enter(object sender, EventArgs e)
        {
            if (ClearInputOnFocus)
            {
                txtInput.Text = string.Empty;
                ClearInputOnFocus = false;
            }
        }

        #endregion
    }
}

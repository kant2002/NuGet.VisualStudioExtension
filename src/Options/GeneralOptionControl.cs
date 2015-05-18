﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Windows.Forms;
using NuGet.Configuration;
using NuGet.PackageManagement.VisualStudio;
using Microsoft.VisualStudio.Shell;

namespace NuGet.Options
{
    public partial class GeneralOptionControl : UserControl
    {
        private readonly IProductUpdateSettings _productUpdateSettings;
        private readonly ISettings _settings;
        private bool _initialized;

        public GeneralOptionControl()
        {
            InitializeComponent();

            _productUpdateSettings = ServiceLocator.GetInstance<IProductUpdateSettings>();
            Debug.Assert(_productUpdateSettings != null);

            _settings = ServiceLocator.GetInstance<ISettings>();
            Debug.Assert(_settings != null);
            // Starting from VS11, we don't need to check for updates anymore because VS will do it.
            Controls.Remove(updatePanel);
        }

        internal void OnActivated()
        {
            if (!_initialized)
            {
                try
                {
                    var packageRestoreConsent = new PackageRestoreConsent(_settings);
                    packageRestoreConsentCheckBox.Checked = packageRestoreConsent.IsGrantedInSettings;
                    packageRestoreAutomaticCheckBox.Checked = packageRestoreConsent.IsAutomatic;
                    packageRestoreAutomaticCheckBox.Enabled = packageRestoreConsentCheckBox.Checked;
                }
                catch(InvalidOperationException)
                {
                    MessageHelper.ShowErrorMessage("Can't access NuGet.Config or NuGet.Config is malformed, Please check NuGet.Config", Resources.ErrorDialogBoxTitle);
                }
                checkForUpdate.Checked = _productUpdateSettings.ShouldCheckForUpdate;
            }

            _initialized = true;
        }

        internal bool OnApply()
        {
            _productUpdateSettings.ShouldCheckForUpdate = checkForUpdate.Checked;

            try
            {
                var packageRestoreConsent = new PackageRestoreConsent(_settings);
                packageRestoreConsent.IsGrantedInSettings = packageRestoreConsentCheckBox.Checked;
                packageRestoreConsent.IsAutomatic = packageRestoreAutomaticCheckBox.Checked;
            }
            catch (InvalidOperationException)
            {              
                MessageHelper.ShowErrorMessage("Can't access NuGet.Config or NuGet.Config is malformed, Please check NuGet.Config", Resources.ErrorDialogBoxTitle);
                return false;
            }
            return true;
        }

        internal void OnClosed()
        {
            _initialized = false;
        }

        private void OnClearPackageCacheClick(object sender, EventArgs e)
        {
            //not implement now
        }

        private void OnBrowsePackageCacheClick(object sender, EventArgs e)
        {
            //not impement now
        }

        private void packageRestoreConsentCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            packageRestoreAutomaticCheckBox.Enabled = packageRestoreConsentCheckBox.Checked;
            if (!packageRestoreConsentCheckBox.Checked)
            {
                packageRestoreAutomaticCheckBox.Checked = false;
            }
        }
    }
}

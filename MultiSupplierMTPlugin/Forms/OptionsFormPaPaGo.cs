﻿using MultiSupplierMTPlugin.Service;
using MemoQ.MTInterfaces;
using System;
using System.Windows.Forms;
using LLK = MultiSupplierMTPlugin.Localized.LocalizedKeyEnum;
using LLH = MultiSupplierMTPlugin.Localized.LocalizedHelper;

namespace MultiSupplierMTPlugin.Forms
{
    public partial class OptionsFormPaPaGo : Form
    {
        private MultiSupplierMTOptions options;

        private IEnvironment environment;

        private bool checkSuccess = false;
        
        public OptionsFormPaPaGo(MultiSupplierMTOptions options, IEnvironment environment)
        {
            InitializeComponent();

            this.options = options;
            this.environment = environment;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            localized();

            loadOptions();

            bindOptionsChangedEvent();
        }

        private void localized()
        {
            Text = LLH.G(LLK.OptionFormPaPaGo);

            labelClientID.Text = LLH.G(LLK.OptionFormPaPaGo_LabelClientID);
            labelClientSecret.Text = LLH.G(LLK.OptionFormPaPaGo_LabelClientSecret);

            linkLabelCheck.Text = LLH.G(LLK.OptionForm_LinkLabelCheck);

            buttonOK.Text = LLH.G(LLK.OptionForm_ButtonOK);
            buttonCancel.Text = LLH.G(LLK.OptionForm_ButtonCancel);
        }

        private void loadOptions()
        {
            textBoxClientID.Text = options.SecureSettings.PaPaGoSecureOptions.ClientID;
            textBoxClientSecret.Text = options.SecureSettings.PaPaGoSecureOptions.ClientSecret;

            buttonOK.Enabled = options.GeneralSettings.PaPaGoGeneralOptions.Checked;
        }

        private void bindOptionsChangedEvent()
        {
            void onOptionsChanged(object sender, EventArgs e)
            {
                buttonOK.Enabled = false;
            }

            foreach (Control control in Controls)
            {
                if (control is TextBox textBox)
                {
                    textBox.TextChanged += onOptionsChanged;
                }
            }
        }

        private async void linkLabelCheck_Click(object sender, LinkLabelLinkClickedEventArgs e)
        {
            void setControlsEnabledState(bool enabled)
            {
                foreach (Control control in Controls)
                {
                    if (control is TextBox || control is LinkLabel)
                    {
                        control.Enabled = enabled;
                    }
                }

                progressBar.Visible = !enabled;
            }

            setControlsEnabledState(false);

            labelCheckResult.Text = "";
            buttonOK.Enabled = false;

            checkSuccess = await ServicePaPaGo.Check(textBoxClientID.Text, textBoxClientSecret.Text);

            if (!IsDisposed) 
            {
                if (checkSuccess)
                {
                    buttonOK.Enabled = true;
                    labelCheckResult.Text = LLH.G(LLK.OptionForm_LabelCheckResult_CheckedSuccees);
                }
                else
                {
                    labelCheckResult.Text = LLH.G(LLK.OptionForm_LabelCheckResult_CheckedFail);
                }

                setControlsEnabledState(true);
            }
        }

        private void onFormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK && checkSuccess)
            {
                options.SecureSettings.PaPaGoSecureOptions.ClientID = textBoxClientID.Text;
                options.SecureSettings.PaPaGoSecureOptions.ClientSecret = textBoxClientSecret.Text;
                options.GeneralSettings.PaPaGoGeneralOptions.Checked = true;
            }
        }
    }
}

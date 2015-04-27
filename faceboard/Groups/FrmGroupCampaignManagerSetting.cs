using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Groups
{
    public partial class FrmGroupCampaignManagerSetting : Form
    {
        public FrmGroupCampaignManagerSetting()
        {
            InitializeComponent();
        }

        private void btn_SaveSettings_Click(object sender, EventArgs e)
        {
            try
            {
                Regex IdCheck1 = new Regex("^[0-9]*$");
                if (!string.IsNullOrEmpty(txt_NoOfMessages.Text) && IdCheck1.IsMatch(txt_NoOfMessages.Text))
                {
                  
                    faceboardpro.FbGroupCampaignManagerGlobals.NoOfMessages = int.Parse(txt_NoOfMessages.Text);
                    faceboardpro.FbGroupCampaignManagerGlobals.NoOfMessageserHour = int.Parse(txt_NoOfMessages.Text);
                }
                else
                {
                    MessageBox.Show("Enter Numeric Value !");
                    
                    return;
                }
                faceboardpro.FbGroupCampaignManagerGlobals.Scheduletime = dtpcampaign.Value.ToString();
                this.Close();
            }
            catch { }
        }
    }
}

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UI.Components
{
    public partial class SegmentPauseSettings : UserControl
    {
        public SegmentPauseSettings()
        {
            InitializeComponent();
        }

        private void SegmentPauseSettings_Load(object sender, EventArgs e)
        {

        }

        private int CreateSettingsNode(XmlDocument document, XmlElement parent)
        {
            return SettingsHelper.CreateSetting(document, parent, "Version", "1.0");
                //SettingsHelper.CreateSetting(document, parent, "Accuracy", Accuracy) ^
                //SettingsHelper.CreateSetting(document, parent, "Display2Rows", Display2Rows);
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            var parent = document.CreateElement("Settings");
            CreateSettingsNode(document, parent);
            return parent;
        }

        public int GetSettingsHashCode()
        {
            return CreateSettingsNode(null, null);
        }

        public void SetSettings(XmlNode node)
        {
            var element = (XmlElement)node;
            //Accuracy = SettingsHelper.ParseEnum<ResetChanceAccuracy>(element["Accuracy"]);
            //Display2Rows = SettingsHelper.ParseBool(element["Display2Rows"], false);
        }
    }
}

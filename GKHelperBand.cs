using SharpShell.SharpDeskBand;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace GKHelper
{
    [ComVisible(true)]
    [DisplayName("GKHelperBand")]
    public class GKHelperBand : SharpDeskBand
    {
        protected override System.Windows.Forms.UserControl CreateDeskBand()
        {
            return new TaskbarApplet();
        }

        protected override BandOptions GetBandOptions()
        {
            return new BandOptions
            {
                HasVariableHeight = false,
                IsSunken = false,
                ShowTitle = true,
                Title = "GK Helper Band",
                UseBackgroundColour = true,
                AlwaysShowGripper = true
            };
        }
    }
}
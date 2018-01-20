using System.Windows.Forms;
using PoeHUD.Hud.Settings;
using PoeHUD.Plugins;

namespace Player_Stats_Dump.Core
{
    public class Settings : SettingsBase
    {
        public Settings() => DumpStatsBotton = new HotkeyNode(Keys.PrintScreen);

        [Menu("Log Stats")]
        public HotkeyNode DumpStatsBotton { get; set; }
    }
}
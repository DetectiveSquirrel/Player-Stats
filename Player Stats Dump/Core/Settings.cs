using PoeHUD.Hud.Settings;
using PoeHUD.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Player_Stats_Dump
{
    public class Settings : SettingsBase
    {
        public Settings()
        {
            TemplateSetting = false;
        }

        [Menu("First Menu", 1)]
        public ToggleNode TemplateSetting { get; set; }

    }
}
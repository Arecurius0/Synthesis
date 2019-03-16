using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoeHUD.Hud.Settings;
using PoeHUD.Plugins;
using SharpDX;

namespace Synthesis
{
    public class Settings : SettingsBase
    {
        public Settings()
        {
            Enable = true;

        }
        public ToggleNode ShowSynthesiser { get; set; } = false;
        public ToggleNode SynthesisThings { get; set; } = true;
        public ToggleNode ShowDecays { get; set; } = true;
        public ToggleNode ShowDecayed { get; set; } = true;
        public ToggleNode BlindLabels { get; set; } = false;
        public HotkeyNode ShowSynthesiserHotkey { get; set; } = new HotkeyNode(System.Windows.Forms.Keys.F6);

    }
}
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;


namespace Synthesis.Main
{
    public partial class Synthesis
    {
        private int FilterOption { get; set; } = 0;
        public void InfoDumpMenu(int idIn, out int idPop)
        {
            idPop = idIn;
            if (ImGui.TreeNode("Synthesis Features"))
            {
                ImGui.PushID(idPop);
                Settings.SynthesisThings.Value = ImGuiExtension.Checkbox(Settings.SynthesisThings.Value ? "Show Memory Map Information" : "Show Memory Map Information", Settings.SynthesisThings);
                idPop++;
                ImGui.Spacing();
                ImGui.PopID();
                Settings.ShowDecays.Value = ImGuiExtension.Checkbox(Settings.ShowDecays.Value ? "Show Zones that will decay on run" : "Show Zones that will decay on run", Settings.ShowDecays);
                idPop++;
                ImGui.PopID();
                Settings.ShowDecayed.Value = ImGuiExtension.Checkbox(Settings.ShowDecayed.Value ? "Show Decayed Area Labels" : "Show Decayed Area Labels", Settings.ShowDecayed);
                idPop++;
                ImGui.PopID();
                Settings.BlindLabels.Value = ImGuiExtension.Checkbox(Settings.BlindLabels.Value ? "Charge Numbers for the Blind" : "Charge Numbers for the Blind", Settings.BlindLabels);
                idPop++;
                ImGui.Spacing();
                ImGui.PopID();
                Settings.ShowSynthesiser.Value = ImGuiExtension.Checkbox(Settings.ShowSynthesiser.Value ? "Show Synthesiser" : "Show Synthesiser", Settings.ShowSynthesiser);
                idPop++;
                ImGui.PopID();
                ImGui.TreePop();
            }
            Settings.ShowSynthesiserHotkey.Value = ImGuiExtension.HotkeySelector($"Refresh Preloads Hotkey", Settings.ShowSynthesiserHotkey.Value);
            if (ImGui.Button("Reload Json Files")) { ReloadJson(); LogMessage("Reloading...", 3); }; ImGui.PopID();
            if (ImGui.Button("Show Synthesiser"))
            {
                Settings.ShowSynthesiser = true;
            }

        }

        public override void DrawSettingsMenu() 
        {
            ImGui.BulletText($"v{PluginVersion}");
            idPop = 1;
            ImGui.PushStyleVar(StyleVar.ChildRounding, 5.0f);
            ImGuiNative.igGetContentRegionAvail(out var newcontentRegionArea);
            if (ImGui.BeginChild("RightSettings", new System.Numerics.Vector2(newcontentRegionArea.X, newcontentRegionArea.Y), true, WindowFlags.Default))
            {
                InfoDumpMenu(idPop, out var newInt);
                idPop = newInt;
            }
            ImGui.EndChild();
        }
    }
}

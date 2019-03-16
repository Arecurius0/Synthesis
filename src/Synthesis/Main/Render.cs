using Newtonsoft.Json;
using PoeHUD.Plugins;
using PoeHUD.Poe.RemoteMemoryObjects;
using SharpDX;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using PoeHUD.Hud.UI;
using ImGuiNET;
using System.Linq;
namespace Synthesis.Main
{
    public partial class Synthesis : BaseSettingsPlugin<Settings>
    {
        public Version version = Assembly.GetExecutingAssembly().GetName().Version;
        public string PluginVersion;
        public DateTime buildDate;
        public int selectedKey;
        public static int idPop;

        public override void Initialise()
        {
            PluginName = "Synthesis";
            PluginVersion = DateTime.UtcNow.ToString("yyyyMMdd.HHmmss", CultureInfo.InvariantCulture);
            var jsonData = File.ReadAllText($@"{PluginDirectory}\data\OtherData.json");
            DataList = JsonConvert.DeserializeObject<DataJson>(jsonData);
        }
        
        public override void Render()
        {
#if !DEBUG
            try
            {
#endif


                if (Settings.ShowSynthesiserHotkey.PressedOnce())
                {
                    Settings.ShowSynthesiser = !Settings.ShowSynthesiser;
                }
                #region imgui interface
                if (Settings.ShowSynthesiser)
                {
                    bool gui = Settings.ShowSynthesiser;
                    ImGui.BeginWindow("Auto-crafting", ref gui, new System.Numerics.Vector2(100, 50), 1.0F, true ? WindowFlags.NoTitleBar | WindowFlags.AlwaysVerticalScrollbar : 0);
                    ImGui.PushItemWidth(ImGui.GetWindowWidth() * 0.65f);
                    if (ImGui.Button("Close Synthesiser"))
                    {
                        Settings.ShowSynthesiser = false;
                    }
                    ImGui.SameLine();
                    if (ImGui.Button("Check Synthesised Implicits"))
                    {
                        CalculateSynthesisImplicit();
                    }
                    ImGui.SameLine();
                    if (ImGui.Button("Clear Results"))
                    {
                        fracturedItemMods.Clear();
                        synthesisedImplcits.Clear();
                        itemClass = "";
                    }/*
                    foreach (var debugItem in coveredExplicits)
                    {
                    ImGui.Text(debugItem.Key.Replace("%", "%%"), new System.Numerics.Vector4(0.0F, 0.8F, 1.0F, 1F));
                    ImGui.SameLine();
                    ImGui.Text("@", new System.Numerics.Vector4(1.0F, 1.0F, 1.0F, 1F));
                    ImGui.SameLine();
                    ImGui.Text(debugItem.Value + "", new System.Numerics.Vector4(0.0F, 0.8F, 1.0F, 1F));
                    }   */
                    
                    ImGui.Text("Class Being Used:", new System.Numerics.Vector4(1.0F, 0.325F, 1.0F, 1F));
                    if (itemClass.Length > 0)
                    {
                        ImGui.Text(itemClass.Replace("%", "%%"), new System.Numerics.Vector4(0.0F, 0.8F, 1.0F, 1F));
                    }
                    ImGui.Spacing();
                    ImGui.Text("Mods Being Used:", new System.Numerics.Vector4(1.0F, 0.325F, 1.0F, 1F));
                    foreach (var mod in fracturedItemMods)
                    {
                        ImGui.Text(mod.Key.Replace("%", "%%"), new System.Numerics.Vector4(0.0F, 0.8F, 1.0F, 1F));
                        ImGui.SameLine();
                        ImGui.Text("@", new System.Numerics.Vector4(1.0F, 1.0F, 1.0F, 1F));
                        ImGui.SameLine();
                        ImGui.Text(mod.Value + "\n", new System.Numerics.Vector4(0.0F, 0.8F, 1.0F, 1F));
                    }
                    ImGui.Spacing();
                    ImGui.Text("Potential Implicit Rolls:", new System.Numerics.Vector4(1.0F, 0.325F, 1.0F, 1F));
                    foreach (var implciit in synthesisedImplcits)
                    {
                        ImGui.Text(implciit.Value.Split('\n')[0].Replace("%", "%%"), new System.Numerics.Vector4(0.0F, 0.8F, 1.0F, 1F));
                        ImGui.Text(implciit.Value.Split('\n')[1].Replace("%", "%%"), new System.Numerics.Vector4(1.0F, 0.8F, 1.0F, 1F));
                        ImGui.Spacing();
                    }

                    ImGui.EndWindow();
                }
                #endregion
                #region memory map
                if (Settings.SynthesisThings)
                {
                    // Synthesis Status
                    int total = 0;
                    int rewards = 0;
                    if (IngameState.UIRoot.Children[1].Children[32].IsVisible)
                    {
                        int node = 0;
                        foreach (var child in IngameState.UIRoot.Children[1].Children[32].Children[0].Children[0].Children)
                        {
                            if (child.ChildCount == 17)
                            {
                                if (child.Children[14].IsVisible && child.Children[0] != null && node != 112)
                                {
                                    // add to total
                                    total++;
                                    // big numbers
                                    if (Settings.BlindLabels)
                                        if (Int32.Parse(child.Children[14].Children[0].Text) >= 1)
                                        {
                                            var color = new Color(255, 255, 255);
                                            if (child.Children[15].IsVisible)
                                                color = new Color(255, 255, 80);
                                            var rec = child.Children[2].GetClientRect();
                                            var textMeasure = Graphics.MeasureText(child.Children[14].Children[0].Text, (int)Math.Round(100 * child.Scale));
                                            DrawTextWithBackground(child.Children[14].Children[0].Text, (int)Math.Round(150 * child.Scale),
                                                new Vector2(rec.X + (rec.Width / 2) - (textMeasure.Width / 2), rec.Y + rec.Height / 4), color);
                                        }
                                    // 1 charge zones
                                    if (Settings.ShowDecays)
                                        if (child.Children[14].Children[0].Text == "1")
                                        {
                                            var rec = child.Children[14].GetClientRect();

                                            var textMeasure = Graphics.MeasureText("Decays", (int)Math.Round(60 * child.Scale));
                                            DrawTextWithBackground("Decays", (int)Math.Round(60 * child.Scale), new Vector2((rec.X + (rec.Width / 2) - textMeasure.Width / 2), rec.Y - rec.Height / 2), new Color(255, 0, 0));
                                        }
                                    //reward counts
                                    if (child.Children[15].IsVisible && node != 112)
                                        rewards++;

                                }
                                //no charges
                                if (child.Children[14].Children[0].Text == "0")
                                {
                                    var rec = child.Children[14].GetClientRect();
                                    DrawTextWithBackground("Decayed", (int)Math.Round(60 * child.Scale), new Vector2(rec.X - (50 * child.Scale), rec.Y - 10), new Color(40, 150, 255));
                                }
                            }

                        node++;
                        }
                        Graphics.DrawText("Placed Nodes: " + (total - rewards) + "\nReward Nodes: " + rewards, 20, new Vector2(18, 12));
                        Graphics.DrawBox(new RectangleF(16, 10, 346, 46), new Color(0,0,0,255));
                    }
                }
                #endregion
#if !DEBUG
            }
            catch (Exception)
            {
            }
#endif
        }
    }
}

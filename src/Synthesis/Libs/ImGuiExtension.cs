﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ImGuiNET;
using PoeHUD.Framework;
using PoeHUD.Hud.Settings;
using PoeHUD.Plugins;
using SharpDX;
using ImGuiVector2 = System.Numerics.Vector2;
using ImGuiVector4 = System.Numerics.Vector4;

namespace Synthesis.Main
{
    public class ImGuiExtension
    {
        public static ImGuiVector4 CenterWindow(int width, int height)
        {
            var centerPos = BasePlugin.API.GameController.Window.GetWindowRectangle().Center;
            return new ImGuiVector4(width + centerPos.X - width / 2, height + centerPos.Y - height / 2, width, height);
        }

        public static bool BeginWindow(string title, ref bool isOpened, int x, int y, int width, int height, bool autoResize = false)
        {
            ImGui.SetNextWindowPos(new ImGuiVector2(width + x, height + y), Condition.Appearing, new ImGuiVector2(1, 1));
            ImGui.SetNextWindowSize(new ImGuiVector2(width, height), Condition.Appearing);
            return ImGui.BeginWindow(title, ref isOpened, autoResize ? WindowFlags.AlwaysAutoResize : WindowFlags.Default);
        }

        public static bool BeginWindow(string title, ref bool isOpened, float x, float y, float width, float height, bool autoResize = false)
        {
            ImGui.SetNextWindowPos(new ImGuiVector2(width + x, height + y), Condition.Appearing, new ImGuiVector2(1, 1));
            ImGui.SetNextWindowSize(new ImGuiVector2(width, height), Condition.Appearing);
            return ImGui.BeginWindow(title, ref isOpened, autoResize ? WindowFlags.AlwaysAutoResize : WindowFlags.Default);
        }

        public static bool BeginWindowCenter(string title, ref bool isOpened, int width, int height, bool autoResize = false)
        {
            var size = CenterWindow(width, height);
            ImGui.SetNextWindowPos(new ImGuiVector2(size.X, size.Y), Condition.Appearing, new ImGuiVector2(1, 1));
            ImGui.SetNextWindowSize(new ImGuiVector2(size.Z, size.W), Condition.Appearing);
            return ImGui.BeginWindow(title, ref isOpened, autoResize ? WindowFlags.AlwaysAutoResize : WindowFlags.Default);
        }

        // Int Sliders
        public static int IntSlider(string labelString, int value, int minValue, int maxValue)
        {
            var refValue = value;
            ImGui.SliderInt(labelString, ref refValue, minValue, maxValue, "%.00f");
            return refValue;
        }

        public static int IntSlider(string labelString, string sliderString, int value, int minValue, int maxValue)
        {
            var refValue = value;
            ImGui.SliderInt(labelString, ref refValue, minValue, maxValue, $"{sliderString}: {value}");
            return refValue;
        }

        public static int IntSlider(string labelString, RangeNode<int> setting)
        {
            var refValue = setting.Value;
            ImGui.SliderInt(labelString, ref refValue, setting.Min, setting.Max, "%.00f");
            return refValue;
        }

        public static int IntSlider(string labelString, string sliderString, RangeNode<int> setting)
        {
            var refValue = setting.Value;
            ImGui.SliderInt(labelString, ref refValue, setting.Min, setting.Max, $"{sliderString}: {setting.Value}");
            return refValue;
        }

        // float Sliders
        public static float FloatSlider(string labelString, float value, float minValue, float maxValue)
        {
            var refValue = value;
            ImGui.SliderFloat(labelString, ref refValue, minValue, maxValue, "%.00f", 1f);
            return refValue;
        }

        public static float FloatSlider(string labelString, float value, float minValue, float maxValue, float power)
        {
            var refValue = value;
            ImGui.SliderFloat(labelString, ref refValue, minValue, maxValue, "%.00f", power);
            return refValue;
        }

        public static float FloatSlider(string labelString, string sliderString, float value, float minValue, float maxValue)
        {
            var refValue = value;
            ImGui.SliderFloat(labelString, ref refValue, minValue, maxValue, $"{sliderString}: {value}", 1f);
            return refValue;
        }

        public static float FloatSlider(string labelString, string sliderString, float value, float minValue, float maxValue, float power)
        {
            var refValue = value;
            ImGui.SliderFloat(labelString, ref refValue, minValue, maxValue, $"{sliderString}: {value}", power);
            return refValue;
        }

        public static float FloatSlider(string labelString, RangeNode<float> setting)
        {
            var refValue = setting.Value;
            ImGui.SliderFloat(labelString, ref refValue, setting.Min, setting.Max, "%.00f", 1f);
            return refValue;
        }

        public static float FloatSlider(string labelString, RangeNode<float> setting, float power)
        {
            var refValue = setting.Value;
            ImGui.SliderFloat(labelString, ref refValue, setting.Min, setting.Max, "%.00f", power);
            return refValue;
        }

        public static float FloatSlider(string labelString, string sliderString, RangeNode<float> setting)
        {
            var refValue = setting.Value;
            ImGui.SliderFloat(labelString, ref refValue, setting.Min, setting.Max, $"{sliderString}: {setting.Value}", 1f);
            return refValue;
        }

        public static float FloatSlider(string labelString, string sliderString, RangeNode<float> setting, float power)
        {
            var refValue = setting.Value;
            ImGui.SliderFloat(labelString, ref refValue, setting.Min, setting.Max, $"{sliderString}: {setting.Value}", power);
            return refValue;
        }

        // Checkboxes
        public static bool Checkbox(string labelString, bool boolValue)
        {
            ImGui.Checkbox(labelString, ref boolValue);
            return boolValue;
        }

        public static bool Checkbox(string labelString, bool boolValue, out bool outBool)
        {
            ImGui.Checkbox(labelString, ref boolValue);
            outBool = boolValue;
            return boolValue;
        }

        // Hotkey Selector
        public static IEnumerable<Keys> KeyCodes() => Enum.GetValues(typeof(Keys)).Cast<Keys>();

        public static Keys HotkeySelector(string buttonName, Keys currentKey)
        {
            if (ImGui.Button($"{buttonName}: {currentKey} ")) ImGui.OpenPopup(buttonName);
            if (ImGui.BeginPopupModal(buttonName, (WindowFlags)35))
            {
                ImGui.Text($"Press a key to set as {buttonName}");
                foreach (var key in KeyCodes())
                {
                    if (!WinApi.IsKeyDown(key)) continue;
                    if (key != Keys.Escape && key != Keys.RButton && key != Keys.LButton)
                    {
                        ImGui.CloseCurrentPopup();
                        ImGui.EndPopup();
                        return key;
                    }

                    break;
                }

                ImGui.EndPopup();
            }

            return currentKey;
        }

        public static Keys HotkeySelector(string buttonName, string popupTitle, Keys currentKey)
        {
            if (ImGui.Button($"{buttonName}: {currentKey} ")) ImGui.OpenPopup(popupTitle);
            if (ImGui.BeginPopupModal(popupTitle, (WindowFlags)35))
            {
                ImGui.Text($"Press a key to set as {buttonName}");
                foreach (var key in KeyCodes())
                {
                    if (!WinApi.IsKeyDown(key)) continue;
                    if (key != Keys.Escape && key != Keys.RButton && key != Keys.LButton)
                    {
                        ImGui.CloseCurrentPopup();
                        ImGui.EndPopup();
                        return key;
                    }

                    break;
                }

                ImGui.EndPopup();
            }

            return currentKey;
        }

        // Color Pickers
        public static Color ColorPicker(string labelName, Color inputColor)
        {
            var color = inputColor.ToVector4();
            var colorToVect4 = new ImGuiVector4(color.X, color.Y, color.Z, color.W);
            if (ImGui.ColorEdit4(labelName, ref colorToVect4, ColorEditFlags.AlphaBar)) return new Color(colorToVect4.X, colorToVect4.Y, colorToVect4.Z, colorToVect4.W);
            return inputColor;
        }

        // Combo Box

        public static int ComboBox(string sideLabel, int currentSelectedItem, List<string> objectList, ComboFlags comboFlags = ComboFlags.HeightRegular)
        {
            ImGui.Combo(sideLabel, ref currentSelectedItem, objectList.ToArray());
            return currentSelectedItem;
        }

        public static string ComboBox(string sideLabel, string currentSelectedItem, List<string> objectList, ComboFlags comboFlags = ComboFlags.HeightRegular)
        {
            if (ImGui.BeginCombo(sideLabel, currentSelectedItem, comboFlags))
            {
                var refObject = currentSelectedItem;
                for (var n = 0; n < objectList.Count; n++)
                {
                    var isSelected = refObject == objectList[n];
                    if (ImGui.Selectable(objectList[n], isSelected))
                    {
                        ImGui.EndCombo();
                        return objectList[n];
                    }

                    if (isSelected) ImGui.SetItemDefaultFocus();
                }

                ImGui.EndCombo();
            }

            return currentSelectedItem;
        }

        public static string ComboBox(string sideLabel, string currentSelectedItem, List<string> objectList, out bool didChange, ComboFlags comboFlags = ComboFlags.HeightRegular)
        {
            if (ImGui.BeginCombo(sideLabel, currentSelectedItem, comboFlags))
            {
                var refObject = currentSelectedItem;
                for (var n = 0; n < objectList.Count; n++)
                {
                    var isSelected = refObject == objectList[n];
                    if (ImGui.Selectable(objectList[n], isSelected))
                    {
                        didChange = true;
                        ImGui.EndCombo();
                        return objectList[n];
                    }

                    if (isSelected) ImGui.SetItemDefaultFocus();
                }

                ImGui.EndCombo();
            }

            didChange = false;
            return currentSelectedItem;
        }

        // Unput Text
        public static unsafe string InputText(string label, string currentValue, uint maxLength, InputTextFlags flags)
        {
            var currentStringBytes = Encoding.Default.GetBytes(currentValue);
            var buffer = new byte[maxLength];
            Array.Copy(currentStringBytes, buffer, Math.Min(currentStringBytes.Length, maxLength));

            int Callback(TextEditCallbackData* data)
            {
                var pCursorPos = (int*)data->UserData;
                if (!data->HasSelection()) *pCursorPos = data->CursorPos;
                return 0;
            }

            ImGui.InputText(label, buffer, maxLength, flags, Callback);
            return Encoding.Default.GetString(buffer).TrimEnd('\0');
        }

        // ImColor_HSV Maker
        public static ImGuiVector4 ImColor_HSV(float h, float s, float v)
        {
            ImGui.ColorConvertHSVToRGB(h, s, v, out var r, out var g, out var b);
            return new ImGuiVector4(r, g, b, 255);
        }

        public static ImGuiVector4 ImColor_HSV(float h, float s, float v, float a)
        {
            ImGui.ColorConvertHSVToRGB(h, s, v, out var r, out var g, out var b);
            return new ImGuiVector4(r, g, b, a);
        }

        // Color menu tabs
        public static void ImGuiExtension_ColorTabs(string idString, int height, IReadOnlyList<string> settingList, ref int selectedItem, ref int uniqueIdPop)
        {
            ImGuiNative.igGetContentRegionAvail(out var newcontentRegionArea);
            var boxRegion = new ImGuiVector2(newcontentRegionArea.X, height);
            if (ImGui.BeginChild(idString, boxRegion, true, WindowFlags.HorizontalScrollbar))
            {
                for (var i = 0; i < settingList.Count; i++)
                {
                    ImGui.PushID(uniqueIdPop);
                    var hue = 1f / settingList.Count * i;
                    ImGui.PushStyleColor(ColorTarget.Button, ImColor_HSV(hue, 0.6f, 0.6f, 0.8f));
                    ImGui.PushStyleColor(ColorTarget.ButtonHovered, ImColor_HSV(hue, 0.7f, 0.7f, 0.9f));
                    ImGui.PushStyleColor(ColorTarget.ButtonActive, ImColor_HSV(hue, 0.8f, 0.8f, 1.0f));
                    ImGui.PushStyleVar(StyleVar.FrameRounding, 3.0f);
                    ImGui.PushStyleVar(StyleVar.FramePadding, 2.0f);
                    if (i > 0) ImGui.SameLine();
                    if (ImGui.Button(settingList[i])) selectedItem = i;
                    uniqueIdPop++;
                    ImGui.PopStyleVar();
                    ImGui.PopStyleColor(3);
                    ImGui.PopID();
                }

            }
            ImGui.EndChild();
        }

        //Begin Child Frames - Full Width
        public static bool BeginChild(string id, bool border, WindowFlags flags)
        {
            ImGuiNative.igGetContentRegionAvail(out var newcontentRegionArea);
            return ImGui.BeginChild(id, new ImGuiVector2(newcontentRegionArea.X, newcontentRegionArea.Y), border, flags);
        }

        //Spacing
        public static void Spacing(int amount)
        {
            for (var i = 0; i < amount; i++)
                ImGui.Spacing();
        }

        //End Columns
        public static void EndColumn()
        {
            ImGui.Columns(1, null, false);
        }
    }
}
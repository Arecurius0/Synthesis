using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PoeHUD.Controllers;
using PoeHUD.Models;
using PoeHUD.Plugins;
using PoeHUD.Poe.Components;
using PoeHUD.Poe.RemoteMemoryObjects;
using PoeHUD.Poe;
using PoeHUD;
using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.IO;
using System.Linq;
using System.Reflection;
using PoeHUD.Framework;
using PoeHUD.Framework.Helpers;
using PoeHUD.Hud.UI;
using System.Collections;


namespace Synthesis.Main
{

    public static class StringExt
    {
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
    }

    public partial class Synthesis
    {
        public void DrawEllipseToWorld(Vector3 vector3Pos, int radius, int points, int lineWidth, Color color)
        {
            var camera = GameController.Game.IngameState.Camera;

            var plottedCirclePoints = new List<Vector3>();
            var slice = 2 * Math.PI / points;
            for (var i = 0; i < points; i++)
            {
                var angle = slice * i;
                var x = (decimal)vector3Pos.X + decimal.Multiply((decimal)radius, (decimal)Math.Cos(angle));
                var y = (decimal)vector3Pos.Y + decimal.Multiply((decimal)radius, (decimal)Math.Sin(angle));
                plottedCirclePoints.Add(new Vector3((float)x, (float)y, vector3Pos.Z));
            }

            var rndEntity = GameController.Entities.FirstOrDefault(x =>
                x.HasComponent<Render>() && GameController.Player.Address != x.Address);

            for (var i = 0; i < plottedCirclePoints.Count; i++)
            {
                if (i >= plottedCirclePoints.Count - 1)
                {
                    var pointEnd1 = camera.WorldToScreen(plottedCirclePoints.Last(), rndEntity);
                    var pointEnd2 = camera.WorldToScreen(plottedCirclePoints[0], rndEntity);
                    Graphics.DrawLine(pointEnd1, pointEnd2, lineWidth, color);
                    return;
                }

                var point1 = camera.WorldToScreen(plottedCirclePoints[i], rndEntity);
                var point2 = camera.WorldToScreen(plottedCirclePoints[i + 1], rndEntity);
                Graphics.DrawLine(point1, point2, lineWidth, color);
            }
        }


        public static string WordWrap(string input, int maxCharacters)
        {
            string[] words = input.Split(' ');
            var lines = new List<string>();
            string line = "";
            foreach (string word in words)
            {
                if ((line + word).Length > maxCharacters)
                {
                    lines.Add(line);
                    line = "";
                }
                line += string.Format("{0} ", word);
            }
            if (line.Length > 0)
                lines.Add(line);
            var finalLine = "";
            foreach (var connectLine in lines)
            {
                finalLine += connectLine + "\n\r";
            }
            return finalLine;
        }


        public static Size2 DrawTextWithBackground(string text, int height, Vector2 position)
        {
            return DrawTextWithBackground(text, height, position, Color.White);
        }

        public static Size2 DrawTextWithBackground(string text, int height, Vector2 position, Color color)
        {
            if (string.IsNullOrEmpty(text))
                text = "Missing";
            var textSize = API.Graphics.DrawText(text, height, position, color);
            API.Graphics.DrawBox(new RectangleF(position.X - 5, position.Y, textSize.Width + 10, textSize.Height + 1), Color.Black);
            return textSize;
        }

        public static Size2 DrawTextWithBackground(string text, int height, Vector2 position, Color color, Color background)
        {
            if (string.IsNullOrEmpty(text))
                text = "Missing";
            var textSize = API.Graphics.DrawText(text, height, position, color);
            API.Graphics.DrawBox(new RectangleF(position.X - 5, position.Y, textSize.Width + 10, textSize.Height + 1), background);
            return textSize;
        }

        public static Size2 DrawTextWithBackground(string text, int height, Vector2 position, Color color, int WrapWidth)
        {

            if (string.IsNullOrEmpty(text))
                text = "Missing";
            var textSize = API.Graphics.DrawText(WordWrap(text, WrapWidth), height, position, color);
            API.Graphics.DrawBox(new RectangleF(position.X - 5, position.Y, textSize.Width + 20, textSize.Height + 1), Color.Black);
            return textSize;
        }
    }
}

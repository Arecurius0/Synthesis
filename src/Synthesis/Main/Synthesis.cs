using System;
using System.Collections.Generic;
using System.Linq;
using PoeHUD.Poe.Elements;
using PoeHUD.Controllers;
using PoeHUD.Poe.Components;
using PoeHUD.Poe.RemoteMemoryObjects;
using PoeHUD.Hud.AdvancedTooltip;
using PoeHUD.Models.Enums;

namespace Synthesis.Main
{
    public partial class Synthesis
    {
        public List<NormalInventoryItem> InventoryItemList { get; set; } = new List<NormalInventoryItem>();
        public Dictionary<String, int> fracturedItemMods = new Dictionary<String, int>();
        public Dictionary<String, String> synthesisedImplcits = new Dictionary<String, String>();
        public Dictionary<String, int> coveredExplicits = new Dictionary<String, int>();
        public string fracturedImplicit = "";
        public string itemClass = "";
        public List<String> debugList = new List<String>();
        public string BaseName;
        public string ClassName;

        public void CalculateSynthesisImplicit()
        {
            fracturedItemMods.Clear();
            synthesisedImplcits.Clear();
            coveredExplicits.Clear();
            fracturedImplicit = "";
            
            var inventory = new List<NormalInventoryItem>();
            if (GameController.Game.IngameState.UIRoot.Children[1].Children[71].Children[0].IsVisible)
            {
                inventory.Clear();
                if (GameController.Game.IngameState.UIRoot.Children[1].Children[71].Children[0].Children[2].Children[1].Children[1].AsObject<NormalInventoryItem>() == null)
                    return;
                inventory.Add(GameController.Game.IngameState.UIRoot.Children[1].Children[71].Children[0].Children[2].Children[1].Children[1].AsObject<NormalInventoryItem>());
                inventory.Add(GameController.Game.IngameState.UIRoot.Children[1].Children[71].Children[0].Children[3].Children[1].Children[1].AsObject<NormalInventoryItem>());
                inventory.Add(GameController.Game.IngameState.UIRoot.Children[1].Children[71].Children[0].Children[4].Children[1].Children[1].AsObject<NormalInventoryItem>());
            }
            else
            {
                foreach (NormalInventoryItem item in IngameUi.InventoryPanel[InventoryIndex.PlayerInventory].VisibleInventoryItems)
                {
                    inventory.Add(item.AsObject<NormalInventoryItem>());
                }
            }

            foreach (NormalInventoryItem item in inventory)
            {
                var entity = item.Item;
                // item things
                var modsComponent = entity.GetComponent<Mods>();
                List<ItemMod> itemMods = modsComponent.ItemMods;
                List<ModValue> mods =
                    itemMods.Select(
                        it => new ModValue(it, GameController.Files, modsComponent.ItemLevel, GameController.Files.BaseItemTypes.Translate(entity.Path))).ToList();


                var baseItemType = API.GameController.Files.BaseItemTypes.Translate(entity.Path);
                ClassName = baseItemType.ClassName;

                itemClass = ClassName;
                foreach (var mod in mods)
                {
                    // implicits don't have names, lazily replace to match mods until I can be bothered to fix
                    if (mod.Record.UserFriendlyName != "")
                        if (fracturedItemMods.ContainsKey(mod.Record.StatNames[0].Key.Replace("minimum", "maximum")))
                        {
                            fracturedItemMods[mod.Record.StatNames[0].Key.Replace("minimum", "maximum")] = fracturedItemMods[mod.Record.StatNames[0].Key.Replace("minimum", "maximum")] + mod.StatValue[0];
                        }
                        else
                        {
                            fracturedItemMods.Add(mod.Record.StatNames[0].Key.Replace("minimum", "maximum"), mod.StatValue[0]);
                        }
                }
            }
            

            foreach (var fracturedMod in DataList.Synthesis)
            {
                if (fracturedItemMods.ContainsKey(fracturedMod.Group) && fracturedMod.Class.Contains(ClassName))
                {
                    string modPrune = fracturedMod.Implicit.ToLower().Replace("1", "").Replace("2", "").Replace("3", "").Replace("4", "").Replace("5", "").Replace("6", "").Replace("7", "").Replace("8", "").Replace("9", "").Replace("0", "")
                        .Replace("%", "").Replace(" ", "").Replace("(", "").Replace(")", "").Replace("tototo", "to").Replace("toto", "to");
                    if (modPrune.Contains(","))
                        modPrune = modPrune.Split(',')[0];
                    // always add first
                    if (!coveredExplicits.ContainsKey(modPrune))
                    {
                        string adding = fracturedMod.Group + " @ " + fracturedItemMods[fracturedMod.Group] + "/" + fracturedMod.Rarity + "\n" + fracturedMod.Implicit + "\n";
                        synthesisedImplcits.Add(modPrune, adding);
                        coveredExplicits.Add(modPrune, Int32.Parse(fracturedMod.Rarity));
                    }
                    // apply logic on following
                    else
                    {
                        //LogMessage(fracturedMod.Group + " group / " + coveredExplicits[modPrune] + " covered / " + Int32.Parse(fracturedMod.Rarity) + " mod / " + fracturedItemMods[fracturedMod.Group] + " item", 10);

                        if (fracturedItemMods[fracturedMod.Group] > coveredExplicits[modPrune] && Int32.Parse(fracturedMod.Rarity) > coveredExplicits[modPrune])
                        {
                            if(fracturedItemMods[fracturedMod.Group] < Int32.Parse(fracturedMod.Rarity))
                            { 
                            string adding = fracturedMod.Group + " @ " + fracturedItemMods[fracturedMod.Group] + "/" + fracturedMod.Rarity + "\n" + fracturedMod.Implicit + "\n";
                            synthesisedImplcits[modPrune] = adding;
                            coveredExplicits[modPrune] = Int32.Parse(fracturedMod.Rarity);
                            }
                        }
                        if (fracturedItemMods[fracturedMod.Group] > coveredExplicits[modPrune] && fracturedItemMods[fracturedMod.Group] > Int32.Parse(fracturedMod.Rarity))
                        {
                            if(Int32.Parse(fracturedMod.Rarity) > coveredExplicits[modPrune]) { 
                            string adding = fracturedMod.Group + " @ " + fracturedItemMods[fracturedMod.Group] + "/" + fracturedMod.Rarity + "\n" + fracturedMod.Implicit + "\n";
                            synthesisedImplcits[modPrune] = adding;
                            coveredExplicits[modPrune] = Int32.Parse(fracturedMod.Rarity);
                            }
                        }
                        if (Int32.Parse(fracturedMod.Rarity) < coveredExplicits[modPrune] && Int32.Parse(fracturedMod.Rarity) > fracturedItemMods[fracturedMod.Group])
                        { 
                            string adding = fracturedMod.Group + " @ " + fracturedItemMods[fracturedMod.Group] + "/" + fracturedMod.Rarity + "\n" + fracturedMod.Implicit + "\n";
                            synthesisedImplcits[modPrune] = adding;
                            coveredExplicits[modPrune] = Int32.Parse(fracturedMod.Rarity);
                        } 
                    } 
                }
            }
        }
        }
}

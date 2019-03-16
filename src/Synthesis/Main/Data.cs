using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Synthesis.Main
{
    public partial class Synthesis
    {
        public DataJson DataList = new DataJson();
        public Dictionary<String, String> mapDrops = new Dictionary<String, String>();
        public Dictionary<String, String> craftTargets = new Dictionary<String, String>();
        public Dictionary<String, String> bestiaryRecipes = new Dictionary<String, String>();
        public Dictionary<String, String> prophecyDesc = new Dictionary<String, String>();
        public Dictionary<String, String> uniqueNames = new Dictionary<String, String>();

        public Dictionary<String, int> craftMod = new Dictionary<String, int>();
        public Dictionary<String, int> craftGroup = new Dictionary<String, int>();

        public void ReloadJson()
        {
            var jsonData = File.ReadAllText($@"{PluginDirectory}\data\OtherData.json");
            DataList = JsonConvert.DeserializeObject<DataJson>(jsonData);          
        }

        public static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
        
        public class DataJson
        {
            [JsonProperty("synthesis")]
            public List<SynthesisJson> Synthesis { get; set; }
        }
        public class SynthesisJson
        {
            [JsonProperty("implicit")]
            public string Implicit { get; set; }
            [JsonProperty("class")]
            public string Class { get; set; }
            [JsonProperty("stats")]
            public string Group { get; set; }
            [JsonProperty("rarity")]
            public string Rarity { get; set; }
        }

    }
}

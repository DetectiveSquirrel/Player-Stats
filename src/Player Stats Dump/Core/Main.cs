using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using PoeHUD.Models.Enums;
using PoeHUD.Plugins;

namespace Player_Stats_Dump.Core
{
    public class Main : BaseSettingsPlugin<Settings>
    {
        private readonly Dictionary<PlayerStats, int> _playerDataDictionary = new Dictionary<PlayerStats, int>();

        public Main() => PluginName = "Player Stats";

        public override void Render() { DebugFunction(); }

        private void DebugFunction()
        {
            var statToJson = new List<StatsToJson>();
            if (!Settings.DumpStatsBotton.PressedOnce() || !LocalPlayer.Entity.IsValid) return;
            var _string = string.Empty;
            GetCachedPlayerStats();
            _string = _playerDataDictionary.Aggregate(_string, (current, stat) => current + $"{DescriptionAttr(stat.Key)} = {stat.Value}{Environment.NewLine}");
            using (var file = new StreamWriter($@"{LocalPluginDirectory}\Player Stat Collection.txt", false))
            {
                file.Write(_string);
            }

            foreach (var stats in GameController.EntityListWrapper.PlayerStats)
            {
                var tempStat = new StatsToJson { Name = DescriptionAttr(stats.Key), Data = new StatGunk { PlayerStat = stats.Key.ToString(), Id = (int) stats.Key, Value = stats.Value } };
                statToJson.Add(tempStat);
            }

            using (var file = new StreamWriter($@"{LocalPluginDirectory}\Player Stat Collection Json.json", false))
            {
                var jsonString = new List<string> { "{" };
                foreach (var dataStuff in statToJson)
                {
                    jsonString.Add("\t\"" + dataStuff.Name + "\": {");
                    jsonString.Add("\t\t\"Value\": " + dataStuff.Data.Value + ",");
                    jsonString.Add("\t\t\"PlayerStat\": \"" + dataStuff.Data.PlayerStat + "\",");
                    jsonString.Add("\t\t\"Id\": " + dataStuff.Data.Id);
                    jsonString.Add("\t},");
                }

                jsonString.Add("}");
                foreach (var line in jsonString) file.WriteLine(line);
            }
        }

        private void GetCachedPlayerStats()
        {
            _playerDataDictionary.Clear();
            foreach (var stat in GameController.EntityListWrapper.PlayerStats) _playerDataDictionary.Add(stat.Key, stat.Value);
        }

        public static string DescriptionAttr<T>(T source)
        {
            var fi         = source.GetType().GetField(source.ToString());
            var attributes = (DescriptionAttribute[]) fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : source.ToString();
        }
    }
}
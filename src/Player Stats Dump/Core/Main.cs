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
        private Dictionary<PlayerStats, int> _playerDataDictionary = new Dictionary<PlayerStats, int>();

        public Main() => PluginName = "Player Stats";

        public override void Render() { DebugFunction(); }

        private void DebugFunction()
        {
            if (!Settings.DumpStatsBotton.PressedOnce() || !LocalPlayer.Entity.IsValid) return;
            GetCachedPlayerStats();

            ExportTo_TXT();
            ExportTo_JSON();
            ExportTo_CSV();
        }

        private void ExportTo_JSON()
        {
            var statToJson = new List<StatsToJson>();
            foreach (var stats in GameController.EntityListWrapper.PlayerStats)
            {
                var tempStat = new StatsToJson { Name = DescriptionAttr(stats.Key), Data = new StatGunk { PlayerStat = "PlayerStat." + stats.Key, Value = stats.Value } };
                statToJson.Add(tempStat);
            }

            using (var file = new StreamWriter($@"{LocalPluginDirectory}\Player Stat Collection JSON.json", false))
            {
                var jsonString = new List<string> { "{" };
                var count = 1;
                foreach (var dataStuff in statToJson)
                {
                    jsonString.Add("\t\"" + dataStuff.Name + "\": {");
                    jsonString.Add("\t\t\"Value\": " + dataStuff.Data.Value + ",");
                    jsonString.Add("\t\t\"Id\": \"" + dataStuff.Data.PlayerStat + "\"");
                    jsonString.Add(count >= statToJson.Count ? "\t}" : "\t},");
                    count++;
                }

                jsonString.Add("}");
                foreach (var line in jsonString) file.WriteLine(line);
            }
        }

        private void ExportTo_CSV()
        {
            var _string = string.Empty;
            _string += _playerDataDictionary.Aggregate("Description,Value,Player Stat Enum\n", (current, stat) => current + $"{DescriptionAttr(stat.Key)},{stat.Value},PlayerStat.{stat.Key}{Environment.NewLine}");
            using (var file = new StreamWriter($@"{LocalPluginDirectory}\Player Stat Collection CSV.csv", false))
            {
                file.Write(_string);
            }
        }

        private void ExportTo_TXT()
        {
            var _string = string.Empty;
            _string     = _playerDataDictionary.Aggregate(_string, (current, stat) => current + $"{stat.Value.ToString().PadRight(7)}| \"{DescriptionAttr(stat.Key)}\"{Environment.NewLine}");
            using (var file = new StreamWriter($@"{LocalPluginDirectory}\Player Stat Collection TXT.txt", false))
            {
                file.Write(_string);
            }
        }

        private void GetCachedPlayerStats()
        {
            _playerDataDictionary = GameController.EntityListWrapper.PlayerStats;
        }

        public static string DescriptionAttr<T>(T source)
        {
            var fi         = source.GetType().GetField(source.ToString());
            var attributes = (DescriptionAttribute[]) fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : source.ToString();
        }
    }
}
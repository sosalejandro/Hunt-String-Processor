using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace hunt_string_processor
{
    public class HuntProcessor
    {
        private readonly RegexPatternsStruct Patterns;
        public Dictionary<string, HuntAnalyzerDataStruct> PlayersData;

        public HuntProcessor(string huntAnalyzerData)
        {
            Patterns = new(
                @"[a-zA-Z]+: (([+-]?(?=\.\d|\d)(?:\d+)?(?:\.?\d*))(?:[eE]([+-]?\d+))?(,([+-]?(?=\.\d|\d)(?:\d+)?(?:\.?\d*))(?:[eE]([+-]?\d+))?)+)(\s([a-zA-Z]+\s)+)    Loot: (([+-]?(?=\.\d|\d)(?:\d+)?(?:\.?\d*))(?:[eE]([+-]?\d+))?(,([+-]?(?=\.\d|\d)(?:\d+)?(?:\.?\d*))(?:[eE]([+-]?\d+))?)+)",
                @"Loot: (\-*[0-9]+(,[0-9]+)+)",
                @"Supplies: (\-*[0-9]+(,[0-9]+)+)",
                @"Balance: (\-*[0-9]+(,[0-9]+)+)",
                @"Damage: (\-*[0-9]+(,[0-9]+)+)",
                @"Healing: (\-*[0-9]+(,[0-9]+)+)"
                );

            try
            {
                AnalyzerDataStruct analyzerData = SegmentString(huntAnalyzerData);
                byte length = ValidateSecondSegmentState(analyzerData.PlayersData);
                PlayersData = SegmentPlayerData(analyzerData.PlayersData, ref length);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception(ex.Message);
            }
        }


        private AnalyzerDataStruct SegmentString(string huntAnalyzerData)
        {
            ReadOnlySpan<char> mainString = huntAnalyzerData.AsSpan();
            var balanceMatch = Regex.Match(huntAnalyzerData, Patterns.BalancePattern);

            var balanceIndex = mainString.IndexOf(balanceMatch.Value);
            var firstSegment = mainString.Slice(0, balanceIndex + balanceMatch.Length);
            var secondSegment = mainString.Slice(balanceIndex + balanceMatch.Length);

            return new AnalyzerDataStruct(firstSegment.ToString(), secondSegment.ToString());
        }

        private byte ValidateSecondSegmentState(string secondSegment)
        {
            byte lootMatches = ((byte)Regex.Matches(secondSegment, Patterns.LootPattern)
                .Count);
            byte suppliesMatches = ((byte)Regex.Matches(secondSegment, Patterns.SuppliesPattern)
                .Count);
            byte balanceMatches = ((byte)Regex.Matches(secondSegment, Patterns.BalancePattern)
                .Count);
            byte damageMatches = ((byte)Regex.Matches(secondSegment, Patterns.DamagePattern)
                .Count);
            byte healingMatches = ((byte)Regex.Matches(secondSegment, Patterns.HealingPattern)
                .Count);

            if ((lootMatches & suppliesMatches & balanceMatches & damageMatches) == healingMatches)
            {
                return healingMatches;
            }

            throw new Exception("Invalid string!");
        }

        private Dictionary<string, HuntAnalyzerDataStruct> SegmentPlayerData(
            string playersData, ref byte size)
        {
            Dictionary<string, HuntAnalyzerDataStruct> playersStructData = new(size);
            ReadOnlySpan<char> data = playersData.AsSpan();

            MatchCollection lootMatches = Regex.Matches(playersData, Patterns.LootPattern);
            MatchCollection suppliesMatches = Regex.Matches(playersData, Patterns.SuppliesPattern);
            MatchCollection balanceMatches = Regex.Matches(playersData, Patterns.BalancePattern);
            MatchCollection damageMatches = Regex.Matches(playersData, Patterns.DamagePattern);
            MatchCollection healingMatches = Regex.Matches(playersData, Patterns.HealingPattern);

            short startIndex = 0;
            short lootIndex = ((short)data.IndexOf(lootMatches[0].Value));

            #region For Loop
            for (int i = 0, length = lootMatches.Count; i < length; i++)
            {
                try
                {
                    short refLength = (short)lootMatches[i].Value.Length;
                    ReadOnlySpan<char> name = GetName(ref data, ref startIndex, ref lootIndex, ref refLength);

                    // ASSIGN DATA
                    HuntAnalyzerDataStruct huntData = new(
                        name.ToString(),
                        lootMatches[i].ToString(),
                        suppliesMatches[i].ToString(),
                        balanceMatches[i].ToString(),
                        damageMatches[i].ToString(),
                        healingMatches[i].ToString()
                        );

                    playersStructData.Add(huntData.MyName(), huntData);

                    // Re-assign new node (LAST "Healing:" MATCH + IT'S OWN LENGTH) 
                    startIndex = ((short)(data.IndexOf(healingMatches[i].Value) + healingMatches[i].Value.Length));

                    // CHECK: NOT LAST NODE
                    if (i < length - 1)
                    {
                        lootIndex = (short)(((short)data.IndexOf(lootMatches[i + 1].Value)) - startIndex);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            #endregion For Loop

            return playersStructData;
        }

        private ReadOnlySpan<char> GetName(ref ReadOnlySpan<char> data, ref short startIndex, ref short lootIndex, ref short lootMatchLength)
        {
            ReadOnlySpan<char> name = data.Slice(startIndex, lootIndex - lootMatchLength)
                .Trim();

            // CHECK LEADER
            if (name.Contains("(Leader)", StringComparison.Ordinal))
            {
                short leaderMatch = ((short)name.IndexOf("(Leader)"));
                name = name.Slice(0, leaderMatch);
            }

            return name;
        }
    }
}

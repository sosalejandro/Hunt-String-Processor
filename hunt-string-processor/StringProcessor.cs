using HuntProcessor.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


namespace HuntProcessor
{
    internal class StringProcessor
    {
        private readonly RegexPatternsStruct Patterns;
        private readonly Dictionary<string, HuntAnalyzerDataStruct> PlayersData;

        public StringProcessor()
        {
            // TODO: Check performance with int32 vs short/bytes as the maximum use cases per usage is of 5 players - not constrained yet
            Patterns = new(
                @"[a-zA-Z]+: (([+-]?(?=\.\d|\d)(?:\d+)?(?:\.?\d*))(?:[eE]([+-]?\d+))?(,([+-]?(?=\.\d|\d)(?:\d+)?(?:\.?\d*))(?:[eE]([+-]?\d+))?)+)(\s([a-zA-Z]+\s)+)    Loot: (([+-]?(?=\.\d|\d)(?:\d+)?(?:\.?\d*))(?:[eE]([+-]?\d+))?(,([+-]?(?=\.\d|\d)(?:\d+)?(?:\.?\d*))(?:[eE]([+-]?\d+))?)+)",
                @"Loot: (\-*[0-9]+(,[0-9]+)+)",
                @"Supplies: (\-*[0-9]+(,[0-9]+)+)",
                @"Balance: (\-*[0-9]+(,[0-9]+)+)",
                @"Damage: (\-*[0-9]+(,[0-9]+)+)",
                @"Healing: (\-*[0-9]+(,[0-9]+)+)"
                );
        }

        public Dictionary<string, HuntAnalyzerDataStruct> GetHuntResults()
        {
            return PlayersData;
        }


        public AnalyzerDataStruct SegmentString(string huntAnalyzerData)
        {
            ReadOnlySpan<char> mainString = huntAnalyzerData.AsSpan();
            Match balanceMatch = Regex.Match(huntAnalyzerData, Patterns.BalancePattern);

            short balanceIndex = (short)(mainString.IndexOf(balanceMatch.Value) + balanceMatch.Length);
            ReadOnlySpan<char> firstSegment = mainString[..balanceIndex];
            ReadOnlySpan<char> secondSegment = mainString[balanceIndex..];

            return new AnalyzerDataStruct(firstSegment.ToString(), secondSegment.ToString());
        }

        public byte ValidateSecondSegmentState(string secondSegment)
        {
            byte lootMatches = (byte)Regex.Matches(secondSegment, Patterns.LootPattern)
                .Count;
            byte suppliesMatches = (byte)Regex.Matches(secondSegment, Patterns.SuppliesPattern)
                .Count;
            byte balanceMatches = (byte)Regex.Matches(secondSegment, Patterns.BalancePattern)
                .Count;
            byte damageMatches = (byte)Regex.Matches(secondSegment, Patterns.DamagePattern)
                .Count;
            byte healingMatches = (byte)Regex.Matches(secondSegment, Patterns.HealingPattern)
                .Count;

            if ((lootMatches & suppliesMatches & balanceMatches & damageMatches) == healingMatches)
            {
                return healingMatches;
            }

            throw new Exception("Invalid string!");
        }

        public Dictionary<string, HuntAnalyzerDataStruct> SegmentPlayerData(
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
                    startIndex = (short)(data.IndexOf(healingMatches[i].Value) + healingMatches[i].Value.Length);

                    // CHECK: NOT LAST NODE
                    if (i < length - 1)
                    {
                        lootIndex = (short)(data.IndexOf(lootMatches[i + 1].Value) - startIndex);
                        if (lootIndex is -1)
                        {
                            ThrowArgumentOutOfRangeException();
                        }
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

        private static Exception ThrowArgumentOutOfRangeException()
        {
            return new ArgumentOutOfRangeException("The index is out of range.");
        }

        private static ReadOnlySpan<char> GetName(ref ReadOnlySpan<char> data, ref short startIndex, ref short lootIndex, ref short lootMatchLength)
        {
            ReadOnlySpan<char> name = data[startIndex..(lootIndex - lootMatchLength)]
                .Trim();

            // CHECK LEADER
            if (name.Contains("(Leader)", StringComparison.Ordinal))
            {
                short leaderMatch = ((short)name.IndexOf("(Leader)"));
                name = name[..leaderMatch];
            }

            return name;
        }
    }
}

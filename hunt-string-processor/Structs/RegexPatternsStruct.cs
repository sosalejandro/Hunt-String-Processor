using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuntProcessor.Structs
{
    internal record struct RegexPatternsStruct
    {
        public readonly string PlayerPattern;
        public readonly string LootPattern;
        public readonly string SuppliesPattern;
        public readonly string BalancePattern;
        public readonly string DamagePattern;
        public readonly string HealingPattern;

        public RegexPatternsStruct(string playerPattern, string lootPattern, string suppliesPattern, string balancePattern, string damagePattern, string healingPattern)
        {
            PlayerPattern = playerPattern;
            LootPattern = lootPattern;
            SuppliesPattern = suppliesPattern;
            BalancePattern = balancePattern;
            DamagePattern = damagePattern;
            HealingPattern = healingPattern;
        }
    }
}

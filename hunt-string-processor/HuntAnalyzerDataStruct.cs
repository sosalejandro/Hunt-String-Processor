using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hunt_string_processor
{
    public struct HuntAnalyzerDataStruct
    {
        readonly string Name;
        readonly string Loot;
        readonly string Supplies;
        readonly string Balance;
        readonly string Damage;
        readonly string Healing;


        public HuntAnalyzerDataStruct(string _name, string _loot, string _supplies, string _balance, string _damage, string _healing)
        {
            Name = _name;
            Loot = _loot;
            Supplies = _supplies;
            Balance = _balance;
            Damage = _damage;
            Healing = _healing;
        }

        public string MyName()
        {
            return Name;
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}, {3}, {4}, {5}", Name, Loot, Supplies, Balance, Damage, Healing);
        }
    }
}

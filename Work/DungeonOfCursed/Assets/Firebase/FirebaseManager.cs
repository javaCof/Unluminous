using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using System.Threading.Tasks;

public class FirebaseManager
{
    public static DatabaseReference databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    public static DataSnapshot unitDataLoad;
    public static DataSnapshot itemDataLoad;
    public static DataSnapshot equipDataLoad;
    public static DataSnapshot setDataLoad;

    public class Unit
    {
        public float hp;
        public float atk;
        public float def;
        public float spd;
        public int type;
        public string dec;
        public string name;
        public string res;
    }
    public class Item
    {
        public int price;
        public string dec;
        public string name;
    }
    public class Equip
    {
        public int set;
        public float hp;
        public float atk;
        public float def;
        public float speed;
        public int price;
        public string dec;
        public string name;
    }

    public static Dictionary<int, Unit> units = new Dictionary<int, Unit>();
    public static Dictionary<int, Item> items = new Dictionary<int, Item>();
    public static Dictionary<int, Equip> equips = new Dictionary<int, Equip>();

    public static async Task UnitLoadData()
    {
        unitDataLoad = await databaseReference.Child("Unit").GetValueAsync();
        var datas = unitDataLoad.Value as Dictionary<string, object>;

        foreach (var data in datas)
        {
            int id = int.Parse(data.Key);
            string json = "{";

            var dataValues = data.Value as Dictionary<string, object>;
            foreach (var dataValue in dataValues)
            {
                string value = dataValue.Value.ToString();

                float parse_float; bool parse_bool;
                if (!float.TryParse(value, out parse_float) && !bool.TryParse(value, out parse_bool))
                    value = "\"" + value + "\"";

                json += "\"" + dataValue.Key + "\":" + value + ",";
            }
            json = json.Substring(0, json.Length - 1) + "}";

            Unit unit = JsonUtility.FromJson<Unit>(json);
            units.Add(id, unit);
        }
    }
    public static async Task ItemLoadData()
    {
        itemDataLoad = await databaseReference.Child("Item").GetValueAsync();
        var datas = itemDataLoad.Value as Dictionary<string, object>;

        foreach (var data in datas)
        {
            int id = int.Parse(data.Key);
            string json = "{";

            var dataValues = data.Value as Dictionary<string, object>;
            foreach (var dataValue in dataValues)
            {
                string value = dataValue.Value.ToString();

                float parse_float; bool parse_bool;
                if (!float.TryParse(value, out parse_float) && !bool.TryParse(value, out parse_bool))
                    value = "\"" + value + "\"";

                json += "\"" + dataValue.Key + "\":" + value + ",";
            }
            json = json.Substring(0, json.Length - 1) + "}";

            Item item = JsonUtility.FromJson<Item>(json);
            items.Add(id, item);
        }
    }
    public static async Task EquipLoadData()
    {
        equipDataLoad = await databaseReference.Child("Equip").GetValueAsync();
        var datas = equipDataLoad.Value as Dictionary<string, object>;

        foreach (var data in datas)
        {
            int id = int.Parse(data.Key);
            string json = "{";

            var dataValues = data.Value as Dictionary<string, object>;
            foreach (var dataValue in dataValues)
            {
                string value = dataValue.Value.ToString();

                float parse_float; bool parse_bool;
                if (!float.TryParse(value, out parse_float) && !bool.TryParse(value, out parse_bool))
                    value = "\"" + value + "\"";

                json += "\"" + dataValue.Key + "\":" + value + ",";
            }
            json = json.Substring(0, json.Length - 1) + "}";

            Equip equip = JsonUtility.FromJson<Equip>(json);
            equips.Add(id, equip);
        }
    }
}
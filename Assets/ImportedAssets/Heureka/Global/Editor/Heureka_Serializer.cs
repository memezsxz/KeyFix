using System;
using System.Collections.Generic;
using UnityEngine;

namespace HeurekaGames
{
    public class Heureka_Serializer
    {
        public static string Serialize(List<string> items)
        {
            return JsonUtility.ToJson(new StringList(items));
        }

        public static List<string> DeserializeStringList(string json)
        {
            var list = JsonUtility.FromJson<StringList>(json);

            return list != null ? list.Items : new List<string>();
        }

        public static Type DeSerializeType(string serializedType)
        {
            return Type.GetType(serializedType);
        }

        public static string SerializeType(Type type)
        {
            return type.AssemblyQualifiedName;
        }

        public class StringList
        {
            public List<string> Items = new();

            public StringList(List<string> items)
            {
                Items = items;
            }
        }
    }
}
using UnityEngine;
using FishNet;
using System.Collections.Generic;
using FishNet.Serializing;

public class SerializableDictionary
{
    List<CustomPair> pairs = new List<CustomPair>();

    public static SerializableDictionary FromDictionary(Dictionary<string, List<string>> dict)
    {
        SerializableDictionary serializableDictionary = new SerializableDictionary();

        foreach (var key in dict.Keys)
        {
            serializableDictionary.AddPair(key, dict[key]);
        }

        return serializableDictionary;
    }

    public void AddPair(string key, object value)
    {
        foreach (var pair in pairs)
        {
            if (pair.key == key)
            {
                Debug.LogError("Key already assigned");
                return;
            }
        }

        pairs.Add(new CustomPair(key, value));
    }

    public void SetValue(string key, object value)
    {
        CustomPair foundPair = new CustomPair();

        foreach (var pair in pairs)
        {
            if (pair.key == key)
            {
                foundPair = pair;
            }
        }

        if (foundPair.key == key)
        {
            foundPair.value = value;
        }
    }

    public object GetValue(string key)
    {
       foreach (var pair in pairs)
       {
           if (pair.key == key)
           {
               return pair.value;
           }
       }

       return null;
    }

    /*public bool TryGetValue(string key, out object valueOut)
    {
        foreach (var pair in pairs)
        {
            if (pair.key == key)
            {
                valueOut = pair.value;
                return true;
            }
        }

        valueOut = null;
        return false;
    }*/

    public void Clear()
    {
        pairs.Clear();
    }

}

/*public static class SerializableDictionarySerializer
{
    public static void WriteSerializableDictionary(this Writer writer, SerializableDictionary value)
    {

       
    }

    public static SerializableDictionary ReadSerializableDictionary(this Reader reader)
    {
        SerializableDictionary result = new SerializableDictionary();

        

        return result;
    }
}*/



public struct CustomPair
{
    //public object key;
    public string key;
    public object value;

    public CustomPair(string key, object value)
    {
        this.key = key;
        this.value = value;
    }
}

public static class CustomPairSerializer
{
    public static void WriteCustomPair(this Writer writer, CustomPair value)
    {

        //writer.WriteUInt8Unpacked(GetTypeId(value.key));
        writer.Write(value.key);

        writer.WriteUInt8Unpacked(GetTypeId(value.value));
        writer.Write(value.value);
    }

    private static byte GetTypeId(object value)
    {
        if (value is int)
        {
            return 1;
        }
        else if (value is float)
        {
            return 2;
        }
        else if (value is string)
        {
            return 3;
        } else if (value is List<string>)
        {
            return 4;
        }
        else
        {
            return 0;       // unknown type
        }
    }

    public static CustomPair ReadCustomPair(this Reader reader)
    {
        CustomPair result = new CustomPair();

        /*byte keyTypeId = reader.ReadUInt8Unpacked();

        switch (keyTypeId)
        {
            case 1:
                result.key = reader.ReadInt32();
                break;
            case 2:
                result.key = reader.ReadSingle();
                break;
            case 3:
                result.key = reader.ReadStringAllocated();
                break;
            case 4:
                result.key = reader.ReadSingle();
                break;
            default:
                Debug.LogError("Unkown Type");
                break;
        }*/

        result.key = reader.ReadStringAllocated();

        byte valueTypeId = reader.ReadUInt8Unpacked();

        switch (valueTypeId)
        {
            case 1:
                result.value = reader.ReadInt32();
                break;
            case 2:
                result.value = reader.ReadSingle();
                break;
            case 3:
                result.value = reader.ReadStringAllocated();
                break;
            case 4:
                result.value = reader.ReadSingle();
                break;
            default:
                Debug.LogError("Unkown Type");
                break;
        }

        return result;
    }
}






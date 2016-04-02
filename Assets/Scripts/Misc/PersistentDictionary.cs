using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

public class PersistentData<T>
{
    public T value;
    public bool saveToDisk;

    public PersistentData(T val, bool save = true)
    {
        value = val;
        saveToDisk = save;
    }
}

class PersistentDictionary<T>
{
    private Dictionary<string, PersistentData<T>> _dictionary;
    private int _writeCount;

    public PersistentDictionary(int capacity)
    {
        _dictionary = new Dictionary<string, PersistentData<T>>(capacity);
        _writeCount = 0;
    }

    public void SetEntry(string key, T val, bool saveToDisk = true)
    {
        _dictionary[key] = new PersistentData<T>(val, saveToDisk);
        if (saveToDisk) _writeCount += 1;
    }

    public T GetEntry(string key, T defaultVal = default(T))
    {
        PersistentData<T> ret;
        return _dictionary.TryGetValue(key, out ret) ? ret.value : defaultVal;
    }

    public void RemoveEntry(string key)
    {
        if(_dictionary.ContainsKey(key))
        {
            _dictionary.Remove(key);
            _writeCount -= 1;
        }
    }

    public bool HasEntry(string key)
    {
        return _dictionary.ContainsKey(key);
    }

    public void Serialize(System.IO.BinaryWriter writer)
    {
        Action<BinaryWriter, T> serializer;

        Type[] arguments = _dictionary.GetType().GetGenericArguments();
        Type keyType = arguments[0];
        Type valueType = arguments[1].GetGenericArguments()[0];

        if (!supportedWriterTypes.TryGetValue(valueType, out serializer))
        {
            throw new NotSupportedException(string.Format("Type of property '{0}' isn't supported ({1}).", _dictionary.GetType(), valueType));
        }
        else
        {
            writer.Write((int)_writeCount);
            foreach (KeyValuePair<string, PersistentData<T>> entry in _dictionary)
            {
                if (entry.Value.saveToDisk)
                {
                    writer.Write((string)entry.Key);
                    serializer(writer, entry.Value.value);
                }
            }
        }
    }

    public void Deserialize(System.IO.BinaryReader reader)
    {
        Func<BinaryReader, object> deserializer;

        Type[] arguments = _dictionary.GetType().GetGenericArguments();
        Type keyType = arguments[0];
        Type valueType = arguments[1].GetGenericArguments()[0];

        if (!supportedReaderTypes.TryGetValue(valueType, out deserializer))
        {
            throw new NotSupportedException(string.Format("Type of property '{0}' isn't supported ({1}).", _dictionary.GetType(), valueType));
        }
        else
        {
            int buffCount = reader.ReadInt32();
            for (int i = 0; i < buffCount; ++i)
            {
                string tkey = reader.ReadString();
                T tval = (T)deserializer(reader);
                SetEntry(tkey, tval, true);
            }
        }
    }

    public void Clear()
    {
        _dictionary.Clear();
    }

    #region Serialization Dictionaries

    private static Dictionary<Type, Action<BinaryWriter, T>> supportedWriterTypes = new Dictionary<Type, Action<BinaryWriter, T>>
    {
        { typeof(bool), (bw, val) => bw.Write((bool)(object)val) },

        { typeof(byte), (bw, val) => bw.Write((byte)(object)val) },
        { typeof(sbyte), (bw, val) => bw.Write((sbyte)(object)val) },
        { typeof(char), (bw, val) => bw.Write((char)(object)val) },

        { typeof(decimal), (bw, val) => bw.Write((decimal)(object)val) },
        { typeof(double), (bw, val) => bw.Write((double)(object)val) },
        { typeof(float), (bw, val) => bw.Write((float)(object)val) },

        { typeof(int), (bw, val) => bw.Write((int)(object)val) },
        { typeof(uint), (bw, val) => bw.Write((uint)(object)val) },

        { typeof(long), (bw, val) => bw.Write((long)(object)val) },
        { typeof(ulong), (bw, val) => bw.Write((ulong)(object)val) },

        //{ typeof(object), (bw, val) => null },

        { typeof(short), (bw, val) => bw.Write((short)(object)val) },
        { typeof(ushort), (bw, val) => bw.Write((ushort)(object)val) },

        { typeof(string), (bw, val) => bw.Write((string)(object)val) },
    };

    private static Dictionary<Type, Func<BinaryReader, object>> supportedReaderTypes = new Dictionary<Type, Func<BinaryReader, object>>
    {
        { typeof(bool), br => br.ReadBoolean() },

        { typeof(byte), br => br.ReadByte() },
        { typeof(sbyte), br => br.ReadSByte() },
        { typeof(char), br => br.ReadChar() },

        { typeof(decimal), br => br.ReadDecimal() },
        { typeof(double), br => br.ReadDouble() },
        { typeof(float), br => br.ReadSingle() },

        { typeof(int), br => br.ReadInt32() },
        { typeof(uint), br => br.ReadUInt32() },

        { typeof(long), br => br.ReadInt64() },
        { typeof(ulong), br => br.ReadUInt64() },

        //{ typeof(object), br => null },

        { typeof(short), br => br.ReadInt16() },
        { typeof(ushort), br => br.ReadUInt16() },

        { typeof(string), br => br.ReadString() },
    };

    #endregion Serialization Dictionaries

}

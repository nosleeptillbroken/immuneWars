using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

public class SerializableObject
{
    public object Object;
    public bool Serialize;

    public SerializableObject(object Object, bool Serialize = true)
    {
        this.Object = Object;
        this.Serialize = Serialize;
    }
}


public class TypedDatabase : ITypedDatabase
{
    // PROPERTIES ////////////////

    private SortedDictionary<Type, Dictionary<string, SerializableObject>> database;

    public int Count
    {
        get
        {
            int count = 0;
            foreach(var typedb in database)
            {
                count += typedb.Value.Count;
            }
            return count;
        }
    }

    public Type[] Types
    {
        get
        {
            return database.Keys.ToArray();
        }
    }

    public string[] Keys
    {
        get
        {
            List<string> keys = new List<string>();
            foreach (var typedb in database)
            {
                keys.AddRange(typedb.Value.Keys.ToArray());
            }
            return keys.ToArray();
        }
    }

    public bool IsSynchronized
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public object SyncRoot
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public Dictionary<string, SerializableObject> this[Type type]
    {
        get
        {
            return database[type];
        }
    }


    // METHODS ////////////////

    public void Add<T>(string key, T value, bool serialize = true)
    {
        Type type = typeof(T);
        Dictionary<string, SerializableObject> typedb;

        SerializableObject tval = new SerializableObject(value, serialize);

        if (database.TryGetValue(type, out typedb))
        {
            typedb.Add(key, tval);
        }
        else
        {
            typedb = new Dictionary<string, SerializableObject>();
            typedb.Add(key, tval);
            database.Add(type, typedb);
        }
    }

    public void Add(Type type, string key, object value, bool serialize = true)
    {
        database[type].Add(key, new SerializableObject(value, serialize));
        Dictionary<string, SerializableObject> typedb;

        SerializableObject tval = new SerializableObject(value, serialize);

        if (database.TryGetValue(type, out typedb))
        {
            typedb.Add(key, tval);
        }
        else
        {
            typedb = new Dictionary<string, SerializableObject>();
            typedb.Add(key, tval);
            database.Add(type, typedb);
        }
    }

    public void Clear()
    {
        foreach (var typedb in database)
        {
            typedb.Value.Clear();
        }
        database.Clear();
    }

    public void Clear(Type type)
    {
        Dictionary<string, SerializableObject> typedb;

        if (database.TryGetValue(type, out typedb))
        {
            typedb.Clear();
        }
    }

    public bool ContainsType(Type type)
    {
        return database.ContainsKey(type);
    }

    public bool ContainsKey(string key)
    {
        List<Type> types = new List<Type>(database.Count);
        foreach (var typedb in database)
        {
            if (typedb.Value.ContainsKey(key)) return true;
        }
        return false;
    }

    public bool ContainsKey(Type type, string key)
    {
        List<Type> types = new List<Type>(database.Count);
        foreach (var typedb in database)
        {
            if (typedb.Value.ContainsKey(key)) return true;
        }
        return false;
    }

    public bool ContainsValue<T>(T value)
    {
        Type type = typeof(T);
        Dictionary<string, SerializableObject> typedb;

        if (database.TryGetValue(type, out typedb))
        {
            foreach (var entry in typedb)
            {
                if (((T)entry.Value.Object).Equals(value)) return true;
            }
        }

        return false;
    }

    public bool ContainsValue(Type type, object value)
    {
        Dictionary<string, SerializableObject> typedb;

        if (database.TryGetValue(type, out typedb))
        {
            foreach (var entry in typedb)
            {
                if ((entry.Value.Object).Equals(value)) return true;
            }
        }

        return false;
    }

    public void Remove(string key)
    {
        foreach(var typedb in database)
        {
            if(typedb.Value.ContainsKey(key)) typedb.Value.Remove(key);
        }
    }

    public void Remove(Type type, string key)
    {
        foreach (var typedb in database)
        {
            if (typedb.Value.ContainsKey(key)) typedb.Value.Remove(key);
        }
    }

    public bool TryGetValue<T>(string key, out T value)
    {
        Type type = typeof(T);

        if(database.ContainsKey(type))
        {
            SerializableObject obj;
            bool res = database[type].TryGetValue(key, out obj);
            value = res ? (T)obj.Object : default(T);
            return res;
        }
        else
        {
            value = default(T);
            return false;
        }
    }

    public bool TryGetValue(Type type, string key, out object value)
    {
        if (database.ContainsKey(type))
        {
            SerializableObject obj;
            bool res = database[type].TryGetValue(key, out obj);
            value = res ? obj.Object : null;
            return res;
        }
        else
        {
            value = null;
            return false;
        }
    }
    
    public void CopyTo(Array array, int index)
    {
        throw new NotImplementedException();
    }

    public IEnumerator GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public bool Serialize(System.IO.BinaryWriter writer)
    {

        writer.Write(Count);

        foreach (var typedb_kvp in database)
        {
            try
            {
                Type type = typedb_kvp.Key;
                var typedb = typedb_kvp.Value;

                if (type != null)
                {
                    Action<BinaryWriter, object> writeFunc = supportedWriterTypes[type];

                    if (writeFunc != null)
                    {
                        writer.Write((string)type.AssemblyQualifiedName);
                        writer.Write((int)typedb.Count);

                        foreach (var kvp in typedb)
                        {
                            string key = kvp.Key;
                            SerializableObject value = kvp.Value;
                            if (value.Serialize && value != null)
                            {
                                writer.Write(key);
                                writeFunc(writer, value.Object);
                            }
                        }
                    }
                }
            }
            catch (IOException e)
            {
                return false;
            }
        }
        return true;
    }

    public bool Deserialize(BinaryReader reader)
    {
        try
        {
            int types = reader.ReadInt32();

            for (int t = 0; t < types; t++)
            {
                string typeQualName = reader.ReadString();
                Type type = Type.GetType(typeQualName);

                if (type != null)
                {
                    var readFunc = supportedReaderTypes[type];

                    if (readFunc != null)
                    {
                        database[type] = new Dictionary<string, SerializableObject>();

                        int vals = reader.ReadInt32();
                        for (int v = 0; v < vals; v++)
                        {
                            string key = reader.ReadString();
                            object val = readFunc(reader);

                            database[type].Add(key, new SerializableObject(val, true));
                        }
                    }
                }
                else
                {
                    return false;
                }

            }
        }
        catch(IOException e)
        {
            return false;
        }
        return true;
    }

    // FIELDS ////////////////

    #region Serialization Dictionaries

    private static Dictionary<Type, Action<BinaryWriter, object>> supportedWriterTypes = new Dictionary<Type, Action<BinaryWriter, object>>
    {
        { typeof(bool), (bw, val) => bw.Write((bool)val) },

        { typeof(byte), (bw, val) => bw.Write((byte)val) },
        { typeof(sbyte), (bw, val) => bw.Write((sbyte)val) },
        { typeof(char), (bw, val) => bw.Write((char)val) },

        { typeof(decimal), (bw, val) => bw.Write((decimal)val) },
        { typeof(double), (bw, val) => bw.Write((double)val) },
        { typeof(float), (bw, val) => bw.Write((float)val) },

        { typeof(int), (bw, val) => bw.Write((int)val) },
        { typeof(uint), (bw, val) => bw.Write((uint)val) },

        { typeof(long), (bw, val) => bw.Write((long)val) },
        { typeof(ulong), (bw, val) => bw.Write((ulong)val) },

        //{ typeof(object), (bw, val) => null },

        { typeof(short), (bw, val) => bw.Write((short)val) },
        { typeof(ushort), (bw, val) => bw.Write((ushort)val) },

        { typeof(string), (bw, val) => bw.Write((string)val) },
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

    // CLASSES ////////////////


}

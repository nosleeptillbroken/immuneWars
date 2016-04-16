using System;
using System.IO;

using System.Runtime;
using System.Runtime.Serialization;

using System.Collections;
using System.Collections.Generic;

public interface ITypedDatabase : ICollection
{
    Type[] Types { get; }
    string[] Keys { get; }

    /// <summary>
    /// Adds the specified typed key-value entry to the database.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="key">The key of the database entry.</param>
    /// <param name="value">The specified value.</param>
    /// <param name="serialize">Whether or not this entry should be included in serialization.</param>
    void Add<T>(string key, T value, bool serialize = true);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="key">The key of the database entry.</param>
    /// <param name="value"></param>
    /// <param name="serialize">Whether or not this entry should be included in serialization.</param>
    void Add(Type type, string key, object value, bool serialize = true);

    /// <summary>
    /// Clears the entire database.
    /// </summary>
    void Clear();

    /// <summary>
    /// Clears the entire database of entries of the given type.
    /// </summary>
    /// <param name="type">The type to clear.</param>
    void Clear(Type type);

    /// <summary>
    /// Determines whether the database contains any entries of the given type.
    /// </summary>
    /// <param name="type">The specified type.</param>
    /// <returns>Whether or not the database contains the given type.</returns>
    bool ContainsType(Type type);

    /// <summary>
    /// Determines whether the database contains any entries with the given key in any type.
    /// </summary>
    /// <param name="key">The key of the database entry.</param>
    /// <returns>Whether or not the database contains the given key of any type.</returns>
    bool ContainsKey(string key);

    /// <summary>
    /// Determines whether the database contains any entries with the given key in a given type.
    /// </summary>
    /// <param name="key">The key of the database entry.</param>
    /// <returns>Whether or not the database contains the given key for the given type.</returns>
    bool ContainsKey(Type type, string key);

    /// <summary>
    /// Determines whether the database contains any entries with the given value, type-sensitive.
    /// </summary>
    /// <typeparam name="T">The type of the entry.</typeparam>
    /// <param name="value">The spceified value.</param>
    /// <returns>Whether or not the database contains the given value for the given type.</returns>
    bool ContainsValue<T>(T value);

    /// <summary>
    /// Determines whether the database contains any entries with the given value, type-sensitive.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value">The spceified value.</param>
    /// <returns>Whether or not the database contains the given value for the given type.</returns>
    bool ContainsValue(Type type, object value);

    /// <summary>
    /// Removes all entries from the database with the given key.
    /// </summary>
    /// <param name="key">The key of the database entry.</param>
    void Remove(string key);

    /// <summary>
    /// Removes entry of a given type and key from the database.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="key">The key of the database entry.</param>
    void Remove(Type type, string key);

    /// <summary>
    /// Attempts to get a value at the specified entry in the database and store it in value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The key of the database entry.</param>
    /// <param name="value">The value of the database entry if the fetch was successful. Otherwise, the default value of the type.</param>
    /// <returns>Determines whether or not the value exists and the fetch was successful.</returns>
    bool TryGetValue<T>(string key, out T value);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type">The type of the value.</param>
    /// <param name="key">The key of the database entry.</param>
    /// <param name="value">The value of the database entry if the fetch was successful. Otherwise, the default value of the type.</param>
    /// <returns>Determines whether or not the value exists and the fetch was successful.</returns>
    bool TryGetValue(Type type, string key, out object value);

    bool Serialize(System.IO.BinaryWriter writer);

    bool Deserialize(System.IO.BinaryReader reader);

}
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using Xunit;

namespace System.Text.Json.Serialization.Tests;

// - set through attribute and metadata
// - polymorphism (i.e. populate polymorphic object, ensure it's within allowed hierarchy or do base type)
// - all cases from main issue for dictionary, collection and object
// - incompatible options
// - put Populate on property with null value
// - deserialize null on property which is supposed to be populated
// - validate value types have setter
// - null values (reading, initially set to null)
// - F#
// - parametrized ctor
// - TODO: replace error messages with resource strings
// - CreationHandlingSetWithAttribute_PopulateWithInvalidTypeThrows: dictionary, parametrized ctor
public sealed partial class JsonCreationHandlingTests_String : JsonCreationHandlingTests
{
    public JsonCreationHandlingTests_String() : base(JsonSerializerWrapper.StringSerializer) { }
}
public sealed partial class JsonCreationHandlingTests_AsyncStream : JsonCreationHandlingTests
{
    public JsonCreationHandlingTests_AsyncStream() : base(JsonSerializerWrapper.AsyncStreamSerializer) { }
}
public sealed partial class JsonCreationHandlingTests_AsyncStreamWithSmallBuffer : JsonCreationHandlingTests
{
    public JsonCreationHandlingTests_AsyncStreamWithSmallBuffer() : base(JsonSerializerWrapper.AsyncStreamSerializerWithSmallBuffer) { }
}
public sealed partial class JsonCreationHandlingTests_SyncStream : JsonCreationHandlingTests
{
    public JsonCreationHandlingTests_SyncStream() : base(JsonSerializerWrapper.SyncStreamSerializer) { }
}

internal struct StructList<T> : IList<T>, IList
{
    private List<T> _list = new List<T>();
    // we track count separately to make sure tests are not passing by accident because we use reference to list inside of struct
    private int _count;

    public T this[int index]
    {
        get => _list[index];
        set => _list[index] = value;
    }

    public int Count => _count;
    public bool IsReadOnly => false;

    public bool IsFixedSize => false;

    public object SyncRoot => this;

    public bool IsSynchronized => false;

    object IList.this[int index]
    {
        get => _list[index];
        set => _list[index] = (T)value;
    }

    public StructList() { }

    public void Add(T item)
    {
        _count++;
        _list.Add(item);
    }

    public void Clear()
    {
        _count = 0;
        _list.Clear();
    }

    public bool Contains(T item) => _list.Contains(item);
    public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);
    public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
    public int IndexOf(T item) => _list.IndexOf(item);
    public void Insert(int index, T item)
    {
        _count++;
        _list.Insert(index, item);
    }
    public bool Remove(T item)
    {
        if (_list.Remove(item))
        {
            _count--;
            return true;
        }

        return false;
    }

    public void RemoveAt(int index)
    {
        _count--;
        _list.RemoveAt(index);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public int Add(object value)
    {
        _count++;
        return ((IList)_list).Add(value);
    }

    public bool Contains(object value) => _list.Contains((T)value);
    public int IndexOf(object value) => _list.IndexOf((T)value);
    public void Insert(int index, object value)
    {
        _count++;
        _list.Insert(index, (T)value);
    }

    public void Remove(object value)
    {
        if (_list.Remove((T)value))
        {
            _count--;
        }
    }

    public void CopyTo(Array array, int index) => ((IList)_list).CopyTo(array, index);

    public void Validate()
    {
        // This can fail only if we modified a copy of this struct
        Assert.Equal(_count, _list.Count);
    }
}

internal struct StructCollection<T> : ICollection<T>
{
    private List<T> _list = new List<T>();

    // we track count separately to make sure tests are not passing by accident because we use reference to list inside of struct
    private int _count;

    public int Count => _count;
    public bool IsReadOnly => false;

    public StructCollection() { }

    public void Add(T item)
    {
        _count++;
        _list.Add(item);
    }

    public void Clear()
    {
        _count = 0;
        _list.Clear();
    }

    public bool Contains(T item) => _list.Contains(item);
    public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);
    public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
    public bool Remove(T item)
    {
        if (_list.Remove(item))
        {
            _count--;
            return true;
        }

        return false;
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Validate()
    {
        // This can fail only if we modified a copy of this struct
        Assert.Equal(_count, _list.Count);
    }
}

internal struct StructSet<T> : ISet<T>
{
    private HashSet<T> _set = new HashSet<T>();

    // we track count separately to make sure tests are not passing by accident because we use reference to list inside of struct
    private int _count;

    public int Count => _count;
    public bool IsReadOnly => false;

    public StructSet() { }

    public bool Add(T item)
    {
        if (_set.Add(item))
        {
            _count++;
            return true;
        }

        return false;
    }

    public void Clear()
    {
        _count = 0;
        _set.Clear();
    }

    public bool Contains(T item) => _set.Contains(item);
    public void CopyTo(T[] array, int arrayIndex) => _set.CopyTo(array, arrayIndex);
    public void ExceptWith(IEnumerable<T> other)
    {
        int prevCount = _set.Count;
        _set.ExceptWith(other);
        _count -= prevCount - _set.Count;
    }
    public IEnumerator<T> GetEnumerator() => _set.GetEnumerator();

    public void IntersectWith(IEnumerable<T> other)
    {
        int prevCount = _set.Count;
        _set.IntersectWith(other);
        _count -= prevCount - _set.Count;
    }

    public bool IsProperSubsetOf(IEnumerable<T> other) => _set.IsProperSubsetOf(other);
    public bool IsProperSupersetOf(IEnumerable<T> other) => _set.IsProperSupersetOf(other);
    public bool IsSubsetOf(IEnumerable<T> other) => _set.IsSubsetOf(other);
    public bool IsSupersetOf(IEnumerable<T> other) => _set.IsSupersetOf(other);
    public bool Overlaps(IEnumerable<T> other) => _set.Overlaps(other);
    public bool Remove(T item)
    {
        if (_set.Remove(item))
        {
            _count--;
            return true;
        }

        return false;
    }
    public bool SetEquals(IEnumerable<T> other) => _set.SetEquals(other);

    public void SymmetricExceptWith(IEnumerable<T> other)
    {
        int prevCount = _set.Count;
        _set.SymmetricExceptWith(other);
        _count -= prevCount - _set.Count;
    }

    public void UnionWith(IEnumerable<T> other)
    {
        int prevCount = _set.Count;
        _set.UnionWith(other);
        _count -= prevCount - _set.Count;
    }

    void ICollection<T>.Add(T item)
    {
        if (_set.Add(item))
        {
            _count++;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Validate()
    {
        // This can fail only if we modified a copy of this struct
        Assert.Equal(_count, _set.Count);
    }
}

public abstract partial class JsonCreationHandlingTests : SerializerTests
{
    public JsonCreationHandlingTests(JsonSerializerWrapper serializerUnderTest) : base(serializerUnderTest)
    {
    }

    [Theory]
    [InlineData(typeof(ClassWithWritableProperty<int>))]
    [InlineData(typeof(ClassWithWritableProperty<int?>))]
    [InlineData(typeof(ClassWithWritableProperty<int[]>))]
    [InlineData(typeof(ClassWithWritableProperty<List<int>>))] // custom converter
    [InlineData(typeof(ClassWithWritableProperty<IEnumerable<int>>))]
    [InlineData(typeof(ClassWithWritableProperty<IEnumerable>))]
    [InlineData(typeof(ClassWithWritableProperty<ImmutableArray<int>>))]
    [InlineData(typeof(ClassWithWritableProperty<ImmutableHashSet<int>>))]
    [InlineData(typeof(ClassWithWritableProperty<ImmutableList<int>>))]
    [InlineData(typeof(ClassWithWritableProperty<ImmutableQueue<int>>))]
    [InlineData(typeof(ClassWithWritableProperty<ImmutableStack<int>>))]
    public async Task CreationHandlingSetWithAttribute_PopulateWithInvalidTypeThrows(Type type)
    {
        JsonSerializerOptions options = new()
        {
            TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
            Converters = { new ThrowingCustomConverter<List<int>>() }
        };

        options.MakeReadOnly();

        string json = """{"Property":[4,5,6]}""";
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await Serializer.DeserializeWrapper(json, type, options));

        Assert.Throws<InvalidOperationException>(() => options.GetTypeInfo(type));
    }

    private static void CheckDictionaryContent(IDictionary<string, int> dict)
    {
        Assert.Equal(6, dict.Count);
        Assert.True(dict.ContainsKey("a"), "Dictionary does not contain 'a' key.");
        Assert.True(dict.ContainsKey("b"), "Dictionary does not contain 'b' key.");
        Assert.True(dict.ContainsKey("c"), "Dictionary does not contain 'c' key.");
        Assert.True(dict.ContainsKey("d"), "Dictionary does not contain 'd' key.");
        Assert.True(dict.ContainsKey("e"), "Dictionary does not contain 'e' key.");
        Assert.True(dict.ContainsKey("f"), "Dictionary does not contain 'f' key.");

        Assert.Equal(1, dict["a"]);
        Assert.Equal(2, dict["b"]);
        Assert.Equal(3, dict["c"]);
        Assert.Equal(4, dict["d"]);
        Assert.Equal(5, dict["e"]);
        Assert.Equal(6, dict["f"]);
    }

    internal class ClassWithReadOnlyProperty<T>
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public T Property { get; }
    }

    internal class ClassWithWritableProperty<T>
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public T Property { get; set; }
    }

    internal class ThrowingCustomConverter<T> : JsonConverter<T>
    {
        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Assert.True(false, "This converter should never be used");
            return default;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            Assert.True(false, "This converter should never be used");
        }
    }
}

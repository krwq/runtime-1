// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
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
public class JsonCreationHandlingTests_String : JsonCreationHandlingTests
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

public abstract class JsonCreationHandlingTests : SerializerTests
{
    public JsonCreationHandlingTests(JsonSerializerWrapper serializerUnderTest) : base(serializerUnderTest)
    {
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ListOfInt()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_ListOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    internal class ClassWithReadOnlyProperty_ListOfInt
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public List<int> Property { get; } = new List<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ListOfInt_WithNumberHandling()
    {
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_ListOfIntWithNumberHandling>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    internal class ClassWithReadOnlyProperty_ListOfIntWithNumberHandling
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public List<int> Property { get; } = new List<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IListOfInt_BackedBy_ListOfInt()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_IListOfInt_BackedBy_ListOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    internal class ClassWithReadOnlyProperty_IListOfInt_BackedBy_ListOfInt
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public IList<int> Property { get; } = new List<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IListOfInt_BackedBy_ListOfInt_WithNumberHandling()
    {
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_IListOfInt_BackedBy_ListOfIntWithNumberHandling>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    internal class ClassWithReadOnlyProperty_IListOfInt_BackedBy_ListOfIntWithNumberHandling
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public IList<int> Property { get; } = new List<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IListOfInt_BackedBy_StructListOfInt()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_IListOfInt_BackedBy_StructListOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        ((StructList<int>)obj.Property).Validate();
    }

    internal class ClassWithReadOnlyProperty_IListOfInt_BackedBy_StructListOfInt
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public IList<int> Property { get; } = new StructList<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IListOfInt_BackedBy_StructListOfInt_WithNumberHandling()
    {
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_IListOfInt_BackedBy_StructListOfIntWithNumberHandling>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        ((StructList<int>)obj.Property).Validate();
    }

    internal class ClassWithReadOnlyProperty_IListOfInt_BackedBy_StructListOfIntWithNumberHandling
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public IList<int> Property { get; } = new StructList<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IList_BackedBy_ListOfJsonElement()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_IList_BackedBy_ListOfJsonElement>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property.Cast<JsonElement>().Select(x => x.GetInt32()));
    }

    internal class ClassWithReadOnlyProperty_IList_BackedBy_ListOfJsonElement
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public IList Property { get; } = JsonSerializer.Deserialize<List<JsonElement>>("[1,2,3]");
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IList_BackedBy_StructListOfJsonElement()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_IList_BackedBy_StructListOfJsonElement>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property.Cast<JsonElement>().Select(x => x.GetInt32()));
        ((StructList<JsonElement>)obj.Property).Validate();
    }

    internal class ClassWithReadOnlyProperty_IList_BackedBy_StructListOfJsonElement
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public IList Property { get; } = JsonSerializer.Deserialize<StructList<JsonElement>>("[1,2,3]");
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_StructListOfInt()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_StructListOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        obj.Property.Validate();
    }

    internal class ClassWithReadOnlyProperty_StructListOfInt
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public StructList<int> Property { get; set; } = new StructList<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_StructListOfInt_WithNumberHandling()
    {
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_StructListOfIntWithNumberHandling>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        obj.Property.Validate();
    }

    internal class ClassWithReadOnlyProperty_StructListOfIntWithNumberHandling
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public StructList<int> Property { get; set; } = new StructList<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_QueueOfInt()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_QueueOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    internal class ClassWithReadOnlyProperty_QueueOfInt
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public Queue<int> Property { get; } = new Queue<int>(new int[] { 1, 2, 3 });
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_QueueOfInt_WithNumberHandling()
    {
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_QueueOfIntWithNumberHandling>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    internal class ClassWithReadOnlyProperty_QueueOfIntWithNumberHandling
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public Queue<int> Property { get; } = new Queue<int>(new int[] { 1, 2, 3 });
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_Queue()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_Queue>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property.Cast<JsonElement>().Select(x => x.GetInt32()));
    }

    internal class ClassWithReadOnlyProperty_Queue
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public Queue Property { get; } = JsonSerializer.Deserialize<Queue>("[1,2,3]");
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ConcurrentQueueOfInt()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_ConcurrentQueueOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    internal class ClassWithReadOnlyProperty_ConcurrentQueueOfInt
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public ConcurrentQueue<int> Property { get; } = new ConcurrentQueue<int>(new int[] { 1, 2, 3 });
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ConcurrentQueueOfInt_WithNumberHandling()
    {
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_ConcurrentQueueOfIntWithNumberHandling>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    internal class ClassWithReadOnlyProperty_ConcurrentQueueOfIntWithNumberHandling
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public ConcurrentQueue<int> Property { get; } = new ConcurrentQueue<int>(new int[] { 1, 2, 3 });
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_StackOfInt()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_StackOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 6).Reverse(), obj.Property);
    }

    internal class ClassWithReadOnlyProperty_StackOfInt
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public Stack<int> Property { get; } = new Stack<int>(new int[] { 1, 2, 3 });
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_StackOfInt_WithNumberHandling()
    {
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_StackOfIntWithNumberHandling>(json);
        Assert.Equal(Enumerable.Range(1, 6).Reverse(), obj.Property);
    }

    internal class ClassWithReadOnlyProperty_StackOfIntWithNumberHandling
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public Stack<int> Property { get; } = new Stack<int>(new int[] { 1, 2, 3 });
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_Stack()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_Stack>(json);
        Assert.Equal(Enumerable.Range(1, 6).Reverse(), obj.Property.Cast<JsonElement>().Select(x => x.GetInt32()));
    }

    internal class ClassWithReadOnlyProperty_Stack
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public Stack Property { get; } = JsonSerializer.Deserialize<Stack>("[1,2,3]");
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ConcurrentStackOfInt()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_ConcurrentStackOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 6).Reverse(), obj.Property);
    }

    internal class ClassWithReadOnlyProperty_ConcurrentStackOfInt
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public ConcurrentStack<int> Property { get; } = new ConcurrentStack<int>(new int[] { 1, 2, 3 });
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ConcurrentStackOfInt_WithNumberHandling()
    {
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_ConcurrentStackOfIntWithNumberHandling>(json);
        Assert.Equal(Enumerable.Range(1, 6).Reverse(), obj.Property);
    }

    internal class ClassWithReadOnlyProperty_ConcurrentStackOfIntWithNumberHandling
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public ConcurrentStack<int> Property { get; } = new ConcurrentStack<int>(new int[] { 1, 2, 3 });
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ICollectionOfInt_BackedBy_ListOfInt()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_ICollectionOfInt_BackedBy_ListOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    internal class ClassWithReadOnlyProperty_ICollectionOfInt_BackedBy_ListOfInt
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public ICollection<int> Property { get; } = new List<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ICollectionOfInt_BackedBy_ListOfInt_WithNumberHandling()
    {
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_ICollectionOfInt_BackedBy_ListOfIntWithNumberHandling>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    internal class ClassWithReadOnlyProperty_ICollectionOfInt_BackedBy_ListOfIntWithNumberHandling
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public ICollection<int> Property { get; } = new List<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ICollectionOfInt_BackedBy_StructCollectionOfInt()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_ICollectionOfInt_BackedBy_StructCollectionOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        ((StructCollection<int>)obj.Property).Validate();
    }

    internal class ClassWithReadOnlyProperty_ICollectionOfInt_BackedBy_StructCollectionOfInt
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public ICollection<int> Property { get; } = new StructCollection<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ICollectionOfInt_BackedBy_StructCollectionOfInt_WithNumberHandling()
    {
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_ICollectionOfInt_BackedBy_StructCollectionOfIntWithNumberHandling>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        ((StructCollection<int>)obj.Property).Validate();
    }

    internal class ClassWithReadOnlyProperty_ICollectionOfInt_BackedBy_StructCollectionOfIntWithNumberHandling
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public ICollection<int> Property { get; } = new StructCollection<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_StructCollectionOfInt()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_StructCollectionOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        obj.Property.Validate();
    }

    internal class ClassWithReadOnlyProperty_StructCollectionOfInt
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public StructCollection<int> Property { get; set; } = new StructCollection<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_StructCollectionOfInt_WithNumberHandling()
    {
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_StructCollectionOfIntWithNumberHandling>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        obj.Property.Validate();
    }

    internal class ClassWithReadOnlyProperty_StructCollectionOfIntWithNumberHandling
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public StructCollection<int> Property { get; set; } = new StructCollection<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ISetOfInt_BackedBy_HashSetOfInt()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_ISetOfInt_BackedBy_HashSetOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    internal class ClassWithReadOnlyProperty_ISetOfInt_BackedBy_HashSetOfInt
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public ISet<int> Property { get; } = new HashSet<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ISetOfInt_BackedBy_HashSetOfInt_WithNumberHandling()
    {
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_ISetOfInt_BackedBy_HashSetOfIntWithNumberHandling>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    internal class ClassWithReadOnlyProperty_ISetOfInt_BackedBy_HashSetOfIntWithNumberHandling
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public ISet<int> Property { get; } = new HashSet<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ISetOfInt_BackedBy_StructSetOfInt()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_ISetOfInt_BackedBy_StructSetOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        ((StructSet<int>)obj.Property).Validate();
    }

    internal class ClassWithReadOnlyProperty_ISetOfInt_BackedBy_StructSetOfInt
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public ISet<int> Property { get; } = new StructSet<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ISetOfInt_BackedBy_StructSetOfInt_WithNumberHandling()
    {
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_ISetOfInt_BackedBy_StructSetOfIntWithNumberHandling>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        ((StructSet<int>)obj.Property).Validate();
    }

    internal class ClassWithReadOnlyProperty_ISetOfInt_BackedBy_StructSetOfIntWithNumberHandling
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public ISet<int> Property { get; } = new StructSet<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_StructSetOfInt()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_StructSetOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        obj.Property.Validate();
    }

    internal class ClassWithReadOnlyProperty_StructSetOfInt
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public StructSet<int> Property { get; set; } = new StructSet<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_StructSetOfInt_WithNumberHandling()
    {
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_StructSetOfIntWithNumberHandling>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        obj.Property.Validate();
    }

    internal class ClassWithReadOnlyProperty_StructSetOfIntWithNumberHandling
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public StructSet<int> Property { get; set; } = new StructSet<int>() { 1, 2, 3 };
    }
}

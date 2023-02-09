// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace System.Text.Json.Serialization.Tests;

public abstract partial class JsonCreationHandlingTests : SerializerTests
{
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
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_ListOfIntWithNumberHandling>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    internal struct StructWithReadOnlyProperty_ListOfIntWithNumberHandling
    {
        public StructWithReadOnlyProperty_ListOfIntWithNumberHandling() {}

        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public List<int> Property { get; } = new List<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IListOfInt_BackedBy_ListOfInt()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_IListOfInt_BackedBy_ListOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    internal struct StructWithReadOnlyProperty_IListOfInt_BackedBy_ListOfInt
    {
        public StructWithReadOnlyProperty_IListOfInt_BackedBy_ListOfInt() {}

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
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_IListOfInt_BackedBy_StructListOfIntWithNumberHandling>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        ((StructList<int>)obj.Property).Validate();
    }

    internal struct StructWithReadOnlyProperty_IListOfInt_BackedBy_StructListOfIntWithNumberHandling
    {
        public StructWithReadOnlyProperty_IListOfInt_BackedBy_StructListOfIntWithNumberHandling() {}

        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public IList<int> Property { get; } = new StructList<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IList_BackedBy_ListOfJsonElement()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_IList_BackedBy_ListOfJsonElement>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property.Cast<JsonElement>().Select(x => x.GetInt32()));
    }

    internal struct StructWithReadOnlyProperty_IList_BackedBy_ListOfJsonElement
    {
        public StructWithReadOnlyProperty_IList_BackedBy_ListOfJsonElement() {}

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
        var obj = await Serializer.DeserializeWrapper<ClassWithWritableProperty_StructListOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        obj.Property.Validate();
    }

    internal class ClassWithWritableProperty_StructListOfInt
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public StructList<int> Property { get; set; } = new StructList<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_StructListOfInt_WithNumberHandling()
    {
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithWritableProperty_StructListOfIntWithNumberHandling>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        obj.Property.Validate();
    }

    internal struct StructWithWritableProperty_StructListOfIntWithNumberHandling
    {
        public StructWithWritableProperty_StructListOfIntWithNumberHandling() {}

        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public StructList<int> Property { get; set; } = new StructList<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_NullableStructListOfInt()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithWritableProperty_NullableStructListOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property.Value);
        obj.Property.Value.Validate();
    }

    internal struct StructWithWritableProperty_NullableStructListOfInt
    {
        public StructWithWritableProperty_NullableStructListOfInt() {}

        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public StructList<int>? Property { get; set; } = new StructList<int>() { 1, 2, 3 };
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
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_Queue>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property.Cast<JsonElement>().Select(x => x.GetInt32()));
    }

    internal struct StructWithReadOnlyProperty_Queue
    {
        public StructWithReadOnlyProperty_Queue() {}

        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public Queue Property { get; } = JsonSerializer.Deserialize<Queue>("[1,2,3]");
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ConcurrentQueueOfInt()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_ConcurrentQueueOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    internal struct StructWithReadOnlyProperty_ConcurrentQueueOfInt
    {
        public StructWithReadOnlyProperty_ConcurrentQueueOfInt() {}

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
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_StackOfIntWithNumberHandling>(json);
        Assert.Equal(Enumerable.Range(1, 6).Reverse(), obj.Property);
    }

    internal struct StructWithReadOnlyProperty_StackOfIntWithNumberHandling
    {
        public StructWithReadOnlyProperty_StackOfIntWithNumberHandling() {}

        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public Stack<int> Property { get; } = new Stack<int>(new int[] { 1, 2, 3 });
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_Stack()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_Stack>(json);
        Assert.Equal(Enumerable.Range(1, 6).Reverse(), obj.Property.Cast<JsonElement>().Select(x => x.GetInt32()));
    }

    internal struct StructWithReadOnlyProperty_Stack
    {
        public StructWithReadOnlyProperty_Stack() {}

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
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_ICollectionOfInt_BackedBy_ListOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    internal struct StructWithReadOnlyProperty_ICollectionOfInt_BackedBy_ListOfInt
    {
        public StructWithReadOnlyProperty_ICollectionOfInt_BackedBy_ListOfInt() {}

        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public ICollection<int> Property { get; } = new List<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ICollectionOfInt_BackedBy_ListOfInt_WithNumberHandling()
    {
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_ICollectionOfInt_BackedBy_ListOfIntWithNumberHandling>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    internal struct StructWithReadOnlyProperty_ICollectionOfInt_BackedBy_ListOfIntWithNumberHandling
    {
        public StructWithReadOnlyProperty_ICollectionOfInt_BackedBy_ListOfIntWithNumberHandling() {}

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
        var obj = await Serializer.DeserializeWrapper<StructWithWritableProperty_StructCollectionOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        obj.Property.Validate();
    }

    internal struct StructWithWritableProperty_StructCollectionOfInt
    {
        public StructWithWritableProperty_StructCollectionOfInt() {}

        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public StructCollection<int> Property { get; set; } = new StructCollection<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_StructCollectionOfInt_WithNumberHandling()
    {
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithWritableProperty_StructCollectionOfIntWithNumberHandling>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        obj.Property.Validate();
    }

    internal struct StructWithWritableProperty_StructCollectionOfIntWithNumberHandling
    {
        public StructWithWritableProperty_StructCollectionOfIntWithNumberHandling() {}

        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public StructCollection<int> Property { get; set; } = new StructCollection<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_NullableStructCollectionOfInt()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithWritableProperty_NullableStructCollectionOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property.Value);
        obj.Property.Value.Validate();
    }

    internal class ClassWithWritableProperty_NullableStructCollectionOfInt
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public StructCollection<int>? Property { get; set; } = new StructCollection<int>() { 1, 2, 3 };
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
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_ISetOfInt_BackedBy_HashSetOfIntWithNumberHandling>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    internal struct StructWithReadOnlyProperty_ISetOfInt_BackedBy_HashSetOfIntWithNumberHandling
    {
        public StructWithReadOnlyProperty_ISetOfInt_BackedBy_HashSetOfIntWithNumberHandling() {}

        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public ISet<int> Property { get; } = new HashSet<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ISetOfInt_BackedBy_StructSetOfInt()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_ISetOfInt_BackedBy_StructSetOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        ((StructSet<int>)obj.Property).Validate();
    }

    internal struct StructWithReadOnlyProperty_ISetOfInt_BackedBy_StructSetOfInt
    {
        public StructWithReadOnlyProperty_ISetOfInt_BackedBy_StructSetOfInt() {}

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
        var obj = await Serializer.DeserializeWrapper<ClassWithWritableProperty_StructSetOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        obj.Property.Validate();
    }

    internal class ClassWithWritableProperty_StructSetOfInt
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public StructSet<int> Property { get; set; } = new StructSet<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_StructSetOfInt_WithNumberHandling()
    {
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithWritableProperty_StructSetOfIntWithNumberHandling>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        obj.Property.Validate();
    }

    internal struct StructWithWritableProperty_StructSetOfIntWithNumberHandling
    {
        public StructWithWritableProperty_StructSetOfIntWithNumberHandling() {}

        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public StructSet<int> Property { get; set; } = new StructSet<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_NullableStructSetOfInt()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithWritableProperty_NullableStructSetOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property.Value);
        obj.Property.Value.Validate();
    }

    internal struct StructWithWritableProperty_NullableStructSetOfInt
    {
        public StructWithWritableProperty_NullableStructSetOfInt() {}

        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public StructSet<int>? Property { get; set; } = new StructSet<int>() { 1, 2, 3 };
    }

    [Theory]
    [InlineData(typeof(ClassWithReadOnlyProperty<StructList<int>>))]
    [InlineData(typeof(ClassWithReadOnlyProperty<StructList<int>?>))]
    [InlineData(typeof(ClassWithReadOnlyProperty<StructCollection<int>>))]
    [InlineData(typeof(ClassWithReadOnlyProperty<StructCollection<int>?>))]
    [InlineData(typeof(ClassWithReadOnlyProperty<StructSet<int>>))]
    [InlineData(typeof(ClassWithReadOnlyProperty<StructSet<int>?>))]
    public async Task CreationHandlingSetWithAttribute_PopulateWithoutSetterOnValueTypeThrows(Type type)
    {
        string json = """{"Property":[4,5,6]}""";
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await Serializer.DeserializeWrapper(json, type));
    }
}

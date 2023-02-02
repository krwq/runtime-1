// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xunit;

namespace System.Text.Json.Serialization.Tests;

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

// - set through attribute and metadata
// - polymorphism (i.e. populate polymorphic object, ensure it's within allowed hierarchy or do base type)
// - all cases from main issue for dictionary, collection and object
// - incompatible options
// - put Populate on property with null value
// - deserialize null on property which is supposed to be populated
public abstract class JsonCreationHandlingTests : SerializerTests
{
    public JsonCreationHandlingTests(JsonSerializerWrapper serializerUnderTest) : base(serializerUnderTest)
    {
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ListOfInt()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyListOfIntProperty>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    internal class ClassWithReadOnlyListOfIntProperty
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public List<int> Property { get; } = new List<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ListOfInt_WithNumberHandling()
    {
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyListOfIntPropertyWithNumberHandling>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    internal class ClassWithReadOnlyListOfIntPropertyWithNumberHandling
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public List<int> Property { get; } = new List<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IListOfInt()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyIListOfIntProperty>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    internal class ClassWithReadOnlyIListOfIntProperty
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public IList<int> Property { get; } = new List<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IListOfInt_WithNumberHandling()
    {
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyIListOfIntPropertyWithNumberHandling>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    internal class ClassWithReadOnlyIListOfIntPropertyWithNumberHandling
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public IList<int> Property { get; } = new List<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IList()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyIListProperty>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property.Cast<JsonElement>().Select(x => x.GetInt32()));
    }

    internal class ClassWithReadOnlyIListProperty
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public IList Property { get; } = JsonSerializer.Deserialize<List<JsonElement>>("[1,2,3]");
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_QueueOfInt()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyQueueOfIntProperty>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    internal class ClassWithReadOnlyQueueOfIntProperty
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public Queue<int> Property { get; } = new Queue<int>(new int[] { 1, 2, 3 });
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_QueueOfInt_WithNumberHandling()
    {
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyQueueOfIntPropertyWithNumberHandling>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    internal class ClassWithReadOnlyQueueOfIntPropertyWithNumberHandling
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public Queue<int> Property { get; } = new Queue<int>(new int[] { 1, 2, 3 });
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_Queue()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyQueueProperty>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property.Cast<JsonElement>().Select(x => x.GetInt32()));
    }

    internal class ClassWithReadOnlyQueueProperty
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public Queue Property { get; } = JsonSerializer.Deserialize<Queue>("[1,2,3]");
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ConcurrentQueueOfInt()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyConcurrentQueueOfIntProperty>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    internal class ClassWithReadOnlyConcurrentQueueOfIntProperty
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public ConcurrentQueue<int> Property { get; } = new ConcurrentQueue<int>(new int[] { 1, 2, 3 });
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ConcurrentQueueOfInt_WithNumberHandling()
    {
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyConcurrentQueueOfIntPropertyWithNumberHandling>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    internal class ClassWithReadOnlyConcurrentQueueOfIntPropertyWithNumberHandling
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public ConcurrentQueue<int> Property { get; } = new ConcurrentQueue<int>(new int[] { 1, 2, 3 });
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_StackOfInt()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyStackOfIntProperty>(json);
        Assert.Equal(Enumerable.Range(1, 6).Reverse(), obj.Property);
    }

    internal class ClassWithReadOnlyStackOfIntProperty
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public Stack<int> Property { get; } = new Stack<int>(new int[] { 1, 2, 3 });
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_StackOfInt_WithNumberHandling()
    {
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyStackOfIntPropertyWithNumberHandling>(json);
        Assert.Equal(Enumerable.Range(1, 6).Reverse(), obj.Property);
    }

    internal class ClassWithReadOnlyStackOfIntPropertyWithNumberHandling
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public Stack<int> Property { get; } = new Stack<int>(new int[] { 1, 2, 3 });
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_Stack()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyStackProperty>(json);
        Assert.Equal(Enumerable.Range(1, 6).Reverse(), obj.Property.Cast<JsonElement>().Select(x => x.GetInt32()));
    }

    internal class ClassWithReadOnlyStackProperty
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public Stack Property { get; } = JsonSerializer.Deserialize<Stack>("[1,2,3]");
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ConcurrentStackOfInt()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyConcurrentStackOfIntProperty>(json);
        Assert.Equal(Enumerable.Range(1, 6).Reverse(), obj.Property);
    }

    internal class ClassWithReadOnlyConcurrentStackOfIntProperty
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public ConcurrentStack<int> Property { get; } = new ConcurrentStack<int>(new int[] { 1, 2, 3 });
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ConcurrentStackOfInt_WithNumberHandling()
    {
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyConcurrentStackOfIntPropertyWithNumberHandling>(json);
        Assert.Equal(Enumerable.Range(1, 6).Reverse(), obj.Property);
    }

    internal class ClassWithReadOnlyConcurrentStackOfIntPropertyWithNumberHandling
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public ConcurrentStack<int> Property { get; } = new ConcurrentStack<int>(new int[] { 1, 2, 3 });
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ICollectionOfInt()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyICollectionOfIntProperty>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    internal class ClassWithReadOnlyICollectionOfIntProperty
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public ICollection<int> Property { get; } = new List<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ICollectionOfInt_WithNumberHandling()
    {
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyICollectionOfIntPropertyWithNumberHandling>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    internal class ClassWithReadOnlyICollectionOfIntPropertyWithNumberHandling
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public ICollection<int> Property { get; } = new List<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ISetOfInt()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyISetOfIntProperty>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    internal class ClassWithReadOnlyISetOfIntProperty
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public ISet<int> Property { get; } = new HashSet<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ISetOfInt_WithNumberHandling()
    {
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyISetOfIntPropertyWithNumberHandling>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    internal class ClassWithReadOnlyISetOfIntPropertyWithNumberHandling
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public ISet<int> Property { get; } = new HashSet<int>() { 1, 2, 3 };
    }

}

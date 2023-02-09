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
    public async Task CreationHandlingSetWithAttribute_CanPopulate_DictionaryOfStringToInt()
    {
        string json = """{"Property":{"d":4,"e":5,"f":6}}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_DictionaryOfStringToInt>(json);
        CheckGenericDictionaryContent(obj.Property);
    }

    internal class ClassWithReadOnlyProperty_DictionaryOfStringToInt
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public Dictionary<string, int> Property { get; } = new Dictionary<string, int>() { ["a"] = 1, ["b"] = 2, ["c"] = 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_DictionaryOfStringToInt_WithNumberHandling()
    {
        string json = """{"Property":{"d":"4","e":"5","f":"6"}}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_DictionaryOfStringToIntWithNumberHandling>(json);
        CheckGenericDictionaryContent(obj.Property);
    }

    internal class ClassWithReadOnlyProperty_DictionaryOfStringToIntWithNumberHandling
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public Dictionary<string, int> Property { get; } = new Dictionary<string, int>() { ["a"] = 1, ["b"] = 2, ["c"] = 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IDictionaryOfStringToInt_BackedBy_DictionaryOfStringToInt()
    {
        string json = """{"Property":{"d":4,"e":5,"f":6}}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_IDictionaryOfStringToInt_BackedBy_DictionaryOfStringToInt>(json);
        CheckGenericDictionaryContent(obj.Property);
    }

    internal class ClassWithReadOnlyProperty_IDictionaryOfStringToInt_BackedBy_DictionaryOfStringToInt
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public IDictionary<string, int> Property { get; } = new Dictionary<string, int>() { ["a"] = 1, ["b"] = 2, ["c"] = 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IDictionaryOfStringToInt_BackedBy_DictionaryOfStringToInt_WithNumberHandling()
    {
        string json = """{"Property":{"d":"4","e":"5","f":"6"}}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_IDictionaryOfStringToInt_BackedBy_DictionaryOfStringToIntWithNumberHandling>(json);
        CheckGenericDictionaryContent(obj.Property);
    }

    internal class ClassWithReadOnlyProperty_IDictionaryOfStringToInt_BackedBy_DictionaryOfStringToIntWithNumberHandling
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public IDictionary<string, int> Property { get; } = new Dictionary<string, int>() { ["a"] = 1, ["b"] = 2, ["c"] = 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IDictionaryOfStringToInt_BackedBy_StructDictionaryOfStringToInt()
    {
        string json = """{"Property":{"d":4,"e":5,"f":6}}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_IDictionaryOfStringToInt_BackedBy_StructDictionaryOfStringToInt>(json);
        CheckGenericDictionaryContent(obj.Property);
        ((StructDictionary<string, int>)obj.Property).Validate();
    }

    internal class ClassWithReadOnlyProperty_IDictionaryOfStringToInt_BackedBy_StructDictionaryOfStringToInt
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public IDictionary<string, int> Property { get; } = new StructDictionary<string, int>() { ["a"] = 1, ["b"] = 2, ["c"] = 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IDictionaryOfStringToInt_BackedBy_StructDictionaryOfStringToInt_WithNumberHandling()
    {
        string json = """{"Property":{"d":"4","e":"5","f":"6"}}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_IDictionaryOfStringToInt_BackedBy_StructDictionaryOfStringToIntWithNumberHandling>(json);
        CheckGenericDictionaryContent(obj.Property);
        ((StructDictionary<string, int>)obj.Property).Validate();
    }

    internal class ClassWithReadOnlyProperty_IDictionaryOfStringToInt_BackedBy_StructDictionaryOfStringToIntWithNumberHandling
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public IDictionary<string, int> Property { get; } = new StructDictionary<string, int>() { ["a"] = 1, ["b"] = 2, ["c"] = 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IDictionary_BackedBy_DictionaryOfStringToJsonElement()
    {
        string json = """{"Property":{"d":4,"e":5,"f":6}}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_IDictionary_BackedBy_DictionaryOfStringToJsonElement>(json);
        CheckDictionaryContent(obj.Property);
    }

    internal class ClassWithReadOnlyProperty_IDictionary_BackedBy_DictionaryOfStringToJsonElement
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public IDictionary Property { get; } = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>("""{"a":1,"b":2,"c":3}""");
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IDictionary_BackedBy_StructDictionaryOfStringToJsonElement()
    {
        string json = """{"Property":{"d":4,"e":5,"f":6}}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_IDictionary_BackedBy_StructDictionaryOfStringToJsonElement>(json);
        CheckDictionaryContent(obj.Property);
        ((StructDictionary<string, JsonElement>)obj.Property).Validate();
    }

    internal class ClassWithReadOnlyProperty_IDictionary_BackedBy_StructDictionaryOfStringToJsonElement
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public IDictionary Property { get; } = JsonSerializer.Deserialize<StructDictionary<string, JsonElement>>("""{"a":1,"b":2,"c":3}""");
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_StructDictionaryOfStringToInt()
    {
        string json = """{"Property":{"d":4,"e":5,"f":6}}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithWritableProperty_StructDictionaryOfStringToInt>(json);
        CheckGenericDictionaryContent(obj.Property);
        obj.Property.Validate();
    }

    internal class ClassWithWritableProperty_StructDictionaryOfStringToInt
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public StructDictionary<string, int> Property { get; set; } = new StructDictionary<string, int>() { ["a"] = 1, ["b"] = 2, ["c"] = 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_StructDictionaryOfStringToInt_WithNumberHandling()
    {
        string json = """{"Property":{"d":"4","e":"5","f":"6"}}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithWritableProperty_StructDictionaryOfStringToIntWithNumberHandling>(json);
        CheckGenericDictionaryContent(obj.Property);
        obj.Property.Validate();
    }

    internal class ClassWithWritableProperty_StructDictionaryOfStringToIntWithNumberHandling
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public StructDictionary<string, int> Property { get; set; } = new StructDictionary<string, int>() { ["a"] = 1, ["b"] = 2, ["c"] = 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ConcurrentDictionaryOfStringToInt()
    {
        string json = """{"Property":{"d":4,"e":5,"f":6}}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_ConcurrentDictionaryOfStringToInt>(json);
        CheckGenericDictionaryContent(obj.Property);
    }

    internal class ClassWithReadOnlyProperty_ConcurrentDictionaryOfStringToInt
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public ConcurrentDictionary<string, int> Property { get; } = new ConcurrentDictionary<string, int>() { ["a"] = 1, ["b"] = 2, ["c"] = 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ConcurrentDictionaryOfStringToInt_WithNumberHandling()
    {
        string json = """{"Property":{"d":"4","e":"5","f":"6"}}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_ConcurrentDictionaryOfStringToIntWithNumberHandling>(json);
        CheckGenericDictionaryContent(obj.Property);
    }

    internal class ClassWithReadOnlyProperty_ConcurrentDictionaryOfStringToIntWithNumberHandling
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public ConcurrentDictionary<string, int> Property { get; } = new ConcurrentDictionary<string, int>() { ["a"] = 1, ["b"] = 2, ["c"] = 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_SortedDictionaryOfStringToInt()
    {
        string json = """{"Property":{"d":4,"e":5,"f":6}}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_SortedDictionaryOfStringToInt>(json);
        CheckGenericDictionaryContent(obj.Property);
    }

    internal class ClassWithReadOnlyProperty_SortedDictionaryOfStringToInt
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public SortedDictionary<string, int> Property { get; } = new SortedDictionary<string, int>() { ["a"] = 1, ["b"] = 2, ["c"] = 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_SortedDictionaryOfStringToInt_WithNumberHandling()
    {
        string json = """{"Property":{"d":"4","e":"5","f":"6"}}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_SortedDictionaryOfStringToIntWithNumberHandling>(json);
        CheckGenericDictionaryContent(obj.Property);
    }

    internal class ClassWithReadOnlyProperty_SortedDictionaryOfStringToIntWithNumberHandling
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public SortedDictionary<string, int> Property { get; } = new SortedDictionary<string, int>() { ["a"] = 1, ["b"] = 2, ["c"] = 3 };
    }

}

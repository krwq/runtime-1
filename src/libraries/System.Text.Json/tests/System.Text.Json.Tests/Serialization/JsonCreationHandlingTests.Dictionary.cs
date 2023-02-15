// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization.Metadata;
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
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(ClassWithReadOnlyProperty_DictionaryOfStringToInt));
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_DictionaryOfStringToInt_PropertyOccuringMultipleTimes()
    {
        string json = """{"Property":{"d:":4},"Property":{"e:":5},"Property":{"f:":6}}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_DictionaryOfStringToInt>(json);
        CheckGenericDictionaryContent(obj.Property);
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_DictionaryOfStringToInt_PropertyOccuringMultipleTimes_NullInBetween()
    {
        string json = """{"Property":{"d:":4},"Property":null,"Property":{"a:":1},"Property":{"b:":2}}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithWritableProperty<Dictionary<string, int>>>(json);
        CheckGenericDictionaryContent(obj.Property, 2);
    }

    internal class ClassWithReadOnlyProperty_DictionaryOfStringToInt
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public Dictionary<string, int> Property { get; } = new Dictionary<string, int>() { ["a"] = 1, ["b"] = 2, ["c"] = 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_DictionaryOfStringToInt_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(ClassWithReadOnlyProperty_DictionaryOfStringToIntWithoutPopulateAttribute));
        string json = """{"Property":{"d":4,"e":5,"f":6}}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_DictionaryOfStringToIntWithoutPopulateAttribute>(json, options);
        CheckGenericDictionaryContent(obj.Property);
    }

    internal class ClassWithReadOnlyProperty_DictionaryOfStringToIntWithoutPopulateAttribute
    {
        public Dictionary<string, int> Property { get; } = new Dictionary<string, int>() { ["a"] = 1, ["b"] = 2, ["c"] = 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_DictionaryOfStringToInt_WithNumberHandling()
    {
        string json = """{"Property":{"d":"4","e":"5","f":"6"}}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_DictionaryOfStringToIntWithNumberHandling>(json);
        CheckGenericDictionaryContent(obj.Property);
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(StructWithReadOnlyProperty_DictionaryOfStringToIntWithNumberHandling));
    }

    internal struct StructWithReadOnlyProperty_DictionaryOfStringToIntWithNumberHandling
    {
        public StructWithReadOnlyProperty_DictionaryOfStringToIntWithNumberHandling() {}

        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public Dictionary<string, int> Property { get; } = new Dictionary<string, int>() { ["a"] = 1, ["b"] = 2, ["c"] = 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_DictionaryOfStringToInt_WithNumberHandling_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(StructWithReadOnlyProperty_DictionaryOfStringToIntWithNumberHandlingWithoutPopulateAttribute));
        string json = """{"Property":{"d":"4","e":"5","f":"6"}}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_DictionaryOfStringToIntWithNumberHandlingWithoutPopulateAttribute>(json, options);
        CheckGenericDictionaryContent(obj.Property);
    }

    internal struct StructWithReadOnlyProperty_DictionaryOfStringToIntWithNumberHandlingWithoutPopulateAttribute
    {
        public StructWithReadOnlyProperty_DictionaryOfStringToIntWithNumberHandlingWithoutPopulateAttribute() {}

        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public Dictionary<string, int> Property { get; } = new Dictionary<string, int>() { ["a"] = 1, ["b"] = 2, ["c"] = 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IDictionaryOfStringToInt_BackedBy_DictionaryOfStringToInt()
    {
        string json = """{"Property":{"d":4,"e":5,"f":6}}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_IDictionaryOfStringToInt_BackedBy_DictionaryOfStringToInt>(json);
        CheckGenericDictionaryContent(obj.Property);
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(StructWithReadOnlyProperty_IDictionaryOfStringToInt_BackedBy_DictionaryOfStringToInt));
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IDictionaryOfStringToInt_BackedBy_DictionaryOfStringToInt_PropertyOccuringMultipleTimes()
    {
        string json = """{"Property":{"d:":4},"Property":{"e:":5},"Property":{"f:":6}}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_IDictionaryOfStringToInt_BackedBy_DictionaryOfStringToInt>(json);
        CheckGenericDictionaryContent(obj.Property);
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IDictionaryOfStringToInt_PropertyOccuringMultipleTimes_NullInBetween()
    {
        string json = """{"Property":{"d:":4},"Property":null,"Property":{"a:":1},"Property":{"b:":2}}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithWritableProperty<IDictionary<string, int>>>(json);
        CheckGenericDictionaryContent(obj.Property, 2);
    }

    internal struct StructWithReadOnlyProperty_IDictionaryOfStringToInt_BackedBy_DictionaryOfStringToInt
    {
        public StructWithReadOnlyProperty_IDictionaryOfStringToInt_BackedBy_DictionaryOfStringToInt() {}

        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public IDictionary<string, int> Property { get; } = new Dictionary<string, int>() { ["a"] = 1, ["b"] = 2, ["c"] = 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IDictionaryOfStringToInt_BackedBy_DictionaryOfStringToInt_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(StructWithReadOnlyProperty_IDictionaryOfStringToInt_BackedBy_DictionaryOfStringToIntWithoutPopulateAttribute));
        string json = """{"Property":{"d":4,"e":5,"f":6}}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_IDictionaryOfStringToInt_BackedBy_DictionaryOfStringToIntWithoutPopulateAttribute>(json, options);
        CheckGenericDictionaryContent(obj.Property);
    }

    internal struct StructWithReadOnlyProperty_IDictionaryOfStringToInt_BackedBy_DictionaryOfStringToIntWithoutPopulateAttribute
    {
        public StructWithReadOnlyProperty_IDictionaryOfStringToInt_BackedBy_DictionaryOfStringToIntWithoutPopulateAttribute() {}

        public IDictionary<string, int> Property { get; } = new Dictionary<string, int>() { ["a"] = 1, ["b"] = 2, ["c"] = 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IDictionaryOfStringToInt_BackedBy_DictionaryOfStringToInt_WithNumberHandling()
    {
        string json = """{"Property":{"d":"4","e":"5","f":"6"}}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_IDictionaryOfStringToInt_BackedBy_DictionaryOfStringToIntWithNumberHandling>(json);
        CheckGenericDictionaryContent(obj.Property);
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(ClassWithReadOnlyProperty_IDictionaryOfStringToInt_BackedBy_DictionaryOfStringToIntWithNumberHandling));
    }

    internal class ClassWithReadOnlyProperty_IDictionaryOfStringToInt_BackedBy_DictionaryOfStringToIntWithNumberHandling
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public IDictionary<string, int> Property { get; } = new Dictionary<string, int>() { ["a"] = 1, ["b"] = 2, ["c"] = 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IDictionaryOfStringToInt_BackedBy_DictionaryOfStringToInt_WithNumberHandling_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(ClassWithReadOnlyProperty_IDictionaryOfStringToInt_BackedBy_DictionaryOfStringToIntWithNumberHandlingWithoutPopulateAttribute));
        string json = """{"Property":{"d":"4","e":"5","f":"6"}}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_IDictionaryOfStringToInt_BackedBy_DictionaryOfStringToIntWithNumberHandlingWithoutPopulateAttribute>(json, options);
        CheckGenericDictionaryContent(obj.Property);
    }

    internal class ClassWithReadOnlyProperty_IDictionaryOfStringToInt_BackedBy_DictionaryOfStringToIntWithNumberHandlingWithoutPopulateAttribute
    {
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
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(ClassWithReadOnlyProperty_IDictionaryOfStringToInt_BackedBy_StructDictionaryOfStringToInt));
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IDictionaryOfStringToInt_BackedBy_StructDictionaryOfStringToInt_PropertyOccuringMultipleTimes()
    {
        string json = """{"Property":{"d:":4},"Property":{"e:":5},"Property":{"f:":6}}""";
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
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IDictionaryOfStringToInt_BackedBy_StructDictionaryOfStringToInt_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(ClassWithReadOnlyProperty_IDictionaryOfStringToInt_BackedBy_StructDictionaryOfStringToIntWithoutPopulateAttribute));
        string json = """{"Property":{"d":4,"e":5,"f":6}}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_IDictionaryOfStringToInt_BackedBy_StructDictionaryOfStringToIntWithoutPopulateAttribute>(json, options);
        CheckGenericDictionaryContent(obj.Property);
        ((StructDictionary<string, int>)obj.Property).Validate();
    }

    internal class ClassWithReadOnlyProperty_IDictionaryOfStringToInt_BackedBy_StructDictionaryOfStringToIntWithoutPopulateAttribute
    {
        public IDictionary<string, int> Property { get; } = new StructDictionary<string, int>() { ["a"] = 1, ["b"] = 2, ["c"] = 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IDictionaryOfStringToInt_BackedBy_StructDictionaryOfStringToInt_WithNumberHandling()
    {
        string json = """{"Property":{"d":"4","e":"5","f":"6"}}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_IDictionaryOfStringToInt_BackedBy_StructDictionaryOfStringToIntWithNumberHandling>(json);
        CheckGenericDictionaryContent(obj.Property);
        ((StructDictionary<string, int>)obj.Property).Validate();
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(StructWithReadOnlyProperty_IDictionaryOfStringToInt_BackedBy_StructDictionaryOfStringToIntWithNumberHandling));
    }

    internal struct StructWithReadOnlyProperty_IDictionaryOfStringToInt_BackedBy_StructDictionaryOfStringToIntWithNumberHandling
    {
        public StructWithReadOnlyProperty_IDictionaryOfStringToInt_BackedBy_StructDictionaryOfStringToIntWithNumberHandling() {}

        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public IDictionary<string, int> Property { get; } = new StructDictionary<string, int>() { ["a"] = 1, ["b"] = 2, ["c"] = 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IDictionaryOfStringToInt_BackedBy_StructDictionaryOfStringToInt_WithNumberHandling_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(StructWithReadOnlyProperty_IDictionaryOfStringToInt_BackedBy_StructDictionaryOfStringToIntWithNumberHandlingWithoutPopulateAttribute));
        string json = """{"Property":{"d":"4","e":"5","f":"6"}}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_IDictionaryOfStringToInt_BackedBy_StructDictionaryOfStringToIntWithNumberHandlingWithoutPopulateAttribute>(json, options);
        CheckGenericDictionaryContent(obj.Property);
        ((StructDictionary<string, int>)obj.Property).Validate();
    }

    internal struct StructWithReadOnlyProperty_IDictionaryOfStringToInt_BackedBy_StructDictionaryOfStringToIntWithNumberHandlingWithoutPopulateAttribute
    {
        public StructWithReadOnlyProperty_IDictionaryOfStringToInt_BackedBy_StructDictionaryOfStringToIntWithNumberHandlingWithoutPopulateAttribute() {}

        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public IDictionary<string, int> Property { get; } = new StructDictionary<string, int>() { ["a"] = 1, ["b"] = 2, ["c"] = 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IDictionary_BackedBy_DictionaryOfStringToJsonElement()
    {
        string json = """{"Property":{"d":4,"e":5,"f":6}}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_IDictionary_BackedBy_DictionaryOfStringToJsonElement>(json);
        CheckDictionaryContent(obj.Property);
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(StructWithReadOnlyProperty_IDictionary_BackedBy_DictionaryOfStringToJsonElement));
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IDictionary_BackedBy_DictionaryOfStringToJsonElement_PropertyOccuringMultipleTimes()
    {
        string json = """{"Property":{"d:":4},"Property":{"e:":5},"Property":{"f:":6}}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_IDictionary_BackedBy_DictionaryOfStringToJsonElement>(json);
        CheckDictionaryContent(obj.Property);
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IDictionary_PropertyOccuringMultipleTimes_NullInBetween()
    {
        string json = """{"Property":{"d:":4},"Property":null,"Property":{"a:":1},"Property":{"b:":2}}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithWritableProperty<IDictionary>>(json);
        CheckDictionaryContent(obj.Property, 2);
    }

    internal struct StructWithReadOnlyProperty_IDictionary_BackedBy_DictionaryOfStringToJsonElement
    {
        public StructWithReadOnlyProperty_IDictionary_BackedBy_DictionaryOfStringToJsonElement() {}

        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public IDictionary Property { get; } = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>("""{"a":1,"b":2,"c":3}""");
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IDictionary_BackedBy_DictionaryOfStringToJsonElement_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(StructWithReadOnlyProperty_IDictionary_BackedBy_DictionaryOfStringToJsonElementWithoutPopulateAttribute));
        string json = """{"Property":{"d":4,"e":5,"f":6}}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_IDictionary_BackedBy_DictionaryOfStringToJsonElementWithoutPopulateAttribute>(json, options);
        CheckDictionaryContent(obj.Property);
    }

    internal struct StructWithReadOnlyProperty_IDictionary_BackedBy_DictionaryOfStringToJsonElementWithoutPopulateAttribute
    {
        public StructWithReadOnlyProperty_IDictionary_BackedBy_DictionaryOfStringToJsonElementWithoutPopulateAttribute() {}

        public IDictionary Property { get; } = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>("""{"a":1,"b":2,"c":3}""");
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IDictionary_BackedBy_StructDictionaryOfStringToJsonElement()
    {
        string json = """{"Property":{"d":4,"e":5,"f":6}}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_IDictionary_BackedBy_StructDictionaryOfStringToJsonElement>(json);
        CheckDictionaryContent(obj.Property);
        ((StructDictionary<string, JsonElement>)obj.Property).Validate();
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(ClassWithReadOnlyProperty_IDictionary_BackedBy_StructDictionaryOfStringToJsonElement));
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IDictionary_BackedBy_StructDictionaryOfStringToJsonElement_PropertyOccuringMultipleTimes()
    {
        string json = """{"Property":{"d:":4},"Property":{"e:":5},"Property":{"f:":6}}""";
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
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IDictionary_BackedBy_StructDictionaryOfStringToJsonElement_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(ClassWithReadOnlyProperty_IDictionary_BackedBy_StructDictionaryOfStringToJsonElementWithoutPopulateAttribute));
        string json = """{"Property":{"d":4,"e":5,"f":6}}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_IDictionary_BackedBy_StructDictionaryOfStringToJsonElementWithoutPopulateAttribute>(json, options);
        CheckDictionaryContent(obj.Property);
        ((StructDictionary<string, JsonElement>)obj.Property).Validate();
    }

    internal class ClassWithReadOnlyProperty_IDictionary_BackedBy_StructDictionaryOfStringToJsonElementWithoutPopulateAttribute
    {
        public IDictionary Property { get; } = JsonSerializer.Deserialize<StructDictionary<string, JsonElement>>("""{"a":1,"b":2,"c":3}""");
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_StructDictionaryOfStringToInt()
    {
        string json = """{"Property":{"d":4,"e":5,"f":6}}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithWritableProperty_StructDictionaryOfStringToInt>(json);
        CheckGenericDictionaryContent(obj.Property);
        obj.Property.Validate();
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(ClassWithWritableProperty_StructDictionaryOfStringToInt));
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_StructDictionaryOfStringToInt_PropertyOccuringMultipleTimes()
    {
        string json = """{"Property":{"d:":4},"Property":{"e:":5},"Property":{"f:":6}}""";
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
    public async Task CreationHandlingSetWithAttribute_CanPopulate_StructDictionaryOfStringToInt_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(ClassWithWritableProperty_StructDictionaryOfStringToIntWithoutPopulateAttribute));
        string json = """{"Property":{"d":4,"e":5,"f":6}}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithWritableProperty_StructDictionaryOfStringToIntWithoutPopulateAttribute>(json, options);
        CheckGenericDictionaryContent(obj.Property);
        obj.Property.Validate();
    }

    internal class ClassWithWritableProperty_StructDictionaryOfStringToIntWithoutPopulateAttribute
    {
        public StructDictionary<string, int> Property { get; set; } = new StructDictionary<string, int>() { ["a"] = 1, ["b"] = 2, ["c"] = 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_StructDictionaryOfStringToInt_WithNumberHandling()
    {
        string json = """{"Property":{"d":"4","e":"5","f":"6"}}""";
        var obj = await Serializer.DeserializeWrapper<StructWithWritableProperty_StructDictionaryOfStringToIntWithNumberHandling>(json);
        CheckGenericDictionaryContent(obj.Property);
        obj.Property.Validate();
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(StructWithWritableProperty_StructDictionaryOfStringToIntWithNumberHandling));
    }

    internal struct StructWithWritableProperty_StructDictionaryOfStringToIntWithNumberHandling
    {
        public StructWithWritableProperty_StructDictionaryOfStringToIntWithNumberHandling() {}

        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public StructDictionary<string, int> Property { get; set; } = new StructDictionary<string, int>() { ["a"] = 1, ["b"] = 2, ["c"] = 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_StructDictionaryOfStringToInt_WithNumberHandling_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(StructWithWritableProperty_StructDictionaryOfStringToIntWithNumberHandlingWithoutPopulateAttribute));
        string json = """{"Property":{"d":"4","e":"5","f":"6"}}""";
        var obj = await Serializer.DeserializeWrapper<StructWithWritableProperty_StructDictionaryOfStringToIntWithNumberHandlingWithoutPopulateAttribute>(json, options);
        CheckGenericDictionaryContent(obj.Property);
        obj.Property.Validate();
    }

    internal struct StructWithWritableProperty_StructDictionaryOfStringToIntWithNumberHandlingWithoutPopulateAttribute
    {
        public StructWithWritableProperty_StructDictionaryOfStringToIntWithNumberHandlingWithoutPopulateAttribute() {}

        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public StructDictionary<string, int> Property { get; set; } = new StructDictionary<string, int>() { ["a"] = 1, ["b"] = 2, ["c"] = 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_NullableStructDictionaryOfStringToInt()
    {
        string json = """{"Property":{"d":4,"e":5,"f":6}}""";
        var obj = await Serializer.DeserializeWrapper<StructWithWritableProperty_NullableStructDictionaryOfStringToInt>(json);
        CheckGenericDictionaryContent(obj.Property.Value);
        obj.Property.Value.Validate();
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(StructWithWritableProperty_NullableStructDictionaryOfStringToInt));
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_PopulatedPropertyCanDeserializeNull_NullableStructDictionaryOfStringToInt()
    {
        string json = """{"Property":null}""";
        var obj = await Serializer.DeserializeWrapper<StructWithWritableProperty_NullableStructDictionaryOfStringToInt>(json);
        Assert.Null(obj.Property);
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_NullableStructDictionaryOfStringToInt_PropertyOccuringMultipleTimes()
    {
        string json = """{"Property":{"d:":4},"Property":{"e:":5},"Property":{"f:":6}}""";
        var obj = await Serializer.DeserializeWrapper<StructWithWritableProperty_NullableStructDictionaryOfStringToInt>(json);
        CheckGenericDictionaryContent(obj.Property.Value);
        obj.Property.Value.Validate();
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_NullableStructDictionaryOfStringToInt_PropertyOccuringMultipleTimes_NullInBetween()
    {
        string json = """{"Property":{"d:":4},"Property":null,"Property":{"a:":1},"Property":{"b:":2}}""";
        var obj = await Serializer.DeserializeWrapper<StructWithWritableProperty_NullableStructDictionaryOfStringToInt>(json);
        CheckGenericDictionaryContent(obj.Property.Value, 2);
        obj.Property.Value.Validate();
    }

    internal struct StructWithWritableProperty_NullableStructDictionaryOfStringToInt
    {
        public StructWithWritableProperty_NullableStructDictionaryOfStringToInt() {}

        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public StructDictionary<string, int>? Property { get; set; } = new StructDictionary<string, int>() { ["a"] = 1, ["b"] = 2, ["c"] = 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_NullableStructDictionaryOfStringToInt_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(StructWithWritableProperty_NullableStructDictionaryOfStringToIntWithoutPopulateAttribute));
        string json = """{"Property":{"d":4,"e":5,"f":6}}""";
        var obj = await Serializer.DeserializeWrapper<StructWithWritableProperty_NullableStructDictionaryOfStringToIntWithoutPopulateAttribute>(json, options);
        CheckGenericDictionaryContent(obj.Property.Value);
        obj.Property.Value.Validate();
    }

    internal struct StructWithWritableProperty_NullableStructDictionaryOfStringToIntWithoutPopulateAttribute
    {
        public StructWithWritableProperty_NullableStructDictionaryOfStringToIntWithoutPopulateAttribute() {}

        public StructDictionary<string, int>? Property { get; set; } = new StructDictionary<string, int>() { ["a"] = 1, ["b"] = 2, ["c"] = 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ConcurrentDictionaryOfStringToInt()
    {
        string json = """{"Property":{"d":4,"e":5,"f":6}}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_ConcurrentDictionaryOfStringToInt>(json);
        CheckGenericDictionaryContent(obj.Property);
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(ClassWithReadOnlyProperty_ConcurrentDictionaryOfStringToInt));
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ConcurrentDictionaryOfStringToInt_PropertyOccuringMultipleTimes()
    {
        string json = """{"Property":{"d:":4},"Property":{"e:":5},"Property":{"f:":6}}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_ConcurrentDictionaryOfStringToInt>(json);
        CheckGenericDictionaryContent(obj.Property);
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ConcurrentDictionaryOfStringToInt_PropertyOccuringMultipleTimes_NullInBetween()
    {
        string json = """{"Property":{"d:":4},"Property":null,"Property":{"a:":1},"Property":{"b:":2}}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithWritableProperty<ConcurrentDictionary<string, int>>>(json);
        CheckGenericDictionaryContent(obj.Property, 2);
    }

    internal class ClassWithReadOnlyProperty_ConcurrentDictionaryOfStringToInt
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public ConcurrentDictionary<string, int> Property { get; } = new ConcurrentDictionary<string, int>() { ["a"] = 1, ["b"] = 2, ["c"] = 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ConcurrentDictionaryOfStringToInt_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(ClassWithReadOnlyProperty_ConcurrentDictionaryOfStringToIntWithoutPopulateAttribute));
        string json = """{"Property":{"d":4,"e":5,"f":6}}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_ConcurrentDictionaryOfStringToIntWithoutPopulateAttribute>(json, options);
        CheckGenericDictionaryContent(obj.Property);
    }

    internal class ClassWithReadOnlyProperty_ConcurrentDictionaryOfStringToIntWithoutPopulateAttribute
    {
        public ConcurrentDictionary<string, int> Property { get; } = new ConcurrentDictionary<string, int>() { ["a"] = 1, ["b"] = 2, ["c"] = 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ConcurrentDictionaryOfStringToInt_WithNumberHandling()
    {
        string json = """{"Property":{"d":"4","e":"5","f":"6"}}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_ConcurrentDictionaryOfStringToIntWithNumberHandling>(json);
        CheckGenericDictionaryContent(obj.Property);
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(ClassWithReadOnlyProperty_ConcurrentDictionaryOfStringToIntWithNumberHandling));
    }

    internal class ClassWithReadOnlyProperty_ConcurrentDictionaryOfStringToIntWithNumberHandling
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public ConcurrentDictionary<string, int> Property { get; } = new ConcurrentDictionary<string, int>() { ["a"] = 1, ["b"] = 2, ["c"] = 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ConcurrentDictionaryOfStringToInt_WithNumberHandling_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(ClassWithReadOnlyProperty_ConcurrentDictionaryOfStringToIntWithNumberHandlingWithoutPopulateAttribute));
        string json = """{"Property":{"d":"4","e":"5","f":"6"}}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_ConcurrentDictionaryOfStringToIntWithNumberHandlingWithoutPopulateAttribute>(json, options);
        CheckGenericDictionaryContent(obj.Property);
    }

    internal class ClassWithReadOnlyProperty_ConcurrentDictionaryOfStringToIntWithNumberHandlingWithoutPopulateAttribute
    {
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public ConcurrentDictionary<string, int> Property { get; } = new ConcurrentDictionary<string, int>() { ["a"] = 1, ["b"] = 2, ["c"] = 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_SortedDictionaryOfStringToInt()
    {
        string json = """{"Property":{"d":4,"e":5,"f":6}}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_SortedDictionaryOfStringToInt>(json);
        CheckGenericDictionaryContent(obj.Property);
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(StructWithReadOnlyProperty_SortedDictionaryOfStringToInt));
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_SortedDictionaryOfStringToInt_PropertyOccuringMultipleTimes()
    {
        string json = """{"Property":{"d:":4},"Property":{"e:":5},"Property":{"f:":6}}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_SortedDictionaryOfStringToInt>(json);
        CheckGenericDictionaryContent(obj.Property);
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_SortedDictionaryOfStringToInt_PropertyOccuringMultipleTimes_NullInBetween()
    {
        string json = """{"Property":{"d:":4},"Property":null,"Property":{"a:":1},"Property":{"b:":2}}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithWritableProperty<SortedDictionary<string, int>>>(json);
        CheckGenericDictionaryContent(obj.Property, 2);
    }

    internal struct StructWithReadOnlyProperty_SortedDictionaryOfStringToInt
    {
        public StructWithReadOnlyProperty_SortedDictionaryOfStringToInt() {}

        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public SortedDictionary<string, int> Property { get; } = new SortedDictionary<string, int>() { ["a"] = 1, ["b"] = 2, ["c"] = 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_SortedDictionaryOfStringToInt_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(StructWithReadOnlyProperty_SortedDictionaryOfStringToIntWithoutPopulateAttribute));
        string json = """{"Property":{"d":4,"e":5,"f":6}}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_SortedDictionaryOfStringToIntWithoutPopulateAttribute>(json, options);
        CheckGenericDictionaryContent(obj.Property);
    }

    internal struct StructWithReadOnlyProperty_SortedDictionaryOfStringToIntWithoutPopulateAttribute
    {
        public StructWithReadOnlyProperty_SortedDictionaryOfStringToIntWithoutPopulateAttribute() {}

        public SortedDictionary<string, int> Property { get; } = new SortedDictionary<string, int>() { ["a"] = 1, ["b"] = 2, ["c"] = 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_SortedDictionaryOfStringToInt_WithNumberHandling()
    {
        string json = """{"Property":{"d":"4","e":"5","f":"6"}}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_SortedDictionaryOfStringToIntWithNumberHandling>(json);
        CheckGenericDictionaryContent(obj.Property);
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(StructWithReadOnlyProperty_SortedDictionaryOfStringToIntWithNumberHandling));
    }

    internal struct StructWithReadOnlyProperty_SortedDictionaryOfStringToIntWithNumberHandling
    {
        public StructWithReadOnlyProperty_SortedDictionaryOfStringToIntWithNumberHandling() {}

        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public SortedDictionary<string, int> Property { get; } = new SortedDictionary<string, int>() { ["a"] = 1, ["b"] = 2, ["c"] = 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_SortedDictionaryOfStringToInt_WithNumberHandling_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(StructWithReadOnlyProperty_SortedDictionaryOfStringToIntWithNumberHandlingWithoutPopulateAttribute));
        string json = """{"Property":{"d":"4","e":"5","f":"6"}}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_SortedDictionaryOfStringToIntWithNumberHandlingWithoutPopulateAttribute>(json, options);
        CheckGenericDictionaryContent(obj.Property);
    }

    internal struct StructWithReadOnlyProperty_SortedDictionaryOfStringToIntWithNumberHandlingWithoutPopulateAttribute
    {
        public StructWithReadOnlyProperty_SortedDictionaryOfStringToIntWithNumberHandlingWithoutPopulateAttribute() {}

        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public SortedDictionary<string, int> Property { get; } = new SortedDictionary<string, int>() { ["a"] = 1, ["b"] = 2, ["c"] = 3 };
    }

    [Theory]
    [InlineData(typeof(ClassWithReadOnlyProperty<StructDictionary<string, int>>))]
    [InlineData(typeof(ClassWithReadOnlyProperty<StructDictionary<string, int>?>))]
    public async Task CreationHandlingSetWithAttribute_PopulateWithoutSetterOnValueTypeThrows_Dictionary(Type type)
    {
        string json = "{}";
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await Serializer.DeserializeWrapper(json, type));
    }
}

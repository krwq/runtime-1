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
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ListOfInt()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_ListOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(ClassWithReadOnlyProperty_ListOfInt));
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ListOfInt_PropertyOccuringMultipleTimes()
    {
        string json = """{"Property":[4],"Property":[5],"Property":[6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_ListOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ListOfInt_PropertyOccuringMultipleTimes_NullInBetween()
    {
        string json = """{"Property":[4],"Property":null,"Property":[1],"Property":[2]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithWritableProperty<List<int>>>(json);
        Assert.Equal(Enumerable.Range(1, 2), obj.Property);
    }

    internal class ClassWithReadOnlyProperty_ListOfInt
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public List<int> Property { get; } = new List<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ListOfInt_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(ClassWithReadOnlyProperty_ListOfIntWithoutPopulateAttribute));
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_ListOfIntWithoutPopulateAttribute>(json, options);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    internal class ClassWithReadOnlyProperty_ListOfIntWithoutPopulateAttribute
    {
        public List<int> Property { get; } = new List<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ListOfInt_WithNumberHandling()
    {
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_ListOfIntWithNumberHandling>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(StructWithReadOnlyProperty_ListOfIntWithNumberHandling));
    }

    internal struct StructWithReadOnlyProperty_ListOfIntWithNumberHandling
    {
        public StructWithReadOnlyProperty_ListOfIntWithNumberHandling() {}

        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public List<int> Property { get; } = new List<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ListOfInt_WithNumberHandling_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(StructWithReadOnlyProperty_ListOfIntWithNumberHandlingWithoutPopulateAttribute));
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_ListOfIntWithNumberHandlingWithoutPopulateAttribute>(json, options);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    internal struct StructWithReadOnlyProperty_ListOfIntWithNumberHandlingWithoutPopulateAttribute
    {
        public StructWithReadOnlyProperty_ListOfIntWithNumberHandlingWithoutPopulateAttribute() {}

        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public List<int> Property { get; } = new List<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IListOfInt_BackedBy_ListOfInt()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_IListOfInt_BackedBy_ListOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(StructWithReadOnlyProperty_IListOfInt_BackedBy_ListOfInt));
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IListOfInt_BackedBy_ListOfInt_PropertyOccuringMultipleTimes()
    {
        string json = """{"Property":[4],"Property":[5],"Property":[6]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_IListOfInt_BackedBy_ListOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IListOfInt_PropertyOccuringMultipleTimes_NullInBetween()
    {
        string json = """{"Property":[4],"Property":null,"Property":[1],"Property":[2]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithWritableProperty<IList<int>>>(json);
        Assert.Equal(Enumerable.Range(1, 2), obj.Property);
    }

    internal struct StructWithReadOnlyProperty_IListOfInt_BackedBy_ListOfInt
    {
        public StructWithReadOnlyProperty_IListOfInt_BackedBy_ListOfInt() {}

        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public IList<int> Property { get; } = new List<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IListOfInt_BackedBy_ListOfInt_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(StructWithReadOnlyProperty_IListOfInt_BackedBy_ListOfIntWithoutPopulateAttribute));
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_IListOfInt_BackedBy_ListOfIntWithoutPopulateAttribute>(json, options);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    internal struct StructWithReadOnlyProperty_IListOfInt_BackedBy_ListOfIntWithoutPopulateAttribute
    {
        public StructWithReadOnlyProperty_IListOfInt_BackedBy_ListOfIntWithoutPopulateAttribute() {}

        public IList<int> Property { get; } = new List<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IListOfInt_BackedBy_ListOfInt_WithNumberHandling()
    {
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_IListOfInt_BackedBy_ListOfIntWithNumberHandling>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(ClassWithReadOnlyProperty_IListOfInt_BackedBy_ListOfIntWithNumberHandling));
    }

    internal class ClassWithReadOnlyProperty_IListOfInt_BackedBy_ListOfIntWithNumberHandling
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public IList<int> Property { get; } = new List<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IListOfInt_BackedBy_ListOfInt_WithNumberHandling_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(ClassWithReadOnlyProperty_IListOfInt_BackedBy_ListOfIntWithNumberHandlingWithoutPopulateAttribute));
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_IListOfInt_BackedBy_ListOfIntWithNumberHandlingWithoutPopulateAttribute>(json, options);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    internal class ClassWithReadOnlyProperty_IListOfInt_BackedBy_ListOfIntWithNumberHandlingWithoutPopulateAttribute
    {
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
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(ClassWithReadOnlyProperty_IListOfInt_BackedBy_StructListOfInt));
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IListOfInt_BackedBy_StructListOfInt_PropertyOccuringMultipleTimes()
    {
        string json = """{"Property":[4],"Property":[5],"Property":[6]}""";
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
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IListOfInt_BackedBy_StructListOfInt_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(ClassWithReadOnlyProperty_IListOfInt_BackedBy_StructListOfIntWithoutPopulateAttribute));
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_IListOfInt_BackedBy_StructListOfIntWithoutPopulateAttribute>(json, options);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        ((StructList<int>)obj.Property).Validate();
    }

    internal class ClassWithReadOnlyProperty_IListOfInt_BackedBy_StructListOfIntWithoutPopulateAttribute
    {
        public IList<int> Property { get; } = new StructList<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IListOfInt_BackedBy_StructListOfInt_WithNumberHandling()
    {
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_IListOfInt_BackedBy_StructListOfIntWithNumberHandling>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        ((StructList<int>)obj.Property).Validate();
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(StructWithReadOnlyProperty_IListOfInt_BackedBy_StructListOfIntWithNumberHandling));
    }

    internal struct StructWithReadOnlyProperty_IListOfInt_BackedBy_StructListOfIntWithNumberHandling
    {
        public StructWithReadOnlyProperty_IListOfInt_BackedBy_StructListOfIntWithNumberHandling() {}

        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public IList<int> Property { get; } = new StructList<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IListOfInt_BackedBy_StructListOfInt_WithNumberHandling_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(StructWithReadOnlyProperty_IListOfInt_BackedBy_StructListOfIntWithNumberHandlingWithoutPopulateAttribute));
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_IListOfInt_BackedBy_StructListOfIntWithNumberHandlingWithoutPopulateAttribute>(json, options);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        ((StructList<int>)obj.Property).Validate();
    }

    internal struct StructWithReadOnlyProperty_IListOfInt_BackedBy_StructListOfIntWithNumberHandlingWithoutPopulateAttribute
    {
        public StructWithReadOnlyProperty_IListOfInt_BackedBy_StructListOfIntWithNumberHandlingWithoutPopulateAttribute() {}

        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public IList<int> Property { get; } = new StructList<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IList_BackedBy_ListOfJsonElement()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_IList_BackedBy_ListOfJsonElement>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property.Cast<JsonElement>().Select(x => x.GetInt32()));
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(StructWithReadOnlyProperty_IList_BackedBy_ListOfJsonElement));
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IList_BackedBy_ListOfJsonElement_PropertyOccuringMultipleTimes()
    {
        string json = """{"Property":[4],"Property":[5],"Property":[6]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_IList_BackedBy_ListOfJsonElement>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property.Cast<JsonElement>().Select(x => x.GetInt32()));
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IList_PropertyOccuringMultipleTimes_NullInBetween()
    {
        string json = """{"Property":[4],"Property":null,"Property":[1],"Property":[2]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithWritableProperty<IList>>(json);
        Assert.Equal(Enumerable.Range(1, 2), obj.Property.Cast<JsonElement>().Select(x => x.GetInt32()));
    }

    internal struct StructWithReadOnlyProperty_IList_BackedBy_ListOfJsonElement
    {
        public StructWithReadOnlyProperty_IList_BackedBy_ListOfJsonElement() {}

        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public IList Property { get; } = JsonSerializer.Deserialize<List<JsonElement>>("[1,2,3]");
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IList_BackedBy_ListOfJsonElement_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(StructWithReadOnlyProperty_IList_BackedBy_ListOfJsonElementWithoutPopulateAttribute));
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_IList_BackedBy_ListOfJsonElementWithoutPopulateAttribute>(json, options);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property.Cast<JsonElement>().Select(x => x.GetInt32()));
    }

    internal struct StructWithReadOnlyProperty_IList_BackedBy_ListOfJsonElementWithoutPopulateAttribute
    {
        public StructWithReadOnlyProperty_IList_BackedBy_ListOfJsonElementWithoutPopulateAttribute() {}

        public IList Property { get; } = JsonSerializer.Deserialize<List<JsonElement>>("[1,2,3]");
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IList_BackedBy_StructListOfJsonElement()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_IList_BackedBy_StructListOfJsonElement>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property.Cast<JsonElement>().Select(x => x.GetInt32()));
        ((StructList<JsonElement>)obj.Property).Validate();
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(ClassWithReadOnlyProperty_IList_BackedBy_StructListOfJsonElement));
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IList_BackedBy_StructListOfJsonElement_PropertyOccuringMultipleTimes()
    {
        string json = """{"Property":[4],"Property":[5],"Property":[6]}""";
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
    public async Task CreationHandlingSetWithAttribute_CanPopulate_IList_BackedBy_StructListOfJsonElement_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(ClassWithReadOnlyProperty_IList_BackedBy_StructListOfJsonElementWithoutPopulateAttribute));
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_IList_BackedBy_StructListOfJsonElementWithoutPopulateAttribute>(json, options);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property.Cast<JsonElement>().Select(x => x.GetInt32()));
        ((StructList<JsonElement>)obj.Property).Validate();
    }

    internal class ClassWithReadOnlyProperty_IList_BackedBy_StructListOfJsonElementWithoutPopulateAttribute
    {
        public IList Property { get; } = JsonSerializer.Deserialize<StructList<JsonElement>>("[1,2,3]");
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_StructListOfInt()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithWritableProperty_StructListOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        obj.Property.Validate();
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(ClassWithWritableProperty_StructListOfInt));
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_StructListOfInt_PropertyOccuringMultipleTimes()
    {
        string json = """{"Property":[4],"Property":[5],"Property":[6]}""";
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
    public async Task CreationHandlingSetWithAttribute_CanPopulate_StructListOfInt_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(ClassWithWritableProperty_StructListOfIntWithoutPopulateAttribute));
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithWritableProperty_StructListOfIntWithoutPopulateAttribute>(json, options);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        obj.Property.Validate();
    }

    internal class ClassWithWritableProperty_StructListOfIntWithoutPopulateAttribute
    {
        public StructList<int> Property { get; set; } = new StructList<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_StructListOfInt_WithNumberHandling()
    {
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithWritableProperty_StructListOfIntWithNumberHandling>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        obj.Property.Validate();
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(StructWithWritableProperty_StructListOfIntWithNumberHandling));
    }

    internal struct StructWithWritableProperty_StructListOfIntWithNumberHandling
    {
        public StructWithWritableProperty_StructListOfIntWithNumberHandling() {}

        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public StructList<int> Property { get; set; } = new StructList<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_StructListOfInt_WithNumberHandling_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(StructWithWritableProperty_StructListOfIntWithNumberHandlingWithoutPopulateAttribute));
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithWritableProperty_StructListOfIntWithNumberHandlingWithoutPopulateAttribute>(json, options);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        obj.Property.Validate();
    }

    internal struct StructWithWritableProperty_StructListOfIntWithNumberHandlingWithoutPopulateAttribute
    {
        public StructWithWritableProperty_StructListOfIntWithNumberHandlingWithoutPopulateAttribute() {}

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
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(StructWithWritableProperty_NullableStructListOfInt));
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_NullableStructListOfInt_PropertyOccuringMultipleTimes()
    {
        string json = """{"Property":[4],"Property":[5],"Property":[6]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithWritableProperty_NullableStructListOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property.Value);
        obj.Property.Value.Validate();
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_NullableStructListOfInt_PropertyOccuringMultipleTimes_NullInBetween()
    {
        string json = """{"Property":[4],"Property":null,"Property":[1],"Property":[2]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithWritableProperty_NullableStructListOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 2), obj.Property.Value);
        obj.Property.Value.Validate();
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_PopulatedPropertyCanDeserializeNull_NullableStructListOfInt()
    {
        string json = """{"Property":null}""";
        var obj = await Serializer.DeserializeWrapper<StructWithWritableProperty_NullableStructListOfInt>(json);
        Assert.Null(obj.Property);
    }

    internal struct StructWithWritableProperty_NullableStructListOfInt
    {
        public StructWithWritableProperty_NullableStructListOfInt() {}

        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public StructList<int>? Property { get; set; } = new StructList<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_NullableStructListOfInt_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(StructWithWritableProperty_NullableStructListOfIntWithoutPopulateAttribute));
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithWritableProperty_NullableStructListOfIntWithoutPopulateAttribute>(json, options);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property.Value);
        obj.Property.Value.Validate();
    }

    internal struct StructWithWritableProperty_NullableStructListOfIntWithoutPopulateAttribute
    {
        public StructWithWritableProperty_NullableStructListOfIntWithoutPopulateAttribute() {}

        public StructList<int>? Property { get; set; } = new StructList<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_QueueOfInt()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_QueueOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(ClassWithReadOnlyProperty_QueueOfInt));
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_QueueOfInt_PropertyOccuringMultipleTimes()
    {
        string json = """{"Property":[4],"Property":[5],"Property":[6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_QueueOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_QueueOfInt_PropertyOccuringMultipleTimes_NullInBetween()
    {
        string json = """{"Property":[4],"Property":null,"Property":[1],"Property":[2]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithWritableProperty<Queue<int>>>(json);
        Assert.Equal(Enumerable.Range(1, 2), obj.Property);
    }

    internal class ClassWithReadOnlyProperty_QueueOfInt
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public Queue<int> Property { get; } = new Queue<int>(new int[] { 1, 2, 3 });
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_QueueOfInt_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(ClassWithReadOnlyProperty_QueueOfIntWithoutPopulateAttribute));
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_QueueOfIntWithoutPopulateAttribute>(json, options);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    internal class ClassWithReadOnlyProperty_QueueOfIntWithoutPopulateAttribute
    {
        public Queue<int> Property { get; } = new Queue<int>(new int[] { 1, 2, 3 });
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_QueueOfInt_WithNumberHandling()
    {
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_QueueOfIntWithNumberHandling>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(ClassWithReadOnlyProperty_QueueOfIntWithNumberHandling));
    }

    internal class ClassWithReadOnlyProperty_QueueOfIntWithNumberHandling
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public Queue<int> Property { get; } = new Queue<int>(new int[] { 1, 2, 3 });
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_QueueOfInt_WithNumberHandling_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(ClassWithReadOnlyProperty_QueueOfIntWithNumberHandlingWithoutPopulateAttribute));
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_QueueOfIntWithNumberHandlingWithoutPopulateAttribute>(json, options);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    internal class ClassWithReadOnlyProperty_QueueOfIntWithNumberHandlingWithoutPopulateAttribute
    {
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public Queue<int> Property { get; } = new Queue<int>(new int[] { 1, 2, 3 });
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_Queue()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_Queue>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property.Cast<JsonElement>().Select(x => x.GetInt32()));
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(StructWithReadOnlyProperty_Queue));
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_Queue_PropertyOccuringMultipleTimes()
    {
        string json = """{"Property":[4],"Property":[5],"Property":[6]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_Queue>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property.Cast<JsonElement>().Select(x => x.GetInt32()));
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_Queue_PropertyOccuringMultipleTimes_NullInBetween()
    {
        string json = """{"Property":[4],"Property":null,"Property":[1],"Property":[2]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithWritableProperty<Queue>>(json);
        Assert.Equal(Enumerable.Range(1, 2), obj.Property.Cast<JsonElement>().Select(x => x.GetInt32()));
    }

    internal struct StructWithReadOnlyProperty_Queue
    {
        public StructWithReadOnlyProperty_Queue() {}

        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public Queue Property { get; } = JsonSerializer.Deserialize<Queue>("[1,2,3]");
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_Queue_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(StructWithReadOnlyProperty_QueueWithoutPopulateAttribute));
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_QueueWithoutPopulateAttribute>(json, options);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property.Cast<JsonElement>().Select(x => x.GetInt32()));
    }

    internal struct StructWithReadOnlyProperty_QueueWithoutPopulateAttribute
    {
        public StructWithReadOnlyProperty_QueueWithoutPopulateAttribute() {}

        public Queue Property { get; } = JsonSerializer.Deserialize<Queue>("[1,2,3]");
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ConcurrentQueueOfInt()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_ConcurrentQueueOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(StructWithReadOnlyProperty_ConcurrentQueueOfInt));
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ConcurrentQueueOfInt_PropertyOccuringMultipleTimes()
    {
        string json = """{"Property":[4],"Property":[5],"Property":[6]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_ConcurrentQueueOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ConcurrentQueueOfInt_PropertyOccuringMultipleTimes_NullInBetween()
    {
        string json = """{"Property":[4],"Property":null,"Property":[1],"Property":[2]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithWritableProperty<ConcurrentQueue<int>>>(json);
        Assert.Equal(Enumerable.Range(1, 2), obj.Property);
    }

    internal struct StructWithReadOnlyProperty_ConcurrentQueueOfInt
    {
        public StructWithReadOnlyProperty_ConcurrentQueueOfInt() {}

        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public ConcurrentQueue<int> Property { get; } = new ConcurrentQueue<int>(new int[] { 1, 2, 3 });
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ConcurrentQueueOfInt_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(StructWithReadOnlyProperty_ConcurrentQueueOfIntWithoutPopulateAttribute));
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_ConcurrentQueueOfIntWithoutPopulateAttribute>(json, options);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    internal struct StructWithReadOnlyProperty_ConcurrentQueueOfIntWithoutPopulateAttribute
    {
        public StructWithReadOnlyProperty_ConcurrentQueueOfIntWithoutPopulateAttribute() {}

        public ConcurrentQueue<int> Property { get; } = new ConcurrentQueue<int>(new int[] { 1, 2, 3 });
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ConcurrentQueueOfInt_WithNumberHandling()
    {
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_ConcurrentQueueOfIntWithNumberHandling>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(ClassWithReadOnlyProperty_ConcurrentQueueOfIntWithNumberHandling));
    }

    internal class ClassWithReadOnlyProperty_ConcurrentQueueOfIntWithNumberHandling
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public ConcurrentQueue<int> Property { get; } = new ConcurrentQueue<int>(new int[] { 1, 2, 3 });
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ConcurrentQueueOfInt_WithNumberHandling_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(ClassWithReadOnlyProperty_ConcurrentQueueOfIntWithNumberHandlingWithoutPopulateAttribute));
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_ConcurrentQueueOfIntWithNumberHandlingWithoutPopulateAttribute>(json, options);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    internal class ClassWithReadOnlyProperty_ConcurrentQueueOfIntWithNumberHandlingWithoutPopulateAttribute
    {
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public ConcurrentQueue<int> Property { get; } = new ConcurrentQueue<int>(new int[] { 1, 2, 3 });
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_StackOfInt()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_StackOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 6).Reverse(), obj.Property);
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(ClassWithReadOnlyProperty_StackOfInt));
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_StackOfInt_PropertyOccuringMultipleTimes()
    {
        string json = """{"Property":[4],"Property":[5],"Property":[6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_StackOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 6).Reverse(), obj.Property);
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_StackOfInt_PropertyOccuringMultipleTimes_NullInBetween()
    {
        string json = """{"Property":[4],"Property":null,"Property":[1],"Property":[2]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithWritableProperty<Stack<int>>>(json);
        Assert.Equal(Enumerable.Range(1, 2).Reverse(), obj.Property);
    }

    internal class ClassWithReadOnlyProperty_StackOfInt
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public Stack<int> Property { get; } = new Stack<int>(new int[] { 1, 2, 3 });
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_StackOfInt_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(ClassWithReadOnlyProperty_StackOfIntWithoutPopulateAttribute));
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_StackOfIntWithoutPopulateAttribute>(json, options);
        Assert.Equal(Enumerable.Range(1, 6).Reverse(), obj.Property);
    }

    internal class ClassWithReadOnlyProperty_StackOfIntWithoutPopulateAttribute
    {
        public Stack<int> Property { get; } = new Stack<int>(new int[] { 1, 2, 3 });
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_StackOfInt_WithNumberHandling()
    {
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_StackOfIntWithNumberHandling>(json);
        Assert.Equal(Enumerable.Range(1, 6).Reverse(), obj.Property);
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(StructWithReadOnlyProperty_StackOfIntWithNumberHandling));
    }

    internal struct StructWithReadOnlyProperty_StackOfIntWithNumberHandling
    {
        public StructWithReadOnlyProperty_StackOfIntWithNumberHandling() {}

        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public Stack<int> Property { get; } = new Stack<int>(new int[] { 1, 2, 3 });
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_StackOfInt_WithNumberHandling_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(StructWithReadOnlyProperty_StackOfIntWithNumberHandlingWithoutPopulateAttribute));
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_StackOfIntWithNumberHandlingWithoutPopulateAttribute>(json, options);
        Assert.Equal(Enumerable.Range(1, 6).Reverse(), obj.Property);
    }

    internal struct StructWithReadOnlyProperty_StackOfIntWithNumberHandlingWithoutPopulateAttribute
    {
        public StructWithReadOnlyProperty_StackOfIntWithNumberHandlingWithoutPopulateAttribute() {}

        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public Stack<int> Property { get; } = new Stack<int>(new int[] { 1, 2, 3 });
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_Stack()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_Stack>(json);
        Assert.Equal(Enumerable.Range(1, 6).Reverse(), obj.Property.Cast<JsonElement>().Select(x => x.GetInt32()));
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(StructWithReadOnlyProperty_Stack));
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_Stack_PropertyOccuringMultipleTimes()
    {
        string json = """{"Property":[4],"Property":[5],"Property":[6]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_Stack>(json);
        Assert.Equal(Enumerable.Range(1, 6).Reverse(), obj.Property.Cast<JsonElement>().Select(x => x.GetInt32()));
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_Stack_PropertyOccuringMultipleTimes_NullInBetween()
    {
        string json = """{"Property":[4],"Property":null,"Property":[1],"Property":[2]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithWritableProperty<Stack>>(json);
        Assert.Equal(Enumerable.Range(1, 2).Reverse(), obj.Property.Cast<JsonElement>().Select(x => x.GetInt32()));
    }

    internal struct StructWithReadOnlyProperty_Stack
    {
        public StructWithReadOnlyProperty_Stack() {}

        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public Stack Property { get; } = JsonSerializer.Deserialize<Stack>("[1,2,3]");
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_Stack_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(StructWithReadOnlyProperty_StackWithoutPopulateAttribute));
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_StackWithoutPopulateAttribute>(json, options);
        Assert.Equal(Enumerable.Range(1, 6).Reverse(), obj.Property.Cast<JsonElement>().Select(x => x.GetInt32()));
    }

    internal struct StructWithReadOnlyProperty_StackWithoutPopulateAttribute
    {
        public StructWithReadOnlyProperty_StackWithoutPopulateAttribute() {}

        public Stack Property { get; } = JsonSerializer.Deserialize<Stack>("[1,2,3]");
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ConcurrentStackOfInt()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_ConcurrentStackOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 6).Reverse(), obj.Property);
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(ClassWithReadOnlyProperty_ConcurrentStackOfInt));
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ConcurrentStackOfInt_PropertyOccuringMultipleTimes()
    {
        string json = """{"Property":[4],"Property":[5],"Property":[6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_ConcurrentStackOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 6).Reverse(), obj.Property);
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ConcurrentStackOfInt_PropertyOccuringMultipleTimes_NullInBetween()
    {
        string json = """{"Property":[4],"Property":null,"Property":[1],"Property":[2]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithWritableProperty<ConcurrentStack<int>>>(json);
        Assert.Equal(Enumerable.Range(1, 2).Reverse(), obj.Property);
    }

    internal class ClassWithReadOnlyProperty_ConcurrentStackOfInt
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public ConcurrentStack<int> Property { get; } = new ConcurrentStack<int>(new int[] { 1, 2, 3 });
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ConcurrentStackOfInt_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(ClassWithReadOnlyProperty_ConcurrentStackOfIntWithoutPopulateAttribute));
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_ConcurrentStackOfIntWithoutPopulateAttribute>(json, options);
        Assert.Equal(Enumerable.Range(1, 6).Reverse(), obj.Property);
    }

    internal class ClassWithReadOnlyProperty_ConcurrentStackOfIntWithoutPopulateAttribute
    {
        public ConcurrentStack<int> Property { get; } = new ConcurrentStack<int>(new int[] { 1, 2, 3 });
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ConcurrentStackOfInt_WithNumberHandling()
    {
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_ConcurrentStackOfIntWithNumberHandling>(json);
        Assert.Equal(Enumerable.Range(1, 6).Reverse(), obj.Property);
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(ClassWithReadOnlyProperty_ConcurrentStackOfIntWithNumberHandling));
    }

    internal class ClassWithReadOnlyProperty_ConcurrentStackOfIntWithNumberHandling
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public ConcurrentStack<int> Property { get; } = new ConcurrentStack<int>(new int[] { 1, 2, 3 });
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ConcurrentStackOfInt_WithNumberHandling_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(ClassWithReadOnlyProperty_ConcurrentStackOfIntWithNumberHandlingWithoutPopulateAttribute));
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_ConcurrentStackOfIntWithNumberHandlingWithoutPopulateAttribute>(json, options);
        Assert.Equal(Enumerable.Range(1, 6).Reverse(), obj.Property);
    }

    internal class ClassWithReadOnlyProperty_ConcurrentStackOfIntWithNumberHandlingWithoutPopulateAttribute
    {
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public ConcurrentStack<int> Property { get; } = new ConcurrentStack<int>(new int[] { 1, 2, 3 });
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ICollectionOfInt_BackedBy_ListOfInt()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_ICollectionOfInt_BackedBy_ListOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(StructWithReadOnlyProperty_ICollectionOfInt_BackedBy_ListOfInt));
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ICollectionOfInt_BackedBy_ListOfInt_PropertyOccuringMultipleTimes()
    {
        string json = """{"Property":[4],"Property":[5],"Property":[6]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_ICollectionOfInt_BackedBy_ListOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ICollectionOfInt_PropertyOccuringMultipleTimes_NullInBetween()
    {
        string json = """{"Property":[4],"Property":null,"Property":[1],"Property":[2]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithWritableProperty<ICollection<int>>>(json);
        Assert.Equal(Enumerable.Range(1, 2), obj.Property);
    }

    internal struct StructWithReadOnlyProperty_ICollectionOfInt_BackedBy_ListOfInt
    {
        public StructWithReadOnlyProperty_ICollectionOfInt_BackedBy_ListOfInt() {}

        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public ICollection<int> Property { get; } = new List<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ICollectionOfInt_BackedBy_ListOfInt_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(StructWithReadOnlyProperty_ICollectionOfInt_BackedBy_ListOfIntWithoutPopulateAttribute));
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_ICollectionOfInt_BackedBy_ListOfIntWithoutPopulateAttribute>(json, options);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    internal struct StructWithReadOnlyProperty_ICollectionOfInt_BackedBy_ListOfIntWithoutPopulateAttribute
    {
        public StructWithReadOnlyProperty_ICollectionOfInt_BackedBy_ListOfIntWithoutPopulateAttribute() {}

        public ICollection<int> Property { get; } = new List<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ICollectionOfInt_BackedBy_ListOfInt_WithNumberHandling()
    {
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_ICollectionOfInt_BackedBy_ListOfIntWithNumberHandling>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(StructWithReadOnlyProperty_ICollectionOfInt_BackedBy_ListOfIntWithNumberHandling));
    }

    internal struct StructWithReadOnlyProperty_ICollectionOfInt_BackedBy_ListOfIntWithNumberHandling
    {
        public StructWithReadOnlyProperty_ICollectionOfInt_BackedBy_ListOfIntWithNumberHandling() {}

        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public ICollection<int> Property { get; } = new List<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ICollectionOfInt_BackedBy_ListOfInt_WithNumberHandling_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(StructWithReadOnlyProperty_ICollectionOfInt_BackedBy_ListOfIntWithNumberHandlingWithoutPopulateAttribute));
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_ICollectionOfInt_BackedBy_ListOfIntWithNumberHandlingWithoutPopulateAttribute>(json, options);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    internal struct StructWithReadOnlyProperty_ICollectionOfInt_BackedBy_ListOfIntWithNumberHandlingWithoutPopulateAttribute
    {
        public StructWithReadOnlyProperty_ICollectionOfInt_BackedBy_ListOfIntWithNumberHandlingWithoutPopulateAttribute() {}

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
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(ClassWithReadOnlyProperty_ICollectionOfInt_BackedBy_StructCollectionOfInt));
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ICollectionOfInt_BackedBy_StructCollectionOfInt_PropertyOccuringMultipleTimes()
    {
        string json = """{"Property":[4],"Property":[5],"Property":[6]}""";
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
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ICollectionOfInt_BackedBy_StructCollectionOfInt_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(ClassWithReadOnlyProperty_ICollectionOfInt_BackedBy_StructCollectionOfIntWithoutPopulateAttribute));
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_ICollectionOfInt_BackedBy_StructCollectionOfIntWithoutPopulateAttribute>(json, options);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        ((StructCollection<int>)obj.Property).Validate();
    }

    internal class ClassWithReadOnlyProperty_ICollectionOfInt_BackedBy_StructCollectionOfIntWithoutPopulateAttribute
    {
        public ICollection<int> Property { get; } = new StructCollection<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ICollectionOfInt_BackedBy_StructCollectionOfInt_WithNumberHandling()
    {
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_ICollectionOfInt_BackedBy_StructCollectionOfIntWithNumberHandling>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        ((StructCollection<int>)obj.Property).Validate();
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(ClassWithReadOnlyProperty_ICollectionOfInt_BackedBy_StructCollectionOfIntWithNumberHandling));
    }

    internal class ClassWithReadOnlyProperty_ICollectionOfInt_BackedBy_StructCollectionOfIntWithNumberHandling
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public ICollection<int> Property { get; } = new StructCollection<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ICollectionOfInt_BackedBy_StructCollectionOfInt_WithNumberHandling_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(ClassWithReadOnlyProperty_ICollectionOfInt_BackedBy_StructCollectionOfIntWithNumberHandlingWithoutPopulateAttribute));
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_ICollectionOfInt_BackedBy_StructCollectionOfIntWithNumberHandlingWithoutPopulateAttribute>(json, options);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        ((StructCollection<int>)obj.Property).Validate();
    }

    internal class ClassWithReadOnlyProperty_ICollectionOfInt_BackedBy_StructCollectionOfIntWithNumberHandlingWithoutPopulateAttribute
    {
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
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(StructWithWritableProperty_StructCollectionOfInt));
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_StructCollectionOfInt_PropertyOccuringMultipleTimes()
    {
        string json = """{"Property":[4],"Property":[5],"Property":[6]}""";
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
    public async Task CreationHandlingSetWithAttribute_CanPopulate_StructCollectionOfInt_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(StructWithWritableProperty_StructCollectionOfIntWithoutPopulateAttribute));
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithWritableProperty_StructCollectionOfIntWithoutPopulateAttribute>(json, options);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        obj.Property.Validate();
    }

    internal struct StructWithWritableProperty_StructCollectionOfIntWithoutPopulateAttribute
    {
        public StructWithWritableProperty_StructCollectionOfIntWithoutPopulateAttribute() {}

        public StructCollection<int> Property { get; set; } = new StructCollection<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_StructCollectionOfInt_WithNumberHandling()
    {
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithWritableProperty_StructCollectionOfIntWithNumberHandling>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        obj.Property.Validate();
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(StructWithWritableProperty_StructCollectionOfIntWithNumberHandling));
    }

    internal struct StructWithWritableProperty_StructCollectionOfIntWithNumberHandling
    {
        public StructWithWritableProperty_StructCollectionOfIntWithNumberHandling() {}

        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public StructCollection<int> Property { get; set; } = new StructCollection<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_StructCollectionOfInt_WithNumberHandling_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(StructWithWritableProperty_StructCollectionOfIntWithNumberHandlingWithoutPopulateAttribute));
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithWritableProperty_StructCollectionOfIntWithNumberHandlingWithoutPopulateAttribute>(json, options);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        obj.Property.Validate();
    }

    internal struct StructWithWritableProperty_StructCollectionOfIntWithNumberHandlingWithoutPopulateAttribute
    {
        public StructWithWritableProperty_StructCollectionOfIntWithNumberHandlingWithoutPopulateAttribute() {}

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
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(ClassWithWritableProperty_NullableStructCollectionOfInt));
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_NullableStructCollectionOfInt_PropertyOccuringMultipleTimes()
    {
        string json = """{"Property":[4],"Property":[5],"Property":[6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithWritableProperty_NullableStructCollectionOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property.Value);
        obj.Property.Value.Validate();
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_NullableStructCollectionOfInt_PropertyOccuringMultipleTimes_NullInBetween()
    {
        string json = """{"Property":[4],"Property":null,"Property":[1],"Property":[2]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithWritableProperty_NullableStructCollectionOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 2), obj.Property.Value);
        obj.Property.Value.Validate();
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_PopulatedPropertyCanDeserializeNull_NullableStructCollectionOfInt()
    {
        string json = """{"Property":null}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithWritableProperty_NullableStructCollectionOfInt>(json);
        Assert.Null(obj.Property);
    }

    internal class ClassWithWritableProperty_NullableStructCollectionOfInt
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public StructCollection<int>? Property { get; set; } = new StructCollection<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_NullableStructCollectionOfInt_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(ClassWithWritableProperty_NullableStructCollectionOfIntWithoutPopulateAttribute));
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithWritableProperty_NullableStructCollectionOfIntWithoutPopulateAttribute>(json, options);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property.Value);
        obj.Property.Value.Validate();
    }

    internal class ClassWithWritableProperty_NullableStructCollectionOfIntWithoutPopulateAttribute
    {
        public StructCollection<int>? Property { get; set; } = new StructCollection<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ISetOfInt_BackedBy_HashSetOfInt()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_ISetOfInt_BackedBy_HashSetOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(ClassWithReadOnlyProperty_ISetOfInt_BackedBy_HashSetOfInt));
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ISetOfInt_BackedBy_HashSetOfInt_PropertyOccuringMultipleTimes()
    {
        string json = """{"Property":[4],"Property":[5],"Property":[6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_ISetOfInt_BackedBy_HashSetOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ISetOfInt_PropertyOccuringMultipleTimes_NullInBetween()
    {
        string json = """{"Property":[4],"Property":null,"Property":[1],"Property":[2]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithWritableProperty<ISet<int>>>(json);
        Assert.Equal(Enumerable.Range(1, 2), obj.Property);
    }

    internal class ClassWithReadOnlyProperty_ISetOfInt_BackedBy_HashSetOfInt
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public ISet<int> Property { get; } = new HashSet<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ISetOfInt_BackedBy_HashSetOfInt_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(ClassWithReadOnlyProperty_ISetOfInt_BackedBy_HashSetOfIntWithoutPopulateAttribute));
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_ISetOfInt_BackedBy_HashSetOfIntWithoutPopulateAttribute>(json, options);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    internal class ClassWithReadOnlyProperty_ISetOfInt_BackedBy_HashSetOfIntWithoutPopulateAttribute
    {
        public ISet<int> Property { get; } = new HashSet<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ISetOfInt_BackedBy_HashSetOfInt_WithNumberHandling()
    {
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_ISetOfInt_BackedBy_HashSetOfIntWithNumberHandling>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(StructWithReadOnlyProperty_ISetOfInt_BackedBy_HashSetOfIntWithNumberHandling));
    }

    internal struct StructWithReadOnlyProperty_ISetOfInt_BackedBy_HashSetOfIntWithNumberHandling
    {
        public StructWithReadOnlyProperty_ISetOfInt_BackedBy_HashSetOfIntWithNumberHandling() {}

        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public ISet<int> Property { get; } = new HashSet<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ISetOfInt_BackedBy_HashSetOfInt_WithNumberHandling_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(StructWithReadOnlyProperty_ISetOfInt_BackedBy_HashSetOfIntWithNumberHandlingWithoutPopulateAttribute));
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_ISetOfInt_BackedBy_HashSetOfIntWithNumberHandlingWithoutPopulateAttribute>(json, options);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
    }

    internal struct StructWithReadOnlyProperty_ISetOfInt_BackedBy_HashSetOfIntWithNumberHandlingWithoutPopulateAttribute
    {
        public StructWithReadOnlyProperty_ISetOfInt_BackedBy_HashSetOfIntWithNumberHandlingWithoutPopulateAttribute() {}

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
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(StructWithReadOnlyProperty_ISetOfInt_BackedBy_StructSetOfInt));
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ISetOfInt_BackedBy_StructSetOfInt_PropertyOccuringMultipleTimes()
    {
        string json = """{"Property":[4],"Property":[5],"Property":[6]}""";
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
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ISetOfInt_BackedBy_StructSetOfInt_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(StructWithReadOnlyProperty_ISetOfInt_BackedBy_StructSetOfIntWithoutPopulateAttribute));
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithReadOnlyProperty_ISetOfInt_BackedBy_StructSetOfIntWithoutPopulateAttribute>(json, options);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        ((StructSet<int>)obj.Property).Validate();
    }

    internal struct StructWithReadOnlyProperty_ISetOfInt_BackedBy_StructSetOfIntWithoutPopulateAttribute
    {
        public StructWithReadOnlyProperty_ISetOfInt_BackedBy_StructSetOfIntWithoutPopulateAttribute() {}

        public ISet<int> Property { get; } = new StructSet<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ISetOfInt_BackedBy_StructSetOfInt_WithNumberHandling()
    {
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_ISetOfInt_BackedBy_StructSetOfIntWithNumberHandling>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        ((StructSet<int>)obj.Property).Validate();
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(ClassWithReadOnlyProperty_ISetOfInt_BackedBy_StructSetOfIntWithNumberHandling));
    }

    internal class ClassWithReadOnlyProperty_ISetOfInt_BackedBy_StructSetOfIntWithNumberHandling
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public ISet<int> Property { get; } = new StructSet<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_ISetOfInt_BackedBy_StructSetOfInt_WithNumberHandling_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(ClassWithReadOnlyProperty_ISetOfInt_BackedBy_StructSetOfIntWithNumberHandlingWithoutPopulateAttribute));
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithReadOnlyProperty_ISetOfInt_BackedBy_StructSetOfIntWithNumberHandlingWithoutPopulateAttribute>(json, options);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        ((StructSet<int>)obj.Property).Validate();
    }

    internal class ClassWithReadOnlyProperty_ISetOfInt_BackedBy_StructSetOfIntWithNumberHandlingWithoutPopulateAttribute
    {
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
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(ClassWithWritableProperty_StructSetOfInt));
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_StructSetOfInt_PropertyOccuringMultipleTimes()
    {
        string json = """{"Property":[4],"Property":[5],"Property":[6]}""";
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
    public async Task CreationHandlingSetWithAttribute_CanPopulate_StructSetOfInt_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(ClassWithWritableProperty_StructSetOfIntWithoutPopulateAttribute));
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<ClassWithWritableProperty_StructSetOfIntWithoutPopulateAttribute>(json, options);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        obj.Property.Validate();
    }

    internal class ClassWithWritableProperty_StructSetOfIntWithoutPopulateAttribute
    {
        public StructSet<int> Property { get; set; } = new StructSet<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_StructSetOfInt_WithNumberHandling()
    {
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithWritableProperty_StructSetOfIntWithNumberHandling>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        obj.Property.Validate();
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(StructWithWritableProperty_StructSetOfIntWithNumberHandling));
    }

    internal struct StructWithWritableProperty_StructSetOfIntWithNumberHandling
    {
        public StructWithWritableProperty_StructSetOfIntWithNumberHandling() {}

        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public StructSet<int> Property { get; set; } = new StructSet<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_StructSetOfInt_WithNumberHandling_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(StructWithWritableProperty_StructSetOfIntWithNumberHandlingWithoutPopulateAttribute));
        string json = """{"Property":["4","5","6"]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithWritableProperty_StructSetOfIntWithNumberHandlingWithoutPopulateAttribute>(json, options);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property);
        obj.Property.Validate();
    }

    internal struct StructWithWritableProperty_StructSetOfIntWithNumberHandlingWithoutPopulateAttribute
    {
        public StructWithWritableProperty_StructSetOfIntWithNumberHandlingWithoutPopulateAttribute() {}

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
        CheckFirstPropertyIsPopulated(JsonSerializerOptions.Default, typeof(StructWithWritableProperty_NullableStructSetOfInt));
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_NullableStructSetOfInt_PropertyOccuringMultipleTimes()
    {
        string json = """{"Property":[4],"Property":[5],"Property":[6]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithWritableProperty_NullableStructSetOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property.Value);
        obj.Property.Value.Validate();
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_NullableStructSetOfInt_PropertyOccuringMultipleTimes_NullInBetween()
    {
        string json = """{"Property":[4],"Property":null,"Property":[1],"Property":[2]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithWritableProperty_NullableStructSetOfInt>(json);
        Assert.Equal(Enumerable.Range(1, 2), obj.Property.Value);
        obj.Property.Value.Validate();
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_PopulatedPropertyCanDeserializeNull_NullableStructSetOfInt()
    {
        string json = """{"Property":null}""";
        var obj = await Serializer.DeserializeWrapper<StructWithWritableProperty_NullableStructSetOfInt>(json);
        Assert.Null(obj.Property);
    }

    internal struct StructWithWritableProperty_NullableStructSetOfInt
    {
        public StructWithWritableProperty_NullableStructSetOfInt() {}

        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public StructSet<int>? Property { get; set; } = new StructSet<int>() { 1, 2, 3 };
    }

    [Fact]
    public async Task CreationHandlingSetWithAttribute_CanPopulate_NullableStructSetOfInt_SetWithMetadata()
    {
        JsonSerializerOptions options = GetOptionsCustomizeFirstPropertyToPopulateForType(typeof(StructWithWritableProperty_NullableStructSetOfIntWithoutPopulateAttribute));
        string json = """{"Property":[4,5,6]}""";
        var obj = await Serializer.DeserializeWrapper<StructWithWritableProperty_NullableStructSetOfIntWithoutPopulateAttribute>(json, options);
        Assert.Equal(Enumerable.Range(1, 6), obj.Property.Value);
        obj.Property.Value.Validate();
    }

    internal struct StructWithWritableProperty_NullableStructSetOfIntWithoutPopulateAttribute
    {
        public StructWithWritableProperty_NullableStructSetOfIntWithoutPopulateAttribute() {}

        public StructSet<int>? Property { get; set; } = new StructSet<int>() { 1, 2, 3 };
    }

    [Theory]
    [InlineData(typeof(ClassWithReadOnlyProperty<StructList<int>>))]
    [InlineData(typeof(ClassWithReadOnlyProperty<StructList<int>?>))]
    [InlineData(typeof(ClassWithReadOnlyProperty<StructCollection<int>>))]
    [InlineData(typeof(ClassWithReadOnlyProperty<StructCollection<int>?>))]
    [InlineData(typeof(ClassWithReadOnlyProperty<StructSet<int>>))]
    [InlineData(typeof(ClassWithReadOnlyProperty<StructSet<int>?>))]
    public async Task CreationHandlingSetWithAttribute_PopulateWithoutSetterOnValueTypeThrows_Enumerable(Type type)
    {
        string json = "{}";
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await Serializer.DeserializeWrapper(json, type));
    }
}

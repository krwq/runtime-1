// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
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
        Assert.Equal(new List<int> { 1, 2, 3, 4, 5, 6 }, obj.Property);
    }

    internal class ClassWithReadOnlyListOfIntProperty
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public List<int> Property { get; } = new List<int>() { 1, 2, 3 };
    }
}

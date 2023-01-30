// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization.Metadata;
using Xunit;

namespace System.Text.Json.Serialization.Tests;

// - set through attribute and metadata
// - polymorphism (i.e. populate polymorphic object, ensure it's within allowed hierarchy or do base type)
// - all cases from main issue for dictionry, collection and object
// - incompatible options
public static class JsonCreationHandlingTests
{
    [Fact]
    public static void CreationHandlingSetWithAttribute_CanPopulate_ListOfInt()
    {
        string json = """{"Property":[4,5,6]}""";
        var obj = JsonSerializer.Deserialize<ClassWithReadOnlyListOfIntProperty>(json);
        Assert.Equal(new List<int> { 1, 2, 3, 4, 5, 6 }, obj.Property);
    }

    internal class ClassWithReadOnlyListOfIntProperty
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public List<int> Property { get; } = new List<int>() { 1, 2, 3 };
    }
}

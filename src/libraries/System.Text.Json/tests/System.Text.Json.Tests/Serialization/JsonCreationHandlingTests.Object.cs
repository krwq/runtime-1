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
    public async Task CreationHandlingSetWithAttribute_CanPopulate_Class()
    {
        JsonSerializerOptions options = new()
        {
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
        };

        string json = """
            {
                "PopulatedPropertyReadOnly":
                {
                    "IntValue": 43
                },
                "PopulatedPropertySimple":
                {
                    "IntValue": 44
                }
            }
            """;
        var obj = await Serializer.DeserializeWrapper<ClassWithClassProperty>(json);
        Assert.NotNull(obj);
        Assert.Equal("InitialForPopulate1", obj.PopulatedPropertyReadOnly.StringValue);
        Assert.Equal(43, obj.PopulatedPropertyReadOnly.IntValue);

        Assert.Equal("InitialForPopulate2", obj.PopulatedPropertySimple.StringValue);
        Assert.Equal(44, obj.PopulatedPropertySimple.IntValue);
    }

    internal class ClassWithClassProperty
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public SomeClass PopulatedPropertyReadOnly { get; } = new() { StringValue = "InitialForPopulate1" };

        private SomeClass _populatedSimple = new() { StringValue = "InitialForPopulate2" };

        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public SomeClass PopulatedPropertySimple
        {
            get => _populatedSimple;
            set => Assert.Fail("Setter should not be used");
        }

        private SomeClass _populatedWithChildren =
            new()
            {
                StringValue = "InitialForPopulate3",
                ReplacedChild = new()
                {
                    StringValue = "ShouldBeReplaced",
                    IntValue = 123,
                },
                PopulatedChild = new()
                {
                    StringValue = "InitialForPopulate4",
                    IntValue = 43,
                    ReplacedChild = new() { StringValue = "ShouldBeReplaced" }
                },
            };

        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public SomeClass PopulatedPropertyWithChildren
        {
            get => _populatedWithChildren;
            set => Assert.Fail("Setter should not be used");
        }

    }

    internal class SomeClass
    {
        public string StringValue { get; set; } = "InitialSomeClass";
        public int IntValue { get; set; } = 42;
        public SomeClass? ReplacedChild { get; set; }

        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public SomeClass? PopulatedChild { get; set; }
    }
}

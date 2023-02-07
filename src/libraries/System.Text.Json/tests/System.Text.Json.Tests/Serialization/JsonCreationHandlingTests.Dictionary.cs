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
        CheckDictionaryContent(obj.Property);
    }

    internal class ClassWithReadOnlyProperty_DictionaryOfStringToInt
    {
        [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
        public Dictionary<string, int> Property { get; } = new Dictionary<string, int>() { ["a"] = 1, ["b"] = 2, ["c"] = 3 };
    }

}

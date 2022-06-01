// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using Xunit;

namespace System.Text.Json.Serialization.Tests
{
    public static partial class DefaultJsonTypeInfoResolverTests
    {
        // TODO: type info assigned to different options
        // something similar to ThrowHelper.ThrowInvalidOperationException_SerializationConverterNotCompatible(converter.GetType(), typeToConvert);

        [Theory]
        [InlineData(typeof(object))]
        [InlineData(typeof(int))]
        [InlineData(typeof(string))]
        [InlineData(typeof(SomeClass))]
        public static void TypeInfoWithKindNone(Type type)
        {
            DefaultJsonTypeInfoResolver r = new();
            JsonSerializerOptions o = new();
            o.Converters.Add(new CustomThrowingConverter<SomeClass>());

            JsonTypeInfo ti = r.GetTypeInfo(type, o);

            Assert.Equal(JsonTypeInfoKind.None, ti.Kind);
            Assert.Same(o, ti.Options);
            Assert.Null(ti.CreateObject);
            Assert.Throws<InvalidOperationException>(() => ti.CreateObject = () => Activator.CreateInstance(type));
            Assert.Null(ti.NumberHandling);
            Assert.Throws<InvalidOperationException>(() => ti.NumberHandling = JsonNumberHandling.AllowReadingFromString);
            Assert.NotNull(ti.Properties);
            Assert.Equal(0, ti.Properties.Count);
            Assert.True(ti.Properties.IsReadOnly);

            JsonPropertyInfo property = ti.CreateJsonPropertyInfo(typeof(string), "foo");
            Assert.NotNull(property);
            Assert.Throws<InvalidOperationException>(() => ti.Properties.Add(property));
            Assert.Throws<InvalidOperationException>(() => ti.Properties.Insert(0, property));
            Assert.Throws<InvalidOperationException>(() => ti.Properties.Clear());
        }

        [Theory]
        [InlineData(typeof(SomeClass), typeof(object))]
        [InlineData(typeof(object), typeof(string))]
        [InlineData(typeof(object), typeof(int))]
        [InlineData(typeof(string), typeof(int))]
        //[InlineData(typeof(int), typeof(string))] // TODO: GetTypeInfo(typeof(int), ...) is never called
        //[InlineData(typeof(int), typeof(double))]
        public static void TypeInfoOfWrongTypeOnObject(Type expectedType, Type actualType)
        {
            DefaultJsonTypeInfoResolver dr = new();
            TestResolver r = new((type, options) =>
            {
                if (type == expectedType)
                {
                    return dr.GetTypeInfo(actualType, options);
                }

                return dr.GetTypeInfo(type, options);
            });

            JsonSerializerOptions o = new();
            o.TypeInfoResolver = r;

            SomeClass testObj = new()
            {
                ObjProp = "test",
            };

            Assert.Throws<InvalidOperationException>(() => JsonSerializer.Serialize(testObj, o));
        }

        [Theory]
        [InlineData(typeof(SomeClass), typeof(object))]
        [InlineData(typeof(object), typeof(string))]
        [InlineData(typeof(object), typeof(int))]
        [InlineData(typeof(int), typeof(string))]
        [InlineData(typeof(int), typeof(double))]
        public static void TypeInfoOfWrongTypeDirectCall(Type expectedType, Type actualType)
        {
            DefaultJsonTypeInfoResolver dr = new();
            TestResolver r = new((type, options) =>
            {
                if (type == expectedType)
                {
                    return dr.GetTypeInfo(actualType, options);
                }

                return dr.GetTypeInfo(type, options);
            });

            JsonSerializerOptions o = new();
            o.TypeInfoResolver = r;

            object testObj = Activator.CreateInstance(expectedType);

            Assert.Throws<InvalidOperationException>(() => JsonSerializer.Serialize(testObj, expectedType, o));
        }


        private class SomeClass
        {
            public object ObjProp { get; set; }
            public int IntProp { get; set; }
        }

        private class CustomThrowingConverter<T> : JsonConverter<T>
        {
            public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();
            public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) => throw new NotImplementedException();
        }

        private class TestResolver : IJsonTypeInfoResolver
        {
            private Func<Type, JsonSerializerOptions, JsonTypeInfo> _getTypeInfo;

            public TestResolver(Func<Type, JsonSerializerOptions, JsonTypeInfo> getTypeInfo)
            {
                _getTypeInfo = getTypeInfo;
            }

            public JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
            {
                return _getTypeInfo(type, options);
            }
        }
    }
}

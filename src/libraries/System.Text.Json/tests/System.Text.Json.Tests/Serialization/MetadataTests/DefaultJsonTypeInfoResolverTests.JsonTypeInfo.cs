// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using Xunit;

namespace System.Text.Json.Serialization.Tests
{
    // TODO: typed create object
    public static partial class DefaultJsonTypeInfoResolverTests
    {
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
            Assert.NotNull(ti.Properties);
            Assert.Equal(0, ti.Properties.Count);
            Assert.True(ti.Properties.IsReadOnly);

            JsonPropertyInfo property = ti.CreateJsonPropertyInfo(typeof(string), "foo");
            Assert.NotNull(property);
            Assert.Throws<InvalidOperationException>(() => ti.Properties.Add(property));
            Assert.Throws<InvalidOperationException>(() => ti.Properties.Insert(0, property));
            Assert.Throws<InvalidOperationException>(() => ti.Properties.Clear());
        }

        [Fact]
        public static void TypeInfoKindNoneNumberHandlingDirect()
        {
            DefaultJsonTypeInfoResolver r = new();
            r.Modifiers.Add((ti) =>
            {
                if (ti.Type == typeof(int))
                {
                    ti.NumberHandling = JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString;
                }
            });

            JsonSerializerOptions o = new();
            o.TypeInfoResolver = r;

            string json = JsonSerializer.Serialize(13, o);
            Assert.Equal(@"""13""", json);

            var deserialized = JsonSerializer.Deserialize<int>(json, o);
            Assert.Equal(13, deserialized);
        }

        [Fact]
        public static void TypeInfoKindNoneNumberHandlingDirectThroughObject()
        {
            DefaultJsonTypeInfoResolver r = new();
            r.Modifiers.Add((ti) =>
            {
                if (ti.Type == typeof(int))
                {
                    ti.NumberHandling = JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString;
                }
            });

            JsonSerializerOptions o = new();
            o.TypeInfoResolver = r;

            string json = JsonSerializer.Serialize<object>(13, o);
            Assert.Equal(@"""13""", json);

            var deserialized = JsonSerializer.Deserialize<object>(json, o);
            Assert.Equal("13", ((JsonElement)deserialized).GetString());
        }

        [Fact]
        public static void TypeInfoKindNoneNumberHandling()
        {
            DefaultJsonTypeInfoResolver r = new();
            r.Modifiers.Add((ti) =>
            {
                if (ti.Type == typeof(int) || ti.Type == typeof(object))
                {
                    ti.NumberHandling = JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString;
                }
            });

            JsonSerializerOptions o = new();
            o.TypeInfoResolver = r;

            SomeClass testObj = new SomeClass()
            {
                ObjProp = 45,
                IntProp = 13,
            };

            string json = JsonSerializer.Serialize(testObj, o);
            Assert.Equal(@"{""ObjProp"":""45"",""IntProp"":""13""}", json);

            var deserialized = JsonSerializer.Deserialize<SomeClass>(json, o);
            Assert.Equal(testObj.ObjProp.ToString(), ((JsonElement)deserialized.ObjProp).GetString());
            Assert.Equal(testObj.IntProp, deserialized.IntProp);
        }

        [Fact]
        public static void RecursiveTypeNumberHandling()
        {
            DefaultJsonTypeInfoResolver r = new();
            r.Modifiers.Add((ti) =>
            {
                if (ti.Type == typeof(SomeRecursiveClass))
                {
                    ti.NumberHandling = JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString;
                }
            });

            JsonSerializerOptions o = new();
            o.TypeInfoResolver = r;

            SomeRecursiveClass testObj = new SomeRecursiveClass()
            {
                IntProp = 13,
                RecursiveProperty = new SomeRecursiveClass()
                {
                    IntProp = 14,
                },
            };

            string json = JsonSerializer.Serialize(testObj, o);
            Assert.Equal(@"{""IntProp"":""13"",""RecursiveProperty"":{""IntProp"":""14"",""RecursiveProperty"":null}}", json);

            var deserialized = JsonSerializer.Deserialize<SomeRecursiveClass>(json, o);
            Assert.Equal(testObj.IntProp, deserialized.IntProp);
            Assert.NotNull(testObj.RecursiveProperty);
            Assert.Equal(testObj.RecursiveProperty.IntProp, deserialized.RecursiveProperty.IntProp);
            Assert.Null(testObj.RecursiveProperty.RecursiveProperty);
        }

        [Theory]
        [InlineData(typeof(SomeClass), typeof(object))]
        [InlineData(typeof(object), typeof(string))]
        [InlineData(typeof(object), typeof(int))]
        [InlineData(typeof(string), typeof(int))]
        [InlineData(typeof(int), typeof(string))]
        [InlineData(typeof(int), typeof(double))]
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

        [Fact]
        public static void TypeInfoOfWrongOptions()
        {
            JsonSerializerOptions wrongOptions = new();
            DefaultJsonTypeInfoResolver dr = new();
            TestResolver r = new((type, options) =>
            {
                if (type == typeof(int))
                {
                    return dr.GetTypeInfo(type, wrongOptions);
                }

                return dr.GetTypeInfo(type, options);
            });

            JsonSerializerOptions o = new();
            o.TypeInfoResolver = r;

            SomeClass testObj = new()
            {
                IntProp = 17,
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

        [Theory]
        [MemberData(nameof(GetTypeInfoTestData))]
        public static void TypeInfoIsImmutableAfterFirstUsage(Type type, object testObj)
        {
            JsonTypeInfo typeInfo = null;
            DefaultJsonTypeInfoResolver dr = new();
            TestResolver r = new((typeToResolve, options) =>
            {
                var ret = dr.GetTypeInfo(typeToResolve, options);
                if (typeToResolve == type)
                {
                    Assert.Null(typeInfo);
                    typeInfo = ret;
                }

                return ret;
            });

            JsonSerializerOptions o = new();
            o.TypeInfoResolver = r;

            Assert.NotNull(JsonSerializer.Serialize(testObj, type, o));
            Assert.NotNull(typeInfo);

            Assert.Equal(type, typeInfo.Type);
            Assert.True(typeInfo.Converter.CanConvert(type));

            if (typeInfo.Kind == JsonTypeInfoKind.None)
            {
                Assert.Null(typeInfo.CreateObject);
            }
            else
            {
                Assert.NotNull(typeInfo.CreateObject);
            }

            Assert.Null(typeInfo.NumberHandling);

            JsonPropertyInfo prop = typeInfo.CreateJsonPropertyInfo(typeof(string), "foo");
            Assert.Throws<InvalidOperationException>(() => typeInfo.CreateObject = typeInfo.CreateObject);
            Assert.Throws<InvalidOperationException>(() => typeInfo.NumberHandling = typeInfo.NumberHandling);
            Assert.Throws<InvalidOperationException>(() => typeInfo.Properties.Clear());
            Assert.Throws<InvalidOperationException>(() => typeInfo.Properties.Add(prop));
            Assert.Throws<InvalidOperationException>(() => typeInfo.Properties.Insert(0, prop));

            foreach (var property in typeInfo.Properties)
            {
                Assert.NotNull(property.PropertyType);
                Assert.Null(property.CustomConverter);
                Assert.NotNull(property.Name);
                Assert.NotNull(property.Get);
                Assert.NotNull(property.Set);
                Assert.Null(property.ShouldSerialize);
                Assert.Null(typeInfo.NumberHandling);

                Assert.Throws<InvalidOperationException>(() => property.CustomConverter = property.CustomConverter);
                Assert.Throws<InvalidOperationException>(() => property.Name = property.Name);
                Assert.Throws<InvalidOperationException>(() => property.Get = property.Get);
                Assert.Throws<InvalidOperationException>(() => property.Set = property.Set);
                Assert.Throws<InvalidOperationException>(() => property.ShouldSerialize = property.ShouldSerialize);
                Assert.Throws<InvalidOperationException>(() => property.NumberHandling = property.NumberHandling);
            }

            typeof(DefaultJsonTypeInfoResolverTests)
                .GetMethod(nameof(TypeInfoIsImmutableAfterFirstUsage_Generic), BindingFlags.NonPublic | BindingFlags.Static)
                .MakeGenericMethod(type)
                .Invoke(null, new object[] { typeInfo });
        }

        private static void TypeInfoIsImmutableAfterFirstUsage_Generic<T>(JsonTypeInfo<T> typeInfo)
        {
            Assert.Throws<InvalidOperationException>(() => typeInfo.CreateObject = typeInfo.CreateObject);
        }

        public static IEnumerable<object[]> GetTypeInfoTestData()
        {
            yield return new object[] { typeof(string), "test" };
            yield return new object[] { typeof(int), 13 };
            yield return new object[] { typeof(SomeClass), new SomeClass { IntProp = 17 } };
            yield return new object[] { typeof(SomeRecursiveClass), new SomeRecursiveClass() };
        }
    }
}

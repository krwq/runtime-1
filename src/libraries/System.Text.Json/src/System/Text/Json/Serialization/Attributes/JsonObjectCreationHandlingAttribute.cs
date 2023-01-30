// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Text.Json.Serialization;

/// <summary>
/// When placed on a member, indicates if member will replaced or populated.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
public sealed class JsonObjectCreationHandlingAttribute : JsonAttribute
{
    /// <summary>
    /// Indicates what settings should be used when serializing or deserializing members.
    /// </summary>
    public JsonObjectCreationHandling Handling { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="JsonObjectCreationHandlingAttribute"/>.
    /// </summary>
    public JsonObjectCreationHandlingAttribute(JsonObjectCreationHandling handling)
    {
        if (!JsonSerializer.IsValidCreationHandlingValue(handling))
        {
            throw new ArgumentOutOfRangeException(nameof(handling));
        }

        Handling = handling;
    }
}

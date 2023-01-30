// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.Json.Serialization.Metadata;

namespace System.Text.Json.Serialization;

/// <summary>
/// Indicates if the annotated member will be populated or replaced during deserialization.
/// </summary>
/// <remarks>
/// This attribute will be mapped to <see cref="JsonPropertyInfo.CreationHandling"/>.
/// </remarks>
public enum JsonObjectCreationHandling
{
    /// <summary>
    /// Member is replaced during deserialization.
    /// </summary>
    Replace = 0,

    /// <summary>
    /// Member is populated during deserialization.
    /// </summary>
    Populate = 1,
}

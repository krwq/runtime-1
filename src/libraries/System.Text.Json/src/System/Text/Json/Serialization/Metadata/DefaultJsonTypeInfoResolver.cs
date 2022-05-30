// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace System.Text.Json.Serialization.Metadata
{
    /// <summary>
    /// Default JsonTypeInfo resolver.
    /// </summary>
    public class DefaultJsonTypeInfoResolver : IJsonTypeInfoResolver
    {
        private bool _mutable;

        /// <summary>
        /// Constructs DefaultJsonTypeInfoResolver.
        /// </summary>
        [RequiresUnreferencedCode(JsonSerializer.SerializationUnreferencedCodeMessage)]
        [RequiresDynamicCode(JsonSerializer.SerializationRequiresDynamicCodeMessage)]
        public DefaultJsonTypeInfoResolver() : this(mutable: true)
        {
        }

        [RequiresUnreferencedCode(JsonSerializer.SerializationUnreferencedCodeMessage)]
        [RequiresDynamicCode(JsonSerializer.SerializationRequiresDynamicCodeMessage)]
        private DefaultJsonTypeInfoResolver(bool mutable)
        {
            JsonSerializerOptions.RootConverters();
            _mutable = mutable;
            Modifiers = new ModifierCollection(this);
        }

        /// <inheritdoc/>
        public virtual JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _mutable = false;

            JsonTypeInfo.ValidateType(type, null, null, options);
            JsonTypeInfo typeInfo = CreateJsonTypeInfo(type, options);

            foreach (Action<JsonTypeInfo> modifier in Modifiers)
            {
                modifier(typeInfo);
            }

            return typeInfo;
        }

        [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode",
            Justification = "The ctor is marked RequiresUnreferencedCode.")]
        [UnconditionalSuppressMessage("AotAnalysis", "IL3050:RequiresDynamicCode",
            Justification = "The ctor is marked RequiresDynamicCode.")]
        private JsonTypeInfo CreateJsonTypeInfo(Type type, JsonSerializerOptions options)
        {
            MethodInfo methodInfo = typeof(JsonSerializerOptions).GetMethod(nameof(JsonSerializerOptions.CreateReflectionJsonTypeInfo), BindingFlags.NonPublic | BindingFlags.Instance)!;
#if NETCOREAPP
            return (JsonTypeInfo)methodInfo.MakeGenericMethod(type).Invoke(options, BindingFlags.NonPublic | BindingFlags.DoNotWrapExceptions, null, null, null)!;
#else
            try
            {
                return (JsonTypeInfo)methodInfo.MakeGenericMethod(type).Invoke(options, null)!;
            }
            catch (TargetInvocationException ex)
            {
                // Some of the validation is done during construction (i.e. validity of JsonConverter, inner types etc.)
                // therefore we need to unwrap TargetInvocationException for better user experience
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw null!;
            }
#endif
        }

        /// <summary>
        /// List of JsonTypeInfo modifiers. Modifying callbacks are called consecutively after initial resolution
        /// and cannot be changed after GetTypeInfo is called.
        /// </summary>
        public IList<Action<JsonTypeInfo>> Modifiers { get; }

        private sealed class ModifierCollection : ConfigurationList<Action<JsonTypeInfo>>
        {
            private readonly DefaultJsonTypeInfoResolver _resolver;

            public ModifierCollection(DefaultJsonTypeInfoResolver resolver)
            {
                _resolver = resolver;
            }

            protected override bool IsLockedInstance => !_resolver._mutable;
            protected override void VerifyMutable()
            {
                if (!_resolver._mutable)
                {
                    ThrowHelper.ThrowInvalidOperationException_TypeInfoResolverImmutable();
                }
            }
        }

        internal static DefaultJsonTypeInfoResolver DefaultInstance
        {
            [RequiresUnreferencedCode(JsonSerializer.SerializationUnreferencedCodeMessage)]
            [RequiresDynamicCode(JsonSerializer.SerializationRequiresDynamicCodeMessage)]
            get
            {
                if (s_defaultInstance is DefaultJsonTypeInfoResolver resolver)
                {
                    return resolver;
                }

                var newInstance = new DefaultJsonTypeInfoResolver(mutable: false);
                DefaultJsonTypeInfoResolver? originalInstance = Interlocked.CompareExchange(ref s_defaultInstance, newInstance, comparand: null);
                return originalInstance ?? newInstance;
            }
        }

        private static DefaultJsonTypeInfoResolver? s_defaultInstance;
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.IO;
using System.Reflection;
using System.Security.Cryptography.EcDsa.Tests;
using Xunit;

namespace System.Security.Cryptography.Tests
{
    [SkipOnPlatform(~TestPlatforms.Linux, "Only supported when ")]
    public class OpenSslNamedKeysTests
    {
        [Fact]
        public static void OpenExistingPrivateKey()
        {
            using (RSAOpenSsl rsa = new RSAOpenSsl())
            {
                //using TempFileHolder
                SafeEvpPKeyHandle.OpenPrivateKeyFromEngine("dntest", "first key").Dispose();
            }
        }

        [Fact]
        public static void OpenExistingPublicKey()
        {
            SafeEvpPKeyHandle.OpenPublicKeyFromEngine("dntest", "first pubkey").Dispose();
        }

        [Fact]
        public static void UseThoseKeys()
        {
            using (SafeEvpPKeyHandle priv = SafeEvpPKeyHandle.OpenPrivateKeyFromEngine("dntest", "first key"))
            using (SafeEvpPKeyHandle pub = SafeEvpPKeyHandle.OpenPublicKeyFromEngine("dntest", "second pubkey"))
            using (SafeEvpPKeyHandle pubBad = SafeEvpPKeyHandle.OpenPublicKeyFromEngine("dntest", "first pubkey"))
            using (RSA rsaPriv = new RSAOpenSsl(priv))
            using (RSA rsaPub = new RSAOpenSsl(pub))
            using (RSA rsaBad = new RSAOpenSsl(pubBad))
            {
                byte[] data = new byte[] { 1, 2, 3, 1, 1, 2, 3 };
                byte[] signature = rsaPriv.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pss);

                Console.WriteLine($"rsaPub: {rsaPub.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pss)}");
                Console.WriteLine($"rsaBad: {rsaBad.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pss)}");
            }
        }

        // [Fact]
        // public static void UseProviderKeys()
        // {
        //     using (SafeEvpPKeyHandle priv = SafeEvpPKeyHandle.OpenKeyFromProvider("base", "file:/git/src/libraries/System.Security.Cryptography/tests/osslplugins/key/first key"))
        //     using (SafeEvpPKeyHandle pub = SafeEvpPKeyHandle.OpenKeyFromProvider("base", "file:/git/src/libraries/System.Security.Cryptography/tests/osslplugins/key/second pubkey"))
        //     using (SafeEvpPKeyHandle pubBad = SafeEvpPKeyHandle.OpenKeyFromProvider("base", "file:/git/src/libraries/System.Security.Cryptography/tests/osslplugins/key/first pubkey"))
        //     using (RSA rsaPriv = new RSAOpenSsl(priv))
        //     using (RSA rsaPub = new RSAOpenSsl(pub))
        //     using (RSA rsaBad = new RSAOpenSsl(pubBad))
        //     {
        //         byte[] data = new byte[] { 1, 2, 3, 1, 1, 2, 3 };
        //         byte[] signature = rsaPriv.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pss);

        //         Console.WriteLine($"rsaPub2: {rsaPub.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pss)}");
        //         Console.WriteLine($"rsaBad2: {rsaBad.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pss)}");
        //     }
        // }

        // [Fact]
        // public static void OpenProviderMissingKey()
        // {
        //     SafeEvpPKeyHandle.OpenKeyFromProvider("dntestprov", "nokey").Dispose();
        // }

        // [Fact]
        // public static void OpenProviderUnmatchedKey()
        // {
        //     SafeEvpPKeyHandle.OpenKeyFromProvider("dntestprov", "cert").Dispose();
        // }
    }
}

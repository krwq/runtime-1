// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.EcDsa.Tests;
using System.Security.Cryptography.X509Certificates.Tests;
using Test.Cryptography;
using Xunit;
using Tpm2Lib;
using static Tpm2Lib.Csp;

namespace System.Security.Cryptography.Tests
{
    [SkipOnPlatform(~TestPlatforms.Linux, "Only supported on ")] // TODO: require openssl
    public class OpenSslNamedKeysTests
    {
        // PKCS#1 format
        private static readonly byte[] s_rsaPrivateKey = (
            "3082025C02010002818100BF67168485215A6AB89BCAB9331F6F5F360F4300BE5CF282F77042957E" +
            "A202908B2279F34A426D62F59D6C1056E36DC9F6EEA9AEB1B31F8122F583EE9CAE2A86A47144905D" +
            "F05441B0A5F29E03C5AC1888D93744D89638D83AC37774B339E4AFB349C714B12238B0F81A71380F" +
            "051C585CB27434FA544BDAC679E1E16581D0E902030100010281810084ED8862F2BEAE37CE0C4CA7" +
            "808CC5615F7F0BEE99469E1A3CD4973991DFDC5E1C730E34DC0EF43F350B668096878EB92428AE69" +
            "A7FA19D82ABA4E2D4A5D5F243D4B7346734D705C4C494FE2B36E2E35C39EE08BFB1172F5AB084AF4" +
            "4BD4D03702D04E6469F026EF3749CBED3ECB310746CF49DA3C2785CC17D54215EF18F3ED024100D0" +
            "63F89E01EB681CEACB781FE807F87C702B522A76B7D0E06DA44BB7D6202D5E9F3E7BE5BCCC3B32B9" +
            "B293AB62F50A8417C2FA9D6A76E465AA962AB61A8A9A13024100EB218F00B7317CC625DF2DFB7181" +
            "1DC5DA91D9A2AD859282DCA6BA3B4C674897E9D03D9E5FD2A9FD4CE7D9A3E5B79E948429C21561E7" +
            "141D90BCA75733D2489302400D07D349FE10BC47E29EAA7A44460B51ACA9E8CF62F1078CA10E7EF5" +
            "95DC193A2B76FAC458D3E477BD88DF16FE6F18233E6120CEAB1398208B542C838A91542502407882" +
            "619D9746A8D191957A26B5FCDBFA8CD455BBF7BD4EE2FD1E02B2E3ACC7DAFC3DFB66D16BD22DFD9D" +
            "92C15ABA2A6FA9F111050E8175A0D58EAB219970BC3B02404DBF36E5DCBF027AD4ED572E6F5F8383" +
            "C08CD5838C0CAE16FA58EE5C5A388B287F9C58647D58609B03912A10D0C772A3259D39651CD1EEB3" +
            "A20C5F9AE58E18C0").HexToByteArray();

        // PKCS#1 format
        private static readonly byte[] s_rsaPubKey = (
            "30818902818100BF67168485215A6AB89BCAB9331F6F5F360F4300BE5CF282F77042957EA202908B" +
            "2279F34A426D62F59D6C1056E36DC9F6EEA9AEB1B31F8122F583EE9CAE2A86A47144905DF05441B0" +
            "A5F29E03C5AC1888D93744D89638D83AC37774B339E4AFB349C714B12238B0F81A71380F051C585C" +
            "B27434FA544BDAC679E1E16581D0E90203010001").HexToByteArray();

        [Fact]
        public static void NullArguments()
        {
            Assert.Throws<ArgumentNullException>("engineName", () => SafeEvpPKeyHandle.OpenPrivateKeyFromEngine(null, "first"));
            Assert.Throws<ArgumentNullException>("keyName", () => SafeEvpPKeyHandle.OpenPrivateKeyFromEngine("dntest", null));

            Assert.Throws<ArgumentNullException>("engineName", () => SafeEvpPKeyHandle.OpenPublicKeyFromEngine(null, "first"));
            Assert.Throws<ArgumentNullException>("keyName", () => SafeEvpPKeyHandle.OpenPublicKeyFromEngine("dntest", null));

            Assert.Throws<ArgumentNullException>("providerName", () => SafeEvpPKeyHandle.OpenKeyFromProvider(null, "first"));
            Assert.Throws<ArgumentNullException>("keyUri", () => SafeEvpPKeyHandle.OpenKeyFromProvider("dntestprov", null));
        }

        [Fact]
        public static void NonExistingEngineOrProvider()
        {
            Assert.ThrowsAny<CryptographicException>(() => SafeEvpPKeyHandle.OpenPrivateKeyFromEngine("dntestnonexisting", "first"));
            Assert.ThrowsAny<CryptographicException>(() => SafeEvpPKeyHandle.OpenPublicKeyFromEngine("dntestnonexisting", "first"));
            Assert.ThrowsAny<CryptographicException>(() => SafeEvpPKeyHandle.OpenKeyFromProvider("dntestnonexisting", "first"));
        }

        [Fact]
        public static void NonExistingKey()
        {
            Assert.ThrowsAny<CryptographicException>(() => SafeEvpPKeyHandle.OpenPrivateKeyFromEngine("dntest", "nonexisting"));
            Assert.ThrowsAny<CryptographicException>(() => SafeEvpPKeyHandle.OpenPublicKeyFromEngine("dntest", "nonexisting"));
            Assert.ThrowsAny<CryptographicException>(() => SafeEvpPKeyHandle.OpenKeyFromProvider("dntestprov", "nonexisting"));
        }

        [Fact]
        public static void Engine_OpenExistingPrivateKey()
        {
            using SafeEvpPKeyHandle priKeyHandle = SafeEvpPKeyHandle.OpenPrivateKeyFromEngine("dntest", "first");
            using RSA priKey = new RSAOpenSsl(priKeyHandle);
            RSAParameters rsaParams = priKey.ExportParameters(includePrivateParameters: true);
            Assert.NotNull(rsaParams.D);
            Assert.Equal(s_rsaPubKey, priKey.ExportRSAPublicKey());
        }

        [Fact]
        public static void Engine_OpenExistingPublicKey()
        {
            using SafeEvpPKeyHandle pubKeyHandle = SafeEvpPKeyHandle.OpenPublicKeyFromEngine("dntest", "first");
            using RSA pubKey = new RSAOpenSsl(pubKeyHandle);
            Assert.ThrowsAny<CryptographicException>(() => pubKey.ExportParameters(includePrivateParameters: true));
            RSAParameters rsaParams = pubKey.ExportParameters(includePrivateParameters: false);
            Assert.Null(rsaParams.D);
            Assert.Equal(s_rsaPubKey, pubKey.ExportRSAPublicKey());
        }

        [Fact]
        public static void Engine_UsePrivateKey()
        {
            using (SafeEvpPKeyHandle priKeyHandle = SafeEvpPKeyHandle.OpenPrivateKeyFromEngine("dntest", "first"))
            using (RSA rsaPri = new RSAOpenSsl(priKeyHandle))
            using (RSA rsaPub = RSA.Create())
            using (RSA rsaBad = RSA.Create(1024))
            {
                rsaPub.ImportRSAPublicKey(s_rsaPubKey, out int bytesRead);
                Assert.Equal(s_rsaPubKey.Length, bytesRead);

                byte[] data = new byte[] { 1, 2, 3, 1, 1, 2, 3 };
                byte[] signature = rsaPri.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pss);

                Assert.True(rsaPub.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pss));
                Assert.False(rsaBad.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pss));

                byte[] encrypted = rsaPub.Encrypt(data, RSAEncryptionPadding.OaepSHA256);
                Assert.NotEqual(encrypted, data);

                byte[] decrypted = rsaPri.Decrypt(encrypted, RSAEncryptionPadding.OaepSHA256);
                Assert.Equal(data, decrypted);
            }
        }

        [Fact]
        public static void Engine_UsePublicKey()
        {
            using (SafeEvpPKeyHandle pubKeyHandle = SafeEvpPKeyHandle.OpenPublicKeyFromEngine("dntest", "first"))
            using (RSA rsaPub = new RSAOpenSsl(pubKeyHandle))
            using (RSA rsaPri = RSA.Create())
            using (RSA rsaBad = RSA.Create(1024))
            {
                rsaPri.ImportRSAPrivateKey(s_rsaPrivateKey, out int bytesRead);
                Assert.Equal(s_rsaPrivateKey.Length, bytesRead);

                byte[] data = new byte[] { 1, 2, 3, 1, 1, 2, 3 };
                byte[] signature = rsaPri.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pss);
                byte[] differentKeySignature = rsaBad.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pss);

                Assert.True(rsaPub.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pss));
                Assert.False(rsaPub.VerifyData(data, differentKeySignature, HashAlgorithmName.SHA256, RSASignaturePadding.Pss));

                byte[] encrypted = rsaPub.Encrypt(data, RSAEncryptionPadding.OaepSHA256);
                Assert.NotEqual(encrypted, data);

                byte[] decrypted = rsaPri.Decrypt(encrypted, RSAEncryptionPadding.OaepSHA256);
                Assert.Equal(data, decrypted);
            }
        }

        [Fact]
        public static void Provider_OpenExistingPrivateKey()
        {
            Console.WriteLine("opening key handle");
            using SafeEvpPKeyHandle priKeyHandle = SafeEvpPKeyHandle.OpenKeyFromProvider("tpm2", "handle:0x81000004");//SafeEvpPKeyHandle.OpenKeyFromProvider("dntestprov", "first");
            Console.WriteLine("creating RSA from handle");
            using RSA rsaPri = new RSAOpenSsl();//new RSAOpenSsl(priKeyHandle);
            typeof(RSAOpenSsl).GetMethod("SetKey", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(rsaPri, new object[] { priKeyHandle });
            Console.WriteLine("getting key");
            using RSA rsaBad = RSA.Create(1024);

            byte[] data = new byte[] { 1, 2, 3, 1, 1, 2, 3 };
            Console.WriteLine("signing");
            byte[] signature = rsaPri.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pss);
            Console.WriteLine("signing with bad key");
            byte[] badSignature = rsaBad.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pss);
            Assert.NotEqual(data, signature);
            Console.WriteLine("verifying good signature");
            Assert.True(rsaPri.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pss));
            Console.WriteLine("verifying bad signature");
            Assert.False(rsaPri.VerifyData(data, badSignature, HashAlgorithmName.SHA256, RSASignaturePadding.Pss));
            // RSAParameters rsaParams = priKey.ExportParameters(includePrivateParameters: true);
            // Assert.NotNull(rsaParams.D);
            // Assert.Equal(s_rsaPubKey, priKey.ExportRSAPublicKey());
        }

        [Fact]
        public static void Engine_OpenExistingTPMPrivateKey()
        {
            Console.WriteLine("opening key handle");
            using SafeEvpPKeyHandle priKeyHandle = SafeEvpPKeyHandle.OpenPrivateKeyFromEngine("tpm2tss", "0x81000007");
            Console.WriteLine("creating RSA from handle");
            using ECDsa ecdsaPri = new ECDsaOpenSsl(priKeyHandle);//new RSAOpenSsl(priKeyHandle); // 
            //typeof(RSAOpenSsl).GetMethod("SetKey", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(rsaPri, new object[] { priKeyHandle });
            Console.WriteLine("getting key");
            using ECDsa ecdsaBad = ECDsa.Create();
            ecdsaBad.KeySize = ecdsaPri.KeySize;

            byte[] data = new byte[] { 1, 2, 3, 1, 1, 2, 3 };
            Console.WriteLine("signing");
            byte[] signature = ecdsaPri.SignData(data, HashAlgorithmName.SHA256);
            Console.WriteLine("signing with bad key");
            byte[] badSignature = ecdsaBad.SignData(data, HashAlgorithmName.SHA256);
            Assert.NotEqual(data, signature);
            Console.WriteLine("verifying good signature");
            Assert.True(ecdsaPri.VerifyData(data, signature, HashAlgorithmName.SHA256));
            Console.WriteLine("verifying bad signature");
            Assert.False(ecdsaPri.VerifyData(data, badSignature, HashAlgorithmName.SHA256));
            // RSAParameters rsaParams = priKey.ExportParameters(includePrivateParameters: true);
            // Assert.NotNull(rsaParams.D);
            // Assert.Equal(s_rsaPubKey, priKey.ExportRSAPublicKey());
        }

        [Fact]
        public static void Engine_OpenExistingTPMPrivateKeyUsingTssMsr()
        {
            Console.WriteLine("connecting to TPM");
            Tpm2Device device = new LinuxTpmDevice();
            device.Connect();
            Tpm2 tpm = new Tpm2(device);

            Console.WriteLine("opening handle");
            TpmHandle handle = TpmHandle.Persistent(0x81000004);
            RSA pri = new RSATssMsr(tpm, handle);
            Console.WriteLine("getting key");
            using RSA bad = RSA.Create();

            byte[] data = new byte[] { 1, 2, 3, 1, 1, 2, 3 };
            Console.WriteLine("signing");
            byte[] signature = pri.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pss);
            Console.WriteLine("signing with bad key");
            byte[] badSignature = bad.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pss);
            Assert.NotEqual(data, signature);
            Console.WriteLine("verifying good signature");
            Assert.True(pri.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pss));
            Console.WriteLine("verifying bad signature");
            Assert.False(pri.VerifyData(data, badSignature, HashAlgorithmName.SHA256, RSASignaturePadding.Pss));
            // RSAParameters rsaParams = priKey.ExportParameters(includePrivateParameters: true);
            // Assert.NotNull(rsaParams.D);
            // Assert.Equal(s_rsaPubKey, priKey.ExportRSAPublicKey());
        }

        //[Fact]
        //public static void UseThoseKeys()
        //{
        //    using (SafeEvpPKeyHandle priv = SafeEvpPKeyHandle.OpenPrivateKeyFromEngine("dntest", "first"))
        //    using (SafeEvpPKeyHandle pub = SafeEvpPKeyHandle.OpenPublicKeyFromEngine("dntest", "first"))
        //    using (SafeEvpPKeyHandle pubBad = SafeEvpPKeyHandle.OpenPublicKeyFromEngine("dntest", "first pubkey"))
        //    using (RSA rsaPriv = new RSAOpenSsl(priv))
        //    using (RSA rsaPub = new RSAOpenSsl(pub))
        //    using (RSA rsaBad = new RSAOpenSsl(pubBad))
        //    {
        //        byte[] data = new byte[] { 1, 2, 3, 1, 1, 2, 3 };
        //        byte[] signature = rsaPriv.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pss);

        //        Console.WriteLine($"rsaPub: {rsaPub.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pss)}");
        //        Console.WriteLine($"rsaBad: {rsaBad.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pss)}");
        //    }
        //}

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

    public class RSATssMsr : RSA
    {
        private Tpm2 _tpm;
        private TpmHandle _keyHandle;

        public RSATssMsr(Tpm2 tpm, TpmHandle keyHandle)
        {
            _tpm = tpm;
            _keyHandle = keyHandle;
        }

        public override RSAParameters ExportParameters(bool includePrivateParameters) => throw new NotImplementedException();
        public override void ImportParameters(RSAParameters parameters) => throw new NotImplementedException();
        public unsafe override byte[] SignHash(byte[] hash, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding)
        {
            // Convert the hash to a TpmHash.
            TpmAlgId algId = MapHashAlgorithm(hashAlgorithm);

            TpmHash tpmHash = new TpmHash(algId, hash);

            if (!padding.Equals(RSASignaturePadding.Pss))
            {
                throw new Exception("Only PSS padding is supported.");
            }

            // Sign the hash.
            ISignatureUnion signatureUnion = _tpm.Sign(_keyHandle, tpmHash, null, TpmHashCheck.Null());

            // Convert the signature to a byte array.
            SignatureRsassa signature = signatureUnion as SignatureRsassa;
            if (signature == null)
            {
                throw new CryptographicException("Invalid signature.");
            }
            return signature.sig;
        }

        private static TpmAlgId MapHashAlgorithm(HashAlgorithmName hashAlgorithm)
        {
            switch (hashAlgorithm.Name)
            {
                case "SHA1":
                    return TpmAlgId.Sha1;
                case "SHA256":
                    return TpmAlgId.Sha256;
                // Add more cases here for other hash algorithms.
                default:
                    throw new CryptographicException("Unsupported hash algorithm.");
            }
        }
    }

    //class RSAEsys : RSA
    //{
    //    private EsysContextHandle _context;
    //    private uint _handle;
    //    public RSAEsys(uint handle)
    //    {
    //        _handle = handle;
    //        Esys.Esys_Initialize(out _context, IntPtr.Zero, IntPtr.Zero);
    //    }

    //    public override RSAParameters ExportParameters(bool includePrivateParameters) => throw new NotImplementedException();
    //    public override void ImportParameters(RSAParameters parameters) => throw new NotImplementedException();
    //    public unsafe override byte[] SignHash(byte[] hash, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding)
    //    {
    //        if (hash.Length > 256)
    //        {
    //            throw new Exception("TODO: up to 256 bytes");
    //        }

    //        Esys.TPM2B_DIGEST digest = new Esys.TPM2B_DIGEST
    //        {
    //            size = (ushort)hash.Length,
    //        };

    //        fixed (byte* hashPtr = hash)
    //        {
    //            Buffer.MemoryCopy(hashPtr, digest.buffer, digest.size, hash.Length);
    //        }

    //        Esys.TPMT_SIG_SCHEME inScheme = new Esys.TPMT_SIG_SCHEME
    //        {
    //            scheme = Esys.TPM2_ALG_RSASSA,
    //            details = new TPMU_SIG_SCHEME
    //            {
    //                rsassa = new TPMS_SCHEME_RSASSA
    //                {
    //                    hashAlg = TPM2_ALG_SHA256
    //                }
    //            }
    //        };

    //        TPMT_TK_HASHCHECK validation = new TPMT_TK_HASHCHECK
    //        {
    //            tag = TPM2_ST_HASHCHECK,
    //            hierarchy = TPM2_RH_OWNER,
    //            digest = new TPM2B_DIGEST()
    //        };

    //        IntPtr signaturePtr;
    //        uint result = Esys.Esys_Sign(_context, _handle, 0, 0, 0, ref digest, ref inScheme, ref validation, out signaturePtr);
    //        if (result != TPM2_RC_SUCCESS)
    //        {
    //            throw new Exception($"Esys_Sign failed with error code: {result}");
    //        }

    //        TPM2B_PUBLIC_KEY_RSA signature = Marshal.PtrToStructure< >(signaturePtr);

    //        byte[] signatureBytes = new byte[signature.size];
    //        Marshal.Copy(signature.buffer, signatureBytes, 0, signature.size);
    //        Esys.Esys_Free(signaturePtr);

    //        return signatureBytes;
    //    }
    //}

    //public class EsysContextHandle : SafeHandle
    //{
    //    private EsysContextHandle() : base(IntPtr.Zero, true) { }

    //    public override bool IsInvalid => handle == IntPtr.Zero;

    //    protected override bool ReleaseHandle()
    //    {
    //        Esys.Esys_Free(ref handle);
    //        return true;
    //    }
    //}

    //public static class Esys
    //{
    //    [StructLayout(LayoutKind.Sequential)]
    //    public unsafe struct TPM2B_DIGEST
    //    {
    //        public ushort size;
    //        // TODO either us max size of all hash algos, ideally we should use span and dynamically allocate
    //        public fixed byte buffer[256];
    //    }

    //    [StructLayout(LayoutKind.Sequential)]
    //    public struct TPMT_SIG_SCHEME
    //    {
    //        public ushort scheme;
    //        public TPMU_SIG_SCHEME details;
    //    }

    //    [StructLayout(LayoutKind.Sequential)]
    //    public struct TPMU_SIG_SCHEME
    //    {
    //        public TPMS_SCHEME_HASH rsassa;
    //    }

    //    [StructLayout(LayoutKind.Sequential)]
    //    public struct TPMS_SCHEME_HASH
    //    {
    //        public ushort hashAlg;
    //    }

    //    [StructLayout(LayoutKind.Sequential)]
    //    public struct TPMT_TK_HASHCHECK
    //    {
    //        public ushort tag;
    //        public ushort hierarchy;
    //        public TPM2B_DIGEST digest;
    //    }

    //    const string Tss2Library = "tss2-esys"; // The name of the TSS2 library

    //    [DllImport(Tss2Library)]
    //    public static extern uint Esys_Initialize(out EsysContextHandle esysContext, IntPtr tctiContext, IntPtr abiVersion);

    //    [DllImport(Tss2Library)]
    //    public static extern void Esys_Free(ref IntPtr esysContext);

    //    [DllImport(Tss2Library)]
    //    public static extern uint Esys_StartAuthSession(EsysContextHandle esysContext, uint tpmKey, uint bind, IntPtr nonceCaller, IntPtr encryptedSalt, uint sessionType, IntPtr symmetric, uint authHash, out uint sessionHandle);

    //    [DllImport(Tss2Library)]
    //    public static unsafe extern uint Esys_Sign(
    //        EsysContextHandle esysContext,
    //        uint keyHandle,
    //        uint shandle1,
    //        uint shandle2,
    //        uint shandle3,
    //        TPM2B_DIGEST* digest,
    //        TPMT_SIG_SCHEME* inScheme,
    //        TPMT_TK_HASHCHECK* validation,
    //        IntPtr* signature
    //);

    //    // Define other necessary functions...
    //}
}

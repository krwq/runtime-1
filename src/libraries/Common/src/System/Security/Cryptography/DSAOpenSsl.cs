// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.IO;
using Internal.Cryptography;
using Microsoft.Win32.SafeHandles;

namespace System.Security.Cryptography
{
#if INTERNAL_ASYMMETRIC_IMPLEMENTATIONS
    public partial class DSA : AsymmetricAlgorithm
    {
        public static new DSA Create()
        {
            return new DSAImplementation.DSAOpenSsl();
        }
    }

    internal static partial class DSAImplementation
    {
#endif
        public sealed partial class DSAOpenSsl : DSA
        {
            private const int BitsPerByte = 8;
            private Lazy<SafeDsaHandle> _key;

            public DSAOpenSsl()
                : this(2048)
            {
            }

            public DSAOpenSsl(int keySize)
            {
                LegalKeySizesValue = s_legalKeySizes;
                base.KeySize = keySize;
                _key = new Lazy<SafeDsaHandle>(GenerateKey);
            }

            public override int KeySize
            {
                set
                {
                    if (KeySize == value)
                    {
                        return;
                    }

                    // Set the KeySize before FreeKey so that an invalid value doesn't throw away the key
                    base.KeySize = value;

                    ThrowIfDisposed();
                    FreeKey();
                    _key = new Lazy<SafeDsaHandle>(GenerateKey);
                }
            }

            private void ForceSetKeySize(int newKeySize)
            {
                // In the event that a key was loaded via ImportParameters or an IntPtr/SafeHandle
                // it could be outside of the bounds that we currently represent as "legal key sizes".
                // Since that is our view into the underlying component it can be detached from the
                // component's understanding.  If it said it has opened a key, and this is the size, trust it.
                KeySizeValue = newKeySize;
            }

            public override KeySizes[] LegalKeySizes
            {
                get
                {
                    return base.LegalKeySizes;
                }
            }

            public override DSAParameters ExportParameters(bool includePrivateParameters)
            {
                // It's entirely possible that this line will cause the key to be generated in the first place.
                SafeDsaHandle key = GetKey();

                DSAParameters dsaParameters = Interop.Crypto.ExportDsaParameters(key, includePrivateParameters);
                bool hasPrivateKey = dsaParameters.X != null;

                if (hasPrivateKey != includePrivateParameters)
                    throw new CryptographicException(SR.Cryptography_CSP_NoPrivateKey);

                return dsaParameters;
            }

            public override void ImportParameters(DSAParameters parameters)
            {
                if (parameters.P == null || parameters.Q == null || parameters.G == null || parameters.Y == null)
                    throw new ArgumentException(SR.Cryptography_InvalidDsaParameters_MissingFields);

                // J is not required and is not even used on CNG blobs. It should however be less than P (J == (P-1) / Q). This validation check
                // is just to maintain parity with DSACNG and DSACryptoServiceProvider, which also perform this check.
                if (parameters.J != null && parameters.J.Length >= parameters.P.Length)
                    throw new ArgumentException(SR.Cryptography_InvalidDsaParameters_MismatchedPJ);

                bool hasPrivateKey = parameters.X != null;

                int keySize = parameters.P.Length;
                if (parameters.G.Length != keySize || parameters.Y.Length != keySize)
                    throw new ArgumentException(SR.Cryptography_InvalidDsaParameters_MismatchedPGY);

                if (hasPrivateKey && parameters.X.Length != parameters.Q.Length)
                    throw new ArgumentException(SR.Cryptography_InvalidDsaParameters_MismatchedQX);

                ThrowIfDisposed();

                SafeDsaHandle key;
                if (!Interop.Crypto.DsaKeyCreateByExplicitParameters(
                    out key,
                    parameters.P, parameters.P.Length,
                    parameters.Q, parameters.Q.Length,
                    parameters.G, parameters.G.Length,
                    parameters.Y, parameters.Y.Length,
                    parameters.X, parameters.X != null ? parameters.X.Length : 0))
                {
                    throw Interop.Crypto.CreateOpenSslCryptographicException();
                }

                SetKey(key);
            }

            public override void ImportEncryptedPkcs8PrivateKey(
                ReadOnlySpan<byte> passwordBytes,
                ReadOnlySpan<byte> source,
                out int bytesRead)
            {
                ThrowIfDisposed();
                base.ImportEncryptedPkcs8PrivateKey(passwordBytes, source, out bytesRead);
            }

            public override void ImportEncryptedPkcs8PrivateKey(
                ReadOnlySpan<char> password,
                ReadOnlySpan<byte> source,
                out int bytesRead)
            {
                ThrowIfDisposed();
                base.ImportEncryptedPkcs8PrivateKey(password, source, out bytesRead);
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    FreeKey();
                    _key = null;
                }

                base.Dispose(disposing);
            }

            private void FreeKey()
            {
                if (_key != null && _key.IsValueCreated)
                {
                    SafeDsaHandle handle = _key.Value;

                    if (handle != null)
                    {
                        handle.Dispose();
                    }
                }
            }

            private static void CheckInvalidKey(SafeDsaHandle key)
            {
                if (key == null || key.IsInvalid)
                {
                    throw new CryptographicException(SR.Cryptography_OpenInvalidHandle);
                }
            }

            private SafeDsaHandle GenerateKey()
            {
                SafeDsaHandle key;

                if (!Interop.Crypto.DsaGenerateKey(out key, KeySize))
                {
                    throw Interop.Crypto.CreateOpenSslCryptographicException();
                }

                return key;
            }

            protected override byte[] HashData(byte[] data, int offset, int count, HashAlgorithmName hashAlgorithm)
            {
                // we're sealed and the base should have checked this already
                Debug.Assert(data != null);
                Debug.Assert(offset >= 0 && offset <= data.Length);
                Debug.Assert(count >= 0 && count <= data.Length);
                Debug.Assert(!string.IsNullOrEmpty(hashAlgorithm.Name));

                return AsymmetricAlgorithmHelpers.HashData(data, offset, count, hashAlgorithm);
            }

            protected override byte[] HashData(Stream data, HashAlgorithmName hashAlgorithm) =>
                AsymmetricAlgorithmHelpers.HashData(data, hashAlgorithm);

            protected override bool TryHashData(ReadOnlySpan<byte> data, Span<byte> destination, HashAlgorithmName hashAlgorithm, out int bytesWritten) =>
                AsymmetricAlgorithmHelpers.TryHashData(data, destination, hashAlgorithm, out bytesWritten);

            public override byte[] CreateSignature(byte[] rgbHash)
            {
                if (rgbHash == null)
                    throw new ArgumentNullException(nameof(rgbHash));

                SafeDsaHandle key = GetKey();
                int signatureSize = Interop.Crypto.DsaEncodedSignatureSize(key);
                byte[] signature = CryptoPool.Rent(signatureSize);
                try
                {
                    bool success = Interop.Crypto.DsaSign(key, rgbHash, new Span<byte>(signature, 0, signatureSize), out signatureSize);
                    if (!success)
                    {
                        throw Interop.Crypto.CreateOpenSslCryptographicException();
                    }

                    Debug.Assert(
                        signatureSize <= signature.Length,
                        "DSA_sign reported an unexpected signature size",
                        "DSA_sign reported signatureSize was {0}, when <= {1} was expected",
                        signatureSize,
                        signature.Length);

                    int signatureFieldSize = Interop.Crypto.DsaSignatureFieldSize(key) * BitsPerByte;
                    return AsymmetricAlgorithmHelpers.ConvertDerToIeee1363(signature.AsSpan(0, signatureSize), signatureFieldSize);
                }
                finally
                {
                    CryptoPool.Return(signature, signatureSize);
                }
            }

#if INTERNAL_ASYMMETRIC_IMPLEMENTATIONS
            protected override bool TryCreateSignatureCore(
                ReadOnlySpan<byte> hash,
                Span<byte> destination,
                DSASignatureFormat signatureFormat,
                out int bytesWritten)
#else
            public override bool TryCreateSignature(ReadOnlySpan<byte> hash, Span<byte> destination, out int bytesWritten)
#endif
            {
                SafeDsaHandle key = GetKey();
                int signatureSize = Interop.Crypto.DsaEncodedSignatureSize(key);

#if INTERNAL_ASYMMETRIC_IMPLEMENTATIONS
                if (signatureFormat == DSASignatureFormat.IeeeP1363FixedFieldConcatenation)
                {
#endif
                    byte[] converted;
                    byte[] signature = CryptoPool.Rent(signatureSize);
                    try
                    {
                        bool success = Interop.Crypto.DsaSign(key, hash, new Span<byte>(signature, 0, signatureSize), out signatureSize);
                        if (!success)
                        {
                            throw Interop.Crypto.CreateOpenSslCryptographicException();
                        }

                        Debug.Assert(
                                     signatureSize <= signature.Length,
                                         "DSA_sign reported an unexpected signature size",
                                         "DSA_sign reported signatureSize was {0}, when <= {1} was expected",
                                         signatureSize,
                                         signature.Length);

                        int signatureFieldSize = Interop.Crypto.DsaSignatureFieldSize(key) * BitsPerByte;
                        converted = AsymmetricAlgorithmHelpers.ConvertDerToIeee1363(signature.AsSpan(0, signatureSize), signatureFieldSize);
                    }
                    finally
                    {
                        CryptoPool.Return(signature, signatureSize);
                    }

                    return Helpers.TryCopyToDestination(converted, destination, out bytesWritten);
#if INTERNAL_ASYMMETRIC_IMPLEMENTATIONS
                }
                else if (signatureFormat == DSASignatureFormat.Rfc3279DerSequence)
                {
                    // The biggest key allowed by FIPS 186-4 has N=256 (bit), which
                    // maximally produces a 72-byte DER signature.
                    // If a future version of the standard continues to enhance DSA,
                    // we may want to bump this limit to allow the max-1 (expected size)
                    // TryCreateSignature to pass.
                    Span<byte> signDestination = stackalloc byte[72];

                    if (destination.Length >= signatureSize)
                    {
                        signDestination = destination;
                    }
                    else if (signatureSize > signDestination.Length)
                    {
                        bytesWritten = 0;
                        return false;
                    }

                    bool success = Interop.Crypto.DsaSign(key, hash, signDestination, out bytesWritten);

                    if (!success)
                    {
                        bytesWritten = 0;
                        throw Interop.Crypto.CreateOpenSslCryptographicException();
                    }

                    if (destination == signDestination)
                    {
                        return true;
                    }

                    if (!signDestination.Slice(0, bytesWritten).TryCopyTo(destination))
                    {
                        bytesWritten = 0;
                        return false;
                    }

                    return true;
                }
                else
                {
                    Debug.Fail($"Missing internal implementation handler for signature format {signatureFormat}");
                    throw new CryptographicException(
                        SR.Cryptography_UnknownSignatureFormat,
                        signatureFormat.ToString());
                }
#endif
            }

            public override bool VerifySignature(byte[] rgbHash, byte[] rgbSignature)
            {
                if (rgbHash == null)
                    throw new ArgumentNullException(nameof(rgbHash));
                if (rgbSignature == null)
                    throw new ArgumentNullException(nameof(rgbSignature));

                return VerifySignature((ReadOnlySpan<byte>)rgbHash, (ReadOnlySpan<byte>)rgbSignature);
            }

#if INTERNAL_ASYMMETRIC_IMPLEMENTATIONS
            protected override bool VerifySignatureCore(
                ReadOnlySpan<byte> hash,
                ReadOnlySpan<byte> signature,
                DSASignatureFormat signatureFormat)
#else
            public override bool VerifySignature(ReadOnlySpan<byte> hash, ReadOnlySpan<byte> signature)
#endif
            {
                SafeDsaHandle key = GetKey();

#if INTERNAL_ASYMMETRIC_IMPLEMENTATIONS
                if (signatureFormat == DSASignatureFormat.IeeeP1363FixedFieldConcatenation)
                {
#endif
                    int expectedSignatureBytes = Interop.Crypto.DsaSignatureFieldSize(key) * 2;
                    if (signature.Length != expectedSignatureBytes)
                    {
                        // The input isn't of the right length (assuming no DER), so we can't sensibly re-encode it with DER.
                        return false;
                    }

                    signature = AsymmetricAlgorithmHelpers.ConvertIeee1363ToDer(signature);
#if INTERNAL_ASYMMETRIC_IMPLEMENTATIONS
                }
                else if (signatureFormat != DSASignatureFormat.Rfc3279DerSequence)
                {
                    Debug.Fail($"Missing internal implementation handler for signature format {signatureFormat}");
                    throw new CryptographicException(
                        SR.Cryptography_UnknownSignatureFormat,
                        signatureFormat.ToString());
                }
#endif
                return Interop.Crypto.DsaVerify(key, hash, signature);
            }

            private void ThrowIfDisposed()
            {
                if (_key == null)
                {
                    throw new ObjectDisposedException(
#if INTERNAL_ASYMMETRIC_IMPLEMENTATIONS
                        nameof(DSA)
#else
                        nameof(DSAOpenSsl)
#endif
                    );
                }
            }

            private SafeDsaHandle GetKey()
            {
                ThrowIfDisposed();

                SafeDsaHandle key = _key.Value;
                CheckInvalidKey(key);

                return key;
            }

            private void SetKey(SafeDsaHandle newKey)
            {
                // Do not call ThrowIfDisposed here, as it breaks the SafeEvpPKey ctor

                // Use ForceSet instead of the property setter to ensure that LegalKeySizes doesn't interfere
                // with the already loaded key.
                ForceSetKeySize(BitsPerByte * Interop.Crypto.DsaKeySize(newKey));

                _key = new Lazy<SafeDsaHandle>(newKey);
            }

            private static readonly KeySizes[] s_legalKeySizes = new KeySizes[] { new KeySizes(minSize: 512, maxSize: 3072, skipSize: 64) };
        }
#if INTERNAL_ASYMMETRIC_IMPLEMENTATIONS
    }
#endif
}

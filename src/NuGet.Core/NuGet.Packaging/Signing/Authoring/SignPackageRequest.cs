// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.


using System.Security.Cryptography.X509Certificates;
using NuGet.Common;

namespace NuGet.Packaging.Signing
{
    /// <summary>
    /// Contains a request for generating package signature.
    /// </summary>
    public class SignPackageRequest
    {
        /// <summary>
        /// Hash algorithm used to create the package signature.
        /// </summary>
        public HashAlgorithmName SignatureHashAlgorithm { get; set; } = HashAlgorithmName.SHA256;

        /// <summary>
        /// Hash algorithm used to timestamping the signed package.
        /// </summary>
        public HashAlgorithmName TimestampHashAlgorithm { get; set; } = HashAlgorithmName.SHA256;

        /// <summary>
        /// X509Certificate2 to be used while signing the package.
        /// </summary>
        public X509Certificate2 Certificate { get; set; }

    }
}
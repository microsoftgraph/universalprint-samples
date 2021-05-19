// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace S2SIntegration_CreateGraphEntities
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Org.BouncyCastle.Asn1;
    using Org.BouncyCastle.Asn1.X509;
    using Org.BouncyCastle.Crypto;
    using Org.BouncyCastle.Crypto.Generators;
    using Org.BouncyCastle.Crypto.Operators;
    using Org.BouncyCastle.Crypto.Parameters;
    using Org.BouncyCastle.OpenSsl;
    using Org.BouncyCastle.Pkcs;
    using Org.BouncyCastle.Security;

    public static class Crypto
    {
        /// <summary>
        /// Generates a RSA Public/Private key pair.
        /// </summary>
        /// <returns>The RSA Public/Private key pair.</returns>
        public static AsymmetricCipherKeyPair GenerateKeyPair()
        {
            RsaKeyPairGenerator generator = new RsaKeyPairGenerator();
            KeyGenerationParameters keyParams = new KeyGenerationParameters(new SecureRandom(), 2048);
            generator.Init(keyParams);
            return generator.GenerateKeyPair();
        }

        /// <summary>
        /// Generates a Certificate Signing Request (CSR) in Pkcs10 PEM format using the RSA public/private key pair.
        /// </summary>
        /// <param name="keyPair">RSA public/private key pair.</param>
        /// <returns>The CSR in PEM format without the PEM headers and footers.</returns>
        public static string GenerateCertificateSigningRequest(AsymmetricCipherKeyPair keyPair)
        {
            var values = new Dictionary<DerObjectIdentifier, string>
            {
                { X509Name.CN, "Microsoft" }, // domain name inside the quotes
                { X509Name.O, "Microsoft Corp" }, // Organisation's Legal name inside the quotes
                { X509Name.L, "Redmond" },
                { X509Name.ST, "Washington" },
                { X509Name.C, "US" },
            };

            var subject = new X509Name(values.Keys.Reverse().ToList(), values);

            var csr = new Pkcs10CertificationRequest(
                new Asn1SignatureFactory("SHA256withRSA", keyPair.Private),
                subject,
                keyPair.Public,
                null);

            // Convert BouncyCastle csr to PEM format
            var csrPem = new StringBuilder();
            var csrPemWriter = new PemWriter(new StringWriter(csrPem, CultureInfo.InvariantCulture));
            csrPemWriter.WriteObject(csr);
            csrPemWriter.Writer.Flush();

            return RemovePemHeaderFooter(csrPem.ToString());
        }

        /// <summary>
        /// Gets the string representation of the Public/Private Key in PEM format.
        /// </summary>
        /// <param name="key">The Public/Private key.</param>
        /// <param name="removePemHeaderFooter">True to remove the PEM header and footer (for example, -----BEGIN CERTIFICATE REQUEST-----). false otherwise.</param>
        /// <returns>String representation of the Public/Private Key in PEM format.</returns>
        public static string ConvertKeyToString(AsymmetricKeyParameter key, bool removePemHeaderFooter)
        {
            var keyPem = new StringBuilder();
            var keyPemWriter = new PemWriter(new StringWriter(keyPem, CultureInfo.InvariantCulture));
            keyPemWriter.WriteObject(key);
            keyPemWriter.Writer.Flush();

            return removePemHeaderFooter ? RemovePemHeaderFooter(keyPem.ToString()) : keyPem.ToString();
        }

        /// <summary>
        /// Creates a Public/Private Key from its string representation in PEM format, with PEM header and footer.
        /// </summary>
        /// <param name="keyStr">String representation of key in PEM format, with PEM header and footer.</param>
        /// <param name="isPrivate">true if this is a private key. false otherwise.</param>
        /// <returns>The Public/Private Key.</returns>
        public static AsymmetricKeyParameter CreateKeyFromString(string keyStr, bool isPrivate)
        {
            AsymmetricKeyParameter key;
            using (var sr = new StringReader(keyStr))
            {
                var pemReader = new PemReader(sr);
                var pemObject = pemReader.ReadObject();
                if (isPrivate)
                {
                    key = ((AsymmetricCipherKeyPair)pemObject).Private;
                }
                else
                {
                    key = (RsaKeyParameters)pemObject;
                }
            }

            return key;
        }

        private static string RemovePemHeaderFooter(string input)
        {
            var headerFooterList = new List<string>()
            {
                "-----BEGIN CERTIFICATE REQUEST-----",
                "-----END CERTIFICATE REQUEST-----",
                "-----BEGIN PUBLIC KEY-----",
                "-----END PUBLIC KEY-----",
                "-----BEGIN RSA PRIVATE KEY-----",
                "-----END RSA PRIVATE KEY-----"
            };

            string trimmed = input;
            foreach (var hf in headerFooterList)
            {
                trimmed = trimmed.Replace(hf, string.Empty);
            }

            return trimmed.Replace("\r\n", string.Empty);
        }
    }
}

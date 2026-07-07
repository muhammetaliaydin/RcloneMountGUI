using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;

namespace RcloneMountGUI
{
    public sealed class UpdatePackage
    {
        public string Version { get; set; }
        public string ExeUrl { get; set; }
        public string ManifestUrl { get; set; }
    }

    public sealed class UpdateManifest
    {
        public string Version { get; set; }
        public string AssetName { get; set; }
        public string Sha256 { get; set; }
        public string Signature { get; set; }
    }

    public static class UpdateSecurity
    {
        public const string ExpectedExeAssetName = "RcloneMountGUI.exe";
        public const string ExpectedManifestAssetName = "RcloneMountGUI.update.json";

        // Replace this value with tools/New-UpdateSigningKey.ps1 output before publishing signed updates.
        public const string UpdatePublicKeyXml = "<RSAKeyValue><Modulus>zXhqjzQQrpXiU/Au+wUcOX7M1S3yaSazmnT/daGpH7udLEeudLFMQjVYoVoou1vIEb3FYqBKvADfMJXZx2k0oD12CNG6eKWi3N+aiqo1rXUQ5aaSsmIYMsMQmvNtHHgEur5S9l6Bh1lG0/TODLYPa//sKZ3JXtvoMnYigCoHcNPUyloMd1hVTi5avCcDiU+WFPk30IDqnvVuGZ1YYl6w0VBAV+dUXcynh6PAxFR/VrUqfPqJxZxd7A1/tguiE/dxxf7uhwx7D/mofeENouMGGn5etyBiB3xMV9bhr2mUqKQilAvz45UU9iFuIQqRmryU+k4Mzof0+/vuvYGZhPvDPArBGgJ7lROLl8rWmmoC4zmc0oyiHBV76qXfmGnI9kcNWWH1lKokgjGqtQ8ddKqPsZSwqmI9/Fcvm+ofaHBUIBsr/qs5rFIijDWk878lf1boiVpDYVJL2uIu1p+R9ed4PBPBkGwnC5728lcByzDPKk1ISqqHvIoQF3wSftvWqQrZ</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        public static bool TryReadReleasePackage(string json, string expectedVersion, out UpdatePackage package, out string error)
        {
            package = null;
            error = null;

            if (String.IsNullOrWhiteSpace(json))
            {
                error = "Release metadata is empty.";
                return false;
            }

            Dictionary<string, object> release;
            try
            {
                release = new JavaScriptSerializer().DeserializeObject(json) as Dictionary<string, object>;
            }
            catch (Exception ex)
            {
                error = "Release metadata is not valid JSON: " + ex.Message;
                return false;
            }

            if (release == null)
            {
                error = "Release metadata has an unexpected shape.";
                return false;
            }

            string tagVersion = TrimVersionPrefix(GetString(release, "tag_name"));
            string normalizedExpected = TrimVersionPrefix(expectedVersion);
            if (String.IsNullOrEmpty(tagVersion) || !String.Equals(tagVersion, normalizedExpected, StringComparison.OrdinalIgnoreCase))
            {
                error = "Release tag does not match the expected update version.";
                return false;
            }

            object assetsObject;
            if (!release.TryGetValue("assets", out assetsObject))
            {
                error = "Release metadata does not contain assets.";
                return false;
            }

            IEnumerable assets = assetsObject as IEnumerable;
            if (assets == null)
            {
                error = "Release assets have an unexpected shape.";
                return false;
            }

            string exeUrl = null;
            string manifestUrl = null;

            foreach (object assetObject in assets)
            {
                Dictionary<string, object> asset = assetObject as Dictionary<string, object>;
                if (asset == null)
                    continue;

                string name = GetString(asset, "name");
                string url = GetString(asset, "browser_download_url");

                if (String.Equals(name, ExpectedExeAssetName, StringComparison.Ordinal))
                {
                    if (!IsAllowedDownloadUrl(url))
                    {
                        error = "Executable asset URL is not on an allowed GitHub download host.";
                        return false;
                    }

                    exeUrl = url;
                }
                else if (String.Equals(name, ExpectedManifestAssetName, StringComparison.Ordinal))
                {
                    if (!IsAllowedDownloadUrl(url))
                    {
                        error = "Manifest asset URL is not on an allowed GitHub download host.";
                        return false;
                    }

                    manifestUrl = url;
                }
            }

            if (String.IsNullOrEmpty(exeUrl))
            {
                error = "Release does not contain the expected executable asset.";
                return false;
            }

            if (String.IsNullOrEmpty(manifestUrl))
            {
                error = "Release does not contain the expected signed update manifest.";
                return false;
            }

            package = new UpdatePackage
            {
                Version = normalizedExpected,
                ExeUrl = exeUrl,
                ManifestUrl = manifestUrl
            };
            return true;
        }

        public static bool TryReadManifest(string json, out UpdateManifest manifest, out string error)
        {
            manifest = null;
            error = null;

            if (String.IsNullOrWhiteSpace(json))
            {
                error = "Update manifest is empty.";
                return false;
            }

            try
            {
                manifest = new JavaScriptSerializer().Deserialize<UpdateManifest>(json);
            }
            catch (Exception ex)
            {
                error = "Update manifest is not valid JSON: " + ex.Message;
                return false;
            }

            if (manifest == null ||
                String.IsNullOrWhiteSpace(manifest.Version) ||
                String.IsNullOrWhiteSpace(manifest.AssetName) ||
                String.IsNullOrWhiteSpace(manifest.Sha256) ||
                String.IsNullOrWhiteSpace(manifest.Signature))
            {
                error = "Update manifest is missing required fields.";
                return false;
            }

            manifest.Version = TrimVersionPrefix(manifest.Version);
            manifest.Sha256 = manifest.Sha256.Trim().ToLowerInvariant();
            return true;
        }

        public static bool VerifyManifestAndPayload(UpdateManifest manifest, string expectedVersion, byte[] executableBytes, string publicKeyXml, out string error)
        {
            error = null;

            if (manifest == null)
            {
                error = "Update manifest is missing.";
                return false;
            }

            if (!String.Equals(manifest.Version, TrimVersionPrefix(expectedVersion), StringComparison.OrdinalIgnoreCase))
            {
                error = "Update manifest version does not match the release version.";
                return false;
            }

            if (!String.Equals(manifest.AssetName, ExpectedExeAssetName, StringComparison.Ordinal))
            {
                error = "Update manifest references an unexpected executable asset.";
                return false;
            }

            if (!IsLowercaseSha256(manifest.Sha256))
            {
                error = "Update manifest SHA-256 is not a lowercase hexadecimal digest.";
                return false;
            }

            if (executableBytes == null || executableBytes.Length == 0)
            {
                error = "Downloaded executable is empty.";
                return false;
            }

            if (!VerifySignature(manifest, publicKeyXml))
            {
                error = "Update manifest signature is invalid.";
                return false;
            }

            string actualHash = ComputeSha256Hex(executableBytes);
            if (!String.Equals(actualHash, manifest.Sha256, StringComparison.Ordinal))
            {
                error = "Downloaded executable SHA-256 does not match the signed manifest.";
                return false;
            }

            return true;
        }

        public static string ComputeSha256Hex(byte[] data)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(data);
                StringBuilder builder = new StringBuilder(hash.Length * 2);
                foreach (byte b in hash)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }

        public static string BuildManifestPayload(UpdateManifest manifest)
        {
            return TrimVersionPrefix(manifest.Version) + "\n" + manifest.AssetName + "\n" + manifest.Sha256 + "\n";
        }

        private static bool VerifySignature(UpdateManifest manifest, string publicKeyXml)
        {
            if (String.IsNullOrWhiteSpace(publicKeyXml))
                return false;

            byte[] signature;
            try
            {
                signature = Convert.FromBase64String(manifest.Signature);
            }
            catch
            {
                return false;
            }

            byte[] payload = Encoding.UTF8.GetBytes(BuildManifestPayload(manifest));

            try
            {
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {
                    rsa.PersistKeyInCsp = false;
                    rsa.FromXmlString(publicKeyXml);
                    return rsa.VerifyData(payload, CryptoConfig.MapNameToOID("SHA256"), signature);
                }
            }
            catch
            {
                return false;
            }
        }

        private static bool IsAllowedDownloadUrl(string url)
        {
            Uri uri;
            if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
                return false;

            if (!String.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
                return false;

            return String.Equals(uri.Host, "github.com", StringComparison.OrdinalIgnoreCase) ||
                   String.Equals(uri.Host, "objects.githubusercontent.com", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsLowercaseSha256(string value)
        {
            if (String.IsNullOrEmpty(value) || value.Length != 64)
                return false;

            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                if (!((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f')))
                    return false;
            }

            return true;
        }

        private static string GetString(Dictionary<string, object> values, string key)
        {
            object value;
            if (!values.TryGetValue(key, out value) || value == null)
                return null;

            return Convert.ToString(value);
        }

        private static string TrimVersionPrefix(string version)
        {
            return String.IsNullOrWhiteSpace(version) ? null : version.Trim().TrimStart('v', 'V');
        }
    }
}

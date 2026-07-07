using System;
using System.Security.Cryptography;
using System.Text;
using RcloneMountGUI;

namespace RcloneMountGUI.Tests
{
    internal static class Program
    {
        private static int failures;

        private static int Main()
        {
            Run("release package ignores attacker exe and selects exact assets", ReleasePackageIgnoresAttackerExe);
            Run("release package requires signed manifest asset", ReleasePackageRequiresManifest);
            Run("manifest rejects invalid signature", ManifestRejectsInvalidSignature);
            Run("manifest rejects hash mismatch", ManifestRejectsHashMismatch);
            Run("manifest rejects version mismatch", ManifestRejectsVersionMismatch);
            Run("manifest accepts valid signature and matching hash", ManifestAcceptsValidSignatureAndHash);

            if (failures == 0)
            {
                Console.WriteLine("All updater security tests passed.");
                return 0;
            }

            Console.Error.WriteLine(failures + " updater security test(s) failed.");
            return 1;
        }

        private static void ReleasePackageIgnoresAttackerExe()
        {
            string json =
                "{\"tag_name\":\"v2.1.1\",\"assets\":[" +
                "{\"name\":\"malware.exe\",\"browser_download_url\":\"https://github.com/muhammetaliaydin/RcloneMountGUI/releases/download/v2.1.1/malware.exe\"}," +
                "{\"name\":\"RcloneMountGUI.exe\",\"browser_download_url\":\"https://github.com/muhammetaliaydin/RcloneMountGUI/releases/download/v2.1.1/RcloneMountGUI.exe\"}," +
                "{\"name\":\"RcloneMountGUI.update.json\",\"browser_download_url\":\"https://github.com/muhammetaliaydin/RcloneMountGUI/releases/download/v2.1.1/RcloneMountGUI.update.json\"}" +
                "]}";

            UpdatePackage package;
            string error;
            Assert(UpdateSecurity.TryReadReleasePackage(json, "2.1.1", out package, out error), error);
            Assert(package.ExeUrl.EndsWith("/RcloneMountGUI.exe", StringComparison.Ordinal), "Unexpected exe URL selected.");
            Assert(package.ManifestUrl.EndsWith("/RcloneMountGUI.update.json", StringComparison.Ordinal), "Unexpected manifest URL selected.");
        }

        private static void ReleasePackageRequiresManifest()
        {
            string json =
                "{\"tag_name\":\"v2.1.1\",\"assets\":[" +
                "{\"name\":\"RcloneMountGUI.exe\",\"browser_download_url\":\"https://github.com/muhammetaliaydin/RcloneMountGUI/releases/download/v2.1.1/RcloneMountGUI.exe\"}" +
                "]}";

            UpdatePackage package;
            string error;
            Assert(!UpdateSecurity.TryReadReleasePackage(json, "2.1.1", out package, out error), "Release without manifest was accepted.");
        }

        private static void ManifestRejectsInvalidSignature()
        {
            using (RSACryptoServiceProvider rsa = NewRsa())
            {
                byte[] exe = Encoding.UTF8.GetBytes("legitimate executable");
                UpdateManifest manifest = BuildSignedManifest(rsa, "2.1.1", exe);
                manifest.Signature = Convert.ToBase64String(Encoding.UTF8.GetBytes("not a signature"));

                string error;
                Assert(!UpdateSecurity.VerifyManifestAndPayload(manifest, "2.1.1", exe, rsa.ToXmlString(false), out error), "Invalid signature was accepted.");
            }
        }

        private static void ManifestRejectsHashMismatch()
        {
            using (RSACryptoServiceProvider rsa = NewRsa())
            {
                byte[] signedExe = Encoding.UTF8.GetBytes("legitimate executable");
                byte[] downloadedExe = Encoding.UTF8.GetBytes("tampered executable");
                UpdateManifest manifest = BuildSignedManifest(rsa, "2.1.1", signedExe);

                string error;
                Assert(!UpdateSecurity.VerifyManifestAndPayload(manifest, "2.1.1", downloadedExe, rsa.ToXmlString(false), out error), "Hash mismatch was accepted.");
            }
        }

        private static void ManifestRejectsVersionMismatch()
        {
            using (RSACryptoServiceProvider rsa = NewRsa())
            {
                byte[] exe = Encoding.UTF8.GetBytes("legitimate executable");
                UpdateManifest manifest = BuildSignedManifest(rsa, "2.1.1", exe);

                string error;
                Assert(!UpdateSecurity.VerifyManifestAndPayload(manifest, "2.1.2", exe, rsa.ToXmlString(false), out error), "Version mismatch was accepted.");
            }
        }

        private static void ManifestAcceptsValidSignatureAndHash()
        {
            using (RSACryptoServiceProvider rsa = NewRsa())
            {
                byte[] exe = Encoding.UTF8.GetBytes("legitimate executable");
                UpdateManifest manifest = BuildSignedManifest(rsa, "2.1.1", exe);

                string error;
                Assert(UpdateSecurity.VerifyManifestAndPayload(manifest, "v2.1.1", exe, rsa.ToXmlString(false), out error), error);
            }
        }

        private static UpdateManifest BuildSignedManifest(RSACryptoServiceProvider rsa, string version, byte[] exe)
        {
            UpdateManifest manifest = new UpdateManifest
            {
                Version = version,
                AssetName = UpdateSecurity.ExpectedExeAssetName,
                Sha256 = UpdateSecurity.ComputeSha256Hex(exe)
            };

            byte[] payload = Encoding.UTF8.GetBytes(UpdateSecurity.BuildManifestPayload(manifest));
            byte[] signature = rsa.SignData(payload, CryptoConfig.MapNameToOID("SHA256"));
            manifest.Signature = Convert.ToBase64String(signature);
            return manifest;
        }

        private static RSACryptoServiceProvider NewRsa()
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(3072);
            rsa.PersistKeyInCsp = false;
            return rsa;
        }

        private static void Run(string name, Action test)
        {
            try
            {
                test();
                Console.WriteLine("PASS " + name);
            }
            catch (Exception ex)
            {
                failures++;
                Console.Error.WriteLine("FAIL " + name + ": " + ex.Message);
            }
        }

        private static void Assert(bool condition, string message)
        {
            if (!condition)
                throw new InvalidOperationException(message);
        }
    }
}

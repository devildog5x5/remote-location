using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using IPManagementInterface.Models;
using Newtonsoft.Json;

namespace IPManagementInterface.Services
{
    public class SecurityService
    {
        private readonly string _credentialsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "IPManagementInterface",
            "credentials.json"
        );

        private List<DeviceCredentials> _credentials = new();
        private readonly byte[] _encryptionKey;

        public SecurityService()
        {
            _encryptionKey = GetOrCreateEncryptionKey();
            LoadCredentials();
        }

        public void SaveCredentials(string deviceIp, string? username, string? password, bool useCredentials)
        {
            var existing = _credentials.FirstOrDefault(c => c.DeviceIpAddress == deviceIp);
            
            if (existing != null)
            {
                existing.Username = username;
                existing.Password = !string.IsNullOrEmpty(password) ? Encrypt(password) : null;
                existing.UseCredentials = useCredentials;
            }
            else
            {
                _credentials.Add(new DeviceCredentials
                {
                    DeviceIpAddress = deviceIp,
                    Username = username,
                    Password = !string.IsNullOrEmpty(password) ? Encrypt(password) : null,
                    UseCredentials = useCredentials
                });
            }

            SaveCredentials();
        }

        public DeviceCredentials? GetCredentials(string deviceIp)
        {
            var creds = _credentials.FirstOrDefault(c => c.DeviceIpAddress == deviceIp);
            if (creds != null && !string.IsNullOrEmpty(creds.Password))
            {
                creds.Password = Decrypt(creds.Password);
            }
            return creds;
        }

        public void RemoveCredentials(string deviceIp)
        {
            var creds = _credentials.FirstOrDefault(c => c.DeviceIpAddress == deviceIp);
            if (creds != null)
            {
                _credentials.Remove(creds);
                SaveCredentials();
            }
        }

        private string Encrypt(string plainText)
        {
            try
            {
                using var aes = Aes.Create();
                aes.Key = _encryptionKey;
                aes.GenerateIV();

                using var encryptor = aes.CreateEncryptor();
                using var ms = new MemoryStream();
                ms.Write(aes.IV, 0, aes.IV.Length);

                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var sw = new StreamWriter(cs))
                {
                    sw.Write(plainText);
                }

                return Convert.ToBase64String(ms.ToArray());
            }
            catch
            {
                return plainText; // Fallback to plain text if encryption fails
            }
        }

        private string Decrypt(string cipherText)
        {
            try
            {
                var fullCipher = Convert.FromBase64String(cipherText);
                var iv = new byte[16];
                var cipher = new byte[fullCipher.Length - 16];

                Array.Copy(fullCipher, 0, iv, 0, iv.Length);
                Array.Copy(fullCipher, iv.Length, cipher, 0, cipher.Length);

                using var aes = Aes.Create();
                aes.Key = _encryptionKey;
                aes.IV = iv;

                using var decryptor = aes.CreateDecryptor();
                using var ms = new MemoryStream(cipher);
                using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                using var sr = new StreamReader(cs);

                return sr.ReadToEnd();
            }
            catch
            {
                return cipherText; // Fallback if decryption fails
            }
        }

        private byte[] GetOrCreateEncryptionKey()
        {
            var keyPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "IPManagementInterface",
                ".encryption_key"
            );

            if (File.Exists(keyPath))
            {
                return File.ReadAllBytes(keyPath);
            }

            // Generate new key
            using var aes = Aes.Create();
            aes.GenerateKey();
            var key = aes.Key;

            Directory.CreateDirectory(Path.GetDirectoryName(keyPath)!);
            File.WriteAllBytes(keyPath, key);

            return key;
        }

        private void LoadCredentials()
        {
            try
            {
                if (File.Exists(_credentialsPath))
                {
                    var json = File.ReadAllText(_credentialsPath);
                    var credentials = JsonConvert.DeserializeObject<List<DeviceCredentials>>(json);
                    if (credentials != null)
                    {
                        _credentials = credentials;
                    }
                }
            }
            catch { }
        }

        private void SaveCredentials()
        {
            try
            {
                var directory = Path.GetDirectoryName(_credentialsPath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var json = JsonConvert.SerializeObject(_credentials, Formatting.Indented);
                File.WriteAllText(_credentialsPath, json);
            }
            catch { }
        }
    }
}

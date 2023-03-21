using Microsoft.VisualBasic;
using System;
using System.Diagnostics;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using System.Threading;
using Aes = System.Security.Cryptography.Aes;

namespace AESEncryptie
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter your credit card number: ");
            string creditCardNumber = Console.ReadLine();

            byte[] key = GenerateRandomKey(); // replace with your own key
            byte[] iv = GenerateRandomIV(); // replace with your own initialization vector
            string Key = "";
            string Iv = "";
            string encryptedCreditCardNumber = EncryptString(creditCardNumber, key, iv);

            Console.WriteLine("Your encrypted credit card number is: " + encryptedCreditCardNumber);
            foreach (var item in key)
                Key += item;

            foreach (var item in iv)
                Iv += item;

            Console.WriteLine("The key is: " + Key);
            Console.WriteLine("The iv is: " + Iv);
        }

        static string EncryptString(string plainText, byte[] key, byte[] iv)
        {
            try
            {
                byte[] encrypted;

                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = iv;

                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                    using (var ms = new System.IO.MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            using (var sw = new System.IO.StreamWriter(cs))
                            {
                                sw.Write(plainText);
                            }
                            encrypted = ms.ToArray();
                        }
                    }
                }
                return Convert.ToBase64String(encrypted);
            }
            catch (Exception)
            {
                return "Misschien heb je iets verkeerd ingevuld?";
                
            }

        }

        static byte[] GenerateRandomKey()
        {
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] key = new byte[32]; // 256-bit key length
                rng.GetBytes(key);
                return key;
            }
        }

        static byte[] GenerateRandomIV()
        {
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] iv = new byte[16]; // 128-bit block size
                rng.GetBytes(iv);
                return iv;
            }
        }
    }
}

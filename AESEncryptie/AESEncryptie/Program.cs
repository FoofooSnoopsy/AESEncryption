using Microsoft.VisualBasic;
using System;
using System.Diagnostics;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using System.Threading;
using Aes = System.Security.Cryptography.Aes;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace AESEncryptie
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.Write("Enter your credit card number: ");
                string creditCardNumber = Console.ReadLine();
                if (creditCardNumber == null || creditCardNumber.All(char.IsDigit) == false)
                {
                    Console.WriteLine("Je moet wel iets geldigs invullen");
                    return;
                }
                Console.Write("Enter your first name: ");
                string fname = Console.ReadLine();
                Console.Write("Enter your last name: ");
                string lname = Console.ReadLine();
                Console.Write("Enter your street: ");
                string street = Console.ReadLine();
                Console.Write("Enter your housenumber: ");
                string housenum = Console.ReadLine();
                Console.Write("Enter your postalcode: ");
                string postalcode = Console.ReadLine();
                Console.Write("Enter your city: ");
                string city = Console.ReadLine();

                if (fname == null || lname == null || street == null || housenum == null || postalcode == null || city == null)
                {
                    Console.WriteLine("Je moet wel iets geldigs invullen");
                    return;
                }
                if (LuhnCheck(creditCardNumber) == false)
                {
                    Console.WriteLine("Dit is geen geldig nummer");
                    return;
                }

                byte[] key = GenerateRandomKey(); // replace with your own key
                byte[] iv = GenerateRandomIV(); // replace with your own initialization vector
                string Key = "";
                string Iv = "";
                string encryptedCreditCardNumber = EncryptString(creditCardNumber, key, iv);
                foreach (var item in key)
                    Key += item;

                foreach (var item in iv)
                    Iv += item;

                using (var context = new DBContext())
                {
                    var person = new Person {FirstName = fname, LastName = lname, Street = street, HouseNumber = housenum , PostalCode = postalcode, City = city, CreditCard = encryptedCreditCardNumber, encryptionKey = Key, encryptionIV = Iv};
                    context.People.Add(person);
                    context.SaveChanges();
                }

                Console.WriteLine("Your encrypted credit card number is: " + encryptedCreditCardNumber);
                Console.WriteLine("The key is: " + Key);
                Console.WriteLine("The iv is: " + Iv);
            }
            catch (Exception ex)
            {

                Console.WriteLine( "Misschien heb je iets verkeerd ingevuld?");
                Console.WriteLine(ex.Message);
            }

        }

        static string EncryptString(string plainText, byte[] key, byte[] iv)
        {
            try
            {
                if (plainText.All(char.IsDigit) == false)
                {
                    
                    return "Je moet wel iets geldigs invullen";
                }
                byte[] encrypted;

                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = iv;

                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                    using var ms = new System.IO.MemoryStream();
                    using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
                    using (var sw = new System.IO.StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }
                    encrypted = ms.ToArray();
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

        static bool LuhnCheck(string creditCardNumber)
        {
            int sum = 0;
            bool isSecondDigit = false;

            // Traverse the credit card number from right to left
            for (int i = creditCardNumber.Length - 1; i >= 0; i--)
            {
                if (char.IsDigit(creditCardNumber[i]))
                {
                    int digit = int.Parse(creditCardNumber[i].ToString());

                    if (isSecondDigit)
                    {
                        digit *= 2;

                        if (digit > 9)
                        {
                            digit -= 9;
                        }
                    }

                    sum += digit;
                    isSecondDigit = !isSecondDigit;
                }
            }

            return sum % 10 == 0;
        }

    }
}

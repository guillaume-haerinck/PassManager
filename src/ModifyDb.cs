using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace ConsoleApp.SQLite
{
    public class ModifyDb
    {

        public void AddUserAndMasterPassword(string[] args)
        {
            if (args.Length >= 3)
            {
                using (var db = new AccessDb())
                {
                    //Cryptography creates objects, 128 bits = 16 octets
                    SHA256 mySHA256 = SHA256Managed.Create();
                    byte[] hash;
                    byte[] salt = GenerateRandomData(128);
                    string stringSalt;

                    //Add the salt to the text
                    stringSalt = Convert.ToBase64String(salt);
                    args[2] += stringSalt;

                    //Cryptography generates hash from password
                    hash = mySHA256.ComputeHash(Encoding.UTF8.GetBytes(args[2]));

                    //Create objects
                    var user = new UserName { Name = args[1] };
                    var masterPassword = new UserPassword {
                        SaltAndHash = stringSalt + ":" + Convert.ToBase64String(hash),
                        Salt = stringSalt
                    };

                    //Auto generates an ID
                    db.Users.Add(user);
                    db.UsersPasswords.Add(masterPassword);

                    //Save the IDs
                    db.SaveChanges();

                    //Access and copy the IDs
                    user.UserPasswordForeignKey = masterPassword.UserPasswordId;
                    masterPassword.UserNameForeignKey = user.UserNameId;

                    //Save foreign Keys
                    db.SaveChanges();
                    System.Console.WriteLine("OK");

                    //TO DEBUG
                    //System.Console.WriteLine("Hash: " + Convert.ToBase64String(hash));
                    //System.Console.WriteLine("SaltedString: " + args[2]);
                    //System.Console.WriteLine("Salt: " + Convert.ToBase64String(salt));
                    //System.Console.WriteLine("user id: " + user.UserNameId);
                }
            }
            else
            {
                System.Console.WriteLine("ERROR");
            }
        }

        public void AddTagAndPassword(string[] args)
        {
            if (args.Length >= 5)
            {
                using (var db = new AccessDb())
                {
                    using (Aes myAes = Aes.Create())
                    {
                        //Get the salt
                        int userId = GetUserId(args);
                        string salt = String.Empty;
                        foreach (var masterPassword in db.UsersPasswords)
                        {
                            if (masterPassword.UserNameForeignKey == userId)
                            {
                                salt = masterPassword.Salt;
                            }
                        }
                        
                        //Get the IV and the key, 32bytes is 256bits
                        myAes.IV = GenerateRandomData(128);
                        myAes.Key = GenerateAesKey(32, args, salt);

                        //Encrypt the string to an array of bytes.
                        byte[] encrypted = EncryptStringToBytes_Aes(args[4], myAes.Key, myAes.IV);

                        //Creates the objects to the db
                        var tag = new Tag { Name = args[3] };
                        var password = new Password { EncryptedPassword = Convert.ToBase64String(myAes.IV) + Convert.ToBase64String(encrypted),
                            Key = Convert.ToBase64String(myAes.Key)
                        };

                        //Auto generates an ID
                        db.Tags.Add(tag);
                        db.Passwords.Add(password);

                        //Save the IDs
                        db.SaveChanges();

                        //Access and copy the IDs
                        tag.PasswordForeignKey = password.PasswordId;
                        tag.UserNameForeignKey = userId;
                        password.TagForeignKey = tag.TagId;
                        password.UserNameForeignKey = userId;

                        //Save the foreign keys
                        db.SaveChanges();
                        System.Console.WriteLine("OK");

                        /*
                        //TO DEBUG
                        System.Console.WriteLine("userId: " + userId);
                        System.Console.WriteLine("tagId: " + tag.TagId);
                        System.Console.WriteLine("salt: " + salt);
                        System.Console.WriteLine("key: " + Convert.ToBase64String(myAes.Key));
                        System.Console.WriteLine("IV: " + Convert.ToBase64String(myAes.IV));
                        string decrypted = DecryptStringFromBytes_Aes(encrypted, myAes.Key, myAes.IV);
                        System.Console.WriteLine("Original: {0}", args[4]);
                        System.Console.WriteLine("Encrypted: " + password.EncryptedPassword);
                        System.Console.WriteLine("Decrypted: {0}", decrypted);
                        */
                    }
                }
            }
            else
            {
                System.Console.WriteLine("ERROR");
            }
        }

        public void GetDecryptedPassword(string[] args)
        {
            if (args.Length >= 4)
            {
                using (var db = new AccessDb())
                {
                    using (Aes myAes = Aes.Create())
                    {
                        var stringIvAndEncrypted = new string[] { string.Empty };
                        string stringKey = string.Empty;

                        //Get the needeed values
                        int tagId = GetTagId(args);
                        foreach (var password in db.Passwords)
                        {
                            if (password.TagForeignKey == tagId)
                            {
                                stringKey = password.Key;
                                stringIvAndEncrypted = password.EncryptedPassword.Split("==");
                            }
                        }

                        //add the == removed by string split
                        if (stringIvAndEncrypted.Length >= 2)
                        {
                            stringIvAndEncrypted[0] += "==";
                            stringIvAndEncrypted[1] += "==";
                        }

                        //Conversion from string to bytes
                        byte[] Key = Convert.FromBase64String(stringKey);
                        byte[] IV = Convert.FromBase64String(stringIvAndEncrypted[0]);
                        byte[] encrypted = Convert.FromBase64String(stringIvAndEncrypted[1]);

                        //Decrypt and output the result
                        string decrypted = DecryptStringFromBytes_Aes(encrypted, Key, IV);
                        System.Console.WriteLine(decrypted);

                        /*
                        //DEBUG
                        System.Console.WriteLine("Tag Id: " + tagId);
                        System.Console.WriteLine("Key: " + stringKey);
                        System.Console.WriteLine("IV: " + stringIvAndEncrypted[0]);
                        System.Console.WriteLine("Encrypted: " + stringIvAndEncrypted[0] + stringIvAndEncrypted[1]);
                        */
                    }
                }
            }
            else
            {
                System.Console.WriteLine("ERROR");
            }
        }

        public void DeleteTagAndPassword(string[] args)
        {
            if (args.Length >= 4)
            {
                using (var db = new AccessDb())
                {
                    foreach (var tag in db.Tags)
                    {
                        if (tag.Name == args[3])
                        {
                            foreach (var password in db.Passwords)
                            {
                                if (tag.PasswordForeignKey == password.PasswordId)
                                {
                                    db.Remove(tag);
                                    db.Remove(password);
                                    db.SaveChanges();
                                    System.Console.WriteLine("OK");
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                System.Console.WriteLine("ERROR");
            }
        }

        public void GetSaltAndHash(string[] args)
        {
            using (var db = new AccessDb())
            {
                int userId = GetUserId(args);
                foreach (var password in db.UsersPasswords)
                {
                    if (userId == password.UserNameForeignKey)
                    {
                        System.Console.WriteLine(password.SaltAndHash);
                    }
                }
            }
        }

        public void GetEncryptedPassword(string[] args)
        {
            using (var db = new AccessDb())
            {
                foreach (var tag in db.Tags)
                {
                    if (args[3] == tag.Name)
                    {
                        foreach(var password in db.Passwords)
                        {
                            if (password.PasswordId == tag.PasswordForeignKey)
                            {
                                System.Console.WriteLine(password.EncryptedPassword);
                            }
                        }
                    }
                }
            }
        }

        public int GetUserId(string[] args)
        {
            using (var db = new AccessDb())
            {
                //Get the user ID from his username
                if (args.Length >= 2)
                {
                    foreach (var user in db.Users)
                    {
                        if (user.Name == args[1])
                        {
                            //System.Console.WriteLine("User id: " + user.UserNameId);
                            return user.UserNameId;
                        }
                    }
                    System.Console.WriteLine("ERROR");
                    return 0;
                }
                else
                {
                    System.Console.WriteLine("ERROR");
                    return 0;
                }
            }
        }

        public int GetTagId(string[] args)
        {
            using (var db = new AccessDb())
            {
                if (args.Length >= 4)
                {
                    foreach (var tag in db.Tags)
                    {
                        if (tag.Name == args[3])
                        {
                            //System.Console.WriteLine("Tag id: " + tag.TagId);
                            return tag.TagId;
                        }
                    }
                    System.Console.WriteLine("ERROR");
                    return 0;
                }
                else
                {
                    System.Console.WriteLine("ERROR");
                    return 0;
                }
            }
        }

        //Need improvement
        public bool IsUserValid(string[] args, bool checkPassword)
        {
            using (var db = new AccessDb())
            {
                if (checkPassword)
                {
                    if (args.Length >= 3)
                    {
                        foreach (var user in db.Users)
                        {
                            if (user.Name == args[1])
                            {
                                return true;
                                //Need to generate the hash and compare it

                            }
                        }
                        return false;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (args.Length >= 2)
                    {
                        foreach (var user in db.Users)
                        {
                            if ((user.Name == args[1]))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        return false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public bool IsTagValid(string[] args)
        {
            using (var db = new AccessDb())
            {
                if (args.Length >= 4)
                {
                    foreach (var tag in db.Tags)
                    {
                        if (tag.Name == args[3])
                        {
                            return true;
                        }
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
        }

        //Need to be written
        public bool IsHashEqual(string hash1, string hash2, string salt)
        {
            return true;
        }

        public static byte[] GenerateRandomData(int bits)
        {
            var result = new byte[bits / 8];
            RandomNumberGenerator.Create().GetBytes(result);
            return result;
        }

        public static byte[] GenerateAesKey(int bytes, string[] args, string stringSalt)
        {
            const int iterations = 10000;
            byte[] salt = Convert.FromBase64String(stringSalt);
            var derivation = KeyDerivationPrf.HMACSHA256;
            return KeyDerivation.Pbkdf2(args[4], salt, derivation, iterations, bytes);
        }

        public static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            byte[] encrypted;
            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        public static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;

        }

    }

}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Hash
{
    public class Hashing
    {
        public static byte[] Hash(string data)
        {
            byte[] byteSecretKey;
            using (var hash = SHA512.Create())
            {
                byteSecretKey = hash.ComputeHash(Encoding.UTF8.GetBytes(data));
            }
            return byteSecretKey;
        }

        public static string HashPassword(string data)
        {
            byte[] byteSecretKey;
            using (var hash = SHA512.Create())
            {
                byteSecretKey = hash.ComputeHash(Encoding.UTF8.GetBytes(data));
            }

            StringBuilder strBuilder = new();

            foreach (var key in byteSecretKey)
            {
                strBuilder.Append(key.ToString("x2"));
            }
            return strBuilder.ToString();
        }
    }
}

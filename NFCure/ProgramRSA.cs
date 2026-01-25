using System;
using System.Collections.Generic;
using System.Text;

namespace NFCure
{
    public class ProgramRSA
    {
        // --- Génération des clés ---
        public long p = 61;
        public long q = 53;
        public long n;
        public long phi;
        public long e;
        public long d;

        public ProgramRSA()
        {
            n = p * q;
            phi = (p - 1) * (q - 1);
            e = 17;  // premier avec phi
            d = ModInverse(e, phi);
        }

        public ProgramRSA(long p, long q)
        {
            this.p = p;
            this.q = q;
            n = p * q;
            phi = (p - 1) * (q - 1);
            e = 17;  // premier avec phi
            d = ModInverse(e, phi);
        }

        // --- Chiffrement avec clé publique ---
        public List<long> Encrypt(byte[] data, long[] clepublique)
        {
            List<long> encrypted = new List<long>();
            foreach (byte b in data)
            {
                long c = ModPow(b, clepublique[1], clepublique[0]);
                encrypted.Add(c);
            }
            return encrypted;
        }

        // --- Déchiffrement avec clé privée ---
        public byte[] Decrypt(List<long> encrypted, long[] cleprivee)
        {
            List<byte> decrypted = new List<byte>();
            foreach (long c in encrypted)
            {
                long m = ModPow(c, cleprivee[1], cleprivee[0]);
                decrypted.Add((byte)m);
            }
            return decrypted.ToArray();
        }

        // --- Exponentiation modulaire rapide ---
        public long ModPow(long baseValue, long exp, long mod)
        {
            long result = 1;
            baseValue = baseValue % mod;
            while (exp > 0)
            {
                if ((exp & 1) == 1)
                    result = (result * baseValue) % mod;
                exp >>= 1;
                baseValue = (baseValue * baseValue) % mod;
            }
            return result;
        }

        // --- Inverse modulaire avec algorithme d'Euclide étendu ---
        public long ModInverse(long a, long m)
        {
            long m0 = m, t, q;
            long x0 = 0, x1 = 1;
            if (m == 1) return 0;
            while (a > 1)
            {
                q = a / m;
                t = m;
                m = a % m;
                a = t;
                t = x0;
                x0 = x1 - q * x0;
                x1 = t;
            }
            if (x1 < 0) x1 += m0;
            return x1;
        }

        // --- Chiffrer un string avec clé publique ---
        public List<long> EncryptString(string text, long[] clepublique)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            return Encrypt(bytes, clepublique);
        }

        // --- Déchiffrer un string à partir d'une liste de valeurs ---
        public string DecryptString(string encryptedText, long[] cleprivee)
        {
            if (string.IsNullOrWhiteSpace(encryptedText))
                return "";

            try
            {
                // Convertir le string en liste de long
                var encryptedValues = encryptedText.Split(',');
                var longValues = new List<long>();
                
                foreach (var val in encryptedValues)
                {
                    if (long.TryParse(val.Trim(), out long num))
                    {
                        longValues.Add(num);
                    }
                }

                // Déchiffrer
                byte[] decryptedBytes = Decrypt(longValues, cleprivee);

                // Convertir en string
                return Encoding.UTF8.GetString(decryptedBytes);
            }
            catch
            {
                return "❌ Erreur déchiffrement";
            }
        }
    }
}

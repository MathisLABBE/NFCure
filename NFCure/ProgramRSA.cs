using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

class RSAFile
{
    // --- Génération des clés ---
    public long p = 61;
    public long q = 53;
    public long n;
    public long phi;
    public long e;
    public long d;

    public RSAFile()
    {
        n = p * q;
        phi = (p - 1) * (q - 1);
        e = 17;  // premier avec phi
        d = ModInverse(e, phi);
    }

    // --- Chiffrement ---
    public List<long> Encrypt(byte[] data)
    {
        List<long> encrypted = new List<long>();
        foreach (byte b in data)
        {
            long c = ModPow(b, e, n);
            encrypted.Add(c);
        }
        return encrypted;
    }

    // --- Déchiffrement ---
    public byte[] Decrypt(List<long> encrypted)
    {
        List<byte> decrypted = new List<byte>();
        foreach (long c in encrypted)
        {
            long m = ModPow(c, d, n);
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

    static void Main()
    {
        RSAFile rsa = new RSAFile();

        // --- Lire fichier texte ---
        byte[] data = File.ReadAllBytes("C:\\Users\\matgr\\OneDrive\\01_Doc\\01_Cours\\02_Univ\\S3\\12_Crypto\\Exo 2 - RSA\\RSACs\\RSACs\\test.txt");

        // --- Chiffrement ---
        List<long> encrypted = rsa.Encrypt(data);

        // --- Écriture fichier chiffré ---
        using (var writer = new BinaryWriter(File.Open("fichier_chiffre.bin", FileMode.Create)))
        {
            foreach (long c in encrypted)
                writer.Write(c);
        }

        // --- Lecture fichier chiffré ---
        List<long> encryptedRead = new List<long>();
        using (var reader = new BinaryReader(File.Open("fichier_chiffre.bin", FileMode.Open)))
        {
            while (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                encryptedRead.Add(reader.ReadInt64());
            }
        }

        // --- Déchiffrement ---
        byte[] decrypted = rsa.Decrypt(encryptedRead);

        // --- Écriture fichier déchiffré ---
        File.WriteAllBytes("fichier_dechiffre.txt", decrypted);

        Console.WriteLine("Chiffrement et déchiffrement terminés !");
    }
}

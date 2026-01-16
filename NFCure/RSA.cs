namespace Cs;

using System;
using System.Collections.Generic;

public class rsa
{
    public long pgcd(long a, long b)
    {
        a = Math.Abs(a);
        b = Math.Abs(b);
        if (a == 0) return b;
        if (b == 0) return a;
        while (b != 0)
        {
            long t = a % b;
            a = b;
            b = t;
        }
        return a;
    }

    public long InverseModulaire(long e, long m)
    {
        // d = (1 + k*m) / e
        long k = 0;
        while (true)
        {
            long num = 1 + k * (long)m;
            if (num % e == 0)
            {
                long d = num / e;
                if (EstPremierEntreEux((long)d, m))
                {
                    return (long)d;
                }
            }
            k++;
        }
    }

    public bool EstPremierEntreEux(long a, long b)
    {
        return pgcd(a, b) == 1;
    }

    public bool EstPremier(long a)
    {
        if (a <= 1) return false;
        if (a == 2) return true;
        if (a % 2 == 0) return false;
        for (long i = 3; (long)i * i <= a; i += 2)
        {
            if (a % i == 0) return false;
        }
        return true;
    }

    public long PremierAleatoire(long inf, long lg)
    {
        var premiers = new List<long>();
        for (long i = inf; i < lg; i++)
        {
            if (EstPremier(i))
            {
                premiers.Add(i);
            }
        }
        if (premiers.Count > 0)
        {
            int idx = Random.Shared.Next(premiers.Count);
            return premiers[idx];
        }
        return -1;
    }

    public long PremierAleatoireAvec(long n)
    {
        var premiers = new List<long>();
        for (long i = 2; i < n; i++)
        {
            if (EstPremierEntreEux(i, n))
            {
                premiers.Add(i);
            }
        }
        if (premiers.Count > 0)
        {
            int idx = Random.Shared.Next(premiers.Count);
            return premiers[idx];
        }
        return -1;
    }

    public long ExpoModulaire(long a, long n, long m)
    {
        if (m == 1) return 0;
        if (n < 0) throw new ArgumentException("Negative exponent not supported");
        long baseVal = ((long)a % m + m) % m;
        long result = 1 % m;
        long exp = n;
        while (exp > 0)
        {
            if ((exp & 1) == 1)
            {
                result = (result * baseVal) % m;
            }
            baseVal = (baseVal * baseVal) % m;
            exp >>= 1;
        }
        return (long)result;
    }

    // codage rsa
    public long[] ChoixCle(long inf, long lg)
    {
        long p = PremierAleatoire(inf, lg);
        long q = PremierAleatoire(p + 1, p + lg + 1);
        long e = PremierAleatoireAvec((p - 1) * (q - 1));
        return new long[] { p, q, e };
    }

    public long[] ClePublic(long p, long q, long e)
    {
        long n = p * q;
        return new long[]{ n, e };
    }

    public long[] ClePrivee(long p, long q, long e)
    {
        long n = p * q;
        long d = InverseModulaire(e, (p - 1) * (q - 1));
        return new long[] { n, d };
    }

    public long CodageRSA(long M, long[] clePublique)
    {
        long n = clePublique[0];
        long e = clePublique[1];
        if (M >= n) return -1;
        return ExpoModulaire(M, e, n);
    }

    public long DecodageRSA(long M, long[] clePrivee)
    {
        long n = clePrivee[0];
        long d = clePrivee[1];
        if (M >= n) return -1;
        return ExpoModulaire(M, d, n);
    }
}
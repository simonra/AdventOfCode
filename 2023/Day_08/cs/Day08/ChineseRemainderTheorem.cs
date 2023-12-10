using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

/// <summary>
/// A solver for the chinese remainder theorem in C#
/// </summary>
public static class ChineseRemainderTheorem
{
    // https://github.com/edoannunziata/jardin/blob/master/misc/Aoc23Day8BonusRound.ipynb
    /*
        Assume a system of congruences like this

        x ≡ a0 mod m0
        x ≡ a1 mod m1
        x ≡ a2 mod m2
        ...
        x ≡ ai mod mi

        CRT requires that all the congruences have to have co-prime divisors, that is that none of the m's share factors.
        The easiest way to achieve this is by splitting up all the equations so that each m only has a (power of) a single prime factor.
        This can be done because

        x ≡ a mod (p0^n0)*(p1^n1)*...*(pi^ni)

        can be split up into

        x ≡ a mod p0^n0
        x ≡ a mod p1^n1
        ...
        x ≡ a mod pi^ni

        and we can then replace the entry with the new collection of entries in the list to process.

        Should there be entries sharing primes, they are either equivalent and all except 1 of them can be removed, or they indicate a conflict that makes the system unsolvable.
        The 2 basic contradicting cases are:
            - The exponents (n's) are the same but the remainders (a's) are different.
            - a_biggerExponent mod (p ^ smaller exponent) ≠ a_smallerExponent
    */

    public static bool TryFindSimultaneousSolution<T>(IEnumerable<(T a, T m)> congruences, out (T a, T m) result)
    where T : IBinaryInteger<T>, System.Numerics.ISignedNumber<T>
    {
        checked
        {
            var originalCongruences = congruences.ToList();

            // Convert system into factored system
            // ToDo: Extract method returning the list
            var primeFactoredCongruences = new List<(T Remainder, T Prime, T Power)>();
            foreach(var originalCongruence in originalCongruences)
            {
                var divisorsPrimeFactors = originalCongruence.m.EnumeratePrimeFactors().ToList();
                if(!divisorsPrimeFactors.Any())
                {
                    // m is prime itself
                    primeFactoredCongruences.Add((Remainder: originalCongruence.a, Prime: originalCongruence.m, Power: T.One));
                }
                else
                {
                    var groupsOfPrimeFactors = divisorsPrimeFactors.GroupBy(x => x);
                    foreach(var primeGroup in groupsOfPrimeFactors)
                    {
                        T power = T.One;
                        if(primeGroup.Count() > 1)
                        {
                            for (int i = 1; i < primeGroup.Count(); i++)
                            {
                                power += T.One;
                            }
                        }
                        primeFactoredCongruences.Add((Remainder: originalCongruence.a, Prime: primeGroup.First(), Power: power));
                    }
                }
            }
            primeFactoredCongruences = primeFactoredCongruences.OrderBy(x => x.Prime).ThenBy(x => x.Power).ThenBy(x => x.Remainder).Distinct().ToList();

            // Check collection of factored congruences for contradictions
            // ToDo: Extract method (maybe return bool for contradictions and out result for reduced collection?)
            var primegruencesByPrime = primeFactoredCongruences.GroupBy(x => x.Prime);
            foreach(var group in primegruencesByPrime.Where(g => g.Count() > 1))
            {
                var previous = group.First();
                for (int i = 1; i < group.Count(); i++)
                {
                    var current = group.Skip(i).First();
                    if(previous.Power == current.Power)
                    {
                        if(previous.Remainder != current.Remainder)
                        {
                            result = default;
                            return false;
                        }
                    }
                    // Might only need 1 check at this point given sorting, but include both branches because I'm paranoid, and it gives some safety should the code be copied alone
                    else if(current.Power > previous.Power)
                    {
                        if(current.Remainder % (previous.Prime.Pow(previous.Power)) != previous.Remainder)
                        {
                            result = default;
                            return false;
                        }
                    }
                    else if(previous.Power > current.Power)
                    {
                        if(previous.Remainder % (current.Prime.Pow(current.Power)) != current.Remainder)
                        {
                            result = default;
                            return false;
                        }
                    }
                }
            }
            var factoredValidSystem = primegruencesByPrime.Select(group => group.First()).ToList();
            // var factoredValidSystem = primeFactoredCongruences;

            // If no conflicts found, take first of each group
            var unFactoredSystem = factoredValidSystem
                .Select(x =>
                    {
                        var divisor = x.Prime.Pow(x.Power);
                        return (Remainder: x.Remainder, Divisor: divisor);
                    })
                .ToList();


            var temporaryResult = unFactoredSystem.First();
            for (int i = 1; i < unFactoredSystem.Count(); i++)
            {
                var currentCongruence = unFactoredSystem.Skip(i).First();
                var solutionForPair = T.Zero;

                // x = a1*M1*y1 + a2*M2*y2
                // x = a1*m2*(modinverse(m2,m1)) + a2*m1*modinverse(m1,m2)
                var y1 = BinaryIntegerFunctions.ModInverseFast(currentCongruence.Divisor, temporaryResult.Divisor);
                var y2 = BinaryIntegerFunctions.ModInverseFast(temporaryResult.Divisor, currentCongruence.Divisor);
                if(y1 < T.Zero || y2 < T.Zero)
                {
                    Console.WriteLine($"WARNING: Mod inverse is negative m1: {temporaryResult.Divisor}, m2 {currentCongruence.Divisor}, y1 {y1}, y2 {y2}");
                }
                solutionForPair = temporaryResult.Remainder * currentCongruence.Divisor * y1 + currentCongruence.Remainder * temporaryResult.Divisor * y2;

                // if(temporaryResult.Remainder > currentCongruence.Remainder)
                // {
                //     var remainderDiff = temporaryResult.Remainder - currentCongruence.Remainder;
                //     var modInverse = BinaryIntegerFunctions.ModInverseFast(currentCongruence.Remainder, temporaryResult.Remainder);
                //     Console.WriteLine($"remainderDiff: {remainderDiff} modInverse: {modInverse}");

                //     solutionForPair = (temporaryResult.Remainder - currentCongruence.Remainder) * BinaryIntegerFunctions.ModInverseFast(currentCongruence.Remainder, temporaryResult.Remainder) * currentCongruence.Remainder + currentCongruence.Remainder;
                // }
                // else
                // {
                //     var remainderDiff = currentCongruence.Remainder - temporaryResult.Remainder;
                //     var modInverse = BinaryIntegerFunctions.ModInverseFast(temporaryResult.Remainder, currentCongruence.Remainder);
                //     Console.WriteLine($"remainderDiff: {remainderDiff} modInverse: {modInverse}");

                //     solutionForPair = (currentCongruence.Remainder - temporaryResult.Remainder) * BinaryIntegerFunctions.ModInverseFast(temporaryResult.Remainder, currentCongruence.Remainder) * temporaryResult.Remainder + temporaryResult.Remainder;
                // }
                var nextRemainder = solutionForPair % (currentCongruence.Divisor * temporaryResult.Divisor);
                var nextDivisor = currentCongruence.Divisor * temporaryResult.Divisor;
                temporaryResult.Remainder = nextRemainder;
                temporaryResult.Divisor = nextDivisor;
            }
            // def solve_crt(a1, m1, a2, m2):
            //     k = (a2 - a1) * pow(m1, -1, m2)
            //     return m1 * k + a1

            // acc = reduced[0]
            // for a, m in reduced[1:]:
            //     acc = solve_crt(acc[0], acc[1], a, m) % (m * acc[1]), m * acc[1]


            result = (a: temporaryResult.Remainder, m: temporaryResult.Divisor);
            return true;
        }
    }

}
public static class BinaryIntegerConstants<T> where T : IBinaryInteger<T>
{
    // https://codereview.stackexchange.com/q/286070
    public static int Size    { get; }
    public static T Two       { get; }
    public static T Three     { get; }
    public static T Four      { get; }
    public static T Five      { get; }
    public static T Six       { get; }
    public static T Seven     { get; }
    public static T Eleven    { get; }
    public static T Thirteen  { get; }
    public static T Seventeen { get; }

    static BinaryIntegerConstants() {
        var size = int.CreateChecked(value: T.PopCount(value: T.AllBitsSet));

        Size      = int.CreateChecked(value: size);
        Two       = T.CreateTruncating(value: 2U);
        Three     = T.CreateTruncating(value: 3U);
        Four      = T.CreateTruncating(value: 4U);
        Five      = T.CreateTruncating(value: 5U);
        Six       = T.CreateTruncating(value: 6U);
        Seven     = T.CreateTruncating(value: 7U);
        Eleven    = T.CreateTruncating(value: 11U);
        Thirteen  = T.CreateTruncating(value: 13U);
        Seventeen = T.CreateTruncating(value: 17U);
    }
}

public static class BinaryIntegerFunctions
{
    public static T Pow<T>(this T number, T exponent)
    where T : System.Numerics.IBinaryInteger<T>
    {
        checked
        {
            var result = T.One;
            for (T i = T.Zero; i < exponent; i++)
            {
                result *= number;
            }
            return result;
        }
    }

    public static T ModInverseSafeButSlow<T>(T a, T m)
    where T : System.Numerics.IBinaryInteger<T>
    {
        checked
        {
            if (m == T.One) return T.Zero;
            T candidate = T.Zero;
            while(candidate < m)
            {
                if((a * candidate) % m == T.One)
                {
                    // break;
                    return candidate;
                }
                else if((a * (m - candidate)) % m == T.One)
                {
                    return (m - candidate);
                }
                else
                {
                    candidate += T.One;
                }
            }
            return candidate;

            // if (m == T.One) return T.Zero;
            // T a0 = a;
            // T m0 = m;
            // (T x, T y) = (T.One, T.Zero);

            // while (a > T.One) {
            //     T q = a / m;
            //     (a, m) = (m, a % m);
            //     if(x > q * y)
            //     {
            //         (x, y) = (y, x - q * y);
            //     }
            //     else
            //     {
            //         Console.WriteLine($"WARNING: When finding mod inverse of a {a0} and m {m0} q * y is greater than x, q {q} y {y} x {x}");
            //         (x, y) = (y, x + m0);
            //     }
            // }
            // return x < T.Zero ? x + m0 : x;
        }
    }

    public static T ModInverse<T>(T a, T m)
    where T : System.Numerics.IBinaryInteger<T>
    {
        checked
        {
            if (m == T.One) return T.Zero;
            T m0 = m;
            (T x, T y) = (T.One, T.Zero);

            while (a > T.One) {
                T q = a / m;
                (a, m) = (m, a % m);
                (x, y) = (y, x - q * y);
            }
            return x < T.Zero ? x + m0 : x;
        }
    }

    public static T ModInverseFast<T>(T a, T m)
    where T : System.Numerics.IBinaryInteger<T>, System.Numerics.ISignedNumber<T>
    {
        checked
        {
            var extendedEuclideanResult = ExtendedEuclideanAlgorithmGcd(a, m);
            if(extendedEuclideanResult.BezoutCoefficientS < T.Zero)
            {
                return extendedEuclideanResult.BezoutCoefficientS + m;
            }
            return extendedEuclideanResult.BezoutCoefficientS;
        }
    }

    public static (T BezoutCoefficientS, T BezoutCoefficientT, T GreatestCommonDivisor, T GcdQuotientS, T GcdQuotientT) ExtendedEuclideanAlgorithmGcd<T>(T a, T b)
    where T : System.Numerics.IBinaryInteger<T>, System.Numerics.ISignedNumber<T>
    {
        checked
        {
            // Based on https://en.wikipedia.org/wiki/Extended_Euclidean_algorithm
            T zero = a - a;
            if (a == zero || b == zero)
            {
                return (BezoutCoefficientS: zero, BezoutCoefficientT: zero, GreatestCommonDivisor: zero, GcdQuotientS: zero, GcdQuotientT: zero);
            }
            T one = a / a;

            T old_r = a;
            T r = b;
            T old_s = one;
            T s = zero;
            T old_t = zero;
            T t = one;
            T quotient = zero;
            T temp = zero;
            while (r != zero)
            {
                quotient = old_r / r; // integer division! Make sure to floor if using floats. Maybe consider Math.DivRem() in dotnet
                temp = r;
                r = old_r - quotient * r;
                old_r = temp;
                temp = s;
                s = old_s - quotient * s;
                old_s = temp;
                temp = t;
                t = old_t - quotient * t;
                old_t = temp;
            }
            return (BezoutCoefficientS: old_s, BezoutCoefficientT: old_t, GreatestCommonDivisor: old_r, GcdQuotientS: s, GcdQuotientT: t);
        }
    }

    public static IEnumerable<T> EnumeratePrimeFactors<T>(this T value) where T : IBinaryInteger<T>/*, IUnsignedNumber<T>*/ {
        checked
        {
            // Based on https://stackoverflow.com/a/76691571
            // Further reading https://en.wikipedia.org/wiki/Wheel_factorization
            if (BinaryIntegerConstants<T>.Four > value) { yield break; }
            if (BinaryIntegerConstants<T>.Five == value) { yield break; }
            if (BinaryIntegerConstants<T>.Seven == value) { yield break; }
            if (BinaryIntegerConstants<T>.Eleven == value) { yield break; }
            if (BinaryIntegerConstants<T>.Thirteen == value) { yield break; }

            var index = value;

            while (T.Zero == (index & T.One)/* enumerate factors of 2 */) {
                yield return BinaryIntegerConstants<T>.Two;

                index >>= 1;
            }
            while (T.Zero == (index % BinaryIntegerConstants<T>.Three)) { // enumerate factors of 3
                yield return BinaryIntegerConstants<T>.Three;

                index /= BinaryIntegerConstants<T>.Three;
            }
            while (T.Zero == (index % BinaryIntegerConstants<T>.Five)/* enumerate factors of 5 */) {
                yield return BinaryIntegerConstants<T>.Five;

                index /= BinaryIntegerConstants<T>.Five;
            }
            while (T.Zero == (index % BinaryIntegerConstants<T>.Seven)/* enumerate factors of 7 */) {
                yield return BinaryIntegerConstants<T>.Seven;

                index /= BinaryIntegerConstants<T>.Seven;
            }
            while (T.Zero == (index % BinaryIntegerConstants<T>.Eleven)/* enumerate factors of 11 */) {
                yield return BinaryIntegerConstants<T>.Eleven;

                index /= BinaryIntegerConstants<T>.Eleven;
            }
            while (T.Zero == (index % BinaryIntegerConstants<T>.Thirteen)/* enumerate factors of 13 */) {
                yield return BinaryIntegerConstants<T>.Thirteen;

                index /= BinaryIntegerConstants<T>.Thirteen;
            }

            var factor = BinaryIntegerConstants<T>.Seventeen;
            var limit = index.SquareRoot();

            if (factor <= limit) {
                do {
                    while (T.Zero == (index % factor)/* enumerate factors of (30k - 13) */) {
                        yield return factor;

                        index /= factor;
                    }

                    factor += BinaryIntegerConstants<T>.Two;

                    while (T.Zero == (index % factor)/* enumerate factors of (30k - 11) */) {
                        yield return factor;

                        index /= factor;
                    }

                    factor += BinaryIntegerConstants<T>.Four;

                    while (T.Zero == (index % factor)/* enumerate factors of (30k - 7) */) {
                        yield return factor;

                        index /= factor;
                    }

                    factor += BinaryIntegerConstants<T>.Six;

                    while (T.Zero == (index % factor)/* enumerate factors of (30k - 1) */) {
                        yield return factor;

                        index /= factor;
                    }

                    factor += BinaryIntegerConstants<T>.Two;

                    while (T.Zero == (index % factor)/* enumerate factors of (30k + 1) */) {
                        yield return factor;

                        index /= factor;
                    }

                    factor += BinaryIntegerConstants<T>.Six;

                    while (T.Zero == (index % factor)/* enumerate factors of (30k + 7) */) {
                        yield return factor;

                        index /= factor;
                    }

                    factor += BinaryIntegerConstants<T>.Four;

                    while (T.Zero == (index % factor)/* enumerate factors of (30k + 11) */) {
                        yield return factor;

                        index /= factor;
                    }

                    factor += BinaryIntegerConstants<T>.Two;

                    while (T.Zero == (index % factor)/* enumerate factors of (30k + 13) */) {
                        yield return factor;

                        index /= factor;
                    }

                    factor += BinaryIntegerConstants<T>.Four;
                    limit = index.SquareRoot();
                } while (factor <= limit);
            }

            if ((index != T.One) && (index != value)) {
                yield return index;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static T As<T>(this bool value) where T : IBinaryInteger<T> =>
        T.CreateTruncating(value: Unsafe.As<bool, byte>(source: ref value));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static T IsGreaterThan<T>(this T value, T other) where T : IBinaryInteger<T> =>
        (value > other).As<T>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T MostSignificantBit<T>(this T value) where T : IBinaryInteger<T> =>
        (T.CreateTruncating(value: BinaryIntegerConstants<T>.Size) - T.LeadingZeroCount(value: value));

    public static T SquareRoot<T>(this T value) where T : IBinaryInteger<T>/*, IUnsignedNumber<T> */{
        return BinaryIntegerConstants<T>.Size switch {
#if !FORCE_SOFTWARE_SQRT
            8 => T.CreateTruncating(value: ((uint)MathF.Sqrt(x: uint.CreateTruncating(value: value)))),
            16 => T.CreateTruncating(value: ((uint)MathF.Sqrt(x: uint.CreateTruncating(value: value)))),
            32 => T.CreateTruncating(value: ((uint)Math.Sqrt(d: uint.CreateTruncating(value: value)))),
            64 => T.CreateTruncating(value: Sqrt(value: ulong.CreateTruncating(value: value))),
#endif
            _ => SoftwareImplementation(value: value),
        };

        /*
            Credit goes to njuffa for providing a reference implementation:
                https://stackoverflow.com/a/31149161/1186165

            Notes:
                - This implementation of the algorithm runs in constant time, based on the size of T.
                - Ignoring the loop that is entered when the size of T exceeds 64, all branches get eliminated during JIT compilation.
        */
        static T SoftwareImplementation(T value) {
            var msb = int.CreateTruncating(value: value.MostSignificantBit());
            var msbIsOdd = (msb & 1);
            var m = ((msb + 1) >> 1);
            var mMinusOne = (m - 1);
            var mPlusOne = (m + 1);
            var x = (T.One << mMinusOne);
            var y = (x - (value >> (mPlusOne - msbIsOdd)));
            var z = y;

            x += x;

            if (BinaryIntegerConstants<T>.Size > 8) {
                y = (((y * y) >> mPlusOne) + z);
                y = (((y * y) >> mPlusOne) + z);
            }

            if (BinaryIntegerConstants<T>.Size > 16) {
                y = (((y * y) >> mPlusOne) + z);
                y = (((y * y) >> mPlusOne) + z);
                y = (((y * y) >> mPlusOne) + z);
                y = (((y * y) >> mPlusOne) + z);
            }

            if (BinaryIntegerConstants<T>.Size > 32) {
                y = (((y * y) >> mPlusOne) + z);
                y = (((y * y) >> mPlusOne) + z);
                y = (((y * y) >> mPlusOne) + z);
                y = (((y * y) >> mPlusOne) + z);
                y = (((y * y) >> mPlusOne) + z);
                y = (((y * y) >> mPlusOne) + z);
                y = (((y * y) >> mPlusOne) + z);
                y = (((y * y) >> mPlusOne) + z);
            }

            if (BinaryIntegerConstants<T>.Size > 64) {
                var i = T.CreateTruncating(value: (BinaryIntegerConstants<T>.Size >> 3));

                do {
                    i -= (T.One << 3);
                    y = (((y * y) >> mPlusOne) + z);
                    y = (((y * y) >> mPlusOne) + z);
                    y = (((y * y) >> mPlusOne) + z);
                    y = (((y * y) >> mPlusOne) + z);
                    y = (((y * y) >> mPlusOne) + z);
                    y = (((y * y) >> mPlusOne) + z);
                    y = (((y * y) >> mPlusOne) + z);
                    y = (((y * y) >> mPlusOne) + z);
                } while (i != T.Zero);
            }

            y = (x - y);
            x = T.CreateTruncating(value: msbIsOdd);
            y -= BinaryIntegerConstants<T>.Size switch {
                8 => (x * ((y * T.CreateChecked(value: 5UL)) >> 4)),
                16 => (x * ((y * T.CreateChecked(value: 75UL)) >> 8)),
                32 => (x * ((y * T.CreateChecked(value: 19195UL)) >> 16)),
                64 => (x * ((y * T.CreateChecked(value: 1257966796UL)) >> 32)),
                128 => (x * ((y * T.CreateChecked(value: 5402926248376769403UL)) >> 64)),
                _ => throw new NotSupportedException(), // TODO: Research a way to calculate the proper constant at runtime.
            };
            x = (T.One << (BinaryIntegerConstants<T>.Size - 1));
            y -= (value - (y * y)).IsGreaterThan(other: x);

            if (BinaryIntegerConstants<T>.Size > 8) {
                y -= (value - (y * y)).IsGreaterThan(other: x);
                y -= (value - (y * y)).IsGreaterThan(other: x);
            }

            if (BinaryIntegerConstants<T>.Size > 32) {
                y -= (value - (y * y)).IsGreaterThan(other: x);
                y -= (value - (y * y)).IsGreaterThan(other: x);
                y -= (value - (y * y)).IsGreaterThan(other: x);
            }

            return (y & (T.AllBitsSet >> 1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint Sqrt(ulong value) {
            var x = ((uint)Math.Sqrt(d: unchecked((long)value)));
            var y = (unchecked(((ulong)x) * x) > value).As<uint>(); // ((x * x) > value) ? 1 : 0
            var z = ((uint)(value >> 63)); // (64 == value.MostSignificantBit()) ? 1 : 0

            return unchecked(x - (y | z));
        }
    }
}

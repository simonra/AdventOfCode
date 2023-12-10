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
    /*
        Assume a system of congruences like this

        x ≡ a0 mod m0
        x ≡ a1 mod m1
        x ≡ a2 mod m2
        ...
        x ≡ ai mod mi

        The Chinese Remainder Theorem (CRT) requires that all the congruences have to have co-prime divisors, that is that none of the m's share factors.
        In a valid system, the easiest way to achieve this is by splitting up all the equations, so that each m only has a (power of) a single prime factor,
        and removing all the entries that share the same prime, leaving only one congruence per prime.

        This can be done because

        x ≡ a mod (p0^e0)*(p1^e1)*...*(pi^ei)

        which can be split up into

        x ≡ a mod p0^e0
        x ≡ a mod p1^e1
        ...
        x ≡ a mod pi^ei

        and we can then replace the entry with the new collection of entries in the list to process.

        Should there be entries sharing primes, they are either equivalent and all except 1 of them can be removed,
        or they indicate a conflict that makes the system unsolvable.
        The 2 basic contradicting cases are:
            - The exponents (e's) are the same but the remainders (a's) are different.
            - a_biggerExponent mod (p ^ smaller exponent) ≠ a_smallerExponent

        After we've ascertained that there are no conflicts, and have removed all duplicates, we can pairwise reduce the set until
        we are left with 1 congruence, which will be our solution.

        To reduce the solution pairwise, use the general solution
            x = a0*M0*y0 + a1*M1*y1 + a2*M2*y2 + ... ai*Mi*yi
        where
            Mx is the product of all the m's except the x'th one (e.g. for x ≡ a0 mod m0 it is m1*m2*...mi, and for x ≡ a1 mod m1 it would be m0*m2*...*mi).\
            yx is the modular inverse of Mx (be careful that this is the capital M)

        The solution for a pair
            x ≡ a0 mod m0
            x ≡ a1 mod m1
        yields
            M0 = m1
            M1 = m0
            y0 = the inverse for m1 mod m0
            y1 = the inverse for m0 mod m1
        and becomes
            x = a0 * m1 *(Inverse(m1 mod m0)) + a1 * m0* Inverse(m0 mod m1)

        We can further use this to construct the next congruence
            x_next ≡ (x mod (m0 * m1)) mod (m0 * m1)
        which we use for the pair
            x_next ≡ (x mod (m0 * m1)) mod (m0 * m1)
            x ≡ a2 mod m2
        all the way up until we have reduced all the congruences in the system.

        Because some use cases have need for the both the solution divisor in addition to, or instead of, the remainder, return both and let the user deal with it.
    */

    public static bool TryFindSimultaneousSolution<T>(IEnumerable<(T a, T m)> congruences, out (T a, T m) result)
    where T : IBinaryInteger<T>, System.Numerics.ISignedNumber<T>
    {
        checked // Throws if number overflow/underflow. Explicit, because I will copy paste it and forget that it can (should in the contexts I use it) be set in project settings
        {
            List<(T Remainder, T Prime, T Power)> primeFactoredCongruences = MakePrimeFactoredSystemOfCongruences(congruences);

            if(!SystemOfPrimeFactoredCongruencesIsValid(primeFactoredCongruences))
            {
                result = default;
                return false;
            }

            // Beware that there is some double work here, because the grouping is also done in the function checking validity.
            // However, I think it's worth it for the readability.
            // The selection of only the first member of the group is because all the others in the group are [co-prime](https://en.wikipedia.org/wiki/Coprime_integers), and are therefore redundant with regards to the solution.
            List<(T Remainder, T Divisor)> reducedSystemToSolve = primeFactoredCongruences
                .GroupBy(x => x.Prime)
                .Select(group => group.First())
                .Select(x =>
                    {
                        var divisor = x.Prime.Pow(x.Power);
                        return (Remainder: x.Remainder, Divisor: divisor);
                    })
                .ToList();


            // ToDo: Can this be prettier with a fold?
            // var temporaryResult = reducedSystemToSolve.First();
            // var solutionForPair = T.Zero;
            // for (int i = 1; i < reducedSystemToSolve.Count(); i++)
            // {
            //     var currentCongruence = reducedSystemToSolve.Skip(i).First();

            //     // x = a1*M1*y1 + a2*M2*y2
            //     // x = a1*m2*(modinverse(m2,m1)) + a2*m1*modinverse(m1,m2)
            //     var y1 = BinaryIntegerHelperFunctions.ModInverseFast(currentCongruence.Divisor, temporaryResult.Divisor);
            //     var y2 = BinaryIntegerHelperFunctions.ModInverseFast(temporaryResult.Divisor, currentCongruence.Divisor);

            //     solutionForPair = temporaryResult.Remainder * currentCongruence.Divisor * y1 + currentCongruence.Remainder * temporaryResult.Divisor * y2;

            //     var nextRemainder = solutionForPair % (currentCongruence.Divisor * temporaryResult.Divisor);
            //     var nextDivisor = currentCongruence.Divisor * temporaryResult.Divisor;

            //     temporaryResult.Remainder = nextRemainder;
            //     temporaryResult.Divisor = nextDivisor;
            // }

            var solution = reducedSystemToSolve.Aggregate(SolveCongruencePair);
            result = (a: solution.Remainder, m: solution.Divisor);

            // result = (a: temporaryResult.Remainder, m: temporaryResult.Divisor);
            return true;
        }
    }

    private static List<(T Remainder, T Prime, T Power)> MakePrimeFactoredSystemOfCongruences<T>(IEnumerable<(T Remainder, T Divisor)> congruences)
    where T : IBinaryInteger<T>
    {
        checked // Throws if number overflow/underflow. Explicit, because I will copy paste it and forget that it can (should in the contexts I use it) be set in project settings
        {
            var originalCongruences = congruences.ToList();

            // Convert system into factored system
            var primeFactoredCongruences = new List<(T Remainder, T Prime, T Power)>();
            foreach(var originalCongruence in originalCongruences)
            {
                var divisorsPrimeFactors = originalCongruence.Divisor.EnumeratePrimeFactors().ToList();
                if(!divisorsPrimeFactors.Any())
                {
                    // m is prime itself
                    primeFactoredCongruences.Add((Remainder: originalCongruence.Remainder, Prime: originalCongruence.Divisor, Power: T.One));
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
                        primeFactoredCongruences.Add((Remainder: originalCongruence.Remainder, Prime: primeGroup.First(), Power: power));
                    }
                }
            }
            // Congruences in system that share the same unique prime factor are redundant, use distinct here to remove any obvious duplicates
            primeFactoredCongruences = primeFactoredCongruences.OrderBy(x => x.Prime).ThenBy(x => x.Power).ThenBy(x => x.Remainder).Distinct().ToList();
            return primeFactoredCongruences;
        }
    }

    private static bool SystemOfPrimeFactoredCongruencesIsValid<T>(IEnumerable<(T Remainder, T Prime, T Power)> congruences)
    where T : IBinaryInteger<T>
    {
        checked
        {
            var congruencesByPrime = congruences.ToList().GroupBy(x => x.Prime);
            foreach(var group in congruencesByPrime.Where(g => g.Count() > 1))
            {
                var previous = group.First();
                for (int i = 1; i < group.Count(); i++)
                {
                    var current = group.Skip(i).First();
                    if(previous.Power == current.Power)
                    {
                        if(previous.Remainder != current.Remainder)
                        {
                            return false;
                        }
                    }
                    // Might only need 1 check at this point given sorting, but include both branches because I'm paranoid, and it gives some safety should the code be copied out alone
                    else if(current.Power > previous.Power)
                    {
                        if(current.Remainder % (previous.Prime.Pow(previous.Power)) != previous.Remainder)
                        {
                            return false;
                        }
                    }
                    else if(previous.Power > current.Power)
                    {
                        if(previous.Remainder % (current.Prime.Pow(current.Power)) != current.Remainder)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }

    private static (T Remainder, T Divisor) SolveCongruencePair<T>((T Remainder, T Divisor) first, (T Remainder, T Divisor) second)
    where T : System.Numerics.IBinaryInteger<T>, System.Numerics.ISignedNumber<T>
    {
        var y1 = BinaryIntegerHelperFunctions.ModInverseFast(second.Divisor, first.Divisor);
        var y2 = BinaryIntegerHelperFunctions.ModInverseFast(first.Divisor, second.Divisor);

        var solutionForPairsX = first.Remainder * second.Divisor * y1 + second.Remainder * first.Divisor * y2;

        var solutionRemainder = solutionForPairsX % (second.Divisor * first.Divisor);
        var solutionDivisor = second.Divisor * first.Divisor;

        return (Remainder: solutionRemainder, Divisor: solutionDivisor);
    }
}

public static class BinaryIntegerHelperFunctions
{
    public static T Pow<T>(this T number, T exponent)
    where T : System.Numerics.IBinaryInteger<T>
    {
        checked // Throws if number overflow/underflow. Explicit, because I will copy paste it and forget that it can (should in the contexts I use it) be set in project settings
        {
            var result = T.One;
            for (T i = T.Zero; i < exponent; i++)
            {
                result *= number;
            }
            return result;
        }
    }

    public static T ModInverseFast<T>(T a, T m)
    where T : System.Numerics.IBinaryInteger<T>, System.Numerics.ISignedNumber<T>
    {
        checked // Throws if number overflow/underflow. Explicit, because I will copy paste it and forget that it can (should in the contexts I use it) be set in project settings
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
        checked // Throws if number overflow/underflow. Explicit, because I will copy paste it and forget that it can (should in the contexts I use it) be set in project settings
        {
            // Based on https://en.wikipedia.org/wiki/Extended_Euclidean_algorithm

            (T old_r, T r) = (a, b);
            (T old_s, T s) = (T.One, T.Zero);
            (T old_t, T t) = (T.Zero, T.One);
            T quotient = T.Zero;
            while (r != T.Zero)
            {
                quotient = old_r / r; // integer division! Make sure to floor if using floats. Maybe consider Math.DivRem() in dotnet
                (old_r, r) = (r, old_r - quotient * r);
                (old_s, s) = (s, old_s - quotient * s);
                (old_t, t) = (t, old_t - quotient * t);
            }
            return (BezoutCoefficientS: old_s, BezoutCoefficientT: old_t, GreatestCommonDivisor: old_r, GcdQuotientS: s, GcdQuotientT: t);
        }
    }

    public static IEnumerable<T> EnumeratePrimeFactors<T>(this T value) where T : IBinaryInteger<T>/*, IUnsignedNumber<T>*/ {
        checked // Throws if number overflow/underflow. Explicit, because I will copy paste it and forget that it can (should in the contexts I use it) be set in project settings
        {
            // Based on the answer Kittoes0124 gave here https://stackoverflow.com/a/76691571
            // Fleshed out with hints from his question here https://codereview.stackexchange.com/q/286070
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

public static class BinaryIntegerConstants<T> where T : IBinaryInteger<T>
{
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

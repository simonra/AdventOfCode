
public static class LeastCommonMultiple
{
    public static T FindLeastCommonMultipleForPair<T>(T a, T b)
    where T : System.Numerics.IBinaryInteger<T>, System.Numerics.ISignedNumber<T>
    {
        checked // Throws if number overflow/underflow. Explicit, because I will copy paste it and forget that it can (should in the contexts I use it) be set in project settings
        {
            var greatestCommonDivisor = ExtendedEuclideanAlgorithmGcd(a, b).GreatestCommonDivisor;
            if(greatestCommonDivisor == T.Zero)
            {
                return T.Zero;
            }
            return (a / greatestCommonDivisor) * b;
        }
    }

    public static T FindLeastCommonMultipleOfCollection<T>(this IEnumerable<T> numbers)
    where T : System.Numerics.IBinaryInteger<T>, System.Numerics.ISignedNumber<T>
    {
        return numbers.Aggregate(FindLeastCommonMultipleForPair);
    }

    private static (T BezoutCoefficientS, T BezoutCoefficientT, T GreatestCommonDivisor, T GcdQuotientS, T GcdQuotientT) ExtendedEuclideanAlgorithmGcd<T>(T a, T b)
    where T : System.Numerics.IBinaryInteger<T>, System.Numerics.ISignedNumber<T>
    {
        checked // Throws if number overflow/underflow. Explicit, because I will copy paste it and forget that it can (should in the contexts I use it) be set in project settings
        {
            // Based on https://en.wikipedia.org/wiki/Extended_Euclidean_algorithm
            T zero = a - a;
            if (a == zero || b == zero)
            {
                return (BezoutCoefficientS: zero, BezoutCoefficientT: zero, GreatestCommonDivisor: zero, GcdQuotientS: zero, GcdQuotientT: zero);
            }
            T one = a / a;

            (T old_r, T r) = (a, b);
            (T old_s, T s) = (T.One, T.Zero);
            (T old_t, T t) = (T.Zero, T.One);
            T quotient = T.Zero;
            while (r != zero)
            {
                quotient = old_r / r; // integer division! Make sure to floor if using floats. Maybe consider Math.DivRem() in dotnet
                (old_r, r) = (r, old_r - quotient * r);
                (old_s, s) = (s, old_s - quotient * s);
                (old_t, t) = (t, old_t - quotient * t);
            }
            return (BezoutCoefficientS: old_s, BezoutCoefficientT: old_t, GreatestCommonDivisor: old_r, GcdQuotientS: s, GcdQuotientT: t);
        }
    }
}

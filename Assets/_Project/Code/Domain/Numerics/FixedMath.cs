using System;

namespace RapWay.Domain.Numerics
{
    public static class FixedMath
    {
        public static long MultiplyDivide(
            long value,
            long multiplier,
            long divisor,
            FixedRounding rounding = FixedRounding.TowardZero)
        {
            if (divisor <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(divisor), "The divisor must be positive.");
            }

            if (!Enum.IsDefined(typeof(FixedRounding), rounding))
            {
                throw new ArgumentOutOfRangeException(nameof(rounding));
            }

            long quotient = value / divisor;
            long remainder = value % divisor;
            long scaledQuotient = checked(quotient * multiplier);
            long remainderProduct = checked(remainder * multiplier);
            long result = checked(scaledQuotient + remainderProduct / divisor);
            long residual = remainderProduct % divisor;

            if (residual == 0 || rounding == FixedRounding.TowardZero)
            {
                return result;
            }

            long direction = residual > 0 ? 1L : -1L;
            if (rounding == FixedRounding.AwayFromZero)
            {
                return checked(result + direction);
            }

            long absoluteResidual = Math.Abs(residual);
            return absoluteResidual >= divisor - absoluteResidual
                ? checked(result + direction)
                : result;
        }
    }
}

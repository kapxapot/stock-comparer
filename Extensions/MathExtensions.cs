using System;

namespace StockComparer.Extensions
{
    public static class MathExtensions
    {
        public static decimal Delta(this decimal baseValue, decimal changedValue)
        {
            if (baseValue == 0)
            {
                return 0;
            }

            var absBaseValue = Math.Abs(baseValue);

            return (decimal)((double)(changedValue - baseValue) / (double)absBaseValue);
        }

        public static decimal ToPercent(this decimal value) => value * 100;
    }
}

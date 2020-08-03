using System;
using System.Text.Json;

namespace StockComparer.Extensions
{
    public static class JsonExtensions
    {
        public static bool IsUndefined(this JsonElement element) =>
            element.ValueKind == JsonValueKind.Undefined;

        public static bool IsObject(this JsonElement element) =>
            element.ValueKind == JsonValueKind.Object;

        public static decimal ToDecimal(this JsonElement element)
        {
            if (element.ValueKind == JsonValueKind.Number)
            {
                return element.GetDecimal();
            }

            if (element.ValueKind == JsonValueKind.String)
            {
                var str = element.ToString();

                if (decimal.TryParse(str, out var value))
                {
                    return value;
                }

                throw new Exception($"Failed to convert string to decimal: {str}");
            }

            throw new Exception("Invalid element type, number or string expected.");
        }

        public static int ToInt(this JsonElement element)
        {
            if (element.ValueKind == JsonValueKind.Number)
            {
                return element.GetInt32();
            }

            if (element.ValueKind == JsonValueKind.String)
            {
                var str = element.ToString();

                if (int.TryParse(str, out var value))
                {
                    return value;
                }

                throw new Exception($"Failed to convert string to int: {str}");
            }

            throw new Exception("Invalid element type, number or string expected.");
        }
    }
}

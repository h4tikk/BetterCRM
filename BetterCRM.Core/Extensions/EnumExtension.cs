namespace BetterCRM.Core.Extensions
{
    public static class EnumExtensions
    {
        public static T ToEnum<T>(this string value) where T : struct, Enum
        {
            if (Enum.TryParse<T>(value, ignoreCase: true, out var result))
                return result;

            throw new InvalidOperationException(
                $"Значение '{value}' не является допустимым для {typeof(T).Name}. " +
                $"Допустимые: {string.Join(", ", Enum.GetNames<T>())}");
        }

        public static string ToDbString<T>(this T value) where T : struct, Enum
            => value.ToString();
    }
}
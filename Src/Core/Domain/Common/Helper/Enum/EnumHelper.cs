using System.Reflection;
using System.Runtime.Serialization;

namespace Domain.Common.Helper.Enum;

public static class EnumHelper
{
    /// <summary>
    /// Gets description from value
    /// </summary>
    /// <param name="value">Enum value</param>
    /// <typeparam name="T">Enum</typeparam>
    /// <returns>Description string</returns>
    public static string GetDescriptionFromEnumValue<T>(T value)
    {
        EnumMemberAttribute attribute = value.GetType()
            .GetField(value.ToString())
            .GetCustomAttributes(typeof(EnumMemberAttribute), false)
            .SingleOrDefault() as EnumMemberAttribute ?? throw new InvalidOperationException("Attribute was null");

        return attribute.Value ?? value.ToString();
    }

    /// <summary>
    /// Get enum value from description
    /// </summary>
    /// <param name="description">description string</param>
    /// <typeparam name="T">Enum</typeparam>
    /// <returns>Enum value</returns>
    /// <exception cref="ArgumentException">If type is not enum</exception>
    public static T GetEnumValueFromDescription<T>(string description)
    {
        var type = typeof(T);

        if (!type.IsEnum)
        {
            throw new ArgumentException("Type is not in enum");
        }

        FieldInfo[] fields = type.GetFields();
        var field = fields
            .SelectMany(f => f.GetCustomAttributes(typeof(EnumMemberAttribute), false),
                (field, attribute) => new { Field = field, Att = attribute })
            .SingleOrDefault(attribute => ((EnumMemberAttribute)attribute.Att).Value == description);

        return field == null ? default(T) : (T)field.Field.GetRawConstantValue();
    }
}
using System.Reflection;
using System.Runtime.Serialization;

namespace Domain.Common.Helper.Enum
{
    public class EnumHelper
    {
        public static string GetDescriptionFromEnumValue<T>(T value)
        {
            EnumMemberAttribute attribute = value.GetType()
                .GetField(value.ToString())
                .GetCustomAttributes(typeof(EnumMemberAttribute), false)
                .SingleOrDefault() as EnumMemberAttribute;

            return attribute == null ? value.ToString() : attribute.Value;
        }

        public static T GetEnumValueFromDescription<T>(string description)
        {
            var type = typeof(T);

            if (!type.IsEnum)
            {
                throw new ArgumentException();
            }

            FieldInfo[] fields = type.GetFields();
            var field = fields
                .SelectMany(f => f.GetCustomAttributes(typeof(EnumMemberAttribute), false),
                (field, attribute) => new { Field = field, Att = attribute })
                .Where(attribute => ((EnumMemberAttribute)attribute.Att).Value == description)
                .SingleOrDefault();

            return field == null ? default(T) : (T)field.Field.GetRawConstantValue();
        }
    }
}

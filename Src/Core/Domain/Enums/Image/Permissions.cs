using System.Runtime.Serialization;

namespace Domain.Enums.Image
{
    /// <summary>
    /// Enum Permissions
    /// </summary>
    [DataContract]
    public enum Permissions
    {
        [EnumMember(Value = "full")]
        full = 0,
        [EnumMember(Value = "readwrite")]
        readwrite = 1,
        [EnumMember(Value = "read")]
        read = 2,
        [EnumMember(Value = "write")]
        write = 3
    }
}

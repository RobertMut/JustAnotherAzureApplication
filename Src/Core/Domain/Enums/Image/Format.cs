using System.Runtime.Serialization;

namespace Domain.Enums.Image
{
    /// <summary>
    /// Enum Format
    /// </summary>
    [DataContract]
    public enum Format
    {
        [EnumMember(Value = "png")]
        png,
        [EnumMember(Value = "jpeg")]
        jpeg,
        [EnumMember(Value = "jpg")]
        jpg,
        [EnumMember(Value = "bmp")]
        bmp,
        [EnumMember(Value = "tiff")]
        tiff,
        [EnumMember(Value = "gif")]
        gif
    }
}

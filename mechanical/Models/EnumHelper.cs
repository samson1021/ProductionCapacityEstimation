using System.ComponentModel.DataAnnotations;
using System.Data;

namespace mechanical.Models
{
    public static class EnumHelper
    {
        public static string GetEnumDisplayName(System.Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var displayAttribute = field.GetCustomAttributes(typeof(DisplayAttribute), false) as DisplayAttribute[];

            return displayAttribute.Length > 0 ? displayAttribute[0].Name : value.ToString();
        }
    }
}

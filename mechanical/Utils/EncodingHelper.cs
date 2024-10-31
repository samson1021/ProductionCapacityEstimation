// using Ganss.XSS;
using System.Reflection;
using System.Text.Encodings.Web;

namespace mechanical.Utils
{
    public static class EncodingHelper
    {
        public static string EncodeInput(string input)
        {
            return HtmlEncoder.Default.Encode(input);
        }

        public static void EncodeObject<T>(T model)
        {
            // Get all string properties of the model
            var stringProperties = typeof(T)
                                        .GetProperties()
                                        .Where(prop => prop.PropertyType == typeof(string) && prop.CanWrite && prop.CanRead);

            foreach (var property in stringProperties)
            {
                var originalValue = (string)property.GetValue(model);
                if (!string.IsNullOrEmpty(originalValue))
                {
                    var encodedValue = EncodeInput(originalValue);
                    property.SetValue(model, encodedValue);
                }
            }
        }
    }

    // public static class EncodingHelper
    // {
    //     public static void EncodeObject<T>(T model)
    //     {
    //         var encoder = HtmlEncoder.Default;

    //         // Get all string properties of the model
    //         var stringProperties = typeof(T)
    //                                     .GetProperties()
    //                                     .Where(prop => prop.PropertyType == typeof(string) && prop.CanWrite && prop.CanRead);

    //         foreach (var property in stringProperties)
    //         {
    //             var originalValue = (string)property.GetValue(model);
    //             if (!string.IsNullOrEmpty(originalValue))
    //             {
    //                 var encodedValue = encoder.Encode(originalValue);
    //                 property.SetValue(model, encodedValue);
    //             }
    //         }
    //     }
    // }

    // public static class SanitizerHelper
    // {
    //     public static void SanitizeObject<T>(T model)
    //     {
    //         var sanitizer = new HtmlSanitizer();

    //         // Get all string properties of the model
    //         var stringProperties = typeof(T)
    //                                     .GetProperties()
    //                                     .Where(prop => prop.PropertyType == typeof(string) && prop.CanWrite && prop.CanRead);

    //         foreach (var property in stringProperties)
    //         {
    //             var originalValue = (string)property.GetValue(model);
    //             if (!string.IsNullOrEmpty(originalValue))
    //             {
    //                 var sanitizedValue = sanitizer.Sanitize(originalValue);
    //                 property.SetValue(model, sanitizedValue);
    //             }
    //         }
    //     }
    // }
}
﻿// <auto-generated />
#nullable enable

namespace Testing.Enums
{
    /// <summary>
    /// Description-related extension methods for <see cref="global::Testing.Enums.Country"/>.
    /// </summary>
    public static class CountryEnumDescriptionExtensions
    {
        // TODO: Some sort of flag enum could be passed to the EnumDescriptionAttribute, which could
        // modify the behavior of the generated methods, as well as their documentation comments.
        // Example: rather than returning string.Empty enum members with no DescriptionAttributes
        // the generator could get the string value of the enum member itself, using ToString(), and return that instead.

        /// <summary>
        /// Returns the description for the provided <paramref name="value" />,
        /// or <c>string.Empty</c> if no description exists.
        /// </summary>
        /// <param name="value">The <see cref="global::Testing.Enums.Country" /> value.</param>
        /// <returns>The <c>string</c> description.</returns>
        public static string GetDescription(this global::Testing.Enums.Country value)
        {
            return value switch
            {
                global::Testing.Enums.Country.US => "United States",
                global::Testing.Enums.Country.UK => "United Kingdom",
                _ => string.Empty
            };
        }

        /// <summary>
        /// Returns the <see cref="global::Testing.Enums.Country" /> value whose description matches the provided <paramref name="description" />,
        /// or <c>null</c> if no match is found.
        /// </summary>
        /// <param name="_"></param>
        /// <param name="description">The <c>string</c> description of the enum.</param>
        /// <returns>The matching <see cref="global::Testing.Enums.Country" /> value, or <c>null</c>.</returns>
        // public static Testing.Enums.Country? GetValueFromDescription<T>(this string description)
        //     where T : Enum, global::Testing.Enums.Country
        // {
        //     return description switch
        //     {
        //         "United States" => global::Testing.Enums.Country.US,
        //         "United Kingdom" => global::Testing.Enums.Country.UK,
        //         _ => null
        //     };
        // }

        // Boxing :(
        public static global::System.Enum? GetTestingEnumsCountryValueFromDescription(string description)
        {
            return description switch
            {
                "United States" => global::Testing.Enums.Country.US,
                "United Kingdom" => global::Testing.Enums.Country.UK,
                _ => default(global::Testing.Enums.Country?)
            };
        }
    }
}

namespace global
{
    public static partial class EnumDescriptionValueExtensions
    {
        private static readonly global::System.Collections.Generic.Dictionary<string, global::System.Func<string, global::System.Enum?>> GetValueFromDescriptionMethods = new(global::System.StringComparer.Ordinal);
        
        static EnumDescriptionValueExtensions()
        {
            var testingEnumsCountryTypeFullName = typeof(global::Testing.Enums.Country)?.FullName;
            if (testingEnumsCountryTypeFullName is null)
            {
                throw new global::System.InvalidOperationException($"The name of the enum is null.");
            }
            GetValueFromDescriptionMethods.Add("Testing.Enums.Country", Testing.Enums.CountryEnumDescriptionExtensions.GetTestingEnumsCountryValueFromDescription);
        }

        public static T? GetValueFromDescription<T>(this string description) where T : Enum
        {
            var typeName = typeof(T)?.FullName ?? string.Empty;

            if (GetValueFromDescriptionMethods.TryGetValue(typeName, out var getValueFromDescriptionMethod))
            {
                return (T?)getValueFromDescriptionMethod(description);
            }

            return default(T?);
        }
    }
}
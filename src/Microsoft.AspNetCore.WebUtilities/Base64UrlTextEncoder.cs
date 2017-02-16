// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Primitives;

namespace Microsoft.AspNetCore.WebUtilities
{
    public static class Base64UrlTextEncoder
    {
        /// <summary>
        /// Encodes supplied data into Base64 and replaces any URL encodable characters into non-URL encodable
        /// characters.
        /// </summary>
        /// <param name="data">Data to be encoded.</param>
        /// <returns>Base64 encoded string modified with non-URL encodable characters</returns>
        public static string Encode(byte[] data)
        {
            var encodedValue = Convert.ToBase64String(data);
            EncodeInternal(encodedValue);
            return encodedValue;
        }

        /// <summary>
        /// Decodes supplied string by replacing the non-URL encodable characters with URL encodable characters and
        /// then decodes the Base64 string.
        /// </summary>
        /// <param name="text">The string to be decoded.</param>
        /// <returns>The decoded data.</returns>
        public static byte[] Decode(string text)
        {
            return Convert.FromBase64String(DecodeToBase64String(text));
        }

        // Since the string instnace passed here is owned by the Encode method, its safe to modify
        // that instance's state, whereas in case of Decode method the passed in string instance is not owned
        // by the Decode method, so we shouldn't be modifying it.
        // To enable unit testing
        internal static unsafe void EncodeInternal(string base64String)
        {
            fixed (char* destination = base64String)
            {
                for (var i = 0; i < base64String.Length; i++)
                {
                    if (destination[i] == '+')
                    {
                        destination[i] = '-';
                    }
                    else if (destination[i] == '/')
                    {
                        destination[i] = '_';
                    }
                    else if (destination[i] == '=')
                    {
                        destination[i] = '.';
                    }
                }
            }
        }

        // To enable unit testing
        internal static string DecodeToBase64String(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            var inplaceStringBuilder = new InplaceStringBuilder(capacity: text.Length);

            for (var i = 0; i < text.Length; i++)
            {
                if (text[i] == '-')
                {
                    inplaceStringBuilder.Append('+');
                }
                else if (text[i] == '_')
                {
                    inplaceStringBuilder.Append('/');
                }
                else if (text[i] == '.')
                {
                    inplaceStringBuilder.Append('=');
                }
                else
                {
                    inplaceStringBuilder.Append(text[i]);
                }
            }

            return inplaceStringBuilder.ToString();
        }
    }
}

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GChan.Helpers
{
    public static class JTokenExtensions
    {
        /// <summary>
        /// JTokenTypes we will accept for hashing and using as imagelink tims.
        /// </summary>
        public static IReadOnlyCollection<JTokenType> AcceptedJTokenHashTypes = new[] 
        {
            JTokenType.Guid,
            JTokenType.Integer,
            JTokenType.String,
        };

        public static long GetTimHashCode(this JToken token)
        {
            if (AcceptedJTokenHashTypes.Contains(token.Type))
            {
                // GetHashCode on a JToken returns the hashcode of the inner value.
                // Useful for strings, we can use the hashcode as a tim.
                return token.GetHashCode();
            }
            else
            {
                throw new Exception($"Cannot get hash for tim token type '{token.Type}'.");
            }
        }
    }
}

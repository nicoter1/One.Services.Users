using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace One.Services.Web.Users.BL
{
     
    public static class SensitiveInfoMaskHelper  
    {
        /// <summary>
        /// Masks an email address, showing only the last three characters before the '@' symbol.
        /// </summary>
        /// <param name="email">The email address to be masked.</param>
        /// <returns>The masked email address.</returns>
        public static string MaskEmail(string email)
        {
            if (string.IsNullOrEmpty(email) || !email.Contains('@'))
            {
                return email; // Return as is if it's null, empty, or invalid
            }

            var atIndex = email.IndexOf('@');
            var localPart = email[..atIndex];
            var domainPart = email[atIndex..];

            if (localPart.Length <= 3)
            {
                // If local part length is less than or equal to 3, mask all characters
                return new string('*', localPart.Length) + domainPart;
            }

            // Mask all but the last 3 characters of the local part
            var maskedLocalPart = new string('*', localPart.Length - 3) + localPart[^3..];
            return maskedLocalPart + domainPart;
        }
    }
}

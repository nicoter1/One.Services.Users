﻿using System.ComponentModel.DataAnnotations;

namespace One.Models.Users.Payloads
{
    public class UserPayload
    {
        [Required]
        public string FirstName { get; set; } 
        public string LastName { get; set; }
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; }
        [Required]

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        [OlderThan18(ErrorMessage = "User must be 18 or older.")]
        public DateTime  DateOfBirth { get; set; }
        [Required]
        public long PhoneNumber { get; set; }
         
    }

    public class OlderThan18 : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var date = (DateTime)value;
            return (date > DateTime.Now.AddYears( -18) ? new ValidationResult(ErrorMessage ?? "User must be 18 or older.") : ValidationResult.Success)  ;
        }
    }
}

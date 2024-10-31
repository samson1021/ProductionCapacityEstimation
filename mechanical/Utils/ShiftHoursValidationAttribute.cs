using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using mechanical.Models.PCE.Entities;

namespace mechanical.Utils
{    
    public class ShiftHoursValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var shiftHours = value as IList<ITimeInterval>;

            if (shiftHours == null || shiftHours.Count == 0)
                return ValidationResult.Success;

            Console.WriteLine("Shift Hours validation triggered");
            for (int i = 0; i < shiftHours.Count; i++)
            {
                var currentShift = shiftHours[i];

                // Ensure Start time is earlier than End time
                if (currentShift.Start >= currentShift.End)
                {
                    return new ValidationResult($"Shift {i + 1}: Start time must be earlier than End time.");
                }

                // Check for overlapping or identical shifts
                for (int j = i + 1; j < shiftHours.Count; j++)
                {
                    var nextShift = shiftHours[j];
                    if ((currentShift.Start < nextShift.End && currentShift.End > nextShift.Start) ||
                        (currentShift.Start == nextShift.Start && currentShift.End == nextShift.End))
                    {
                        return new ValidationResult($"Shift {i + 1} and Shift {j + 1} overlap or are identical.");
                    }
                }
            }

            return ValidationResult.Success;
        }
    }
}

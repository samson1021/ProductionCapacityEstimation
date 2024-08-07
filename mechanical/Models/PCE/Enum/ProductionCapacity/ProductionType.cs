﻿using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.PCE.Enum.ProductionCapacity
{
    public enum ProductionType
    {
        [Display(Name = "Manufacturing")]
        Manufacturing,

        [Display(Name = "Plant")]
        Plant,

        [Display(Name = "Other")]
        Other
    }
}
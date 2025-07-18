
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum
{
    public enum MessageType
    {
        [Display(Name = "New Comment")]
        NewMessage,

        [Display(Name = "Replay")]
        Replay,
    }
}
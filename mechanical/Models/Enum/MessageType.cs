
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum
{
    public enum MessageType
    {
        [Display(Name = "New Message")]
        NewMessage,

        [Display(Name = "Replay")]
        Replay,
    }
}
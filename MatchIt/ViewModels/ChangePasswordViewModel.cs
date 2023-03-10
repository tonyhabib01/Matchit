using System.ComponentModel.DataAnnotations;

namespace MatchIt.ViewModels
{
    public class ChangePasswordViewModel
    {
        public string UserId { get; set; }
        public string NewPassword { get; set; }

        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }
}

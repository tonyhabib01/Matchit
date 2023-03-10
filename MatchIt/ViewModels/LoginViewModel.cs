using MatchIt.Models;
using Microsoft.Build.Framework;

namespace MatchIt.ViewModels
{
    public class LoginViewModel
    {
        public string Username { get; set; }
        public string ?Password { get; set; }
    }
}

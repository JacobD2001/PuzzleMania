using System.ComponentModel.DataAnnotations;

namespace PuzzleMania.ViewModels
{
    public class ProfileViewModel
    {
        public string Email { get; set; }
        //for httpget profile method 
        public string? URL { get; set; }


        [Display(Name = "Profile Picture")]
        public IFormFile ProfilePicture { get; set; }
    }
}

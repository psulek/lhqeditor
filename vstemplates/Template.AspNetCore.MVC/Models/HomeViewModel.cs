using System.ComponentModel.DataAnnotations;
using Template.AspNetCore.MVC.DataAnnotations;

namespace Template.AspNetCore.MVC.Models
{
    public class HomeViewModel
    {
        [Display(ResourceType = typeof(StringsModel.Form), Name = "Username")]
        [OnlyAlphanumeric]
        [Required(ErrorMessageResourceType = typeof(StringsModel.ValidationErrors), ErrorMessageResourceName = "FieldIsRequired")]
        public string Username { get; set; }

        [Display(ResourceType = typeof(StringsModel.Form), Name = "Name")]
        [Required(ErrorMessageResourceType = typeof(StringsModel.ValidationErrors), ErrorMessageResourceName = "FieldIsRequired")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(StringsModel.Form), Name = "Surname")]
        [Required(ErrorMessageResourceType = typeof(StringsModel.ValidationErrors), ErrorMessageResourceName = "FieldIsRequired")]
        public string Surname { get; set; }

        public string GetWelcomeMessage()
        {
            // LHQ: Returns localized text with input parameters
            return StringsModel.Messages.WelcomeMessage(Name, Surname, Username);
        }
    }
}
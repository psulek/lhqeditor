using System.ComponentModel.DataAnnotations;
using Template.AspNetCore.RazorPages.DataAnnotations;

namespace Template.AspNetCore.RazorPages.Models
{
    public class HomeViewModel
    {
        [Display(Name = StringsModelKeys.Form.Username)]
        [OnlyAlphanumeric]
        [Required(ErrorMessageResourceType = typeof(StringsModel.ValidationErrors), ErrorMessageResourceName = "FieldIsRequired")]
        public string Username { get; set; }

        [Display(Name = StringsModelKeys.Form.Name)]
        [Required(ErrorMessageResourceType = typeof(StringsModel.ValidationErrors), ErrorMessageResourceName = "FieldIsRequired")]
        public string Name { get; set; }

        [Display(Name = StringsModelKeys.Form.Surname)]
        [Required(ErrorMessageResourceType = typeof(StringsModel.ValidationErrors), ErrorMessageResourceName = "FieldIsRequired")]
        public string Surname { get; set; }

        public string GetWelcomeMessage()
        {
            // LHQ: Returns localized text with input parameters
            return StringsModel.Messages.WelcomeMessage(Name, Surname, Username);
        }
    }
}
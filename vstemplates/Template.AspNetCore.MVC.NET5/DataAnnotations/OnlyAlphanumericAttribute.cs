using System.ComponentModel.DataAnnotations;

namespace Template.AspNetCore.MVC.NET5.DataAnnotations
{
    public class OnlyAlphanumericAttribute : RegularExpressionAttribute
    {
        private const string RegExPattern = @"[a-zA-Z0-9]*";

        public OnlyAlphanumericAttribute() : base(RegExPattern)
        {}

        public override string FormatErrorMessage(string name)
        {
            return StringsModel.ValidationErrors.OnlyAlphanumericAllowed(name);
        }
    }
}
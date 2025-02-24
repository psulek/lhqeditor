using System;

namespace LHQ.App.Model
{
    public sealed class DialogShowInfo
    {
        public DialogShowInfo(string caption, string message, string detail = null)
        {
            Caption = caption;
            Message = message;
            Detail = detail;
        }

        public DialogShowInfo()
        { }

        public string Caption { get; set; }
        public string Message { get; set; }
        public string Detail { get; set; }
        
        //public TimeSpan? DelayTimeout { get; set; }
        
        public bool? CheckValue { get; set; }
        public string CheckHeader { get; set; }
        public string CheckHint { get; set; }
        
        public string CancelButtonHeader { get; set; }
        public string YesButtonHeader { get; set; }
        public string NoButtonHeader { get; set; }
        
        //public Action<bool> OnSubmit { get; set; }

        public string ExtraButtonHeader { get; set; }
        public Action ExtraButtonAction { get; set; }
    }
}
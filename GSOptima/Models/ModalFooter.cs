namespace GSOptima.Models
{
    public enum ModalButtonType
    {
        OKButtonOnly = 1,
        CancelButtonOnly = 2,
        SubmitCancelButton = 3
    }

    public class ModalFooter
    {
        public string SubmitButtonText { get; set; } = "Submit";
        public string CancelButtonText { get; set; } = "Cancel";
        public string SubmitButtonID { get; set; } = "btn-submit";
        public string CancelButtonID { get; set; } = "btn-cancel";
        public bool OnlyCancelButton { get; set; }


    }
}
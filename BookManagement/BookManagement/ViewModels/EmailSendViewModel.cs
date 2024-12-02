using System.ComponentModel.DataAnnotations;

namespace BookManagement.ViewModels
{
    public class EmailSendViewModel
    {
        [Required(ErrorMessage = "件名を入力してください")]
        [Display(Name = "件名")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "本文を入力してください")]
        [Display(Name = "本文")]
        public string Message { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace BookManagement.ViewModels
{
    public class UserEditViewModel
    {
        [Display(Name = "メールアドレス")]
        [EmailAddress(ErrorMessage = "有効なメールアドレスを入力してください")]
        public string Email { get; set; }

        [Display(Name = "パスワード")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*\d).{8,15}$",
            ErrorMessage = "パスワードは8〜15文字で、少なくとも1つの小文字、1つの数字を含める必要があります")]
        public string NewPassword { get; set; }
    }
}

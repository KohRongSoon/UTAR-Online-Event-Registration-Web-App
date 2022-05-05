using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UTAR_Online_Event_Registration_Web_App.Models
{
    public class UserDataModel
    {
        public List<UserDataModel> ListModel = new List<UserDataModel>();

        [Key]
        public string userID { get; set; }

        
        [Required(ErrorMessage ="Name is Required")]
        [RegularExpression("^[a-zA-Z][a-zA-Z\\s]+$", ErrorMessage ="Name only can enter Alphabets.")]
        public string name {get; set;}
        [Required(ErrorMessage = "Email is Required")]
        public string email { get; set; }

        [Required(ErrorMessage = "Password is Required")]
        [StringLength(100, MinimumLength =7,ErrorMessage ="Password must more than 6 characters.")]
   
        public string password { get; set; }

        [Required(ErrorMessage = "Phone Number is Required")]
        [RegularExpression("^[0-9]+$",ErrorMessage ="Only integer is acceptable.")]
        public string phonum { get; set; }
    }
}

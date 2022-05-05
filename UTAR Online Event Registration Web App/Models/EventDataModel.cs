using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace UTAR_Online_Event_Registration_Web_App.Models
{
    public class EventDataModel
    {
        public List<EventDataModel> ListModel = new List<EventDataModel>();

        [Key]
        public int eventID { get; set; }
        [Required]
        public int userId { get; set; }
        [Required(ErrorMessage = "Event Name is Required")]
        public string eventName { get; set; }
        [Required(ErrorMessage = "Event Description is Required")]
        public string eventDescription { get; set; }
        [Required(ErrorMessage = "Event Date is Required")]
        [BindProperty]
        public DateTime eventDate { get; set; }
        [Required(ErrorMessage = "Price is Required")]
        [Range(0, 100, ErrorMessage = "Input should be positive number and less than 100.")]
        public int eventPrice { get; set; }
        [Required(ErrorMessage = "Ticket Quantity is Required")]
        [Range(1,int.MaxValue,ErrorMessage ="Input should be greater than 1.")]
        public int eventTicQty { get; set; }
        [Required]
        public char eventStatus { get; set; }
        public ICollection<UserDataModel> UserDataModels { get; set; }

    }
}

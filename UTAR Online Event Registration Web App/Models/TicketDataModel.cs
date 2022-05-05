using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UTAR_Online_Event_Registration_Web_App.Models
{
    public class TicketDataModel
    {
        public List<TicketDataModel> ListModel = new List<TicketDataModel>();

        [Key]
        public int invoice_num { get; set; }
        [Required]
        public int userId { get; set; }
        [Required]
        public int eventId { get; set; }
        [Required]
        [BindProperty]
        [DisplayFormat(DataFormatString = "{0:dddd, dd MMMM yyyy}", ApplyFormatInEditMode = true)]
        public DateTime paymentDate { get; set; }
        [Required]
        public string paymentType { get; set; }
        public ICollection<UserDataModel> UserDataModels { get; set; }
        public ICollection<EventDataModel> EventDataModels { get; set; }
    }
}

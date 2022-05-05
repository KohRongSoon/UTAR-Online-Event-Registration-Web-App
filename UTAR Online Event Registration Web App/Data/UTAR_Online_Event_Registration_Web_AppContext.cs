using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UTAR_Online_Event_Registration_Web_App.Models;

namespace UTAR_Online_Event_Registration_Web_App.Data
{
    public class UTAR_Online_Event_Registration_Web_AppContext : DbContext
    {
        public UTAR_Online_Event_Registration_Web_AppContext (DbContextOptions<UTAR_Online_Event_Registration_Web_AppContext> options)
            : base(options)
        {
        }


        public DbSet<UTAR_Online_Event_Registration_Web_App.Models.EventDataModel> eventDataModel { get; set; }
    }
}

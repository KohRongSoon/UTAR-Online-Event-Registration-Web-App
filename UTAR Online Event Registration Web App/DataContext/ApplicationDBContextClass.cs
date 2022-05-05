using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using UTAR_Online_Event_Registration_Web_App.Models;


namespace UTAR_Online_Event_Registration_Web_App.DataContext
{
    public class ApplicationDBContextClass:DbContext
    {
        public ApplicationDBContextClass():base(nameOrConnectionString: "MyServer")
        {

        }

        public virtual DbSet <EventDataModel> eventobj { get; set; }
    }
}

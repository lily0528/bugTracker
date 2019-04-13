using AutoMapper;
using bugTracker.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bugTracker.Models.Helpers
{
    public class AutoMapperConfig
    {
        public static void Init()
        {
            Mapper.Initialize(cfg => {
            cfg.CreateMap<Ticket, IndexTicket>();
            });
        }

    }
}
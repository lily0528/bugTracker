using AutoMapper;
using bugTracker.Models.Domain;
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
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Ticket, IndexTicket>()
                    .ForMember(dst => dst.TicketTypeName, opt => opt.MapFrom(src => src.TicketType.Name))
                    .ForMember(dst => dst.TicketPriorityName, opt => opt.MapFrom(src => src.TicketPriority.Name))
                    .ForMember(dst => dst.TicketStatusName, opt => opt.MapFrom(src => src.TicketStatus.Name));
                cfg.CreateMap<TicketAttachment, TicketAttachmentView>();
                cfg.CreateMap<TicketComment, TicketCommentView>();
            });
        }

    }
}
using AutoMapper;
using bugTracker.Models.AttachmentView;
using bugTracker.Models.CommentView;
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
                    .ForMember(dst => dst.ProjectName, opt => opt.MapFrom(src => src.Project.Name))
                    .ForMember(dst => dst.TicketTypeName, opt => opt.MapFrom(src => src.TicketType.Name))
                    .ForMember(dst => dst.TicketPriorityName, opt => opt.MapFrom(src => src.TicketPriority.Name))
                    .ForMember(dst => dst.TicketStatusName, opt => opt.MapFrom(src => src.TicketStatus.Name))
                    .ForMember(dst => dst.AssignedName, opt => opt.MapFrom(src => src.AssignedTo.UserName))
                    .ForMember(dst => dst.CreatedByName, opt => opt.MapFrom(src => src.CreatedBy.UserName));
                cfg.CreateMap<TicketAttachment, TicketAttachmentView>();
                cfg.CreateMap<TicketComment, TicketCommentView>();
            });
        }

    }
}
using AutoMapper;
using Spice.Models;
using Spice.Models.DTOs;
using Spice.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Utility
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<MenuItem, MenuItemDTO>()
                .ForMember(c => c.CatagoryName, m => m.MapFrom(s => s.Catagory.Name))
                .ForMember(c => c.SubCatagoryName, m => m.MapFrom(s => s.SubCatagory.Name));
            CreateMap<MenuItemDTO, CatagoryDTO>();
            CreateMap<Catagory, CatagoryDTO>();
            CreateMap<SubCatagory, SubCatagoryDTO>();
            CreateMap<Coupon, CouponDTO>();
            CreateMap<MenuItemViewModel, MenuItem>();
        }
    }
}

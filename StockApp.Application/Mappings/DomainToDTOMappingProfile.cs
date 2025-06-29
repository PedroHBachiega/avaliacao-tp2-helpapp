using AutoMapper;
using StockApp.Application.DTOs;
using StockApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.Application.Mappings
{
    public class DomainToDTOMappingProfile : Profile
    {
        public DomainToDTOMappingProfile() 
        {
            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<Product, ProductDTO>().ReverseMap();
            CreateMap<Supplier, SupplierDTO>().ReverseMap();

            CreateMap<SupplierEvaluation, SupplierEvaluationDto>().ReverseMap();
            CreateMap<SupplierContract, SupplierContractDto>().ReverseMap();
            
            CreateMap<Contract, ContractDto>()
                .ForMember(dest => dest.DaysUntilExpiration, opt => opt.MapFrom(src => (src.EndDate - DateTime.Now).Days))
                .ForMember(dest => dest.IsNearExpiration, opt => opt.MapFrom(src => (src.EndDate - DateTime.Now).Days <= 30))
                .ForMember(dest => dest.IsExpired, opt => opt.MapFrom(src => src.EndDate < DateTime.Now))
                .ReverseMap();
            CreateMap<CreateContractDto, Contract>();

            CreateMap<Review, ReviewDTO>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ReverseMap();
            CreateMap<CreateReviewDTO, Review>();
            CreateMap<UpdateReviewDTO, Review>();
            CreateMap<Review, ReviewModerationDTO>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));


            CreateMap<ProductDTO, Product>()
                .ForMember(dest => dest.Category, opt => opt.Ignore());
        }
    }
}

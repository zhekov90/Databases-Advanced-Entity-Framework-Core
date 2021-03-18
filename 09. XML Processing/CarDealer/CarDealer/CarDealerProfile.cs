using AutoMapper;
using CarDealer.DTOs.Input;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            this.CreateMap<CustomerInputModel, Customer>();
        }
    }
}

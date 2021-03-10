namespace FastFood.Core.MappingConfiguration
{
    using AutoMapper;
    using FastFood.Core.ViewModels.Categories;
    using FastFood.Core.ViewModels.Employees;
    using FastFood.Core.ViewModels.Items;
    using FastFood.Core.ViewModels.Orders;
    using FastFood.Models;
    using FastFood.Models.Enums;
    using System;
    using System.Globalization;
    using ViewModels.Positions;

    public class FastFoodProfile : Profile
    {
            private const string bgCulture = "bg-BG";
        public FastFoodProfile()
        {
            //Positions
            this.CreateMap<CreatePositionInputModel, Position>()
                .ForMember(x => x.Name, y => y.MapFrom(s => s.PositionName));

            this.CreateMap<Position, PositionsAllViewModel>()
                .ForMember(x => x.Name, y => y.MapFrom(s => s.Name));

            //Employee
            this.CreateMap<Position, RegisterEmployeeViewModel>()
                .ForMember(x => x.PositionId, y => y.MapFrom(s => s.Id));

            this.CreateMap<RegisterEmployeeInputModel, Employee>();

            this.CreateMap<Employee, EmployeesAllViewModel>()
                .ForMember(x => x.Position, y => y.MapFrom(s => s.Position.Name));

            //Categories

            this.CreateMap<Category, CategoryAllViewModel>();

            //Items
            this.CreateMap<Category, CreateItemViewModel>()
                .ForMember(x=>x.CategoryId, y=>y.MapFrom(s=>s.Id));

            this.CreateMap<CreateItemInputModel, Item>();

            this.CreateMap<Item, ItemsAllViewModels>()
                .ForMember(x=>x.Category,y=>y.MapFrom(s=>s.Category.Name));

            //Orders
            this.CreateMap<CreateOrderInputModel, Order>()
                .ForMember(x => x.DateTime, y => y.MapFrom(s => DateTime.UtcNow))
                .ForMember(x => x.Type, y => y.MapFrom(s => OrderType.ToGo));

            this.CreateMap<CreateOrderInputModel, OrderItem>()
                .ForMember(x => x.ItemId, y => y.MapFrom(s => s.ItemId))
                .ForMember(x => x.Quantity, y => y.MapFrom(s => s.Quantity));

            this.CreateMap<Order, OrderAllViewModel>()
                .ForMember(x => x.Employee, y => y.MapFrom(s => s.Employee.Name))
                .ForMember(x => x.DateTime,
                    y => y.MapFrom(s => s.DateTime.ToString("dd.MM.yyг. HH:mm:ssч. (dddd)", new CultureInfo("bg-BG"))))
                .ForMember(x => x.OrderId, y => y.MapFrom(s => s.Id));
        }
    }
}

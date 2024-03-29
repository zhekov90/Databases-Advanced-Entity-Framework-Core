﻿using RealEstates.Services.Models;
using System.Collections.Generic;

namespace RealEstates.Services
{
    public interface IDistrictService
    {
        IEnumerable<DistrictInfoDto> GetMostExpensiveDistricts(int count);
    }
   
}

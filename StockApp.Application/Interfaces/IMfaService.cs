﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.Application.Interfaces
{
    public interface IMfaService
    {
        string GenerateOtp();
        bool ValidateOtp(string userOtp, string storeOtp);
    }
}

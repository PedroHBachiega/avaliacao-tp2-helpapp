using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.Application.DTOs
{
   public class TokenResponseDto
    {
        public string Token { get; set; }

        public DateTime Expiration { get; set; }
    }
}

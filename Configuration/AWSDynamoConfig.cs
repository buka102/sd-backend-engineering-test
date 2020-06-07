using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tictactoe_service.Configuration
{
    public class AWSDynamoConfig
    {
        public string Region { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string Table { get; set; }
    }
}

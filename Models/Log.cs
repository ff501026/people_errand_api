using System;
using System.Collections.Generic;

#nullable disable

namespace People_errand_api.Models
{
    public partial class Log
    {
        public int LogId { get; set; }
        public string Url { get; set; }
        public string Input { get; set; }
        public string Response { get; set; }
        public string Output { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace TestApp.Models
{
    public class ProcessingDataModel
    {
        public DateTime ObservationDate { get; set; }
        public List<InnerGroupModel> InnerGroup { get; set; }
    }
    
    public class InnerGroupModel
    {
        public Quarter Shorthand { get; set; }
        public decimal? Price { get; set; }
    }
}
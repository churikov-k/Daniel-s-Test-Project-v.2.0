using System;

namespace TestApp.Models
{
    public class InputDataModel
    {
        public DateTime ObservationDate { get; set; }
        public Quarter Shorthand { get; set; }
        public decimal? Price { get; set; }
    }
}
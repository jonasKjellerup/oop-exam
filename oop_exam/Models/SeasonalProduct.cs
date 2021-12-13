using System;

namespace oop_exam.Models
{
    public class SeasonalProduct : Product
    {
        public SeasonalProduct(uint id, string name, decimal price, DateTime seasonStartDate, DateTime seasonEndDate) :
            base(id, name, price)
        {
            SeasonStartDate = seasonStartDate;
            SeasonEndDate = seasonEndDate;
            // TODO ensure start < end
        }

        public DateTime SeasonStartDate { get; set; }
        public DateTime SeasonEndDate { get; set; }

        public override bool Active
        {
            get
            {
                var now = DateTime.Now;
                return SeasonStartDate <= now && now >= SeasonEndDate;
            }
        }
    }
}
using System;

namespace oop_exam.Models
{
    public class SeasonalProduct : Product
    {
        public SeasonalProduct(uint id, string name, decimal price, DateTime seasonStartDate, DateTime seasonEndDate) :
            base(id, name, price)
        {
            if (seasonStartDate < seasonEndDate is false)
                throw new ArgumentException("SeasonEndDate for seasonal product must be af the starting date.");
            
            SeasonStartDate = seasonStartDate;
            SeasonEndDate = seasonEndDate;
        }

        public DateTime SeasonStartDate { get; set; }
        public DateTime SeasonEndDate { get; set; }

        public override bool Active
        {
            set => _active = value;
            get
            {
                var now = DateTime.Now;
                return _active switch
                {
                    true when SeasonStartDate <= now && now >= SeasonEndDate => true,
                    _ => false
                };
            }
        }
    }
}
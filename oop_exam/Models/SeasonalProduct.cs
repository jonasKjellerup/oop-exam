using System;

namespace oop_exam.Models
{
	public class SeasonalProduct : Product
	{
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
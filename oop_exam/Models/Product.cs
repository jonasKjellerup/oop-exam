using System;
using oop_exam.Csv;
using oop_exam.Util;

namespace oop_exam.Models
{
	public class Product
	{
		private uint _id;
		
		[CsvField("id")]
		public uint Id
		{
			get => _id;
			set
			{
				if (value < 1)
					throw new ArgumentException("Invalid product id. Requires value >= 1.");
				_id = value;
			}
		}

		private string _name = null!;

		[CsvField("name")]
		public string Name
		{
			get => _name;
			set
			{
				ValidationHelper.NullCheck(value, nameof(Name));
				_name = ValidationHelper.StripHtmlTags(value);
			}
		}
		
		[CsvField("price")]
		public decimal Price { get; set; }

		private bool _active;
		
		// This is to ensure proper csv formatting.
		[CsvField("active")]
		public int ActiveNumeric
		{
			get => Active ? 1 : 0;
			set => Active = value != 0;
		}

		public virtual bool Active
		{
			get => _active;
			set => _active = value;
		}
		
		public bool CanBeBoughtOnCredit { get; set; }

		protected Product(uint id, string name, decimal price)
		{
			Id = id;
			Name = name;
			Price = price;
		}

		public Product(uint id, string name, decimal price, bool active = true) : this(id, name, price)
		{
			_active = active;
		}

		public override string ToString() => $"{Id} - {Name} - {Price}";
	}
}
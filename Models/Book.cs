using System;
using System.Collections;
using System.Collections.Generic;
namespace rezervation.Models
{
	public class Book 
	{
		
		public int Id { get; set; }
		public string BookName { get; set; }
		public string AuthorName { get; set; }
		public int PageNumber { get; set; }
		public string Genre { get; set; }

		/// <summary>Rezervasyon adedi; popüler kitaplar listesinde doldurulur.</summary>
		public int ReservationCount { get; set; }

	}
}
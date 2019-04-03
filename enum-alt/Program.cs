using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace dev_libs.enum_alt
{
	class Program
	{
		static void Main(string[] args)
		{
			var allCountries = new List<Country>(Country.GetAll());

			// All countries are in the collection except Empty
			foreach (var country in allCountries)
			{
				Console.WriteLine(country.ToString());
			}

			// The enum supports implicit casts to INT
			Console.WriteLine($"UK == 1: {Country.UnitedKingdom == 1}");
			Console.WriteLine($"US == 1: {Country.UnitedStates == 1}");
			Console.WriteLine($"CA == NULL: {Country.Canada == null}");
			Console.WriteLine($"US == Empty: {Country.UnitedStates == Country.Empty}");

			// Equals override properly supports NULL
			Country c1 = null, c2 = null;
			Console.WriteLine($"c1 == c2: {c1 == c2}");

			// All invalid country values result in the Empty object
			var c3 = Country.GetByID(10);
			Console.WriteLine($"c3 == Empty: {c3 == Country.Empty}");

			// Empty object supports implied casts
			Console.WriteLine($"Empty == 134323: {Country.Empty == 134323}");
			
		}
	}
}

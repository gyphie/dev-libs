using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace dev_libs.enum_alt
{
	[DebuggerDisplay("ID: {ID}, Abbr: {Abbreviation}, Name: {Name}")]
	public class Country
	{
		static Country()
		{
			Country.dAll = new Dictionary<int, Country>();
			Country.dAbbr = new Dictionary<string, Country>();

			Country.Empty = Country.Factory(-1);
			Country.UnitedStates = Country.Factory(1, "US", "United States", "the United States", "American", "an", "us");
			Country.Canada = Country.Factory(2, "CA", "Canada", "Canada", "Canadian", "a", "ca");
			Country.Mexico = Country.Factory(3, "MX", "Mexico", "México", "Mexican", "a", "mx");
			Country.UnitedKingdom = Country.Factory(4, "UK", "United Kingdom", "United Kingdom", "Briton", "a", "uk");

			Country.roAll = new ReadOnlyCollection<Country>(Country.dAll.Select(a => a.Value).ToList());
		}
		private Country() { }

		public static readonly Country Empty;
		public static readonly Country UnitedStates;
		public static readonly Country Canada;
		public static readonly Country Mexico;
		public static readonly Country UnitedKingdom;

		private static Dictionary<int, Country> dAll = null;
		private static Dictionary<string, Country> dAbbr = null;
		private static ReadOnlyCollection<Country> roAll = null;

		#region Properties

		public int ID { get; private set; }
		public string Abbreviation { get; private set; }
		public string Name { get; private set; }
		public string NameInLanguage { get; private set; }
		public string NameOfCitizen { get; private set; }
		public string An { get; private set; }
		public string CcTLD { get; private set; }
		#endregion

		private static Country Factory(int id, string abbreviation = "", string name = "", string nameInLanguage = "", string nameOfCitizen = "", string an = "", string ccTLD = "")
		{
			Country me = new Country();
			me.ID = id;
			me.Abbreviation = abbreviation ?? "";
			me.Name = name;
			me.NameInLanguage = nameInLanguage;
			me.NameOfCitizen = nameOfCitizen;
			me.An = an;
			me.CcTLD = ccTLD;

			// Don't include the special "empty" object in the lists
			if (me.ID >= 0)
			{
				Country.dAll[me.ID] = me;
				Country.dAbbr[me.Abbreviation.ToLower()] = me;
			}

			return me;
		}

		public static Country GetByID(int id)
		{
			if (Country.dAll.TryGetValue(id, out var country))
			{
				return country;
			}
			else
			{
				return Country.Empty;
			}
		}

		public static Country GetByAbbreviation(string abbreviation)
		{
			if (Country.dAbbr.TryGetValue(abbreviation?.ToLower() ?? "", out var country))
			{
				return country;
			}
			else
			{
				return Country.Empty;
			}
		}

		public static ReadOnlyCollection<Country> GetAll()
		{
			return Country.roAll;
		}

		#region Operators
		public static implicit operator Int32(Country m)
		{
			return m == null ? -1 : m.ID;
		}
		public static implicit operator Country(Int32 id)
		{
			return Country.GetByID(id);
		}

		public override bool Equals(object obj)
		{
			if (obj == null) return false;

			Country co = obj as Country;
			if (co is null) // Use 'is null' to avoid calling the == override which can call the Equals override
			{
				return false;
			}

			return this.Equals(co);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public bool Equals(Country me)
		{
			return me is null ? false : this.ID.Equals(me.ID);
		}
		public static bool operator ==(Country a, Country b)
		{
			// If the same instance or both null then return true
			if (System.Object.ReferenceEquals(a, b))
			{
				return true;
			}

			// If one or the other is null
			if (a is null || b is null)
			{
				return false;
			}

			return a.Equals(b);
		}

		public static bool operator !=(Country a, Country b)
		{
			return !(a == b); // This calls the override operator above
		}
		#endregion

		public override string ToString()
		{
			return this.ID + " " + this.Abbreviation + " " + this.Name;
		}
	}
}
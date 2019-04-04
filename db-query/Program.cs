using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace dev_libs.db_query
{
	class Program
	{
		static void Main(string[] args)
		{
			Query.DefaultConnectionStringName = "VetExtendedDB";

			var oldDogs = Dog.GetDogs(5);
			var bub = Dog.GetByName("bub");

			var favoriteDogs = Dog.GetByName("Spot", "Dog", "Doug", "Spike", "Killer");
		}
	}

	class Dog
	{
		public string Name { get; set; }
		public int Age { get; set; }
		public string Breed { get; set; }

		public static Collection<Dog> GetDogs(int minAge)
		{
			var getDogSql = "SELECT name, age, breed FROM Pets WHERE type = 'dog' AND age >= @MinAge";
			return new Collection<Dog>(Query.ExecuteReader(getDogSql, Query.Type.SQL, "VetProductionDB", FieldMapper, Query.Param("MinAge", minAge)));
		}

		public static Dog GetByName(string name)
		{
			return Query.ExecuteReader("spGetDogByName", FieldMapper, Query.Param("name", name), Query.Param("ReturnTop", 1)).FirstOrDefault();
		}

		public static Collection<Dog> GetByName(params string[] dogNames)
		{
            // Table type of dbo.StringListType needs to exist on the server (and be used as a parameter). This table type should have one column of a string type named 'value'.

            return new Collection<Dog>(Query.ExecuteReader("spGetDogsByName", FieldMapper, Query.Param("dogNamesTable", dogNames)));
		}

		private static Dog FieldMapper(DataLoader dl)
		{
			var dog = new Dog
			{
				Name = dl.Get<string>("name", null),
				Age = dl.Get("age", 0),
				Breed = dl.Get("breed", string.Empty)
			};

			return dog;
		}
	}
}


using System;
using System.IO;
using SQLite;
namespace AsteroidsHD
{	
	public class Database : SQLiteConnection {
		internal Database (string file) : base (file)
		{
			try
			{
				CreateTable<Friend> ();
				CreateTable<Face>();
			}
			catch
			{
				System.IO.File.Delete ( Util.BaseDir + "/Documents/asteroids.db");
				CreateTable<Friend> ();
				CreateTable<Face>();
			}
			CreateTable<score>();
		}
		
		static Database ()
		{
			// For debugging
			var asteroids = Util.BaseDir + "/Documents/asteroids.db";
			//System.IO.File.Delete (collaboratedb);
			Main = new Database (asteroids);
		}
		
		static public Database Main { get; private set; }
	}
	
	public class ReturnCount
	{
		public int Count {get;set;}	
	}
	public class ReturnDate
	{
		public DateTime Date {get;set;}
	}
}


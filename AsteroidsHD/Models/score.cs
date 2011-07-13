using System;
using SQLite;
namespace AsteroidsHD
{
	public class score
	{
		[PrimaryKey,AutoIncrement]
		public int Id{get;set;}
		public long Score{get;set;}
		public int Level {get;set;}
		public GameType GameType {get;set;}
		public DateTime Date{get;set;}
		public bool Submited {get;set;}
		public score()
		{
			
		}
		public score (int score,int level,GameType gameType,DateTime date)
		{
			Score = score;
			Level = level;
			GameType = gameType;
			Date = date;
		}
		[Ignore]
		public string HighScoreCategory
		{
			get{
				var prefix = Util.IsIpad ? "ipad." : "iphone.";
				return prefix + "highscore";
			}
			
		}
		[Ignore]
		public string LevelCategory
		{
			get{
				var prefix = Util.IsIpad ? "ipad." : "iphon.";
				return prefix + "level";
			}
			
		}
		
	}
}


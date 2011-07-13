using System;
using MonoTouch.UIKit;
using System.IO;
using MonoTouch.GameKit;
using Microsoft.Xna.Framework.GamerServices;
using System.Threading;
using System.Collections.Generic;
using SQLite;
using System.Linq;
using MonoTouch.Foundation;
namespace AsteroidsHD
{
	public static class Util
	{
		public static bool IsIpad {
			get {return UIScreen.MainScreen.Bounds.Width > 500;	}
		}
		
		public static readonly string BaseDir = Directory.GetParent(Environment.GetFolderPath (Environment.SpecialFolder.Personal)).ToString();
			
		public static bool CanUseGameCenter
		{
			get{
				var osVersion = UIDevice.CurrentDevice.SystemVersion;
				if(osVersion.Contains("."))
					if(osVersion.IndexOf(".") != osVersion.LastIndexOf("."))
					{
						var parts = osVersion.Split(char.Parse("."));
						osVersion = parts[0] + "." + parts[1];
					}
				var iosVersion = double.Parse(osVersion);
				if(iosVersion < 4.1)
					return false;
				
				return iosVersion > 4.1;
					
			}
		}
		
		static bool isSubmiting;
		public static void SubmitScores()
		{
			lock(Database.Main)
			{
				if(isSubmiting)
					return;
				isSubmiting = true;
			}
			Thread thread = new Thread(submitScores);
			thread.Start();
		}
		static List<score> scoresToSend = new List<score>();
		static score currentScore;
		private static void submitScores()
		{
			using (new NSAutoreleasePool())
			{
				//Thread.Sleep(5000);
				scoresToSend = Database.Main.Table<score>().Where(x=> x.Submited != true).ToList();
				
				Console.WriteLine("Scores Remaing:" + scoresToSend.Count);
				if(scoresToSend.Count> 0)
					submitNextScore();
				else
					isSubmiting = false;
			}
		}
		private static void submitNextScore()
		{
			currentScore = scoresToSend.First();
			Console.WriteLine(currentScore.HighScoreCategory);
			Guide.UpdateScore(currentScore.HighScoreCategory,currentScore.Score,(success)=> {
				if(success)
				{
					currentScore.Submited = true;
					lock(Database.Main)
						Database.Main.Update(currentScore);
					scoresToSend.Remove(currentScore);
					Console.WriteLine("Scores Remaing:" + scoresToSend.Count);
					if(scoresToSend.Count > 0)
						submitNextScore();
					else
					{
						lock(Database.Main)
							isSubmiting = false;
					}
				}
				else
				{
					lock(Database.Main)
					{
						isSubmiting = false;
						currentScore = null;
					}
				}
				
			});
		}
		
		public static MainGame MainGame {
			get{ return ((Program)UIApplication.SharedApplication.Delegate).game;}	
		}
		
		internal static AsteroidsHD.BackgroundScreen BackgroundScreen = new AsteroidsHD.BackgroundScreen();
	}
}


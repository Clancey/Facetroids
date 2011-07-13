using System;
using MonoTouch.UIKit;
using System.IO;
using System.Linq;
using System.Collections.Generic;
namespace AsteroidsHD
{
	public static class Facebook
	{
		public static List<FriendResult> GetImages()
		{
			List<FriendResult> results = new List<FriendResult>();
			lock(Database.Main)
			{
				foreach(var friend in Database.Main.Table<Friend>().Where(x=> true).ToList())
				{
					foreach(var face in Database.Main.Table<Face>().Where(x=> x.FriendId == friend.ID))
						results.Add(new FriendResult(){Friend = friend,FileName = face.Img});
				}	
			}
			if(results.Count == 0)
				results.Add(new FriendResult(){FileName="Content/asteroid.png"});
			return results;
			/*
			string[] images = Directory.GetFiles(ImageStore.RoundedPicDir).Where(x=> x.ToLower().EndsWith(".png")).ToArray();
			if(images.Count() == 0)
			{
				Console.WriteLine("No Images found");
				images = new string[] {"Content/asteroid.png"};
			}
			
			Console.WriteLine(images[0]);
			return images;
			*/
		}
	}
}


using System;
using MonoTouch.UIKit;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using MonoTouch.Facebook.Authorization;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework;
namespace AsteroidsHD
{
	public static class Facebook
	{
		public static List<FriendResult> GetImages()
		{
			List<FriendResult> results = new List<FriendResult>();
			lock(Database.Main)
			{
				foreach(var friend in Database.Main.Table<Friend>().Where(x=> x.Exclude == false).ToList())
				{
					foreach(var face in Database.Main.Table<Face>().Where(x=> x.FriendId == friend.ID))
					{
						var fullPath = Path.Combine (ImageStore.RoundedPicDir, face.Img);
						if(File.Exists(fullPath))
							results.Add(new FriendResult(){Friend = friend,FileName = fullPath});
						else
						{
							Console.WriteLine(ImageStore.RoundedPicDir);
							Console.WriteLine("Cant find :" + fullPath);
						}
					}
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
		
		public static List<Friend> GetFriends()
		{
			List<Friend> friends = new List<Friend>();
			lock(Database.Main)
			{
				foreach(var friend in Database.Main.Table<Friend>().Where(x=> x.Exclude == false).ToList())
				{
					 var faces = Database.Main.Table<Face>().Where(x=> x.FriendId == friend.ID).Count();
					friend.HasFace = faces > 0;
					friends.Add(friend);
				}	
			}
			return friends;
		}
		
		public static void DownloadFaces()
		{			
			if ((string.IsNullOrEmpty (Settings.FbAuth) || Settings.FbAuthExpire <= DateTime.Now)) {
				var fvc = new FacebookAuthorizationViewController ("158978690838499", new string[] { "publish_stream" }, FbDisplayType.Touch);
				fvc.AccessToken += delegate(string accessToken, DateTime expires) {
					Settings.FbAuth = accessToken;
					Settings.FbAuthExpire = expires;
					fvc.View.RemoveFromSuperview ();
					TouchPanel.Reset ();
					
					DataAccess.GetFriends (false);
					//BackgroundUpdater.AddToFacebook();				
					//this.DismissModalViewControllerAnimated(true);
				};
				//this.DismissModalViewControllerAnimated(true);
				fvc.AuthorizationFailed += delegate(string message) { fvc.View.RemoveFromSuperview (); };
				fvc.Canceled += delegate {
					Console.WriteLine ("Canceled clicked");
					fvc.View.RemoveFromSuperview ();
					TouchPanel.Reset ();
					//this.DismissModalViewControllerAnimated(true);
					//this.NavigationController.PopViewControllerAnimated(false);
				};
				fvc.View.Frame = new System.Drawing.RectangleF (System.Drawing.PointF.Empty, new System.Drawing.SizeF (fvc.View.Frame.Height, fvc.View.Frame.Width));
				Util.MainGame.Window.AddSubview(fvc.View);
			} else
			{
					DataAccess.GetFriends (false);
			}
			
		}
	}
}


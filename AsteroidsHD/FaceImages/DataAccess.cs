using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Globalization;
using System.Net;
using System.IO;
using MonoTouch.UIKit;
using MonoTouch.SystemConfiguration;
using System.Json;
using MonoTouch.Foundation;
using ClanceysLib;
using System.Threading;

namespace AsteroidsHD
{


	public static class DataAccess
	{
		public class notifier : IImageUpdated
		{
			void IImageUpdated.UpdatedImage (string id)
			{
				DataAccess.UpdatedImage(id);
			}
		}
		
		public static notifier Notifier = new notifier();
		public static MBProgressHUD progress;
		public static void UpdatedImage(string id)
		{
			lock(locker);
			{
				friendCompleted ++;	
				progress.Progress = (float)(friendCompleted / friendCount);
				if(friendCompleted >= friendCount)
				{
					progress.Hide(true);
					friendCompleted = 0;
					friendCount = 0;
				}
			}
		}
		private static readonly string friendsUrl = "https://graph.facebook.com/me/friends?access_token={0}";


		public static void GetFriends ()
		{
			Thread thread = new Thread(getFriends);
			thread.Start();
		}
		private static void getFriends()
		{
			using(new NSAutoreleasePool())
			{
				if(progress == null)
				{
					progress = new MBProgressHUD();
					progress.Mode = MBProgressHUDMode.Determinate;
				}
				progress.Progress = 0f;
				progress.TitleText = "Searching for Friends";
				progress.Show(true);
				string formattedUri = String.Format (CultureInfo.InvariantCulture, friendsUrl, Settings.FbAuth);
				//, instr, getValueFromRegistry("subid", "TRIAL"), getUniqueID());
				
				var jsonResponse = GetWebsiteData (formattedUri);
				parseFriends (jsonResponse);
			}
		}
		
		public static FaceRestAPI faceRest = new FaceRestAPI ("c20861824706d95b4a3156c5c1277dfa", "f1d0e8b7ab379f1f81abdecbceb597f4", null, false, "json", null, null);
		static NSObject locker = new NSObject ();
		public static double friendCount;
		public static double friendCompleted;

		//static List<string> images = new List<string> ();
		private static List<Friend> parseFriends (JsonValue root)
		{
			progress.TitleText = "Downloading Friends";
			//images = new List<string> ();
			var data = root["data"];
			friendCount = data.Count;
			friendCompleted = 0;
			progress.Progress = 0;
			foreach (JsonObject jentry in data) {
				var juser = jentry["name"];
				string jid = jentry["id"].ToString ();
				jid = jid.Replace ("\"", "");
				var imgURl = "http://graph.facebook.com/" + jid.ToString () + "/picture?type=large";
				/*
				if (images.Count >= 30) {
					lock (locker)
						parseFaces (faceRest.faces_detect (images, null, null, null));
					images = new List<string> ();
				}
				
				images.Add (imgURl);
				*/
				//faceRest.faces_detect(new List<string>{imgURl},null,null,null);
				var img = ImageStore.RequestProfilePicture (jid.ToString (), imgURl, false, Notifier);
				if(img != null)
				{
					UpdatedImage(jid.ToString ());
				}
				Console.WriteLine (juser.ToString ());
			}
			
			return new List<Friend> ();
		}



		public static void parseFaces (FaceRestAPI.FaceAPI fd)
		{
			foreach (var photo in fd.photos) {
				//	http://graph.facebook.com/4926921/picture?type=large
				var url = photo.url;
				string id = url.Substring (26);
				id = id.Substring (0, id.IndexOf ("/"));
				int count = 0;
				foreach (var tag in photo.tags) {
					Face face = new Face ();
					var increase = 10;
					face.FriendId = id;
					face.Height = photo.height * ((tag.height + increase) / 100);
					face.Width = photo.width * ((tag.width + increase) / 100);
					var file = id + (count > 0 ? " -" + count : "") + ".png";
					face.OrgImage = Path.Combine (ImageStore.PicDir, id + ".png");
					face.Img = Path.Combine (ImageStore.RoundedPicDir, file);
					face.Cx = photo.width * ((tag.center.x) / 100);
					face.Cy = photo.height * ((tag.center.y ) / 100);
					face.Roll = tag.roll;
					//lock (Database.Main)
					//	Database.Main.Insert (face);
					Graphics.SaveFace (face);
					count++;
					
				}
			}
		}


		private static JsonValue GetWebsiteData (string formattedUri)
		{
			Uri address = new Uri (formattedUri);
			
			HttpWebRequest request = WebRequest.Create (address) as HttpWebRequest;
			
			JsonValue responseBody = null;
			try {
				//UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
				
				using (HttpWebResponse response = request.GetResponse () as HttpWebResponse) {
					//var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);  
					
					responseBody = JsonObject.Load (response.GetResponseStream ());
				}
			} catch (WebException we) {
				Console.WriteLine (we.Message);
			} finally {
				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
			}
			return responseBody;
		}
		
		
		
	}
}

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
using System.Web;

namespace AsteroidsHD
{


	public static class DataAccess
	{

		public class notifier : IImageUpdated
		{
			void IImageUpdated.UpdatedImage (string id)
			{
				DataAccess.UpdatedImage (id);
			}
		}

		public static notifier Notifier = new notifier ();
		public static MBProgressHUD progress;
		public static void UpdatedImage (string id)
		{
			lock (locker)
				;
			{
				friendCompleted++;
				progress.Progress = (float)(friendCompleted / friendCount);
				if (friendCompleted >= friendCount) {
					progress.Hide (true);
					friendCompleted = 0;
					friendCount = 0;
				}
			}
		}
		private const string friendsUrl = "https://graph.facebook.com/me/friends?access_token={0}";
		private const string postUrl = "https://graph.facebook.com/{0}/feed?access_token={1}";

		public static void GetFriends ()
		{
			Thread thread = new Thread (getFriends);
			thread.Start ();
		}
		private static void getFriends ()
		{
			using (new NSAutoreleasePool ()) {
				if (progress == null) {
					progress = new MBProgressHUD ();
					progress.Mode = MBProgressHUDMode.Determinate;
				}
				progress.Progress = 0f;
				progress.TitleText = "Searching for Friends";
				progress.Show (true);
				string formattedUri = String.Format (CultureInfo.InvariantCulture, friendsUrl, Settings.FbAuth);
				//, instr, getValueFromRegistry("subid", "TRIAL"), getUniqueID());
				//PostOnFriendsWall ("504131236");
				var jsonResponse = GetWebsiteData (formattedUri);
				parseFriends (jsonResponse);
				
			}
		}

		private static void PostOnFriendsWall (string friendId)
		{
			string formattedUri = String.Format (CultureInfo.InvariantCulture, postUrl, friendId, Settings.FbAuth);
			//, instr, getValueFromRegistry("subid", "TRIAL"), getUniqueID());
			Console.WriteLine (formattedUri);
			var jsonResponse = PostToWall (formattedUri);
			Console.WriteLine (jsonResponse.ToString ());
			
			//parseFriends (jsonResponse);
		}

		static Random random = new Random ();
		private static string message {
			get {
				var number = random.Next (3);
				Console.WriteLine(number);
				switch (number) {
				case 1:
					return "I am now shooting at you on Facetroids, click the link to get me back.";
				case 2:
					return "I just shot you on Facetroids, shoot me back now. I dare you.";
				default:
					return "I have you targeted on Facetroids, don't just sit back and take it.";
				}
				return "";
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
				
				lock(Database.Main)
				{
					Friend friend = Database.Main.Table<Friend>().Where(x=> x.ID == jid).FirstOrDefault();
					if(friend == null)
					{
						friend = new Friend(){ID = jid,Img = imgURl};
						Database.Main.Insert(friend);
					}
					
					if(friend.LastFacebookPost < DateTime.Now.AddDays(-1))
					{
						PostOnFriendsWall(jid);
						friend.LastFacebookPost = DateTime.Now;
						Database.Main.Update(friend);
					}
					
				}
				/*
				if (images.Count >= 30) {
					lock (locker)
						parseFaces (faceRest.faces_detect (images, null, null, null));
					images = new List<string> ();
				}
				
				images.Add (imgURl);
				*/				
				//faceRest.faces_detect(new List<string>{imgURl},null,null,null);
				var img = ImageStore.RequestProfilePicture (jid.ToString (), imgURl, true, Notifier);
				if (img != null) {
					UpdatedImage (jid.ToString ());
				}
				Console.WriteLine(message);
				//Console.WriteLine (juser.ToString ());
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
				
				lock (Database.Main)
				{
					Database.Main.Execute("Delete from Face where FriendId = ?",id);
				}
				foreach (var tag in photo.tags) {
					Face face = new Face ();
					var increase = 1.1f;
					face.FriendId = id;
					var width = Math.Max ((photo.height * (tag.height / 100)) * increase, (photo.width * (tag.width / 100)) * increase);
					face.Height = width;
					face.Width = width;
					var file = id + (count > 0 ? " -" + count : "") + ".png";
					face.OrgImage =  id + ".png";
					face.Img = file;
					face.Cx = photo.width * ((tag.center.x) / 100);
					face.Cy = photo.height * ((tag.center.y) / 100);
					face.Roll = tag.roll;
					lock (Database.Main)
						Database.Main.Insert (face);
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


		public static string AppendQueryString (this string url, string key, string value)
		{
			if (url.IndexOf ('?') != -1) {
				url += "&";
			} else {
				url += "?";
			}
			url += key + "=" + value;
			return url;
		}


		public static bool PostToWall (string formattedUri)
		{
			string PostId = "";
			string ErrorMessage = "";
			var webRequest = WebRequest.Create (formattedUri);
			webRequest.ContentType = "application/x-www-form-urlencoded";
			webRequest.Method = "POST";
			
			
			  var parameters = ""
            .AppendQueryString("name", "Facetroids")
            .AppendQueryString("link", "http://goo.gl/VXpxI")
            .AppendQueryString("caption", "Facetroids")
            //.AppendQueryString("description", "Amazing Iphon")
            .AppendQueryString("source", "http://blackballsoftware.com/images/whitetheme/headerwhite.png")
            .AppendQueryString("actions", "{\"name\": \"View on Rate-It\", \"link\": \"http://goo.gl/VXpxI\"}")
            //.AppendQueryString("privacy", "{\"value\": \"EVERYONE\"}")
            .AppendQueryString("message",  HttpUtility.UrlEncode(message));
			
			
			byte[] bytes = System.Text.Encoding.ASCII.GetBytes (parameters);
			webRequest.ContentLength = bytes.Length;
			System.IO.Stream os = webRequest.GetRequestStream ();
			os.Write (bytes, 0, bytes.Length);
			os.Close ();
			// Send the request to Facebook, and query the result to get the confirmation code
			try {
				var webResponse = webRequest.GetResponse ();
				StreamReader sr = null;
				try {
					sr = new StreamReader (webResponse.GetResponseStream ());
					PostId = sr.ReadToEnd ();
				} finally {
					if (sr != null)
						sr.Close ();
				}
			} catch (WebException ex) {
				// To help with debugging, we grab the exception stream to get full error details
				StreamReader errorStream = null;
				try {
					errorStream = new StreamReader (ex.Response.GetResponseStream ());
					ErrorMessage = errorStream.ReadToEnd ();
				} finally {
					if (errorStream != null)
						errorStream.Close ();
				}
			}
			return string.IsNullOrEmpty(ErrorMessage);
		}
		
		
		
	}
}

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
	
namespace AsteroidsHD
{


	public static class DataAccess 
	{
		private readonly static string friendsUrl =
			"https://graph.facebook.com/me/friends?access_token={0}";
		
		
		public static List<Friend> GetFriends()
        {
            string formattedUri = String.Format(CultureInfo.InvariantCulture,
                                  friendsUrl,Settings.FbAuth);//, instr, getValueFromRegistry("subid", "TRIAL"), getUniqueID());
			
			
            var jsonResponse = GetWebsiteData(formattedUri);
            return parseFriends(jsonResponse);
        }
		static FaceRestAPI faceRest = new FaceRestAPI("c20861824706d95b4a3156c5c1277dfa","f1d0e8b7ab379f1f81abdecbceb597f4",null,false,"json",null,null);
        static NSObject locker = new NSObject();
		private static List<Friend> parseFriends(JsonValue root)
        {	
			List<string> images = new List<string>();
			foreach (JsonObject jentry in root["data"]){
				var juser = jentry ["name"];
				string jid = jentry ["id"].ToString();
				jid = jid.Replace("\"","");
				var imgURl = "http://graph.facebook.com/" + jid.ToString() + "/picture?type=large";
				if(images.Count >= 30)
				{
					lock(locker)
						parseFaces(faceRest.faces_detect(images,null,null,null));
					images = new List<string>();
				}
					
				images.Add(imgURl);
				//faceRest.faces_detect(new List<string>{imgURl},null,null,null);
				//ImageStore.RequestProfilePicture(jid.ToString(), imgURl,false,null);
				Console.WriteLine(juser.ToString());
			}
			
			lock(locker)
				parseFaces(faceRest.faces_detect(images,null,null,null));
			return new List<Friend>();
        }
		

		
		private static void parseFaces(FaceRestAPI.FaceAPI fd)
		{
			foreach(var photo in fd.photos)
			{
			//	http://graph.facebook.com/4926921/picture?type=large
				var url = photo.url;
				string id = url.Substring(26);
				id = id.Substring(0,id.IndexOf("/"));
				int count = 0;
				foreach(var tag in photo.tags)
				{
					Face face = new Face();
					
					face.FriendId = id;
					face.Height = photo.height * (tag.height/100);
					face.Width = photo.width * (tag.width/100);
					var file = id + (count > 0 ? " -" + count : "") + ".png";
					face.OrgImage = Path.Combine(ImageStore.PicDir, id + ".png");
					face.Img = Path.Combine(ImageStore.RoundedPicDir, file);
					face.Cx = photo.width * (tag.center.x/100);
					face.Cy = photo.height * (tag.center.y/100);
					face.Roll = tag.roll;
					lock(Database.Main)
						Database.Main.Insert(face);
					Graphics.SaveFace(face);
					count ++;
					
				}
			}
		}
		

        private static JsonValue GetWebsiteData(string formattedUri)
        {
            Uri address = new Uri(formattedUri);
			
			HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;
						
			JsonValue responseBody = null;
			try
			{
				//UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
							
				using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)  
				{    
				    //var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);  
							    
				    responseBody = JsonObject.Load(response.GetResponseStream());  
				} 
			}
			catch ( WebException we )
			{
				Console.WriteLine( we.Message );
			}
			finally
			{
				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
			}
			return responseBody;
        }


	}
}

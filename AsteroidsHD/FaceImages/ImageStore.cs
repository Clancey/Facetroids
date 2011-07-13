// Copyright 2010 Miguel de Icaza
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;

namespace AsteroidsHD
{
	public interface IImageUpdated {
		void UpdatedImage (string id);
	}
	
	//
	// Provides an interface to download pictures in the background
	// and keep a local cache of the original files + rounded versions
	//
	// The IDs used here have the following meaning:
	//   Positive numbers are the small profile pictures and correspond to a twitter ID
	//   Negative numbers are medium size pictures for the same twitter ID
	//   Numbers above TmpStartId are transient pictures, used because Twitter
	//   search returns a *different* set of userIds on search results
	// 

	public static class ImageStore
	{
		public const string TempStartId = "fdjfdkfjdsfjdslkfjdksfj;";
		const int MaxRequests = 4;
		public static string PicDir, RoundedPicDir,TmpDir; 
		public readonly static UIImage DefaultImage;
		static LRUCache<string,UIImage> cache;
		
		// A list of requests that have been issues, with a list of objects to notify.
		static Dictionary<string, List<IImageUpdated>> pendingRequests;
		
		// A list of updates that have completed, we must notify the main thread about them.
		static HashSet<string> queuedUpdates;
		
		// A queue used to avoid flooding the network stack with HTTP requests
		static Queue<string> requestQueue;
		
		// Keeps id -> url mappings around
		static Dictionary<string, string> idToUrl;

		static NSString nsDispatcher = new NSString ("x");
		
		static ImageStore ()
		{
			
			PicDir = Path.Combine (Util.BaseDir, "Library/Caches/Pictures/");
			RoundedPicDir = Path.Combine (PicDir, "Rounded/");
			TmpDir = Path.Combine (Util.BaseDir, "tmp/");
			
			if (!Directory.Exists (PicDir))
				Directory.CreateDirectory (PicDir);
			
			if (!Directory.Exists (RoundedPicDir))
				Directory.CreateDirectory (RoundedPicDir);

			//DefaultImage = UIImage.FromFile ("Images/gravatar_default.png");
			cache = new LRUCache<string,UIImage> (200);
			pendingRequests = new Dictionary<string,List<IImageUpdated>> ();
			idToUrl = new Dictionary<string,string> ();
			queuedUpdates = new HashSet<string>();
			requestQueue = new Queue<string> ();
		}
		
		public static UIImage GetLocalProfilePicture (string id)
		{
			UIImage ret;
			
			lock (cache){
				ret = cache [id];
				if (ret != null)
					return ret;
			}

			lock (requestQueue){
				if (pendingRequests.ContainsKey (id))
					return null;
			}

			string picfile;
			if (id == TempStartId){
				// Delay execution of this until the user does searches.
				EnsureTmpIsClean ();
				picfile = TmpDir + id + ".png";
			} else 
				picfile = RoundedPicDir + id + ".png";
			
			if (File.Exists (picfile)){
					
				var datecreated = File.GetLastWriteTime(picfile);
				if(datecreated >= DateTime.Now.AddDays(7))
				{
					File.Delete(picfile);
					return null;
				}
				ret = UIImage.FromFileUncached (picfile);
				lock (cache)
					cache [id] = ret;
				return ret;
			}

			

			return null;
		}
		
		public static UIImage RequestProfilePicture (string id, string optionalUrl, IImageUpdated notify)
		{
			return RequestProfilePicture ( id, optionalUrl, false, notify);
		}
		
		//
		// Fetches a profile picture, the ID is used internally 
		public static UIImage RequestProfilePicture (string id, string optionalUrl,bool replaceFile, IImageUpdated notify)
		{
			UIImage pic = replaceFile ? null : GetLocalProfilePicture (id);
			if (pic == null){
				QueueRequestForPicture (id, optionalUrl, notify); 
				
				//if (id < 0)
				//	pic = GetLocalProfilePicture (-id);
				if (pic != null)
					return pic;
				
				return DefaultImage;
			}
			
			return pic;
		}
		
		static Uri GetPicUrlFromId (string id, string optionalUrl)
		{
			Uri url;
			
			if (optionalUrl == null){
				return null;
			}
			//if (id < 0){
			//	int _normalIdx = optionalUrl.LastIndexOf ("_normal");	
			//	if (_normalIdx != -1)
			//		optionalUrl = optionalUrl.Substring (0, _normalIdx) + optionalUrl.Substring (optionalUrl.Length-4);
			//}
			if (!Uri.TryCreate (optionalUrl, UriKind.Absolute, out url))
				return null;
			
			idToUrl [id] = optionalUrl;
			return url;
		}
		
		//
		// Requests that the picture for "id" be downloaded, the optional url prevents
		// one lookup, it can be null if not known
		//
		public static void QueueRequestForPicture (string id, string optionalUrl, IImageUpdated notify)
		{
			//if (notify == null)
			//	throw new ArgumentNullException ("notify");
			
			Uri url;
			lock (requestQueue)
				url = GetPicUrlFromId (id, optionalUrl);
			
			if (url == null)
				return;

			lock (requestQueue){
				if (pendingRequests.ContainsKey (id)){
					pendingRequests [id].Add (notify);
					return;
				}
				var slot = new List<IImageUpdated> (4);
				slot.Add (notify);
				pendingRequests [id] = slot;
				
				if (pendingRequests.Count >= MaxRequests){
					requestQueue.Enqueue (id);
				} else {
					ThreadPool.QueueUserWorkItem (delegate { 
							try {
								StartPicDownload (id, url); 
							} catch (Exception e){
								Console.WriteLine (e);
							//
							lock (queuedUpdates){
									queuedUpdates.Add (id);
								
									// If this is the first queued update, must notify
									if (queuedUpdates.Count == 1)										
										nsDispatcher.BeginInvokeOnMainThread (NotifyImageListeners);
								
							}
										
							//
							}
						});
				}
			}
		}

		static void EnsureTmpIsClean ()
		{
			if (TmpCleaned)
				return;
			
			foreach (string f in Directory.GetFiles (TmpDir, "*.png"))
				File.Delete (f);
			TmpCleaned = true;
		}
		
		static bool TmpCleaned;
		
		static void StartPicDownload (string id, Uri url)
		{
			do {
				var buffer = new byte [4*1024];
				string picdir = id != TempStartId ? PicDir : TmpDir;
				bool downloaded = false;
				
				try {
					var path = picdir + id + ".png";
					if(File.Exists(path))
						File.Delete(path);
					using (var file = new FileStream (path, FileMode.Create, FileAccess.Write, FileShare.Read)) {
		                	var req = WebRequest.Create (url) as HttpWebRequest;
						
		                using (var resp = req.GetResponse()) {
							using (var s = resp.GetResponseStream()) {
								int n;
								while ((n = s.Read (buffer, 0, buffer.Length)) > 0){
									file.Write (buffer, 0, n);
		                        }
							}
		                }
					}
					
					var imgURl = "http://graph.facebook.com/" + id + "/picture?type=large";
					DataAccess.parseFaces(DataAccess.faceRest.faces_detect(new List<string>{imgURl},null,null,null));
					//RoundedPic(path,id);
					downloaded = true;
				} catch (Exception e) {
					Console.WriteLine ("{0} Error fetching picture for {1}", e, id);
				}
				// Cluster all updates together
				bool doInvoke = false;
				
				lock (queuedUpdates){
						queuedUpdates.Add (id);
					
						// If this is the first queued update, must notify
						if (queuedUpdates.Count == 1)
							doInvoke = true;
					
				}
				
				lock (requestQueue){
					idToUrl.Remove (id);

					// Try to get more jobs.
					if (requestQueue.Count > 0){
						id = requestQueue.Dequeue ();
						url = new Uri(idToUrl[id]);
						if (url == null){
							pendingRequests.Remove (id);
						}
					} else
						id = "-1";
				}	
				if (doInvoke)
					nsDispatcher.BeginInvokeOnMainThread (NotifyImageListeners);
			} while (id != "-1");
		}
		
		// Runs on the main thread
		static void NotifyImageListeners ()
		{
			lock (queuedUpdates){
				foreach (var qid in queuedUpdates){
					var list = pendingRequests [qid];
					pendingRequests.Remove (qid);
					foreach (var pr in list){
						try {
							pr.UpdatedImage (qid);
						} catch (Exception e){
							Console.WriteLine (e);
						}
					}
				}
				queuedUpdates.Clear ();
			}
		}
		
		static UIImage RoundedPic (string picfile, string id)
		{
			lock (cache){				
				using (var pic = UIImage.FromFileUncached (picfile)){
					if (pic == null)
						return null;
					
					UIImage cute = Graphics.RemoveSharpEdges (pic);
					var bytes = cute.AsPNG ();
					NSError err;
					var path = RoundedPicDir + id + ".png";
					if(File.Exists(path))
						File.Delete(path);
					bytes.Save (path, false, out err);
					
					// we might as well add it to the cache
					cache [id] = cute;
					
					return cute;
				}
			}
		}
	}
}

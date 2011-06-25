
using System;
using MonoTouch.Facebook.Authorization;
using System.Collections.Generic;
using ClanceysLib;
using MonoTouch.UIKit;
namespace AsteroidsHD
{
	public class FacebookFriendFinder 
	{
		FacebookAuthorizationViewController fvc;
		//Section friendsOnCollaborate;
		//Section friendsToInvite;
		MBProgressHUD loadingView;
		
		public FacebookFriendFinder ()// : base (null,true)
		{
			loadingView = new MBProgressHUD();
			loadingView.TitleText = "Loading";
		}
		public  void ViewWillAppear (bool animated)
		{
			if(string.IsNullOrEmpty(Settings.FbAuth) || Settings.FbAuthExpire <= DateTime.Now)
			{				
				fvc = new FacebookAuthorizationViewController("172317429487848", new string[] {"publish_stream"}, FbDisplayType.Touch);
				fvc.AccessToken += delegate(string accessToken, DateTime expires) {
					Settings.FbAuth = accessToken;
					Settings.FbAuthExpire = expires;
					DataAccess.GetFriends();
					//BackgroundUpdater.AddToFacebook();				
					//this.DismissModalViewControllerAnimated(true);
				};
				fvc.AuthorizationFailed += delegate(string message) {					
					//this.DismissModalViewControllerAnimated(true);
				};
				fvc.Canceled += delegate {	
					Console.WriteLine("Canceled clicked");									
					//this.DismissModalViewControllerAnimated(true);
					//this.NavigationController.PopViewControllerAnimated(false);
				};
	
				//this.NavigationController.PresentModalViewController(fvc, false);
			}
			else
			{
				
				DataAccess.GetFriends();
				loadingView.Show(true);
				//Util.PushNetworkActive();
				//BackgroundUpdater.GetFacebookFriends(foundFriends => {foundFacebookFriends(foundFriends);});
			}
		}
		
	}
}
 

using System;
using System.Linq;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System.IO;

namespace AsteroidsHD
{
	public class FriendsViewController : DialogViewController
	{
		public FriendsViewController () : base(null,false)
		{
			var friends = Facebook.GetFriends();			
			var section = new Section();
			foreach(var friend in friends)
			{
				section.Add(new FriendElement(friend));
			}
			Root = new RootElement("Friends")
			{
				section	
			};
			TableView.BackgroundColor= UIColor.Clear;
		}
	}
	
	public class FriendElement : BooleanElement
	{
		Friend Friend {get;set;}
		public FriendElement (Friend friend) : base (friend.DisplayName,!friend.Exclude)
		{
			Friend = friend;
			this.ValueChanged  += delegate {
				Friend.Exclude = !this.Value;
				Database.Main.Update(Friend);
			};
		}
		
		public override MonoTouch.UIKit.UITableViewCell GetCell (DialogViewController dvc, MonoTouch.UIKit.UITableView tv)
		{
			var cell =  base.GetCell (dvc, tv);
			cell.ImageView.Image = ImageStore.GetLocalProfilePicture(Friend.ID);
			cell.DetailTextLabel.Text = "Shot Count: " + Friend.HitCount;
			return cell;
		}
	}
}


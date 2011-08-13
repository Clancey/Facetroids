using System;
using System.Drawing;
using SQLite;
namespace AsteroidsHD
{
	public class Friend
	{
		public Friend ()
		{
		}
		[PrimaryKey,AutoIncrement]
		public int RecordId {get;set;}
		public string OwnerID {get;set;}
		public string ID {get;set;}
		public int HitCount {get;set;}
		public int KilledByCount {get;set;}
		public string Img {get;set;}
		public string DisplayName {get;set;}
		public DateTime LastFacebookPost {get;set;}
		public bool Exclude {get;set;}
		public bool HasFace {get;set;}
	}
	
	public class Face
	{
		[PrimaryKey,AutoIncrement]
		public int RecordId {get;set;}
		public string Img {get;set;}
		public string FriendId {get;set;}
		public string OrgImage {get;set;}
		public float Height {get;set;}
		public float Width {get;set;}
		public float Cx {get;set;}
		public float Cy {get;set;}
		public float Roll {get;set;}
		[Ignore]
		public PointF Center
		{
			get { return new PointF(Cx,Cy);}
		}
		public RectangleF Rect
		{
			get{var w = (Width /2);
				var h = ( Height /2);
				return new RectangleF( w - Center.X ,h- Center.Y,Width,Height);
			}
		}
	}
}


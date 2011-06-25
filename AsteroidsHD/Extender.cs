using System;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace Astroids
{
	public static class Extender
	{
		public static UIImage FromPdf(string name)
		{
			return imageWithPDFNamed(name,UIScreen.MainScreen.Scale);
		}
		
		static CGPDFDocument CreatePDFDocumentWithName( string pdfName )
		{
			
			var name = Path.GetFileNameWithoutExtension(pdfName);
			var pdfPath = Path.GetDirectoryName(pdfName);
			var path = Path.Combine(pdfPath,name + ".pdf");
			
			CGPDFDocument doc = CGPDFDocument.FromFile(path);
		
			return doc ;
		}	
		
		static UIImage imageWithPDFPage (CGPDFPage page, float scale ,CGAffineTransform t )
		{
			if (page == null)
			{
				return null ;
			}
		
			RectangleF box = page.GetBoxRect(CGPDFBox.Crop);
		
			t.Scale(scale,scale);
			box = new RectangleF(box.Location,new SizeF(box.Size.Width * scale, box.Size.Height * scale));
		
			var pixelWidth = box.Size.Width ;
			CGColorSpace cs = CGColorSpace.CreateDeviceRGB() ;
			//DebugAssert( cs ) ;
			var _buffer = Marshal.AllocHGlobal((int)(box.Width * box.Height));
			UIGraphics.BeginImageContext(box.Size);
			CGContext c = UIGraphics.GetCurrentContext();
			cs.Dispose();
			c.ConcatCTM(t);
			c.DrawPDFPage(page);
		
			var image = UIGraphics.GetImageFromCurrentImageContext();
			return image ;
		}
		
		static UIImage[] imagesWithPDFNamed (string name, float scale ,CGAffineTransform t)
		{
			CGPDFDocument doc = CreatePDFDocumentWithName(name ) ;
			List<UIImage> images = new List<UIImage>();
		
			// PDF pages are numbered starting at page 1
			for( int pageIndex=1; pageIndex <= doc.Pages; ++pageIndex )
			{
				var page = doc.GetPage(pageIndex);
				UIImage image = imageWithPDFPage(page,scale,t);
				if(image != null)
					images.Add(image);
			}
		
			return images.ToArray() ;
		}
		
		
		static UIImage[] imagesWithPDFNamed (string name,float scale)
		{
			return imagesWithPDFNamed(name,scale,CGAffineTransform.MakeIdentity());
		}


		static UIImage imageWithPDFNamed (string name,float scale,CGAffineTransform t)
		{
			CGPDFDocument doc = CreatePDFDocumentWithName(name ) ;
			if ( doc == null)
			{
				return null ;
			}
		
			// PDF pages are numbered starting at page 1
			CGPDFPage page = doc.GetPage(1);
			
			var result = imageWithPDFPage(page,scale,t);
			return result ;
		}

		static UIImage imageWithPDFNamed (string name, float scale)
		{
			return imageWithPDFNamed(name,scale,CGAffineTransform.MakeIdentity());
		}

		
		
		
		
	}
	

}


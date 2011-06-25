using System;
using OpenTK.Graphics.ES11;
using OpenTK;
using System.Collections.Generic;
namespace AsteroidsHD
{
	public class ObjModel
	{
		public Vector3[] Verticies {get;set;}
		public Vector3[] Normals {get;set;}
		public Vector2[] TextureCoords {get;set;}
		public ObjModel()
		{
		}
	}
}


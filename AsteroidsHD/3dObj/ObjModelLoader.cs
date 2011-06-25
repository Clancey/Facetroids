using System;
using System.IO;
using OpenTK;
using System.Collections.Generic;
namespace AsteroidsHD
{
	public static class ObjModelLoader
	{
		public static ObjModel LoadFromFile(string fileName)
		{
			using (var stream = File.OpenRead(fileName))
			{
				//ObjModel model = new ObjModel();
				StreamReader reader = new StreamReader(	stream);
				var normalsV = new List<Vector3>();
				var textureCoordsV = new List<Vector2>();
				var verticesV = new List<Vector3>();
				List<Vector2> textureCoords = new List<Vector2>();
				List<Vector3> normals = new List<Vector3>();
				
				List<Vector3> vectors = new List<Vector3>();
				
				while(!reader.EndOfStream)
				{
					var line = reader.ReadLine();
					// "vn" indicates normal data
					if (line.StartsWith("vn")) {
						normalsV.Add(ReadVector3(line));
					// "vt" indicates texture coordinate data
					} else if (line.StartsWith("vt")) {
						textureCoordsV.Add(ReadVector2(line));
					// "v" indicates vertex data
					} else if (line.StartsWith("v")) {
						verticesV.Add(ReadVector3(line));
					// "f" indicates a face
						
					} else if (line.StartsWith("f")) {	
						//TODO: right now it only works on faces that have 3 points. Need to add splitting to support additional points
						
						var values = line.Split(new char[]{char.Parse(" ")});
						var point1 = values[1].Split(new char[]{char.Parse("/")});
						var point2 = values[2].Split(new char[]{char.Parse("/")});
						var point3 = values[3].Split(new char[]{char.Parse("/")});
						
						//Point1
						Vector3 v1 = verticesV[int.Parse(point1[0]) -1];
						Vector2 t1 = textureCoordsV[int.Parse(point1[1]) -1];
						Vector3 n1 = normalsV[int.Parse(point1[2]) -1];
						
						
						//Point2
						Vector3 v2 = verticesV[int.Parse(point2[0]) -1];
						Vector2 t2 = textureCoordsV[int.Parse(point2[1]) -1];
						Vector3 n2 = normalsV[int.Parse(point2[2]) -1];
						
						
						
						//Point3
						Vector3 v3 = verticesV[int.Parse(point3[0]) -1];
						Vector2 t3 = textureCoordsV[int.Parse(point3[1]) -1];
						Vector3 n3 = normalsV[int.Parse(point3[2]) -1];
						
						
						vectors.Add(v1);
						vectors.Add(v2);
						vectors.Add(v3);
						
						textureCoords.Add(t1);
						textureCoords.Add(t2);
						textureCoords.Add(t3);
						
						normals.Add(n1);
						normals.Add(n2);						
						normals.Add(n3);
						
					}
					
					
				}
				return new ObjModel(){Verticies = vectors.ToArray(),
									Normals = normals.ToArray(),
									TextureCoords = textureCoords.ToArray()};
				
			}
		}
		
		private static Vector3 ReadVector3(string line)
		{
			var values = line.Split(new char[]{char.Parse(" ")});
			return new Vector3{ X = float.Parse(values[1])
				,Y = float.Parse(values[2])
				,Z = float.Parse(values[3]) };	
		}
		private static Vector2 ReadVector2(string line)
		{
			var values = line.Split(new char[]{char.Parse(" ")});
			return new Vector2{ X = float.Parse(values[1]),Y = float.Parse(values[2])};
		}
	}
}


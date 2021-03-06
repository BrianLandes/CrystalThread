﻿using ClipperLib;
using Poly2Tri;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using HexMap.Pathfinding;
using System;
using Porter.Navigation;

namespace HexMap {

	using ClipperPolygon = List<IntPoint>;
	using Polygons = List<List<IntPoint>>;

	/// <summary>
	/// Convenience component that gets added to instantiated HexTiles.
	/// Stores data pertaining to this tile and opens up methods for common functions.
	/// Most of the functionality assumes that the mesh is a hexagon generated by HexagonMaker
	/// </summary>
	[RequireComponent(typeof(MeshFilter))]
	public class HexTile : MonoBehaviour {
		
		
		/// <summary>
		/// Axial coordinates of the tile
		/// </summary>
		public int column;
		public int row;
		
		public HexMap map;

		public HexMetrics metrics;

		public NavChunk navChunk;

		public List<Mesh> reusableMeshes = new List<Mesh>();
		

		public void AddStaticObstacle( PolyShape obstacle ) {

		}

		public void RemoveStaticObstacle(PolyShape obstacle) {

		}
		

		public void BuildNavMesh() {
			navChunk = new NavChunk(map, metrics, column, row);
			navChunk.BuildNavMesh();
		}
		
		IEnumerable<DelaunayTriangle> GetTriangles() {
			foreach( var triangle in navChunk.triangles ) {
				yield return triangle;
			}
		}

		public bool CheckQuadCollidesWithConstrainedEdges( Vector2[] quad ) {
			return navChunk.CheckQuadCollidesWithConstrainedEdges(quad);
		}
		
	}
	
	
}

﻿using UnityEngine;
using System.Collections;

public class MapPreview : MonoBehaviour {

	public Renderer textureRender;
	public MeshFilter meshFilter;
	public MeshRenderer meshRenderer;


	public enum DrawMode {NoiseMap, Mesh, FalloffMap, HeatMap, TreeMap};
	public DrawMode drawMode;

	public MeshSettings meshSettings;
	public HeightMapSettings heightMapSettings;
    public HeatMapSettings heatMapSettings;
    public TreeHeightMapSettings treeMapSettings;
	public TextureData textureData;

	public Material terrainMaterial;

//	public Object[] Trees;

	[Range(0,MeshSettings.numSupportedLODs-1)]
	public int editorPreviewLOD;
	public bool autoUpdate;




	public void DrawMapInEditor() {
		textureData.ApplyToMaterial (terrainMaterial);
		textureData.UpdateMeshHeights (terrainMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);
		HeightMap heightMap = HeightMapGenerator.GenerateHeightMap (meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, Vector2.zero);
        HeatMap heatMap = HeatMapGenerator.GenerateHeatMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heatMapSettings, Vector2.zero);
        TreeMap treeMap = TreeMapGenerator.generateVegetationMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine,heightMap, heatMap, treeMapSettings, Vector2.zero);

        if (drawMode == DrawMode.NoiseMap)
        {
            DrawTexture(TextureGenerator.TextureFromHeightMap(heightMap));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            DrawMesh(MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, editorPreviewLOD));
			DrawTrees (TreeMapGenerator.getTreeTransforms(treeMap, heightMap));
        }
        else if (drawMode == DrawMode.FalloffMap)
        {
            DrawTexture(TextureGenerator.TextureFromHeightMap(new HeightMap(FalloffGenerator.GenerateFalloffMap(meshSettings.numVertsPerLine), 0, 1)));
        }
        else if (drawMode == DrawMode.HeatMap)
        {
            DrawTexture(TextureGenerator.TextureFromHeatMap(heatMap));
        }
        else if (drawMode == DrawMode.TreeMap) {
            DrawTexture(TextureGenerator.TextureFromTreeMap(treeMap));
        }
	}





	public void DrawTexture(Texture2D texture) {
		textureRender.sharedMaterial.mainTexture = texture;
		textureRender.transform.localScale = new Vector3 (texture.width, 1, texture.height) /10f;

		textureRender.gameObject.SetActive (true);
		meshFilter.gameObject.SetActive (false);
	}

	public void DrawMesh(MeshData meshData) {
		meshFilter.sharedMesh = meshData.CreateMesh ();

		textureRender.gameObject.SetActive (false);
		meshFilter.gameObject.SetActive (true);
	}

	public void DrawTrees(Vector4[] transforms){
		Object tree = Resources.Load ("Trees/Prefabs/Fir_Tree");
//		Trees = new Object[transforms.GetLength];
		for (int i = 0; i < transforms.Length; i++) {
			Vector4 transform = transforms [i];
			if (transform[3] != 0) {
				Instantiate (tree, new Vector3(transform[0], transform[1], transform[2]), Quaternion.identity);
			}
		}
	}

//	public void DestroyObjects(){
////		foreach (Object tree in Trees) {
////			this.DestroyObjects()
////		}
//		this.DestroyObjects();
//	}



	void OnValuesUpdated() {
		if (!Application.isPlaying) {
			DrawMapInEditor ();
		}
	}

	void OnTextureValuesUpdated() {
		textureData.ApplyToMaterial (terrainMaterial);
	}

	void OnValidate() {

		if (meshSettings != null) {
			meshSettings.OnValuesUpdated -= OnValuesUpdated;
			meshSettings.OnValuesUpdated += OnValuesUpdated;
		}
		if (heightMapSettings != null) {
			heightMapSettings.OnValuesUpdated -= OnValuesUpdated;
			heightMapSettings.OnValuesUpdated += OnValuesUpdated;
		}
		if (textureData != null) {
			textureData.OnValuesUpdated -= OnTextureValuesUpdated;
			textureData.OnValuesUpdated += OnTextureValuesUpdated;
		}
        if (heatMapSettings != null)
        {
            heatMapSettings.OnValuesUpdated -= OnValuesUpdated;
            heatMapSettings.OnValuesUpdated += OnValuesUpdated;
        }
        if (treeMapSettings != null)
        {
            treeMapSettings.OnValuesUpdated -= OnValuesUpdated;
            treeMapSettings.OnValuesUpdated += OnValuesUpdated;
        }
    }

}

using UnityEngine;
using System.Collections;

public class MapPreview : MonoBehaviour {

	public Renderer textureRender;
	public MeshFilter meshFilter;
	public MeshRenderer meshRenderer;


	public enum DrawMode {HeightMap, Mesh, FalloffMap, HeatMap, TreeMap};
	public DrawMode drawMode;

	public MeshSettings meshSettings;
	public HeightMapSettings heightMapSettings;
    public HeatMapSettings heatMapSettings;
    public TreeHeightMapSettings treeMapSettings;
	public TextureData textureData;

	public Material terrainMaterial;

	[Range(0,MeshSettings.numSupportedLODs-1)]
	public int editorPreviewLOD;
	public bool autoUpdate;

	public void DrawMapInEditor() {
		textureData.ApplyToMaterial (terrainMaterial);
		textureData.UpdateMeshHeights (terrainMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);
		HeightMap heightMap = HeightMapGenerator.GenerateHeightMap (meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, Vector2.zero);
        HeatMap heatMap = HeatMapGenerator.GenerateHeatMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMap, heatMapSettings, Vector2.zero);
        TreeMap treeMap = TreeMapGenerator.generateVegetationMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMap, heatMap, treeMapSettings, Vector2.zero);
        
		DestroyTrees (); //temporary fix

        if (drawMode == DrawMode.HeightMap)
        {
            DrawTexture(TextureGenerator.TextureFromHeightMap(heightMap));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            DrawMesh(MeshGenerator.GenerateTerrainMesh2(heightMap.values, meshSettings, editorPreviewLOD,heatMap.values));
            DrawTrees(heightMap.values, meshSettings,treeMap.values);
        }
        else if (drawMode == DrawMode.FalloffMap)
        {
            DrawTexture(TextureGenerator.TextureFromHeightMap(new HeightMap(FalloffGenerator.GenerateFalloffMap(meshSettings.numVertsPerLine), 0, 1)));
        }
        else if (drawMode == DrawMode.HeatMap)
        {
            DrawTexture(TextureGenerator.TextureColorizedFromHeatMap(heatMap));
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

	void DestroyTrees(){
		GameObject[] fooObjs = GameObject.FindGameObjectsWithTag("Tree");
		foreach (GameObject fooObj in fooObjs)
		{
			if (fooObj.name == "Olive_Tree_Prefab(Clone)")
			{
				DestroyImmediate(fooObj); //temporary fix
			}
		}
	}

    public void DrawTrees(float[,] heightMap, MeshSettings meshSettings, float[,] treeMap) {
		Object tree = Resources.Load("Trees/Olive_Tree/Olive_Prefab/Olive_Tree_Prefab");
        int numVertsPerLine = meshSettings.numVertsPerLine;
        Vector2 topLeft = new Vector2(-1, 1) * meshSettings.meshWorldSize / 2f;
		System.Random rng = new System.Random();
        for (int y = 0; y < numVertsPerLine; y++) {
            for (int x = 0; x < numVertsPerLine; x++){
                Vector2 percent = new Vector2(x - 1, y - 1) / (numVertsPerLine - 3);
                Vector2 vertexPosition2D = topLeft + new Vector2(percent.x, -percent.y) * meshSettings.meshWorldSize;
                float height = heightMap[numVertsPerLine - 1 - x, y];
//                float p = treeMap[x,y]*0.1f;
				float rotation = (float) rng.NextDouble() * 180;
//				Debug.Log (rotation);

				if (treeMap[x, y] > 0) { 
					GameObject treeObject = (GameObject) Instantiate(tree, 
								new Vector3(-vertexPosition2D.x, height, vertexPosition2D.y), //position
						Quaternion.Euler(new Vector3(-90, rotation, 0))); //rotation 
					treeObject.transform.localScale *= treeMap[x,y] * 2;
				}

            }
        }
    }


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

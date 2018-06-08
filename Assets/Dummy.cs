using UnityEngine;
using System.Collections;

public class Dummy : MonoBehaviour {

	#region Public fields
	// simulation itterations
	public uint			itterations = 10; 

	// render texture width/height
	public uint			RenderTextureWidth = 512;

	// reaction diffusion parameters
	[Range(0, 50f)]
	public float RDDiffuseA = 0.8f;
	[Range(0, 50f)]
	public float RDDiffuseB = 0.8f;
	[Range(0, 5f)]
	public float RDFeed = 0.037f;
	[Range(0, 5f)]
	public float RDKill = 0.06f;
	[Range(0,1)]
	public float 		RDClear = 0;
	public float		RDBrushSize = 1;
		public Material 		mReactionDiffusionMaterial;

	#endregion


	#region Private fields
	// holds the final colorized image (rgb channels)
	private RenderTexture mRenderTexture1 = null;

	// these two 'ping pong' textures hold the chemical quantities (in rg channels)
	private RenderTexture mRenderTexture3 = null;
	private RenderTexture mRenderTexture4 = null;

	// materials used for drawing into the textures
	private Material		mReactionDiffusionColorMapMaterial = null;
	#endregion


	#region Private functions
	// ------------------------------------------------------------------------------------------------------------------------
	private RenderTexture _createTexture(int w, int h){
		RenderTexture t = new RenderTexture(w, h, 0, RenderTextureFormat.ARGBFloat);
		t.filterMode = FilterMode.Point;
		t.anisoLevel = 1;
		t.autoGenerateMips = false;
		t.useMipMap = false;
		t.Create();
		return t;
	}
	#endregion


	#region GameObject functions
	// ------------------------------------------------------------------------------------------------------------------------
	void Start () {

		// create some textures
	 	mRenderTexture1 = _createTexture((int)RenderTextureWidth, (int)RenderTextureWidth);
		mRenderTexture3 = _createTexture((int)RenderTextureWidth, (int)RenderTextureWidth);
		mRenderTexture4 = _createTexture((int)RenderTextureWidth, (int)RenderTextureWidth);

		mRenderTexture1.filterMode = FilterMode.Bilinear;
	
		// create some shaders
		mReactionDiffusionColorMapMaterial = new Material(Shader.Find("Custom/ReactionDiffusion-ColorMap"));

		// clear initial values
		RDClear = 1;
		Update ();
		RDClear = 0;
	}
	
	// ------------------------------------------------------------------------------------------------------------------------
	void Update () {
		RenderTexture temp = null;

		// step 1: update reaction-diffusion parameters
		mReactionDiffusionMaterial.SetFloat("feed", RDFeed);
		mReactionDiffusionMaterial.SetFloat("kill", RDKill);
		mReactionDiffusionMaterial.SetFloat("diffuseA", RDDiffuseA);
		mReactionDiffusionMaterial.SetFloat("diffuseB", RDDiffuseB);
		mReactionDiffusionMaterial.SetFloat("clear", RDClear);

		// set brush position if user clicked somewhere in the screen
		Vector4 brushPosition = Vector4.zero;
		if (Input.GetMouseButton (0)) {
			brushPosition = new Vector4(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height, 0, 1);
		}
		mReactionDiffusionMaterial.SetVector("brushPosition", brushPosition);
		mReactionDiffusionMaterial.SetFloat ("brushSize", RDBrushSize / RenderTextureWidth);

		// do the quantity update calculations
		// mRenderTexture3 contains the most up-to-date quantities
		// mRenderTexture4 is being rendered into
		if (itterations < 1) {
			itterations = 1; 
		}

		for (int i = 0; i < itterations; i++) {
			Graphics.Blit (mRenderTexture3, mRenderTexture4, mReactionDiffusionMaterial);
			Graphics.Blit (mRenderTexture4, mRenderTexture3, mReactionDiffusionMaterial);
		}

		// swap mRenderTexture3 and 4 (ping-pong)
		//temp = mRenderTexture3;
		//mRenderTexture3 = mRenderTexture4;
		//mRenderTexture4 = temp;

		// step 2 - do the colormapping
		// apply colormapping to the most up to date quantities
		Graphics.Blit (mRenderTexture3, mRenderTexture1, mReactionDiffusionColorMapMaterial);
	}


	// ------------------------------------------------------------------------------------------------------------------------
	void OnGUI(){
		// draw the colorized texture fullscreen
		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), mRenderTexture1, ScaleMode.StretchToFill);

		// draw the two buffer textures
		GUI.DrawTexture (new Rect (10, 10, 100, 100), mRenderTexture3, ScaleMode.ScaleToFit);
		GUI.DrawTexture (new Rect (120, 10, 100, 100), mRenderTexture4, ScaleMode.ScaleToFit);

		GUI.skin.label.normal.textColor = Color.black;

	}
	#endregion
}



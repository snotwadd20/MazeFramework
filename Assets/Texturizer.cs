using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class Texturizer
{
    //*******************************************************
    //PUBLIC
    //*******************************************************
    public Texture2D texture = null;

    public Color color = Color.white;

    public string theName = "";

    public int pixelWidth = 16;
    public int pixelHeight = 16;

    public float pixelsToUnits = 16;

    public bool decorativeMesh = false;

    public static Dictionary<string, Texture2D[]> tileLists = null;
    public static Dictionary<int, int> autoTileLookup = null;

    public float outlineSize = 0.2f;
    public float outlineBrightness = 0.75f;

    //*******************************************************
    //ACCESSORS
    //*******************************************************

    public int texWidth
    {
        get 
        {
            int wd = 0;
            if(texture)
                wd = texture.width;
            
            return wd;
        }//get
    }//texWidth
    
    public int texHeight
    {
        get 
        {
            int ht = 0;
            if(texture)
                ht = texture.height;
            
            return ht;
        }//get
    }//texHeight

    public Texturizer(Color theColor,int pixelWidth = 16, int pixelHeight = 16)
    {
        this.pixelWidth = pixelWidth;
        this.pixelHeight = pixelHeight;
        color = theColor;
        
        if(tileLists == null)
            tileLists = new Dictionary<string, Texture2D[]>();
        
        if(autoTileLookup == null)
        {
            autoTileLookup = new Dictionary<int, int>();
            initializeLookuptable();
        }//if
    }//Constructor

    //*******************************************************
    // SETUP FUNCTIONS
    //*******************************************************
    public void setupFromTexture(Texture2D theTexture)
    {
        texture = theTexture;
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Repeat;
    }//setupFromTexture

    public void setupCircle()
    {
        texture = new Texture2D(pixelWidth, pixelHeight);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Repeat;

        CircleOnTex(texture, pixelWidth/2, pixelHeight/2, pixelWidth/2-1, color, outlineSize, outlineBrightness);

        texture.Apply();
    }//setupCircle

    public void setupTileStrip()
    {
        Texture2D[] allTextures = new Texture2D[16];

        //Make all 16 tiles
        for(int i = 0; i < allTextures.Length; i++)
        {
            Texture2D tex = new Texture2D(pixelWidth, pixelHeight);
            tex.filterMode = FilterMode.Point;
            tex.wrapMode = TextureWrapMode.Repeat;
            
            int outlineInt = Mathf.FloorToInt(outlineSize * 10 + 1);
            for(int y=0; y < pixelHeight; y++)
            {
                for(int x=0; x < pixelWidth; x++)
                {
                    Color pixelColor = color;
                    
                    Color altColor;
                    altColor = UColor.ChangeRelativeBrightness(color, outlineBrightness);
                    altColor.a = 1.0f;
                    
                    if(isNo (i, 1)) // up
                    {
                        if(y >= pixelHeight-outlineInt - 1)
                            pixelColor = altColor;
                    }//if
                    
                    if(isNo (i, 2)) // right
                    {
                        if(x >= pixelWidth-outlineInt - 1)
                            pixelColor = altColor;
                    }//if
                    
                    if(isNo (i, 4)) // down
                    {
                        if(y <= outlineInt)
                            pixelColor = altColor;
                    }//if
                    
                    if(isNo (i, 8)) // left
                    {
                        if(x <= outlineInt)
                            pixelColor = altColor;
                    }//if
                    
                    tex.SetPixel(x,y, pixelColor);
                }//for
            }//for
            tex.Apply();
            allTextures[i] = tex;
        }//for
    
        tileLists.Add(color.ToString(), allTextures);

        Texture2D stripTex = new Texture2D(pixelWidth*allTextures.Length, pixelHeight);
        stripTex.filterMode = FilterMode.Point;
        stripTex.wrapMode = TextureWrapMode.Clamp;

        for(int i=0; i < allTextures.Length; i++)
        {
            stripTex.SetPixels(pixelWidth*i, 0, pixelWidth, pixelHeight, allTextures[i].GetPixels());
        }//for
        stripTex.Apply();
        texture = stripTex;
    }//setupTileStrip

    public void setupTile(string texName, int links)
    {
        texture = tileLists[texName][links];
    }//setupTile

    public void setupTile(int links)
    {
        //If we've done it already, set the pointer to the existing texture
        if(!tileLists.ContainsKey(color.ToString()))
        {
            Texture2D[] allTextures = new Texture2D[16];
            //Make all 16 tiles
            for(int i = 0; i < allTextures.Length; i++)
            {
                Texture2D tex = new Texture2D(pixelWidth, pixelHeight);
                tex.filterMode = FilterMode.Point;
                tex.wrapMode = TextureWrapMode.Repeat;
                
                int outlineInt = Mathf.FloorToInt(outlineSize * 10 + 1);
                for(int y=0; y < pixelHeight; y++)
                {
                    for(int x=0; x < pixelWidth; x++)
                    {
                        Color pixelColor = color;

                        Color altColor;
                        altColor = UColor.ChangeRelativeBrightness(color, outlineBrightness);
                        altColor.a = 1.0f;

                        if(isNo (i, 1)) // up
                        {
                            if(y >= pixelHeight-outlineInt - 1)
                                pixelColor = altColor;
                        }//if

                        if(isNo (i, 2)) // right
                        {
                            if(x >= pixelWidth-outlineInt - 1)
                                pixelColor = altColor;
                        }//if

                        if(isNo (i, 4)) // down
                        {
                            if(y <= outlineInt)
                                pixelColor = altColor;
                        }//if

                        if(isNo (i, 8)) // left
                        {
                            if(x <= outlineInt)
                                pixelColor = altColor;
                        }//if
                       
                        tex.SetPixel(x,y, pixelColor);
                    }//for
                }//for
                tex.Apply();
                allTextures[i] = tex;
            }//for
            tileLists.Add(color.ToString(), allTextures);
        }//if

        //Point it at the requested one
        texture = tileLists[color.ToString()][links];
    }//setupTile

    //*******************************************************
    //AUTOTILE
    //*******************************************************

    public bool loadAutoTile(string path, int size = 16)
    {
        Texture2D autoTex = (Texture2D)Resources.Load(path);
        if(autoTex == null)
            return false;

        tileLists[path] = new Texture2D[16];
        tileLists[path][0] = new Texture2D(size, size);

        tileLists[path][0].wrapMode = TextureWrapMode.Clamp;
        tileLists[path][0].filterMode = FilterMode.Point;

        Color[] colors = tileLists[path][0].GetPixels();

        for(int n=0; n < colors.Length; n++)
            colors[n] = Color.white;

        tileLists[path][0].SetPixels(colors);
        tileLists[path][0].Apply();

        int tilesWide = autoTex.width / size;
        int tilesHigh = autoTex.height / size;


        int i=1;
        for(int y=tilesHigh-1; y >= 0; y--)
        {
            for(int x=0; x < tilesWide; x++)
            {
                colors = autoTex.GetPixels(x*size,y*size, size,size);
                tileLists[path][atlu(i)] = new Texture2D(size, size);
                tileLists[path][atlu(i)].SetPixels(colors);
                tileLists[path][atlu(i)].wrapMode = TextureWrapMode.Clamp;
                tileLists[path][atlu(i)].filterMode = FilterMode.Point;
                tileLists[path][atlu(i)].Apply();

                i++;
                if(i > 15)
                    return true;
            }//for
        }//for

        return true;
    }//loadAutoTile

    //*******************************************************
    // HELPERS
    //*******************************************************
    //Autotile Lookup (atlu)
    private int atlu(int num)
    {
        num = Mathf.Clamp(num, 0, autoTileLookup.Count-1);
        return autoTileLookup[num];
    }//atlu
    private bool isNo(int corners, int num)
    {
        return (corners & num) == 0;
    }//corners
    private void initializeLookuptable()
    {
        autoTileLookup[0] = 0;
        autoTileLookup[1] = 6;
        autoTileLookup[2] = 14;
        autoTileLookup[3] = 12;
        autoTileLookup[4] = 7;
        autoTileLookup[5] = 15;
        autoTileLookup[6] = 13;
        autoTileLookup[7] = 3;
        autoTileLookup[8] = 11;
        autoTileLookup[9] = 9;
        autoTileLookup[10] = 5;
        autoTileLookup[11] = 10;
        autoTileLookup[12] = 4;
        autoTileLookup[13] = 2;
        autoTileLookup[14] = 8;
        autoTileLookup[15] = 1;
    }//initializeLookuptable

    //*******************************************************
    //*******************************************************
    public GameObject makeObject(float width, float height, Vector3 pivotOffset, Vector2 texScale, TextureWrapMode wrapMode = TextureWrapMode.Repeat)
    {
        texture.wrapMode = wrapMode;

        Shader shader = Shader.Find("Diffuse");

        GameObject quad = MakePlane(width, height, (int)width, (int)height, color, pivotOffset, false, theName, shader);

        //quad.transform.localScale = new Vector3(width, height, 1.0f);
        quad.GetComponent<Renderer>().material.mainTexture = texture;

        quad.GetComponent<Renderer>().material.mainTextureScale = texScale;

        if(pivotOffset != Vector3.zero)
        {
            MeshFilter mf = quad.GetComponent<MeshFilter>();
            Mesh mesh = mf.mesh;
            
            //This next bit is to set the origin of the mesh as the top-left For science
            Vector3[] verts = mesh.vertices;
            for(int i=0; i < verts.Length; i++)
            {
                verts[i] += pivotOffset; 
            }//for
            mesh.vertices = verts;
        }//if

        if(theName != "")
            quad.name = "Tile from: " + theName;
        else
            quad.name = "Texturizer texture: " + color.ToString();


        return quad;

    }//makeObject

    public GameObject makeObject()
    {
        GameObject spt = makeObject(1,1,Vector3.zero, Vector2.one);
        return spt;
    }//spawnSprite

    //*******************************************************
    //*******************************************************
    public static void CircleOnTex(Texture2D tex, int cx, int cy, int rad, Color color, float outlineThickness = 0.2f, float outlineBrightness = 0.75f)
    {
        int x, y, px, nx, py, ny, d;
        
        Color altColor;
        if(color != Color.clear)
            altColor = UColor.ChangeRelativeBrightness(color, outlineBrightness);
        else
        {
            altColor = Color.black;
        }//else
        
        Color[] fillColorArray =  tex.GetPixels();
        
        for(var i = 0; i < fillColorArray.Length; ++i)
        {
            fillColorArray[i] = Color.clear;
        }
        
        tex.SetPixels( fillColorArray );
        
        for (x = 0; x <= rad; x++)
        {
            d = (int)Mathf.Ceil(Mathf.Sqrt(rad * rad - x * x));
            for (y = 0; y <= d; y++)
            {
                Vector2 mag = new Vector2(x,y);
                Color theColor = color;
                if(mag.magnitude > rad*(1-outlineThickness))
                {
                    theColor = altColor;
                }//if
                
                px = cx + x;
                nx = cx - x;
                py = cy + y;
                ny = cy - y;
                
                tex.SetPixel(px, py, theColor);
                tex.SetPixel(nx, py, theColor);
                
                tex.SetPixel(px, ny, theColor);
                tex.SetPixel(nx, ny, theColor);
            }//for
        }//for    
    }//circleOnTex

    public static GameObject MakePlane(float width, float length, int widthSegments, int lengthSegments, Color color, Vector3 anchorOffset, bool addCollider = false, string optionalName = "", Shader shader = null)
    {
        GameObject plane = new GameObject();
        
        if(shader == null)
            shader = Shader.Find("Diffuse");
        
        
        if (!string.IsNullOrEmpty(optionalName))
            plane.name = optionalName;
        else
            plane.name = "Plane";
        
        plane.transform.position = Vector3.zero;

        MeshFilter meshFilter = (MeshFilter)plane.AddComponent(typeof(MeshFilter));
        
        MeshRenderer mr = plane.AddComponent<MeshRenderer>();
        mr.material.shader = shader;
		mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		mr.receiveShadows = false;
		mr.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
		mr.useLightProbes = false;

        if(color.a == 1)
            mr.material.color = color;
        
        string planeAssetName = plane.name + widthSegments + "x" + lengthSegments + "W" + width + "L" + length + ".asset";
        plane.name = planeAssetName;

        Mesh m = new Mesh();
        m.name = plane.name;
        
        int hCount2 = widthSegments+1;
        int vCount2 = lengthSegments+1;
        int numTriangles = widthSegments * lengthSegments * 6;
        int numVertices = hCount2 * vCount2;
        
        Vector3[] vertices = new Vector3[numVertices];
        Vector2[] uvs = new Vector2[numVertices];
        int[] triangles = new int[numTriangles];
        
        int index = 0;
        float uvFactorX = 1.0f/widthSegments;
        float uvFactorY = 1.0f/lengthSegments;
        float scaleX = width/widthSegments;
        float scaleY = length/lengthSegments;
        for (float y = 0.0f; y < vCount2; y++)
        {
            for (float x = 0.0f; x < hCount2; x++)
            {
                vertices[index] = new Vector3(x*scaleX - width/2f - anchorOffset.x, y*scaleY - length/2f - anchorOffset.y, 0.0f);
                uvs[index++] = new Vector2(x*uvFactorX, y*uvFactorY);
            }
        }
        
        index = 0;
        for (int y = 0; y < lengthSegments; y++)
        {
            for (int x = 0; x < widthSegments; x++)
            {
                triangles[index]   = (y     * hCount2) + x;
                triangles[index+1] = ((y+1) * hCount2) + x;
                triangles[index+2] = (y     * hCount2) + x + 1;
                
                triangles[index+3] = ((y+1) * hCount2) + x;
                triangles[index+4] = ((y+1) * hCount2) + x + 1;
                triangles[index+5] = (y     * hCount2) + x + 1;
                index += 6;
            }
        }
        
        m.vertices = vertices;
        m.uv = uvs;
        m.triangles = triangles;
        m.RecalculateNormals();
        
        meshFilter.sharedMesh = m;
        m.RecalculateBounds();
        
        if (addCollider)
            plane.AddComponent(typeof(BoxCollider));
        
        return plane;
        //Selection.activeObject = plane;
    }//MakePlane
}//Tile

using UnityEngine;

public static class TextureMaker
{
    public static Texture2D Generate(Vector2Int size, Material material)
    {
        Texture2D texture = new Texture2D(size.x, size.y, TextureFormat.RGBA32, false);
        RenderTexture sampleTexture = RenderTexture.GetTemporary(size.x, size.y, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);   

        Graphics.Blit(Texture2D.whiteTexture, sampleTexture, material);
        RenderTexture.active = sampleTexture;
        texture.ReadPixels(new Rect(0, 0, size.x, size.y), 0, 0);
        texture.Apply();
        RenderTexture.ReleaseTemporary(sampleTexture);
        Object.DestroyImmediate(material);

        return texture;
    }
    
    
}
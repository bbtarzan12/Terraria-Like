using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

public static class TextureMaker
{

    public static Texture2D Generate(Vector2Int size, Material material)
    {
        if (!Application.isPlaying)
        {
            Texture2D texture = new Texture2D(size.x, size.y, TextureFormat.RGBA32, false);
            RenderTexture sampleTexture = RenderTexture.GetTemporary(size.x, size.y, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);   
            CommandBuffer commandBuffer = new CommandBuffer();
            commandBuffer.SetRenderTarget(sampleTexture);
            commandBuffer.Blit(Texture2D.whiteTexture, sampleTexture, material);
            commandBuffer.CopyTexture(sampleTexture, texture);
        
            Graphics.ExecuteCommandBuffer(commandBuffer);
        
            RenderTexture.ReleaseTemporary(sampleTexture);
            commandBuffer.Dispose();
            Object.DestroyImmediate(material);
            return texture;
        }
        else
        {
            Texture2D texture = new Texture2D(size.x, size.y, TextureFormat.RGBA32, false);
            RenderTexture sampleTexture = RenderTexture.GetTemporary(size.x, size.y, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);   
            RenderTexture temp = RenderTexture.active;
        
            Graphics.Blit(Texture2D.whiteTexture, sampleTexture, material);
            RenderTexture.active = sampleTexture;
            texture.ReadPixels(new Rect(0, 0, size.x, size.y), 0, 0);
            texture.Apply();

            RenderTexture.active = temp;
            RenderTexture.ReleaseTemporary(sampleTexture);
            Object.DestroyImmediate(material);
            return texture;   
        }
    }

    public static Texture2D Generate(Texture2D source)
    {
        Texture2D texture = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
        RenderTexture sampleTexture = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
        Graphics.Blit(source, sampleTexture);
        RenderTexture temp = RenderTexture.active;
        
        RenderTexture.active = sampleTexture;
        texture.ReadPixels(new Rect(0, 0, source.width, source.height), 0, 0);
        texture.Apply();
        
        RenderTexture.ReleaseTemporary(sampleTexture);
        RenderTexture.active = temp;
        return texture; 
    }

}
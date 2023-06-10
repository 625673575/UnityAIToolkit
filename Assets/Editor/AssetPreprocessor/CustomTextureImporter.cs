using UnityEditor;
using UnityEngine;

public class CustomTextureImporter : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        TextureImporter textureImporter = (TextureImporter)assetImporter;
        var assetPathLower = assetPath.ToLower();
        //passing the textures that not exist in Assets
        if(!assetPathLower.StartsWith("assets"))
            return;
        
        if (assetPathLower.Contains("bumpmap")&& assetPathLower.Contains("normal"))
        {
            textureImporter.convertToNormalmap = true;
        }
        textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
        textureImporter.mipmapEnabled = false;
        textureImporter.isReadable = true;
        textureImporter.npotScale = TextureImporterNPOTScale.None;
        textureImporter.SaveAndReimport();
    }
}

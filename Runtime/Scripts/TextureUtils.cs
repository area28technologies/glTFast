using System.IO;
using UnityEditor;
using UnityEngine;


namespace GLTFast
{
    static class TextureUtils
    {
        internal static string ConvertTexture(string texturePath, ImageFormat format)
        {
            string extension = format == ImageFormat.Jpeg ? ".jpg" : ".png";
            string temp = "Assets/tempTexture" + extension;
            File.WriteAllBytes(temp, EncodeToFormat(new Texture2D(2, 2), format));
            AssetDatabase.ImportAsset(temp, ImportAssetOptions.ForceUpdate);
            TextureImporter textureImporter = AssetImporter.GetAtPath(temp) as TextureImporter;
            textureImporter.isReadable = true;
            textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
            textureImporter.SaveAndReimport();

            File.Copy(texturePath, temp, true);
            AssetDatabase.ImportAsset(temp, ImportAssetOptions.ForceUpdate);
            byte[] textureData = EncodeToFormat(AssetDatabase.LoadAssetAtPath<Texture2D>(temp), format);

            string currentExtension = Path.GetExtension(texturePath);
            string fileName = texturePath.Replace(currentExtension, extension);
            string projectPath = Directory.GetParent(Application.dataPath).ToString();
            string fullPath = Path.Combine(projectPath, fileName);
            File.WriteAllBytes(fullPath, textureData);
            File.Delete(temp);
            AssetDatabase.Refresh();

            return fileName;
        }

        internal static byte[] EncodeToFormat(Texture2D texture, ImageFormat format)
        {
            if (format == ImageFormat.Jpeg)
                return texture.EncodeToJPG(60);
            if (format == ImageFormat.PNG)
                return texture.EncodeToPNG();
            return null;
        }

    }
}

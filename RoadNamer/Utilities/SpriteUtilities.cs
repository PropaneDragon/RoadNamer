using ColossalFramework;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace RoadNamer.Utilities
{
    /// <summary>
    /// Stores all sprite stuff RoadNamer needs
    /// </summary>
    public static class SpriteUtilities
    {
        /*
          FYI - Atlas' are essentially sprite maps. They contain
          multiple sprites, and locations to those areas of the atlas.
          It's much better than storing individual sprites.
        */

        public static Dictionary<string, UITextureAtlas> m_atlasStore = new Dictionary<string, UITextureAtlas>();

        /// <summary>
        /// Returns a stored atlas.
        /// </summary>
        /// <param name="atlasName">The name of the atlas to return.</param>
        /// <returns></returns>
        public static UITextureAtlas GetAtlas(string atlasName)
        {
            UITextureAtlas returnAtlas = null;

            if(m_atlasStore.ContainsKey(atlasName))
            {
                returnAtlas = m_atlasStore[atlasName];
            }

            return returnAtlas;
        }

        /// <summary>
        /// Creates a new atlas from a texture and a name.
        /// </summary>
        /// <param name="texturePath">The relative (to the mod) path of the texture.</param>
        /// <param name="atlasName">The name to give the atlas. Used for finding and using later.</param>
        /// <returns></returns>
        public static bool InitialiseAtlas(string texturePath, string atlasName)
        {
            bool returnValue = false;
            string modPath = null;
            PluginManager pluginManager = Singleton<PluginManager>.instance;

            foreach (PluginManager.PluginInfo pluginInfo in pluginManager.GetPluginsInfo())
            {
                if (pluginInfo.name == "RoadNamer")
                {
                    modPath = pluginInfo.modPath;
                }
            }

            if(modPath != null)
            {
                Shader shader = Shader.Find("UI/Default UI Shader");

                if(shader != null)
                {
                    string fullPath = modPath + "/" + texturePath;

                    if(File.Exists(fullPath))
                    {
                        Texture2D spriteTexture = new Texture2D(2, 2);
                        FileStream fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
                        byte[] imageData = new byte[fileStream.Length];

                        fileStream.Read(imageData, 0, (int)fileStream.Length);
                        spriteTexture.LoadImage(imageData);

                        Material atlasMaterial = new Material(shader)
                        {
                            mainTexture = spriteTexture
                        };

                        UITextureAtlas atlas = ScriptableObject.CreateInstance<UITextureAtlas>();
                        atlas.name = atlasName;
                        atlas.material = atlasMaterial;

                        m_atlasStore.Add(atlasName, atlas);

                        returnValue = true;
                    }
                    else
                    {
                        Debug.LogError("Could not find atlas at " + fullPath);
                    }
                }
                else
                {
                    Debug.LogError("Couldn't find the default UI Shader!");
                }
            }
            else
            {
                Debug.LogError("Could not find the mod path, which is odd.");
            }

            return returnValue;
        }

        /// <summary>
        /// Creates a new sprite using the size of the image inside the atlas.
        /// </summary>
        /// <param name="dimensions">The location and size of the sprite within the atlas (in pixels).</param>
        /// <param name="spriteName">The name of the sprite to create</param>
        /// <param name="atlasName">The name of the atlas to add the sprite to.</param>
        /// <returns></returns>
        public static bool AddSpriteToAtlas(Rect dimensions, string spriteName, string atlasName)
        {
            bool returnValue = false;

            if (m_atlasStore.ContainsKey(atlasName))
            {
                UITextureAtlas foundAtlas = m_atlasStore[atlasName];
                Texture2D atlasTexture = foundAtlas.texture;
                Vector2 atlasSize = new Vector2(atlasTexture.width, atlasTexture.height);
                Rect relativeLocation = new Rect(new Vector2(dimensions.position.x / atlasSize.x, dimensions.position.y / atlasSize.y), new Vector2(dimensions.width / atlasSize.x, dimensions.height / atlasSize.y));

                Debug.Log(atlasSize.x + ", " + atlasSize.y);
                Debug.Log(relativeLocation.position.x.ToString() + ", " + relativeLocation.position.y.ToString() + ", " + relativeLocation.width.ToString() + ", " + relativeLocation.height.ToString());
                Debug.Log(dimensions.position.x.ToString() + ", " + dimensions.position.y.ToString() + ", " + dimensions.width.ToString() + ", " + dimensions.height.ToString());

                Texture2D spriteTexture = new Texture2D((int)Math.Round(dimensions.width), (int)Math.Round(dimensions.height));
                spriteTexture.SetPixels(atlasTexture.GetPixels((int)dimensions.position.x, (int)dimensions.position.y, (int)dimensions.width, (int)dimensions.height));

                UITextureAtlas.SpriteInfo createdSprite = new UITextureAtlas.SpriteInfo()
                {
                    name = spriteName,
                    region = relativeLocation,
                    texture = spriteTexture,
                    border = new RectOffset()
                };

                foundAtlas.AddSprite(createdSprite);
                returnValue = true;
            }

            return returnValue;
        }
    }
}

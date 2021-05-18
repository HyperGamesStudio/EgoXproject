//------------------------------------------
//  EgoXproject
//  Copyright © 2013-2019 Egomotion Limited
//------------------------------------------
//using System.IO;
//using System.Reflection;
//using UnityEngine;
//using System.Collections.Generic;

using System;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Egomotion.EgoXproject.UI.Internal
{
    internal class TextureResources {
     
        
        Dictionary<string, Texture2D> _resources = new Dictionary<string, Texture2D>();

        //explictly load to ensure it is done at the right time
        public void Load()
        {
            LoadAll();
        }

        

        //explictly unload to ensure it is done at the right time
        public void Unload()
        {
            foreach (var tex in _resources.Values)
            {
                if (tex != null)
                {
                    UnityEngine.Object.DestroyImmediate(tex);
                }
            }

            _resources.Clear();
        }

        public Texture2D Plus
        {
            get
            {
                return GetSkinDependentTexture("plus");
            }
        }

        public Texture2D Minus
        {
            get
            {
                return GetSkinDependentTexture("minus");
            }
        }

        public Texture2D Edit
        {
            get
            {
                return GetSkinDependentTexture("edit");
            }
        }

        public Texture2D Help
        {
            get
            {
                return GetSkinDependentTexture("help");
            }
        }

        public Texture2D Warning
        {
            get
            {
                return GetTexture("warning");
            }
        }

        public Texture2D Refresh
        {
            get
            {
                return GetSkinDependentTexture("refresh");
            }
        }

        public Texture2D TextAreaBorder
        {
            get
            {
                return GetTexture("textAreaBorder");
            }
        }

        public Texture2D TopBorder
        {
            get
            {
                return GetTexture("topBorder");
            }
        }

        Texture2D GetSkinDependentTexture(string name)
        {
            name += (EditorGUIUtility.isProSkin ? "-dark" : "-light");
            return GetTexture(name);
        }

        Texture2D GetTexture(string name)
        {
            Texture2D tex = null;
            _resources.TryGetValue(name, out tex);
            return tex;
        }


        void LoadAll()
        {
            _resources.Clear();
            LoadTexturesInResourceFile("Resources.txt");

            if (EditorGUIUtility.isProSkin)
            {
                LoadTexturesInResourceFile("Resources-dark.txt");
            }
            else
            {
                LoadTexturesInResourceFile("Resources-light.txt");
            }
        }

        void LoadTexturesInResourceFile(string fileName) {
            using (StreamReader reader = new StreamReader(XcodeEditor.BasePath()+"/Resources/"+fileName))
            {
                while (!reader.EndOfStream)
                {
                    string entry = reader.ReadLine().Trim();

                    if (entry.StartsWith("//"))
                    {
                        continue;
                    }

                    string[] elements = entry.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);

                    if (elements.Length != 3)
                    {
                        continue;
                    }

                    int w = 0, h = 0;

                    if (!int.TryParse(elements[1], out w))
                    {
                        continue;
                    }

                    if (!int.TryParse(elements[2], out h))
                    {
                        continue;
                    }

                    var tex = LoadTexture(elements[0], w, h);

                    if (tex != null)
                    {
                        _resources.Add(elements[0], tex);
                    }
                }
            }
        }

        
        Texture2D LoadTexture(string resourceName, int width, int height) {
            
            Texture2D asset = AssetDatabase.LoadAssetAtPath<Texture2D>(XcodeEditor.BasePath() + "/Resources/" + resourceName+".png");
            if (asset == null) {
                Debug.LogError("Failed to load texture: "+XcodeEditor.BasePath() + "/Resources/" + resourceName+".png");
                return null;
            }
            return asset;

        }

    }
}

// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------
//
using UnityEditor;
using UnityEngine;
using Egomotion.EgoXproject.Internal;

namespace Egomotion.EgoXproject.UI
{
    internal class MacOSXcodeEditorWindow : XcodeEditorWindow
    {
        [MenuItem("Window/EgoXproject/MacOS Xcode Project Editor", false, 1)]
        static void CreateWindow()
        {
            var win = EditorWindow.GetWindow<MacOSXcodeEditorWindow>("MacOS Xcode Editor");
            win.minSize = new Vector2(800, 400);
            win.Platform = BuildPlatform.MacOS;
            win.Show();
        }
    }
}

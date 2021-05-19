// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;

using UnityEditor.iOS.Xcode;

using Debug = UnityEngine.Debug;

namespace Egomotion.EgoXproject.Internal
{
    internal class XcodeController
    {
        public static event System.Action OnRefresh;

        static XcodeController _instance = null;

        XcodeSettings _settings;
        XcodeConfigurations _configurations;

        string _lastSaveDirectory;
        HashSet<string> _iosChangeFiles  = new HashSet<string>();
        HashSet<string> _tvosChangeFiles = new HashSet<string>();
        HashSet<string> _macosChangeFiles = new HashSet<string>();

        bool _creationOrDeletionInProgress = false;

        XcodeChangeFile _scriptingChangeFile = new XcodeChangeFile();

        XcodeController()
        {
            var upgrader = new Upgrader();
            upgrader.Upgrade();
            _lastSaveDirectory =  DllUtils.DllLocation();
            _settings = new XcodeSettings(_lastSaveDirectory);
            _configurations = new XcodeConfigurations(_lastSaveDirectory);
            FindChangeFiles();
            RefreshConfigurations();
        }

        public static XcodeController Instance()
        {
            if (_instance == null)
            {
                _instance = new XcodeController();
            }

            return _instance;
        }

        public XcodeSettings Settings
        {
            get
            {
                return _settings;
            }
        }

        public PlatformConfiguration Configuration(BuildPlatform platform)
        {
            return _configurations.Configuration(platform);
        }

        public bool IsDirty
        {
            get
            {
                //TODO others?
                return _settings.IsDirty || _configurations.IsDirty;
            }
        }

        public void Save()
        {
            if (_settings.IsDirty)
            {
                _settings.Save();
            }

            if (_configurations.IsDirty)
            {
                _configurations.Save();
            }

            //TODO save others
        }

        public string[] ChangeFiles(BuildPlatform platform)
        {
            if (platform == BuildPlatform.tvOS)
            {
                return _tvosChangeFiles.ToArray();
            }
            else if (platform == BuildPlatform.MacOS)
            {
                return _macosChangeFiles.ToArray();
            }
            else
            {
                return _iosChangeFiles.ToArray();
            }
        }

        public int ChangeFileCount(BuildPlatform platform)
        {
            if (platform == BuildPlatform.tvOS)
            {
                return _tvosChangeFiles.Count;
            }
            else if (platform == BuildPlatform.MacOS)
            {
                return _macosChangeFiles.Count;
            }
            else
            {
                return _iosChangeFiles.Count;
            }
        }

        public void Refresh()
        {
            if (_creationOrDeletionInProgress)
            {
                return;
            }

            FindChangeFiles();
            RefreshConfigurations();

            if (OnRefresh != null)
            {
                OnRefresh();
            }
        }

        void FindChangeFiles()
        {
            _iosChangeFiles.Clear();
            _tvosChangeFiles.Clear();
            _macosChangeFiles.Clear();
            string[] allPListPaths = Directory.GetFiles(Application.dataPath, "*" + XcodeChangeFile.Extension, SearchOption.AllDirectories);

            if (allPListPaths.Length > 0)
            {
                foreach (var path in allPListPaths)
                {
                    string fileName = Path.GetFileName(path);

                    if (string.IsNullOrEmpty(fileName) || fileName.StartsWith(".", StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }

                    //load and validate the files
                    var tmp = XcodeChangeFile.Load(path);

                    if (tmp != null)
                    {
                        string relativePath = ProjectUtil.MakePathRelativeToProject(path);

                        if (tmp.Platform == BuildPlatform.tvOS)
                        {
                            _tvosChangeFiles.Add(relativePath);
                        }
                        else if (tmp.Platform == BuildPlatform.MacOS)
                        {
                            _macosChangeFiles.Add(relativePath);
                        }
                        else
                        {
                            _iosChangeFiles.Add(relativePath);
                        }
                    }
                    else
                    {
                        Debug.LogError("EgoXproject: Failed to load " + path);
                    }
                }
            }
        }

        public string LastSaveDirectory
        {
            get
            {
                return _lastSaveDirectory;
            }

            private set
            {
                if (!string.IsNullOrEmpty(value) && Directory.Exists(value))
                {
                    _lastSaveDirectory = value;
                }
            }
        }

        public XcodeChangeFile CreateChangeFile(BuildPlatform platform, string filePath)
        {
            _creationOrDeletionInProgress = true;

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            var changeFile = new XcodeChangeFile();
            changeFile.Platform = platform;
            changeFile.Save(filePath);

            if (platform == BuildPlatform.tvOS)
            {
                _tvosChangeFiles.Add(filePath);
            }
            else if (platform == BuildPlatform.MacOS)
            {
                _macosChangeFiles.Add(filePath);
            }
            else
            {
                _iosChangeFiles.Add(filePath);
            }

            RefreshConfigurations();
            LastSaveDirectory = Path.GetDirectoryName(filePath);
            AssetDatabase.ImportAsset(filePath);
            _creationOrDeletionInProgress = false;
            return changeFile;
        }

        public void DeleteChangeFile(string filePath)
        {
            _creationOrDeletionInProgress = true;
            bool doDelete = false;

            if (_iosChangeFiles.Contains(filePath))
            {
                _iosChangeFiles.Remove(filePath);
                doDelete = true;
            }
            else if (_tvosChangeFiles.Contains(filePath))
            {
                _tvosChangeFiles.Remove(filePath);
                doDelete = true;
            } else if (_macosChangeFiles.Contains(filePath))
            {
                _macosChangeFiles.Remove(filePath);
                doDelete = true;
            }

            if (doDelete && File.Exists(filePath))
            {
                AssetDatabase.DeleteAsset(filePath);
            }

            RefreshConfigurations();
            _creationOrDeletionInProgress = false;
        }

        public void RemoveChangeFileFromList(BuildPlatform platform, string filePath)
        {
            if (platform == BuildPlatform.tvOS)
            {
                _tvosChangeFiles.Remove(filePath);
            }
            else if (platform == BuildPlatform.MacOS)
            {
                _macosChangeFiles.Remove(filePath);
            }
            else
            {
                _iosChangeFiles.Remove(filePath);
            }

            RefreshConfigurations();
        }

        public XcodeChangeFile MergedChanges(BuildPlatform platform)
        {
            var configuration = _configurations.Configuration(platform);
            string[] changeFiles;

            //TODO this is a reason for the Platform Configuration to hold all the change files.
            if (configuration.ActiveConfiguration == PlatformConfiguration.DEFAULT_CONFIG_NAME)
            {
                changeFiles = ChangeFiles(platform);
            }
            else
            {
                changeFiles = configuration.ChangeFilesInConfiguration(configuration.ActiveConfiguration);
            }

            XcodeChangeFile merged = new XcodeChangeFile();
            merged.Platform = platform;

            if (changeFiles == null || changeFiles.Length == 0)
            {
                return merged;
            }

            foreach (var c in changeFiles)
            {
                XcodeChangeFile cf = XcodeChangeFile.Load(c);

                if (cf != null)
                {
                    merged.Merge(cf);
                }
            }

            return merged;
        }

        public XcodeChangeFile ScriptingChangeFile
        {
            get
            {
                if (_scriptingChangeFile == null)
                {
                    _scriptingChangeFile = new XcodeChangeFile();
                }

                return _scriptingChangeFile;
            }
        }

        void RefreshConfigurations()
        {
            _configurations.Configuration(BuildPlatform.iOS).Refresh(_iosChangeFiles.ToArray());
            _configurations.Configuration(BuildPlatform.tvOS).Refresh(_tvosChangeFiles.ToArray());
            _configurations.Configuration(BuildPlatform.MacOS).Refresh(_macosChangeFiles.ToArray());
        }

        #region PostProcessBuild

        static bool IsValidBuildTarget(BuildTarget target)
        {
            if (target == BuildTarget.iOS ||
                    target == BuildTarget.tvOS)
            {
                return true;
            }

            if (target == BuildTarget.StandaloneOSX &&
                EditorUserBuildSettings.GetPlatformSettings("OSXUniversal", "CreateXcodeProject") == "true") {
                return true;
            } 
            
            return false;
        }

        [MenuItem("Test/Fake")]
        public static void FakePostBuild() {
            OnPostprocessBuild(BuildTarget.StandaloneOSX, "/Users/aresundnes/Documents/GitProjects/EgoXproject/Builds/Test");
        }


        [PostProcessBuild(5000)]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
            if (!IsValidBuildTarget(target))
            {
                return;
            }

            var controller = XcodeController.Instance();

            if (!controller.Settings.AutoRunEnabled)
            {
                return;
            }

            Debug.Log("::" + pathToBuiltProject);
            Debug.Log("::" + target);
            controller.ModifyXcodeProject(target, pathToBuiltProject);
        }
        
        private string ExecuteProcessTerminal(string argument)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo()
                {
                    FileName = "/bin/bash",
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    Arguments = " -c \"" + argument + " \""
                };
                Process myProcess = new Process
                {
                    StartInfo = startInfo
                };
                myProcess.Start();
                string output = myProcess.StandardOutput.ReadToEnd();
                string error = myProcess.StandardError.ReadToEnd();
                Debug.Log("Bash command: " + argument);
                if(!string.IsNullOrEmpty(error)) Debug.LogError(error);
                if(!string.IsNullOrEmpty(output)) Debug.Log("Bash output: "+output);
                myProcess.WaitForExit();

                return output;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return null;
            }
        }

        public void PrepareBundlePlugins(BuildTarget target, string pathToXcodeProject) {

            if(target==BuildTarget.StandaloneOSX) {

                // Loop through Xcode project to find all .bundle plugins:
                foreach(string filePath in Directory.GetDirectories(pathToXcodeProject, "*.bundle", SearchOption.AllDirectories)) {
                    // This should be the path to the main Info.plist file of the plugin
                    string plistPath = filePath + "/Contents/Info.plist";
                    
                    // This bundle does not have a Info.plist file, that's strange - let's stop now to make sure we don't break anything
                    if(!File.Exists(plistPath)) continue;

                    // Multiple Info.plist files cause problems, and the Prime31 iCloud Mac plugin has these, so let's remove them
                    string[] allInfoPlists = Directory.GetFiles(filePath, "Info.plist", SearchOption.AllDirectories);
                    if (allInfoPlists.Length > 1) {
                        for (int i = 0; i < allInfoPlists.Length; i++) {
                            // Delete any Info.plist file that's not the main one
                            if(allInfoPlists[i] != plistPath) File.Delete(allInfoPlists[i]);   
                        }
                    }
                    
                    // Delete all metafiles inside the bundle. Not necessary after Unity 2019, but let's check to be sure
                    string[] allMetaFiles = Directory.GetFiles(filePath, "*.meta", SearchOption.AllDirectories);
                    for (int i = 0; i < allMetaFiles.Length; i++) {
                        File.Delete(allInfoPlists[i]);  
                    }

                    // Let's get ready to manipulate the Plist file
                    PlistDocument pluginInfo = new PlistDocument();
                    pluginInfo.ReadFromFile(plistPath);
                    PlistElementDict pluginRoot = pluginInfo.root;
                    
                    // This is the filename of the plugin, minus extension:
                    string bundleName = Path.GetFileName(filePath).Replace(".bundle", "");
                    
                    // Set the CFBundleIdentifier to be com.company.product.bundleName
                    pluginRoot.SetString("CFBundleIdentifier",Application.identifier+"."+bundleName);

                    // Save the Plist file
                    pluginInfo.WriteToFile(plistPath);
                    
                    // Let's remove any codesigning from this plugin, so only the main app code sign is being used
                    ExecuteProcessTerminal(string.Format("codesign --remove-signature {0}", filePath));

                }
            }
        }

        public void ModifyXcodeProject(BuildTarget target, string pathToXcodeProject)
        {
            if (!IsValidBuildTarget(target))
            {
                return;
            }

            BuildPlatform platform = BuildPlatform.iOS;

            if (target == BuildTarget.tvOS)
            {
                platform = BuildPlatform.tvOS;
            } else if (target == BuildTarget.StandaloneOSX)
            {
                platform = BuildPlatform.MacOS;
            }

            PrepareBundlePlugins(target, pathToXcodeProject);

            var changes = MergedChanges(platform);

            //abort if no changes to apply
            if (!changes.HasChanges() && !_scriptingChangeFile.HasChanges())
            {
                Debug.Log("EgoXproject: No changes to apply.");
                return;
            }

            //prepare the project manipulator
            var manipulator = new XcodeProjectManipulator();

            try
            {
                if (!manipulator.Load(pathToXcodeProject))
                {
                    Debug.LogError("EgoXproject: Failed to load the Xcode project.");
                    return;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("EgoXproject had to abort due to an error: " + e.Message);
                return;
            }

            //apply configuration changes
            if (changes.HasChanges())
            {
                Debug.Log("EgoXproject: Modifying Xcode project with " + _configurations.Configuration(platform).ActiveConfiguration + " configuration.");

                try
                {
                    if (!manipulator.ApplyChanges(changes))
                    {
                        Debug.LogError("EgoXproject: Failed to fully apply the changes to the Xcode project.");
                        return;
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError("EgoXproject had to abort due to an error: " + e.Message);
                    return;
                }
            }

            //apply scripting changes
            if (_scriptingChangeFile.HasChanges())
            {
                _scriptingChangeFile.Platform = platform;
                Debug.Log("EgoXproject: Modifying Xcode project with changes set via script.");

                try
                {
                    if (!manipulator.ApplyChanges(_scriptingChangeFile))
                    {
                        Debug.LogError("EgoXproject: Failed to apply changes set via script.");
                        return;
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError("EgoXproject had to abort due to an error: " + e.Message);
                    return;
                }
            }

            Debug.Log("EgoXproject completed successfully.");
        }

        #endregion
    }
}

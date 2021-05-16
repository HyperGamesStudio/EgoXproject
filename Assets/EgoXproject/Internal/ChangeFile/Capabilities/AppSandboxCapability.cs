// ------------------------------------------
//   EgoXproject
//   Copyright © 2013-2019 Egomotion Limited
// ------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace Egomotion.EgoXproject.Internal
{
    internal class AppSandboxCapability : BaseCapability
    {
        public enum READ_WRITE_OPTIONS {
            None = 0,
            ReadOnly = 1,
            ReadWrite = 2
        }
        // Network
        public const string KEY_NETWORK_SERVER = "com.apple.security.network.server";
        public const string KEY_NETWORK_CLIENT = "com.apple.security.network.client";
        
        // Hardware
        public const string KEY_CAMERA = "com.apple.security.device.camera";
        public const string KEY_AUDIO = "com.apple.security.device.audio-input";
        public const string KEY_USB = "com.apple.security.device.usb";
        public const string KEY_PRINTING = "com.apple.security.print";
        public const string KEY_BLUETOOTH = "com.apple.security.device.bluetooth";
        
        // App Data
        public const string KEY_CONTACTS = "com.apple.security.personal-information.addressbook";
        public const string KEY_LOCATION = "com.apple.security.personal-information.location";
        public const string KEY_CALENDAR = "com.apple.security.personal-information.calendars";
        
        // File access
        public const string KEY_USER_SELECTED_FILES = "com.apple.security.files.user-selected";
        public const string KEY_DOWNLOADS_FOLDER = "com.apple.security.files.downloads";
        public const string KEY_PICTURES_FOLDER = "com.apple.security.assets.pictures";
        public const string KEY_MUSIC_FOLDER = "com.apple.security.assets.music";
        public const string KEY_MOVIES_FOLDER = "com.apple.security.assets.movies";
        
        // Access types
        public const string EXT_READ_WRITE = ".read-write";
        public const string EXT_READ_ONLY = ".read-only";



        public AppSandboxCapability()
        {
        }

        public AppSandboxCapability(PListDictionary dic)
        {
            KeyNetworkServer = dic.BoolValue(KEY_NETWORK_SERVER);
            KeyNetworkClient = dic.BoolValue(KEY_NETWORK_CLIENT);
            
            KeyCamera = dic.BoolValue(KEY_CAMERA);
            KeyAudio = dic.BoolValue(KEY_AUDIO);
            KeyUSB = dic.BoolValue(KEY_USB);
            KeyPrinting = dic.BoolValue(KEY_PRINTING);
            KeyBluetooth = dic.BoolValue(KEY_BLUETOOTH);
            
            KeyContacts = dic.BoolValue(KEY_CONTACTS);
            KeyLocation = dic.BoolValue(KEY_LOCATION);
            KeyCalendar = dic.BoolValue(KEY_CALENDAR);

            if (dic.BoolValue(KEY_USER_SELECTED_FILES + EXT_READ_ONLY)) {
                KeyUserSelectedFiles = READ_WRITE_OPTIONS.ReadOnly;
            } else if (dic.BoolValue(KEY_USER_SELECTED_FILES + EXT_READ_WRITE)) {
                KeyUserSelectedFiles = READ_WRITE_OPTIONS.ReadWrite;
            }
            else KeyUserSelectedFiles = READ_WRITE_OPTIONS.None;
            
            if (dic.BoolValue(KEY_DOWNLOADS_FOLDER + EXT_READ_ONLY)) {
                KeyDownloadsFolder = READ_WRITE_OPTIONS.ReadOnly;
            } else if (dic.BoolValue(KEY_DOWNLOADS_FOLDER + EXT_READ_WRITE)) {
                KeyDownloadsFolder = READ_WRITE_OPTIONS.ReadWrite;
            }
            else KeyDownloadsFolder = READ_WRITE_OPTIONS.None;
            
            if (dic.BoolValue(KEY_PICTURES_FOLDER + EXT_READ_ONLY)) {
                KeyPicturesFolder = READ_WRITE_OPTIONS.ReadOnly;
            } else if (dic.BoolValue(KEY_PICTURES_FOLDER + EXT_READ_WRITE)) {
                KeyPicturesFolder = READ_WRITE_OPTIONS.ReadWrite;
            }
            else KeyPicturesFolder = READ_WRITE_OPTIONS.None;
            
            if (dic.BoolValue(KEY_MUSIC_FOLDER + EXT_READ_ONLY)) {
                KeyMusicFolder = READ_WRITE_OPTIONS.ReadOnly;
            } else if (dic.BoolValue(KEY_MUSIC_FOLDER + EXT_READ_WRITE)) {
                KeyMusicFolder = READ_WRITE_OPTIONS.ReadWrite;
            }
            else KeyMusicFolder = READ_WRITE_OPTIONS.None;
            
            if (dic.BoolValue(KEY_MOVIES_FOLDER + EXT_READ_ONLY)) {
                KeyMoviesFolder = READ_WRITE_OPTIONS.ReadOnly;
            } else if (dic.BoolValue(KEY_MOVIES_FOLDER + EXT_READ_WRITE)) {
                KeyMoviesFolder = READ_WRITE_OPTIONS.ReadWrite;
            }
            else KeyMoviesFolder = READ_WRITE_OPTIONS.None;

        }

        public AppSandboxCapability(AppSandboxCapability other)
        : base (other)
        {
            KeyNetworkServer = other.KeyNetworkServer;
            KeyNetworkClient = other.KeyNetworkClient;
            
            KeyCamera = other.KeyCamera;
            KeyAudio = other.KeyAudio;
            KeyUSB = other.KeyUSB;
            KeyPrinting = other.KeyPrinting;
            KeyBluetooth = other.KeyBluetooth;
            
            KeyContacts = other.KeyContacts;
            KeyLocation = other.KeyLocation;
            KeyCalendar = other.KeyCalendar;

            KeyUserSelectedFiles = other.KeyUserSelectedFiles;
            KeyDownloadsFolder = other.KeyDownloadsFolder;
            KeyPicturesFolder = other.KeyPicturesFolder;
            KeyMusicFolder = other.KeyMusicFolder;
            KeyMoviesFolder = other.KeyMoviesFolder;
        }

        #region implemented abstract members of BaseCapability

        public override PListDictionary Serialize()
        {
            var dic = new PListDictionary();
            dic.AddIfTrue(KEY_NETWORK_SERVER, KeyNetworkServer);
            dic.AddIfTrue(KEY_NETWORK_CLIENT, KeyNetworkClient);
            
            dic.AddIfTrue(KEY_CAMERA, KeyCamera);
            dic.AddIfTrue(KEY_AUDIO, KeyAudio);
            dic.AddIfTrue(KEY_USB, KeyUSB);
            dic.AddIfTrue(KEY_PRINTING, KeyPrinting);
            dic.AddIfTrue(KEY_BLUETOOTH, KeyBluetooth);
            
            dic.AddIfTrue(KEY_CONTACTS, KeyContacts);
            dic.AddIfTrue(KEY_LOCATION, KeyLocation);
            dic.AddIfTrue(KEY_CALENDAR, KeyCalendar);
            
            if(KeyUserSelectedFiles==READ_WRITE_OPTIONS.ReadOnly) dic.Add(KEY_USER_SELECTED_FILES+EXT_READ_ONLY,true);
            else if(KeyUserSelectedFiles==READ_WRITE_OPTIONS.ReadWrite) dic.Add(KEY_USER_SELECTED_FILES+EXT_READ_WRITE,true);
            
            if(KeyDownloadsFolder==READ_WRITE_OPTIONS.ReadOnly) dic.Add(KEY_DOWNLOADS_FOLDER+EXT_READ_ONLY,true);
            else if(KeyDownloadsFolder==READ_WRITE_OPTIONS.ReadWrite) dic.Add(KEY_DOWNLOADS_FOLDER+EXT_READ_WRITE,true);
            
            if(KeyPicturesFolder==READ_WRITE_OPTIONS.ReadOnly) dic.Add(KEY_PICTURES_FOLDER+EXT_READ_ONLY,true);
            else if(KeyPicturesFolder==READ_WRITE_OPTIONS.ReadWrite) dic.Add(KEY_PICTURES_FOLDER+EXT_READ_WRITE,true);
            
            if(KeyMusicFolder==READ_WRITE_OPTIONS.ReadOnly) dic.Add(KEY_MUSIC_FOLDER+EXT_READ_ONLY,true);
            else if(KeyMusicFolder==READ_WRITE_OPTIONS.ReadWrite) dic.Add(KEY_MUSIC_FOLDER+EXT_READ_WRITE,true);
            
            if(KeyMoviesFolder==READ_WRITE_OPTIONS.ReadOnly) dic.Add(KEY_MOVIES_FOLDER+EXT_READ_ONLY,true);
            else if(KeyMoviesFolder==READ_WRITE_OPTIONS.ReadWrite) dic.Add(KEY_MOVIES_FOLDER+EXT_READ_WRITE,true);

            return dic;
        }

        public override BaseCapability Clone()
        {
            return new AppSandboxCapability(this);
        }

        #endregion
        
        // Network
        public bool KeyNetworkServer
        {
            get;
            set;
        }
        
        public bool KeyNetworkClient
        {
            get;
            set;
        }
        
        // Hardware
        public bool KeyCamera
        {
            get;
            set;
        }
        
        public bool KeyAudio
        {
            get;
            set;
        }
        
        public bool KeyUSB
        {
            get;
            set;
        }
        
        public bool KeyPrinting
        {
            get;
            set;
        }
        
        public bool KeyBluetooth
        {
            get;
            set;
        }
        
        // App Data
        public bool KeyContacts
        {
            get;
            set;
        }
        
        public bool KeyLocation
        {
            get;
            set;
        }
        
        public bool KeyCalendar
        {
            get;
            set;
        }
        
        // File access
        public READ_WRITE_OPTIONS KeyUserSelectedFiles
        {
            get;
            set;
        }
        
        public READ_WRITE_OPTIONS KeyDownloadsFolder
        {
            get;
            set;
        }
        
        public READ_WRITE_OPTIONS KeyPicturesFolder
        {
            get;
            set;
        }
        
        public READ_WRITE_OPTIONS KeyMusicFolder
        {
            get;
            set;
        }
        
        public READ_WRITE_OPTIONS KeyMoviesFolder
        {
            get;
            set;
        }

    }
}

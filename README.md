Hyper Games fork of EgoXproject README
==================
Latest version: 1.0.0, May 19 2021

GitHub: https://github.com/HyperGamesStudio/Packages-no.hypergames.egoxproject

## Overview
EgoXproject is an editor plugin for Unity. It automates Xcode project modification for iOS, tvOS and MacOS projects where Unity does not provide support.

This code was originally developed and later abandoned by Egomotion and has just been modified and added some new features, mainly MacOS support and some new config settings. Lincense and rights for the source still belongs to Egomotion and the original repo:
https://github.com/Egomotion/EgoXproject

## Installation
Install either by Git Package URL or as a Git Submodule:

### Git Package
Go to Unity Package Manager and add package by Git URL: https://github.com/HyperGamesStudio/Packages-no.hypergames.egoxproject.git 

### Sub module
Clone this repo as a submodule inside your /Packages folder in the Unity Project

## Usage
The assembly will only load in a Unity Editor running on a Mac, and with target platform set to either MacOS, iOS or tvOS
* Install package, and select Window > EgoXproject > MacOS/iOS/tvOS Xcode Editor
* Create a profile and save it in your project
* Modify your settings
* Build xcode project like normal

## Targeted Unity Versions
This code was last tested and released using:
* Unity 2019.4
* Unity 2020.3

## License
* EgoXproject is licensed under LGPL v3. If this causes you issues, please get in touch for an alternative licence.
* You may use EgoXproject in personal and commercial projects.
* As this is an editor tool that does not ship with your software it does not affect the licensing of your software or require you to release the EgoXproject source code with it.
* If you modify EgoXproject for use inside your organization (including 3rd parties, contractors etc), you do not need to publish those changes.
* You only need to publish changes if you distribute the DLL/source externally, such as part of another Unity plugin.
* You can include the DLL/source in free and paid Unity plugins or similarly distributed software. You only need to make the source available if you change it.
* You may not sell EgoXproject or modified versions on their own, or trade on the EgoXproject name. 
* If you have any questions regarding licensing please contact support@egomotion.co.uk

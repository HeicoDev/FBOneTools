<div align="center">
  <a href="https://github.com/HeicoDev/BFBC2Toolkit">
    <img alt="BFBC2Toolkit" width="200" heigth="200" src="https://i.ibb.co/ZJ4Z4sp/Battlefield-Modding-Icon.png">
  </a>
  <h1>FBOneTools</h1>
  <p>
    A collection of tools for the Frostbite 1 Engine.
  </p>
  <h1>BFBC2 Toolkit</h1>
  <p>
    A modding tool for the Frostbite 1 Engine.
  </p>
  <p>
    Supports Battlefield: Bad Company, Bad Company 2, 1943 and probably other Frostbite 1 games.
  </p>   
</div>

## Download

You can download the latest stable build on [Nexus Mods](https://www.nexusmods.com/battlefieldbadcompany2/mods/15)

## Features

* Game profiles for BFBC2 and its server
* Create mods for [BFBC2 Mod Loader](https://www.nexusmods.com/battlefieldbadcompany2/mods/4) with a few clicks 
* Edit .fbrb archives directly similar to how it works with Mod Tools v2/v3
* Edit .dbx files with the built-in text editor
* Preview textures, videos, heightmaps and several text formats
* Convert several game files with BFBC2 File Converter
* Port several game files from console to PC with BFBC2 File Porter (experimental)
* Includes all the features of Mod Tools v2/v3 and more...

Watch a preview of the features on [YouTube](https://www.youtube.com/watch?v=-WeeXXNA87M)

## Supported File Formats

* .fbrb - Extract, Archive & View Content
* .dbx - Export, Import, Preview & Editing
* .binkmemory - Export, Import & Preview
* .itexture - Export, Import & Preview
* .ps3texture & .xenontexture - Export & Preview (partially)
* .terrainheightfield - Export & Preview (16 bpp grayscale image only)
* .swfmovie - Export/Extract (for research only)

Files that can be ported from console to PC via BFBC2 File Porter:
* .terrainheightfield
* .watermesh 
* .visualwater
* .ps3texture & .xenontexture (partially)

## Build & Run Requirements

* [Visual Studio 2017](https://visualstudio.microsoft.com/vs/older-downloads/)
* [.NET Framework 4.6.1](https://www.microsoft.com/en-us/download/details.aspx?id=49982)
* [Python 2.7](https://www.python.org/downloads/release/python-2718/)
* (Optional) [Windows 10](https://www.microsoft.com/en-us/windows/get-windows-10) - It's most likely required to preview dds files properly
* (Optional) [Bink Video Codec](http://www.radgametools.com/bnkdown.htm) - Required to preview video files

NuGet Packages:
* [AvalonEdit 6.0.0](https://www.nuget.org/packages/AvalonEdit/6.0.0)
* [MahApps.Metro 1.6.5](https://www.nuget.org/packages/MahApps.Metro/1.6.5)
* [ControlzEx 3.0.2.4](https://www.nuget.org/packages/ControlzEx/3.0.2.4)

<div align="center">
  <h1>BFBC2 Mod Loader</h1>
  <p>
    A mod manager that allows you to install your client & server side mods easy & fast without modifying or replacing original game files. Works with Singleplayer, Multiplayer & Vietnam Add-On!
  </p> 
</div>

## Download

You can download the latest stable build on [Nexus Mods](https://www.nexusmods.com/battlefieldbadcompany2/mods/4)

## Features

* Easy & fast installation of BFBC2 Mod Loader compatible mods
* Modifies only a few mod archives instead of ~1000 original game archives
* Does not modify original archives/files except two tiny text files 
* Detects conflicts and blocks installation so you don't mess up your mod archive
* Disable & enable all mods and revert modified files back to original without deleting all mods
* A server and map browser that let you easily download and install custom maps
* UI for managing your mods (enable/disable and delete single mods, change load order etc.)

## Build & Run Requirements

* [Visual Studio 2017](https://visualstudio.microsoft.com/vs/older-downloads/)
* [.NET Framework 4.6.1](https://www.microsoft.com/en-us/download/details.aspx?id=49982)
* [Python 2.7](https://www.python.org/downloads/release/python-2718/)

NuGet Packages:
* [MahApps.Metro 1.6.5](https://www.nuget.org/packages/MahApps.Metro/1.6.5)
* [ControlzEx 3.0.2.4](https://www.nuget.org/packages/ControlzEx/3.0.2.4)

<div align="center">
  <h1>General</h1>
</div>

## Credits

* [Heico](https://www.nexusmods.com/users/45260312) - Author of FBOneTools
* [Frankelstner](http://www.bfeditor.org/forums/index.php?/profile/6706-frankelstner/) - For his Python scripts that handle .fbrb & .dbx files. Without his work in the BF Community my tools would not be possible! 
* [Napisal](https://www.youtube.com/channel/UCIcx-pztQ3rGfO3pbcd52OQ) & [bad .baubau](https://www.youtube.com/user/cssbaubau) - for all the time they invested to figure out how Frostbite works. Without them I would not even know about BFBC2 modding.
* [Emil Hernvall](https://github.com/EmilHernvall) - For his Python script that can extract .swfmovie files.
* [Danny Becket](https://stackoverflow.com/users/1563422/danny-beckett) - For providing the code of his IniFile class, which my tools use to handle ini files.
* [Yasirkula](https://github.com/yasirkula) for providing the code of his FileDownloader class, which BFBC2 Mod Loader uses to download large files from Google Drive.
* [Icons8](https://icons8.de/) for providing several icons, which my tools use for some buttons. ([license](https://creativecommons.org/licenses/by-nd/3.0/))
* [MahApps.Metro](https://github.com/MahApps/MahApps.Metro) library © Copyright 2019 MahApps ([license](https://github.com/MahApps/MahApps.Metro/blob/develop/LICENSE))
* [ControlzEx](https://github.com/ControlzEx/ControlzEx) library © Copyright 2015-2019 Jan Karger, Bastian Schmidt ([license](https://github.com/ControlzEx/ControlzEx/blob/develop/LICENSE))
* [AvalonEdit](https://github.com/icsharpcode/AvalonEdit) library © Copyright AvalonEdit Contributors ([license](https://github.com/icsharpcode/AvalonEdit/blob/master/LICENSE))
* [DICE](https://www.dice.se/) & [Electronic Arts](https://www.ea.com/) - For the Frostbite Engine and game series "Battlefield".
* [Microsoft](https://www.microsoft.com/) - For the development environment "Visual Studio".

## Support & Contact

If you need support, you can join [Battlefield Modding Discord](https://discord.me/battlefieldmodding) and contact me there or just contact me directly: Heico#5562.

If you enjoy my work and want to provide support, you can do so by reporting bugs, giving feedback, making suggestions, contributing to this project, helping to increase its popularity or by [donating](https://www.nexusmods.com/users/45260312) me a cup of coffee. Donations are completely optional and in no way required, but are still very much appreciated! All donations will either help to keep up and expand our servers (game servers, websites, Discord bots etc.) or for a cup of coffee that keeps me awake while working. Additionally, as a form of gratitude and recognition, donators can contact me on Discord to receive the "Donator" role and VIP access to our game servers. 

## Disclaimer

I'm not affiliated, associated, authorized, endorsed by, or in any way officially connected with Electronic Arts or DICE, or any of its subsidiaries or its affiliates.
The names Electronic Arts and DICE as well as related emblems, images, marks and names a such as Battlefield and Frostbite are registered trademarks of their respective owners.

Game files or any other form of copyrighted materials will NOT be shipped with these tools! Everybody must provide the game files on his own.
I do NOT support piracy in any way, so if you face issues, first make sure that you own a legal copy of the game!

FBOneTools © 2020 Nico Hellmund is licensed under GNU General Public License v3.0.

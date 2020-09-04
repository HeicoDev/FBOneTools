<div align="center">
  <a href="https://github.com/HeicoDev/BFBC2Toolkit">
    <img alt="BFBC2Toolkit" width="200" heigth="200" src="https://i.ibb.co/ZJ4Z4sp/Battlefield-Modding-Icon.png">
  </a>
  <h1>BFBC2Toolkit</h1>
  <p>
    A collection of tools for the Frostbite 1 Engine.
  </p>
  <p>
    Supports Battlefield: Bad Company, Bad Company 2, 1943 and probably other Frostbite 1 games.
  </p>  
</div>

## Description

For now you will need to build the tool on your own. A proper build will be released on [Nexus Mods](https://www.nexusmods.com/battlefieldbadcompany2/mods/) soon.

<b>Note:</b> The whole tool is still WIP so expect unfinished features, bugs and other issues!

<b>Features Preview:</b> https://www.youtube.com/watch?v=-WeeXXNA87M

## Supported File Formats

* .fbrb - Extract, Archive & View Content
* .dbx - Export, Import, Preview & Editing
* .binkmemory - Export, Import & Preview
* .itexture - Export, Import & Preview
* .ps3texture & xenontexture - Export & Preview (partially)
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

## Credits

* Heico - Author of BFBC2Toolkit
* [Frankelstner](http://www.bfeditor.org/forums/index.php?/profile/6706-frankelstner/) - For his Python scripts that handle .fbrb & .dbx files. Without his work in the BF Community my tools would not be possible! 
* [Napisal](https://www.youtube.com/channel/UCIcx-pztQ3rGfO3pbcd52OQ) & [bad .baubau](https://www.youtube.com/user/cssbaubau) - for all the time they invested to figure out how Frostbite works. Without them I would not even know about BFBC2 modding.
* [Emil Hernvall](https://github.com/EmilHernvall) - For his Python script that can extract .swfmovie files.
* [Danny Becket](https://stackoverflow.com/users/1563422/danny-beckett) - For providing the code of his IniFile class, which my tool uses to handle ini files.
* [Icons8](https://icons8.de/) for providing several icons, which my tool uses for some buttons. ([license](https://creativecommons.org/licenses/by-nd/3.0/))
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

Game files or any other form of copyrighted materials will NOT be shipped with this tool! Everybody must provide the game files on his own.
I do NOT support piracy in any way, so if you face issues, first make sure that you own a legal copy of the game!

BFBC2Toolkit © 2020 Nico Hellmund is licensed under GNU General Public License v3.0.

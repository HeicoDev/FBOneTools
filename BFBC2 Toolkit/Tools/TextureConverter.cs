using System;
using System.Data;
using System.Linq;
using System.IO;
using System.Windows;
using BFBC2_Toolkit.Data;
using BFBC2_Toolkit.Functions;

namespace BFBC2_Toolkit.Tools
{
    public class TextureConverter
    {
        public static void ConvertFile(string[] fileNames, bool copyToOutputFolder, bool isStandalone)
        {
            foreach (string file in fileNames)
            {
                if (file.EndsWith(".dds"))
                {
                    string fileName = Path.GetFileName(file.Replace(".dds", ".itexture")),
                           fileLocation = file.Replace(".dds", ".itexture");

                    if (copyToOutputFolder)
                        fileLocation = Dirs.OutputiTexture + @"\" + fileName;

                    File.Copy(file, fileLocation, true);

                    ConvertFileToITexture(fileLocation, isStandalone);
                }
                else if (file.EndsWith(".itexture"))
                {
                    string fileName = Path.GetFileName(file.Replace(".itexture", ".dds")),
                           fileLocation = file.Replace(".itexture", ".dds");

                    if (copyToOutputFolder)
                        fileLocation = Dirs.OutputDDS + @"\" + fileName;

                    File.Copy(file, fileLocation, true);

                    ConvertFileToDDS(fileLocation, isStandalone, false);
                }
                else if (file.EndsWith(".ps3texture"))
                {
                    string fileName = Path.GetFileName(file.Replace(".ps3texture", ".dds")),
                           fileLocation = file.Replace(".ps3texture", ".dds");

                    if (copyToOutputFolder)
                        fileLocation = Dirs.OutputDDS + @"\" + fileName;

                    File.Copy(file, fileLocation, true);

                    ConvertFileToDDS(fileLocation, isStandalone, true);
                }
                else if (file.EndsWith(".xenontexture"))
                {
                    string fileName = Path.GetFileName(file.Replace(".xenontexture", ".dds")),
                           fileLocation = file.Replace(".xenontexture", ".dds");

                    if (copyToOutputFolder)
                        fileLocation = Dirs.OutputDDS + @"\" + fileName;

                    File.Copy(file, fileLocation, true);

                    ConvertFileToDDS(fileLocation, isStandalone, true);
                }
                else if (file.EndsWith(".terrainheightfield"))
                {
                    string fileName = Path.GetFileName(file.Replace(".terrainheightfield", ".raw")),
                           fileLocation = file.Replace(".terrainheightfield", ".raw");

                    if (copyToOutputFolder)
                        fileLocation = Dirs.OutputHeightmap + @"\" + fileName;

                    File.Copy(file, fileLocation, true);

                    ConvertFileToRaw(fileLocation, isStandalone);
                }
            }
        }

        private static void ConvertFileToITexture(string fileLocation, bool isStandalone)
        {
            byte[] hexHeader = { };
            byte[] hexMain = { };

            using (var br = new BinaryReader(File.OpenRead(fileLocation)))
                hexMain = br.ReadBytes(Convert.ToInt32(br.BaseStream.Length));

            int format = hexMain[87],
                width = BitConverter.ToInt32(hexMain, 16),
                height = BitConverter.ToInt32(hexMain, 12),
                mipmapCount = BitConverter.ToInt32(hexMain, 28),
                mipmapSize = BitConverter.ToInt32(hexMain, 20),
                mipmapMinSize = 16,
                headerLength = 128;

            string textureFormat = String.Empty;

            if (format == 49)
            {
                hexHeader = StringToByteArray(FileHeaders.PcDXT1iT);
                mipmapMinSize = 8;
                textureFormat = "DXT1 BC1";
            }
            else if (format == 51)
            {
                hexHeader = StringToByteArray(FileHeaders.PcDXT3iT);
                textureFormat = "DXT3 BC2";
            }
            else if (format == 53)
            {
                hexHeader = StringToByteArray(FileHeaders.PcDXT5iT);
                textureFormat = "DXT5 BC3";
            }
            else if (format == 32)
            {
                if (hexMain[88] == 32)
                {
                    hexHeader = StringToByteArray(FileHeaders.PcARGBiT);
                    textureFormat = "ARGB8888";
                }
                else if (hexMain[88] == 8)
                {
                    hexHeader = StringToByteArray(FileHeaders.PcGrayiT);
                    textureFormat = "Grayscale";
                }
            }

            if (!isStandalone)
            {
                Vars.TextureFormat = textureFormat;
                Vars.TextureWidth = width;
                Vars.TextureHeight = height;
                Vars.MipmapCount = mipmapCount;
            }

            if (hexHeader.Length > 0)
            {
                hexHeader[16] = hexMain[16];
                hexHeader[17] = hexMain[17];
                hexHeader[18] = hexMain[18];
                hexHeader[19] = hexMain[19];
                hexHeader[20] = hexMain[12];
                hexHeader[21] = hexMain[13];
                hexHeader[22] = hexMain[14];
                hexHeader[23] = hexMain[15];
                hexHeader[28] = hexMain[28];
                hexHeader[29] = hexMain[29];
                hexHeader[30] = hexMain[30];
                hexHeader[31] = hexMain[31];

                int offset = 32;

                int[] mipmapSizes = CalculateMipmapSizes(height, width, mipmapCount, mipmapMinSize);

                for (int i = 0; i < mipmapCount; i++)
                {
                    byte[] size = BitConverter.GetBytes(mipmapSizes[i]);

                    hexHeader[offset] = size[0];
                    hexHeader[offset + 1] = size[1];
                    hexHeader[offset + 2] = size[2];
                    hexHeader[offset + 3] = size[3];

                    offset += 4;
                }

                hexMain = hexMain.Skip(headerLength).ToArray();
                hexMain = hexHeader.Concat(hexMain).ToArray();

                using (var bw = new BinaryWriter(File.OpenWrite(fileLocation)))
                {
                    bw.BaseStream.Position = 0x0;
                    bw.Write(hexMain);
                }
            }
            else
            {
                if (File.Exists(fileLocation))
                    File.Delete(fileLocation);
            }
        }

        private static void ConvertFileToDDS(string fileLocation, bool isStandalone, bool isConsoleTexture)
        {
            byte[] hexHeader = { };
            byte[] hexMain = { };

            using (var br = new BinaryReader(File.OpenRead(fileLocation)))
                hexMain = br.ReadBytes(Convert.ToInt32(br.BaseStream.Length));

            string RES = BitConverter.ToString(hexMain, 0, 3);

            int headerLength = 92;

            if (RES == "52-45-53")
                headerLength = 156;

            if (isConsoleTexture)
                hexMain = ReverseHeader(hexMain, headerLength);

            int format = BitConverter.ToInt32(hexMain, 8),
                width = BitConverter.ToInt32(hexMain, 16),
                height = BitConverter.ToInt32(hexMain, 20),
                mipmapCount = BitConverter.ToInt32(hexMain, 28);                

            if (RES == "52-45-53")
            {
                format = BitConverter.ToInt32(hexMain, 72);
                width = BitConverter.ToInt32(hexMain, 80);
                height = BitConverter.ToInt32(hexMain, 84);
                mipmapCount = BitConverter.ToInt32(hexMain, 92);                
            }            

            string textureFormat = String.Empty;

            if (format == 0 || format == 18)        
            {
                hexHeader = StringToByteArray(FileHeaders.PcDXT1);
                textureFormat = "DXT1 BC1";
            }
            else if (format == 1)                  
            {
                hexHeader = StringToByteArray(FileHeaders.PcDXT3);
                textureFormat = "DXT3 BC2";
            }
            else if (format == 2 || format == 19 || format == 20 || format == 13)
            {
                hexHeader = StringToByteArray(FileHeaders.PcDXT5);
                textureFormat = "DXT5 BC3";
            }
            else if (format == 9) 
            {
                hexHeader = StringToByteArray(FileHeaders.PcARGB);
                textureFormat = "ARGB8888";
            }
            else if (format == 10) 
            {
                hexHeader = StringToByteArray(FileHeaders.PcGray);
                textureFormat = "Grayscale";
            }

            if (!isStandalone)
            {
                Vars.TextureFormat = textureFormat;
                Vars.TextureWidth = width;
                Vars.TextureHeight = height;
                Vars.MipmapCount = mipmapCount;
            }

            if (hexHeader.Length > 0)
            {
                if (RES == "52-45-53")
                {
                    hexHeader[16] = hexMain[80];
                    hexHeader[17] = hexMain[81];
                    hexHeader[18] = hexMain[82];
                    hexHeader[19] = hexMain[83];
                    hexHeader[12] = hexMain[84];
                    hexHeader[13] = hexMain[85];
                    hexHeader[14] = hexMain[86];
                    hexHeader[15] = hexMain[87];
                    hexHeader[28] = hexMain[92];
                    hexHeader[29] = hexMain[93];
                    hexHeader[30] = hexMain[94];
                    hexHeader[31] = hexMain[95];
                    hexHeader[20] = hexMain[96];
                    hexHeader[21] = hexMain[97];
                    hexHeader[22] = hexMain[98];
                    hexHeader[23] = hexMain[99];
                }
                else
                {
                    hexHeader[16] = hexMain[16];
                    hexHeader[17] = hexMain[17];
                    hexHeader[18] = hexMain[18];
                    hexHeader[19] = hexMain[19];
                    hexHeader[12] = hexMain[20];
                    hexHeader[13] = hexMain[21];
                    hexHeader[14] = hexMain[22];
                    hexHeader[15] = hexMain[23];
                    hexHeader[28] = hexMain[28];
                    hexHeader[29] = hexMain[29];
                    hexHeader[30] = hexMain[30];
                    hexHeader[31] = hexMain[31];
                    hexHeader[20] = hexMain[32];
                    hexHeader[21] = hexMain[33];
                    hexHeader[22] = hexMain[34];
                    hexHeader[23] = hexMain[35];
                }

                hexMain = hexMain.Skip(headerLength).ToArray();
                hexMain = hexHeader.Concat(hexMain).ToArray();

                using (var bw = new BinaryWriter(File.OpenWrite(fileLocation)))
                {
                    bw.BaseStream.Position = 0x0;
                    bw.Write(hexMain);
                }
            }
            else
            {
                if (File.Exists(fileLocation))
                    File.Delete(fileLocation);
            }
        }

        private static void ConvertFileToRaw(string fileLocation, bool isStandalone)
        {
            byte[] hexMain = { };

            using (var br = new BinaryReader(File.OpenRead(fileLocation)))
                hexMain = br.ReadBytes(Convert.ToInt32(br.BaseStream.Length));

            if (!isStandalone)
            {
                Vars.TextureWidth = BitConverter.ToInt32(hexMain, 24);
                Vars.TextureHeight = BitConverter.ToInt32(hexMain, 37);
            }

            int headerLength = 49;

            if (hexMain[0] != 0)
                headerLength = 45;

            hexMain = hexMain.Skip(headerLength).ToArray();

            using (var bw = new BinaryWriter(File.OpenWrite(fileLocation)))
            {
                bw.BaseStream.Position = 0x0;
                bw.Write(hexMain);
            }
        }

        private static int[] CalculateMipmapSizes(int height, int width, int mipmapCount, int minSize)
        {
            int[] mipmaps = new int[mipmapCount];

            int mipmapSize = Math.Max(1, ((width + 3) / 4)) * Math.Max(1, ((height + 3) / 4)) * minSize;

            mipmaps[0] = mipmapSize;

            for (int i = 1; i < mipmapCount; i++)
            {
                if (mipmapSize != minSize)
                    mipmapSize = mipmapSize / 4;

                mipmaps[i] = mipmapSize;
            }

            return mipmaps;
        }

        private static byte[] ReverseHeader(byte[] hex, int length)
        {
            int offset = 0;

            while (true)
            {
                if (offset >= length)
                    break;

                hex = Reverse.FourByteBlock(hex, offset);

                offset += 4;
            }

            return hex;
        }

        private static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
    }
}

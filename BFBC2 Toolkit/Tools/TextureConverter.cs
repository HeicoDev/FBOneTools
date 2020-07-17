using System;
using System.Data;
using System.Linq;
using System.IO;
using System.Windows;
using BFBC2_Toolkit.Data;

namespace BFBC2_Toolkit.Tools
{
    public class TextureConverter
    {
        public static void ConvertFile(string[] fileNames, bool isStandalone)
        {
            int i = 0,
                j = 0;

            if (File.Exists("SkippedFiles.txt"))
                File.Delete("SkippedFiles.txt");

            foreach (string file in fileNames)
                i++;

            try
            {
                foreach (string file in fileNames)
                {
                    if (file.EndsWith(".dds"))
                    {
                        string fileName = Path.GetFileName(file.Replace(".dds", ".itexture")),
                               fileLocation = file.Replace(".dds", ".itexture");

                        if (isStandalone == true)
                            fileLocation = Dirs.outputiTexture + @"\" + fileName;

                        File.Copy(file, fileLocation, true);

                        BinaryReader br = new BinaryReader(File.OpenRead(fileLocation));
                        byte[] hexMain = br.ReadBytes(Convert.ToInt32(br.BaseStream.Length));
                        br.Close();

                        byte[] hexHeader = { };

                        //string RES = BitConverter.ToString(hexMain, 0, 3);

                        int type = hexMain[0],
                            format = hexMain[87],
                            width = BitConverter.ToInt32(hexMain, 16),
                            height = BitConverter.ToInt32(hexMain, 20),
                            mipmapN = hexMain[28],
                            mipmapS = BitConverter.ToInt32(hexMain, 21),
                            mipmapS2 = mipmapS / 4,
                            mipmapS3 = mipmapS2 / 4,
                            mipmapS4 = mipmapS3 / 4,
                            sMipmap = 16,
                            headerL = 256 / 2;

                        if (mipmapN <= 9)
                        {
                            mipmapS = BitConverter.ToInt32(hexMain, 20);
                            mipmapS2 = mipmapS / 4;
                            mipmapS3 = mipmapS2 / 4;
                            mipmapS4 = mipmapS3 / 4;
                        }

                        Vars.textureWidth = width;
                        Vars.textureHeight = height;
                        Vars.mipmapCount = Convert.ToByte(mipmapN);

                        if (format == 49)                       //DXT1
                        {
                            hexHeader = StringToByteArray(HexDB.pcDXT1iT);
                            sMipmap = 8;
                            Vars.textureFormat = "DXT1 BC1";
                        }
                        else if (format == 51)                  //DXT3
                        {
                            hexHeader = StringToByteArray(HexDB.pcDXT3iT);
                            Vars.textureFormat = "DXT3 BC2";
                        }
                        else if (format == 53)                  //DXT5
                        {
                            hexHeader = StringToByteArray(HexDB.pcDXT5iT);
                            Vars.textureFormat = "DXT5 BC3";
                        }
                        else if (format == 32)
                        {
                            if (hexMain[88] == 32)
                            {
                                hexHeader = StringToByteArray(HexDB.pcARGBiT);   //ARGB8888
                                Vars.textureFormat = "ARGB8888";
                            }
                            else if (hexMain[88] == 8)
                            {
                                hexHeader = StringToByteArray(HexDB.pcGrayiT);   //Grayscale
                                Vars.textureFormat = "Grayscale";
                            }
                        }

                        if (hexHeader.Length > 0)
                        {
                            hexHeader[16] = hexMain[16];
                            hexHeader[17] = hexMain[17];
                            hexHeader[20] = hexMain[12];
                            hexHeader[21] = hexMain[13];
                            hexHeader[28] = hexMain[28];

                            int a = 33;

                            for (int n = 0; n < mipmapN; n++)
                            {
                                switch (n)
                                {
                                    case 0:
                                        hexHeader[a] = hexMain[21];
                                        hexHeader[a + 1] = hexMain[22];
                                        a = a + 4;
                                        break;
                                    case 1:
                                        byte[] array = BitConverter.GetBytes(mipmapS2);
                                        if (mipmapN <= 9)
                                        {
                                            hexHeader[a - 1] = array[0];
                                            hexHeader[a] = array[1];
                                            a = a + 4;
                                            break;
                                        }
                                        hexHeader[a] = array[0];
                                        hexHeader[a + 1] = array[1];
                                        a = a + 4;
                                        break;
                                    case 2:
                                        byte[] array2 = BitConverter.GetBytes(mipmapS3);
                                        if (mipmapN <= 9)
                                        {
                                            hexHeader[a - 1] = array2[0];
                                            hexHeader[a] = array2[1];
                                            a = a + 4;
                                            break;
                                        }
                                        hexHeader[a] = array2[0];
                                        hexHeader[a + 1] = array2[1];
                                        a = a + 4;
                                        break;
                                    case 3:
                                        byte[] array3 = BitConverter.GetBytes(mipmapS4);
                                        if (mipmapN <= 9)
                                        {
                                            hexHeader[a - 1] = array3[0];
                                            hexHeader[a] = array3[1];
                                            a = a + 3;
                                            break;
                                        }
                                        hexHeader[a] = array3[0];
                                        hexHeader[a + 1] = array3[1];
                                        a = a + 3;
                                        break;
                                    case 4:
                                        hexHeader[a] = hexMain[21];
                                        hexHeader[a + 1] = hexMain[22];
                                        a = a + 4;
                                        break;
                                    case 5:
                                        byte[] array4 = BitConverter.GetBytes(mipmapS2);
                                        if (mipmapS <= 32 || hexHeader[48] <= 32 && mipmapN <= 9)
                                        {
                                            hexHeader[52] = Convert.ToByte(sMipmap);
                                            hexHeader[53] = 0;
                                            a = a + 4;
                                            break;
                                        }
                                        else if (mipmapN <= 9)
                                        {
                                            hexHeader[a - 1] = array4[0];
                                            hexHeader[a] = array4[1];
                                            a = a + 4;
                                            break;
                                        }
                                        hexHeader[a] = array4[0];
                                        hexHeader[a + 1] = array4[1];
                                        a = a + 4;
                                        break;
                                    case 6:
                                        byte[] array5 = BitConverter.GetBytes(mipmapS3);
                                        if (mipmapN > 11)
                                        {
                                            mipmapS3 = mipmapS3 * 2;
                                            array5 = BitConverter.GetBytes(mipmapS3);
                                        }
                                        if (mipmapS2 <= 32 || hexHeader[52] <= 32 && mipmapN <= 9)
                                        {
                                            hexHeader[56] = Convert.ToByte(sMipmap);
                                            hexHeader[57] = 0;
                                            a = a + 4;
                                            break;
                                        }
                                        else if (mipmapN <= 9)
                                        {
                                            hexHeader[a - 1] = array5[0];
                                            hexHeader[a] = array5[1];
                                            a = a + 4;
                                            break;
                                        }
                                        hexHeader[a] = array5[0];
                                        hexHeader[a + 1] = array5[1];
                                        a = a + 4;
                                        break;
                                    case 7:
                                        byte[] array6 = BitConverter.GetBytes(mipmapS4);
                                        if (mipmapN > 11)
                                        {
                                            array6 = BitConverter.GetBytes(mipmapS3 / 2);
                                        }
                                        if (mipmapS3 <= 32 || hexHeader[56] <= 32 && mipmapN <= 9)
                                        {
                                            hexHeader[60] = Convert.ToByte(sMipmap);
                                            hexHeader[61] = 0;
                                            break;
                                        }
                                        else if (mipmapN <= 9)
                                        {
                                            hexHeader[a - 1] = array6[0];
                                            hexHeader[a] = array6[1];
                                            a = a + 4;
                                            break;
                                        }
                                        hexHeader[a] = array6[0];
                                        hexHeader[a + 1] = array6[1];
                                        break;
                                    case 8:
                                        if (mipmapN > 11)
                                        {
                                            hexHeader[64] = Convert.ToByte(mipmapS3 / 4);
                                            hexHeader[65] = 0;
                                            break;
                                        }
                                        hexHeader[64] = Convert.ToByte(sMipmap);
                                        break;
                                    case 9:
                                        hexHeader[68] = Convert.ToByte(sMipmap);
                                        break;
                                    case 10:
                                        hexHeader[72] = Convert.ToByte(sMipmap);
                                        break;
                                    case 11:
                                        hexHeader[76] = Convert.ToByte(sMipmap);
                                        break;
                                    case 12:
                                        hexHeader[80] = Convert.ToByte(sMipmap);
                                        break;
                                }
                            }

                            hexMain = hexMain.Skip(headerL).ToArray();
                            hexMain = hexHeader.Concat(hexMain).ToArray();

                            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileLocation));
                            bw.BaseStream.Position = 0x0;
                            bw.Write(hexMain);
                            bw.Close();
                        }
                        else
                        {
                            if (File.Exists(fileLocation))
                                File.Delete(fileLocation);

                            StreamWriter sw = new StreamWriter("SkippedFiles.txt", true);
                            sw.WriteLine(fileName);
                            sw.Close();

                            j++;
                        }
                    }
                    else if (file.EndsWith(".itexture"))
                    {
                        string fileName = Path.GetFileName(file.Replace(".itexture", ".dds")),
                               fileLocation = file.Replace(".itexture", ".dds");

                        if (isStandalone == true)
                            fileLocation = Dirs.outputDDS + @"\" + fileName;

                        File.Copy(file, fileLocation, true);

                        BinaryReader br = new BinaryReader(File.OpenRead(fileLocation));
                        byte[] hexMain = br.ReadBytes(Convert.ToInt32(br.BaseStream.Length));
                        br.Close();

                        byte[] hexHeader = { };

                        string RES = BitConverter.ToString(hexMain, 0, 3);

                        int type = hexMain[0],
                            format = hexMain[8],
                            width = BitConverter.ToInt32(hexMain, 16),
                            height = BitConverter.ToInt32(hexMain, 20),
                            mipmapN = hexMain[28],
                            unknown1 = hexMain[34],
                            headerL = 184 / 2;

                        if (RES == "52-45-53")
                        {
                            type = hexMain[64];
                            format = hexMain[72];
                            width = BitConverter.ToInt32(hexMain, 80);
                            height = BitConverter.ToInt32(hexMain, 84);
                            mipmapN = hexMain[92];
                            unknown1 = hexMain[98];
                            headerL = 312 / 2;
                        }

                        Vars.textureWidth = width;
                        Vars.textureHeight = height;
                        Vars.mipmapCount = Convert.ToByte(mipmapN);

                        if (format == 0 || format == 18)        //DXT1
                        {
                            hexHeader = StringToByteArray(HexDB.pcDXT1);
                            Vars.textureFormat = "DXT1 BC1";
                        }
                        else if (format == 1)                   //DXT3
                        {
                            hexHeader = StringToByteArray(HexDB.pcDXT3);
                            Vars.textureFormat = "DXT3 BC2";
                        }
                        else if (format == 2 || format == 19 || format == 20 || format == 13)   //DXT5
                        {
                            hexHeader = StringToByteArray(HexDB.pcDXT5);
                            Vars.textureFormat = "DXT5 BC3";
                        }
                        else if (format == 9)                   //ARGB8888
                        {
                            hexHeader = StringToByteArray(HexDB.pcARGB);
                            Vars.textureFormat = "ARGB8888";
                        }
                        else if (format == 10)                  //Grayscale
                        {
                            hexHeader = StringToByteArray(HexDB.pcGray);
                            Vars.textureFormat = "Grayscale";
                        }

                        if (hexHeader.Length > 0)
                        {
                            hexHeader[16] = hexMain[16];
                            hexHeader[17] = hexMain[17];
                            hexHeader[12] = hexMain[20];
                            hexHeader[13] = hexMain[21];
                            hexHeader[28] = hexMain[28];
                            hexHeader[21] = hexMain[33];
                            hexHeader[22] = hexMain[34];

                            if (RES == "52-45-53")
                            {
                                hexHeader[16] = hexMain[80];
                                hexHeader[17] = hexMain[81];
                                hexHeader[12] = hexMain[84];
                                hexHeader[13] = hexMain[85];
                                hexHeader[28] = hexMain[92];
                                hexHeader[21] = hexMain[97];
                                hexHeader[22] = hexMain[98];
                            }

                            hexMain = hexMain.Skip(headerL).ToArray();
                            hexMain = hexHeader.Concat(hexMain).ToArray();

                            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileLocation));
                            bw.BaseStream.Position = 0x0;
                            bw.Write(hexMain);
                            bw.Close();
                        }
                        else
                        {
                            if (File.Exists(fileLocation))
                                File.Delete(fileLocation);

                            StreamWriter sw = new StreamWriter("SkippedFiles.txt", true);
                            sw.WriteLine(fileName);
                            sw.Close();

                            j++;
                        }
                    }
                    else if (file.EndsWith(".ps3texture"))
                    {
                        string fileName = Path.GetFileName(file.Replace(".ps3texture", ".dds")),
                               fileLocation = file.Replace(".ps3texture", ".dds");

                        if (isStandalone == true)
                            fileLocation = Dirs.outputDDS + @"\" + fileName;

                        File.Copy(file, fileLocation, true);

                        BinaryReader br = new BinaryReader(File.OpenRead(fileLocation));
                        byte[] hexMain = br.ReadBytes(Convert.ToInt32(br.BaseStream.Length));
                        br.Close();

                        byte[] hexHeader = { };

                        string RES = BitConverter.ToString(hexMain, 0, 3);

                        int type = hexMain[3],
                            format = hexMain[11],
                            width = BitConverter.ToInt32(hexMain, 17),
                            height = BitConverter.ToInt32(hexMain, 21),
                            mipmapN = hexMain[31],
                            unknown1 = hexMain[42],
                            headerL = 184 / 2;

                        if (RES == "52-45-53")
                        {
                            type = hexMain[67];
                            format = hexMain[75];
                            width = BitConverter.ToInt32(hexMain, 81);
                            height = BitConverter.ToInt32(hexMain, 85);
                            mipmapN = hexMain[95];
                            unknown1 = hexMain[106];
                            headerL = 312 / 2;
                        }

                        Vars.textureWidth = width;
                        Vars.textureHeight = height;
                        Vars.mipmapCount = Convert.ToByte(mipmapN);

                        if (format == 0 || format == 18)        //DXT1
                        {
                            hexHeader = StringToByteArray(HexDB.pcDXT1);
                            Vars.textureFormat = "DXT1 BC1";
                        }
                        else if (format == 1)                   //DXT3
                        {
                            hexHeader = StringToByteArray(HexDB.pcDXT3);
                            Vars.textureFormat = "DXT3 BC2";
                        }
                        else if (format == 2 || format == 19 || format == 20 || format == 13)   //DXT5
                        {
                            hexHeader = StringToByteArray(HexDB.pcDXT5);
                            Vars.textureFormat = "DXT5 BC3";
                        }
                        else if (format == 9)                   //ARGB8888
                        {
                            hexHeader = StringToByteArray(HexDB.pcARGB);
                            Vars.textureFormat = "ARGB8888";
                        }
                        else if (format == 10)                  //Grayscale
                        {
                            hexHeader = StringToByteArray(HexDB.pcGray);
                            Vars.textureFormat = "Grayscale";
                        }

                        if (hexHeader.Length > 0)
                        {
                            if (RES == "52-45-53")
                            {
                                if (hexMain[81] == 0 && hexMain[82] == 0)
                                {
                                    hexHeader[16] = hexMain[83];
                                    hexHeader[17] = hexMain[84];
                                }
                                else
                                {
                                    hexHeader[16] = hexMain[81];
                                    hexHeader[17] = hexMain[82];
                                }

                                if (hexMain[85] == 0 && hexMain[86] == 0)
                                {
                                    hexHeader[12] = hexMain[87];
                                    hexHeader[13] = hexMain[88];
                                }
                                else
                                {
                                    hexHeader[12] = hexMain[85];
                                    hexHeader[13] = hexMain[86];
                                }

                                hexHeader[28] = hexMain[95];
                                hexHeader[21] = hexMain[105];
                                hexHeader[22] = hexMain[106];
                            }
                            else
                            {
                                if (hexMain[17] == 0 && hexMain[18] == 0)
                                {
                                    hexHeader[16] = hexMain[19];
                                    hexHeader[17] = hexMain[20];
                                }
                                else
                                {
                                    hexHeader[16] = hexMain[17];
                                    hexHeader[17] = hexMain[18];
                                }

                                if (hexMain[21] == 0 && hexMain[22] == 0)
                                {
                                    hexHeader[12] = hexMain[23];
                                    hexHeader[13] = hexMain[24];
                                }
                                else
                                {
                                    hexHeader[12] = hexMain[21];
                                    hexHeader[13] = hexMain[22];
                                }

                                hexHeader[28] = hexMain[31];
                                hexHeader[21] = hexMain[41];
                                hexHeader[22] = hexMain[42];
                            }

                            hexMain = hexMain.Skip(headerL).ToArray();
                            hexMain = hexHeader.Concat(hexMain).ToArray();

                            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileLocation));
                            bw.BaseStream.Position = 0x0;
                            bw.Write(hexMain);
                            bw.Close();
                        }
                        else
                        {
                            if (File.Exists(fileLocation))
                                File.Delete(fileLocation);

                            StreamWriter sw = new StreamWriter("SkippedFiles.txt", true);
                            sw.WriteLine(fileName);
                            sw.Close();

                            j++;
                        }
                    }
                    else if (file.EndsWith(".xenontexture"))
                    {
                        string fileName = Path.GetFileName(file.Replace(".xenontexture", ".dds")),
                               fileLocation = file.Replace(".xenontexture", ".dds");

                        if (isStandalone == true)
                            fileLocation = Dirs.outputDDS + @"\" + fileName;

                        File.Copy(file, fileLocation, true);

                        BinaryReader br = new BinaryReader(File.OpenRead(fileLocation));
                        byte[] hexMain = br.ReadBytes(Convert.ToInt32(br.BaseStream.Length));
                        br.Close();

                        byte[] hexHeader = { };

                        string RES = BitConverter.ToString(hexMain, 0, 3);

                        int type = hexMain[3],
                            format = hexMain[11],
                            width = BitConverter.ToInt32(hexMain, 17),
                            height = BitConverter.ToInt32(hexMain, 21),
                            mipmapN = hexMain[31],
                            unknown1 = hexMain[42],
                            headerL = 184 / 2;

                        if (RES == "52-45-53")
                        {
                            type = hexMain[67];
                            format = hexMain[75];
                            width = BitConverter.ToInt32(hexMain, 81);
                            height = BitConverter.ToInt32(hexMain, 85);
                            mipmapN = hexMain[95];
                            unknown1 = hexMain[106];
                            headerL = 312 / 2;
                        }

                        Vars.textureWidth = width;
                        Vars.textureHeight = height;
                        Vars.mipmapCount = Convert.ToByte(mipmapN);

                        if (format == 0 || format == 18)        //DXT1
                        {
                            hexHeader = StringToByteArray(HexDB.pcDXT1);
                            Vars.textureFormat = "DXT1 BC1";
                        }
                        else if (format == 1)                   //DXT3
                        {
                            hexHeader = StringToByteArray(HexDB.pcDXT3);
                            Vars.textureFormat = "DXT3 BC2";
                        }
                        else if (format == 2 || format == 19 || format == 20 || format == 13)   //DXT5
                        {
                            hexHeader = StringToByteArray(HexDB.pcDXT5);
                            Vars.textureFormat = "DXT5 BC3";
                        }
                        else if (format == 9)                   //ARGB8888
                        {
                            hexHeader = StringToByteArray(HexDB.pcARGB);
                            Vars.textureFormat = "ARGB8888";
                        }
                        else if (format == 10)                  //Grayscale
                        {
                            hexHeader = StringToByteArray(HexDB.pcGray);
                            Vars.textureFormat = "Grayscale";
                        }

                        if (hexHeader.Length > 0)
                        {
                            if (RES == "52-45-53")
                            {
                                if (hexMain[81] == 0 && hexMain[82] == 0)
                                {
                                    hexHeader[16] = hexMain[83];
                                    hexHeader[17] = hexMain[84];
                                }
                                else
                                {
                                    hexHeader[16] = hexMain[81];
                                    hexHeader[17] = hexMain[82];
                                }

                                if (hexMain[85] == 0 && hexMain[86] == 0)
                                {
                                    hexHeader[12] = hexMain[87];
                                    hexHeader[13] = hexMain[88];
                                }
                                else
                                {
                                    hexHeader[12] = hexMain[85];
                                    hexHeader[13] = hexMain[86];
                                }

                                hexHeader[28] = hexMain[95];
                                hexHeader[21] = hexMain[105];
                                hexHeader[22] = hexMain[106];
                            }
                            else
                            {
                                if (hexMain[17] == 0 && hexMain[18] == 0)
                                {
                                    hexHeader[16] = hexMain[19];
                                    hexHeader[17] = hexMain[20];
                                }
                                else
                                {
                                    hexHeader[16] = hexMain[17];
                                    hexHeader[17] = hexMain[18];
                                }

                                if (hexMain[21] == 0 && hexMain[22] == 0)
                                {
                                    hexHeader[12] = hexMain[23];
                                    hexHeader[13] = hexMain[24];
                                }
                                else
                                {
                                    hexHeader[12] = hexMain[21];
                                    hexHeader[13] = hexMain[22];
                                }

                                hexHeader[28] = hexMain[31];
                                hexHeader[21] = hexMain[41];
                                hexHeader[22] = hexMain[42];
                            }

                            hexMain = hexMain.Skip(headerL).ToArray();
                            hexMain = hexHeader.Concat(hexMain).ToArray();

                            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileLocation));
                            bw.BaseStream.Position = 0x0;
                            bw.Write(hexMain);
                            bw.Close();
                        }
                        else
                        {
                            if (File.Exists(fileLocation))
                                File.Delete(fileLocation);

                            StreamWriter sw = new StreamWriter("SkippedFiles.txt", true);
                            sw.WriteLine(fileName);
                            sw.Close();

                            j++;
                        }
                    }
                    else if (file.EndsWith(".terrainheightfield"))
                    {
                        string fileName = Path.GetFileName(file.Replace(".terrainheightfield", ".raw")),
                               fileLocation = file.Replace(".terrainheightfield", ".raw");

                        if (isStandalone == true)
                            fileLocation = Dirs.outputHeightmap + @"\" + fileName;

                        File.Copy(file, fileLocation, true);

                        BinaryReader br = new BinaryReader(File.OpenRead(fileLocation));
                        byte[] hexMain = br.ReadBytes(Convert.ToInt32(br.BaseStream.Length));
                        br.Close();

                        int headerL = 90 / 2;

                        if (hexMain[0] != 0)
                            headerL = 84 / 2;

                        hexMain = hexMain.Skip(headerL).ToArray();

                        BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileLocation));
                        bw.BaseStream.Position = 0x0;
                        bw.Write(hexMain);
                        bw.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
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

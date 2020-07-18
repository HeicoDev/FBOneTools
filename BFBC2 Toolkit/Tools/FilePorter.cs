using System;
using System.IO;
using BFBC2_Toolkit.Functions;

namespace BFBC2_Toolkit.Tools
{
    public class FilePorter
    {
        public static void ConvertFile(string filePath)
        {
            byte[] fileData = { };

            using (var br = new BinaryReader(File.OpenRead(filePath)))
            {
                fileData = br.ReadBytes(Convert.ToInt32(br.BaseStream.Length));
            }

            int offset = 0;
            int length = fileData.Length;

            string fileExtension = String.Empty;

            if (filePath.EndsWith(".terrainheightfield"))
            {
                //Heightmap (WIP)
                fileExtension = ".terrainheightfield";

                //Reverse header

                //Reverse first data chunk
                while (true)
                {
                    if (offset >= length)
                        break;

                    fileData = Reverse.TwoByteBlock(fileData, offset);

                    offset += 2;
                }

                //Reverse second data chunk
            }
            else if (filePath.EndsWith(".watermesh"))
            {
                //WaterMesh
                fileExtension = ".watermesh";

                //Whole file including header can be reversed in 4 byte blocks
                while (true)
                {
                    if (offset >= length)
                        break;

                    fileData = Reverse.FourByteBlock(fileData, offset);

                    offset += 4;
                }
            }
            else if (filePath.EndsWith(".visualwater"))
            {
                //VisualWater
                fileExtension = ".visualwater";

                byte[] pointer = { fileData[11], fileData[10], fileData[9], fileData[8] };

                int pointerOffset = BitConverter.ToInt32(pointer, 0);

                //Reverse header
                for (int i = 0; i < 3; i++)
                {
                    fileData = Reverse.FourByteBlock(fileData, offset);

                    offset += 4;
                }

                //Reverse first data chunk
                while (true)
                {
                    if (offset == pointerOffset)
                    {
                        fileData = Reverse.TwoByteBlock(fileData, offset);
                        fileData = Reverse.TwoByteBlock(fileData, offset + 2);

                        offset += 4;

                        break;
                    }

                    fileData = Reverse.TwoByteBlock(fileData, offset);

                    offset += 2;
                }

                //Reverse second data chunk
                int materialsCount;

                while (true)
                {
                    if (fileData[offset] == 84 && fileData[offset + 1] == 101 && fileData[offset + 2] == 114 && fileData[offset + 3] == 114 ||
                        fileData[offset] == 83 && fileData[offset + 1] == 104 && fileData[offset + 2] == 97 && fileData[offset + 3] == 100)
                    {
                        materialsCount = BitConverter.ToInt32(fileData, offset - 4);
                        offset += 4;
                        break;
                    }

                    fileData = Reverse.FourByteBlock(fileData, offset);

                    offset += 4;
                }

                //Skip strings/materials
                while (true)
                {
                    if (fileData[offset] == 0 && fileData[offset + 1] == 0)
                        materialsCount--;

                    if (materialsCount == 0)
                    {
                        offset++;
                        break;
                    }

                    offset++;
                }

                //Reverse actual footer
                while (true)
                {
                    if (offset >= length)
                        break;

                    fileData = Reverse.FourByteBlock(fileData, offset);

                    offset += 4;
                }
            }
            else if (filePath.EndsWith(".terrainmaterialmap"))
            {
                //TerrainMaterialMap (WIP)
                fileExtension = ".terrainmaterialmap";

            }
            else if (filePath.EndsWith(".visualterrain"))
            {
                //VisualTerrain (WIP)
                fileExtension = ".visualterrain";

            }
            else if (filePath.EndsWith(".ps3texture") || filePath.EndsWith(".xenontexture"))
            {
                //Texture
                fileExtension = ".itexture";

                //Reverse header
                for (int i = 0; i < 23; i++)
                {
                    fileData = Reverse.FourByteBlock(fileData, offset);

                    offset += 4;
                }

                //Reverse data
                while (true)
                {
                    if (offset >= length)
                        break;

                    fileData = Reverse.TwoByteBlock(fileData, offset);

                    offset += 2;
                }
            }

            string outputFile = Path.GetDirectoryName(filePath) + @"\output" + fileExtension;

            if (File.Exists(outputFile))
                File.Delete(outputFile);

            using (var bw = new BinaryWriter(File.OpenWrite(outputFile)))
            {
                bw.BaseStream.Position = 0x0;
                bw.Write(fileData);
            }                
        }
    }
}

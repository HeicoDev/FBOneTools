using System;
using System.IO;
using System.Linq;
using BFBC2_Toolkit.Functions;

namespace BFBC2_Toolkit.Tools
{
    public class FilePorter
    {
        /* WIP */

        public static void ConvertFile(string filePath)
        {
            string fileExtension = Path.GetExtension(filePath);

            byte[] fileData = { };

            using (var br = new BinaryReader(File.OpenRead(filePath)))
            {
                fileData = br.ReadBytes(Convert.ToInt32(br.BaseStream.Length));
            }   
            
            switch (fileExtension)
            {
                case ".terrainheightfield":
                    fileData = ConvertTerrainHeightField(fileData);
                    break;
                case ".watermesh":
                    fileData = ConvertWaterMesh(fileData);
                    break;
                case ".visualwater":
                    fileData = ConvertVisualWater(fileData);
                    break;
                case ".terrainmaterialmap":
                    fileData = ConvertTerrainMaterialMap(fileData);
                    break;
                case ".visualterrain":
                    fileData = ConvertVisualTerrain(fileData);
                    break;
                case ".ps3texture":
                    fileData = ConvertTexture(fileData);
                    break;
                case ".xenontexture":
                    fileData = ConvertTexture(fileData);
                    break;
            }           

            string outputFile = Path.GetDirectoryName(filePath) + @"\Output" + fileExtension;

            if (File.Exists(outputFile))
                File.Delete(outputFile);

            using (var bw = new BinaryWriter(File.OpenWrite(outputFile)))
            {
                bw.BaseStream.Position = 0x0;
                bw.Write(fileData);
            }                
        }

        private static byte[] ConvertTerrainHeightField(byte[] fileData)
        {
            /* Experimental */

            //Return unmodified data if PC header (4 zero bytes at position 41). The console files of BFBC2 also have 4 zero bytes at position 41.           
            //TODO: Porting the heightmaps of BFBC2 from console to PC is probably unnecessary, but I will still try to look for a better check.
            if (fileData[41] == 0 && fileData[42] == 0 && fileData[43] == 0 && fileData[44] == 0)
                return fileData;

            //Extend header by 4 zero bytes at position 41 to fit the structure and size of the PC header (TODO: Do only if BFBC or BF1943 file)
            byte[] fileHeader = fileData.Take(49).ToArray();

            fileHeader[45] = fileHeader[41];
            fileHeader[46] = fileHeader[42];
            fileHeader[47] = fileHeader[43];
            fileHeader[48] = fileHeader[44];
            fileHeader[41] = 0;
            fileHeader[42] = 0;
            fileHeader[43] = 0;
            fileHeader[44] = 0;

            fileData = fileData.Skip(45).ToArray();
            fileData = fileHeader.Concat(fileData).ToArray();

            int offset = 0;
            int length = fileData.Length;

            //Reverse header
            while (true)
            {
                if (offset >= 49)
                    break;

                //Skip one unknown zero byte at position 36
                if (offset == 36)
                    offset++;

                fileData = Reverse.FourByteBlock(fileData, offset);

                offset += 4;
            }

            //Reverse first data chunk
            while (true)
            {                
                if (offset >= 8388653)
                    break;

                fileData = Reverse.TwoByteBlock(fileData, offset);

                offset += 2;
            }

            //Reverse second data chunk
            while (true)
            {
                if (offset >= length)
                    break;

                fileData = Reverse.FourByteBlock(fileData, offset);

                offset += 4;
            }

            //The output file must have the same amount of bytes (or more!) as the file that will be replaced or the BFBC2 server/client will throw an exception.  
            //Since we can't know the file that will be replaced, we just add some bloat (zero bytes) until we reach 10 MB at the bottom of the file (workaround).
            int bloatAmount = 10485760 - length;

            byte[] bloat = new byte[bloatAmount];

            for (int i = 0; i < bloat.Length; i++)
            {
                bloat[i] = 0;
            }

            fileData = fileData.Concat(bloat).ToArray();

            return fileData;
        }

        private static byte[] ConvertWaterMesh(byte[] fileData)
        {
            int offset = 0;
            int length = fileData.Length;

            //Whole file including header can be reversed in 4 byte blocks
            while (true)
            {
                if (offset >= length)
                    break;

                fileData = Reverse.FourByteBlock(fileData, offset);

                offset += 4;
            }

            return fileData;
        }

        private static byte[] ConvertVisualWater(byte[] fileData)
        {
            int offset = 0;
            int length = fileData.Length;

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
                //Check if strings/materials start (Should be either 'Terr'ain or 'Shad'ers)
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

            return fileData;
        }

        private static byte[] ConvertTerrainMaterialMap(byte[] fileData)
        {
            int offset = 0;
            int length = fileData.Length;

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

            return fileData;
        }

        private static byte[] ConvertVisualTerrain(byte[] fileData)
        {
            return fileData;
        }

        private static byte[] ConvertTexture(byte[] fileData)
        {
            /* Experimental */

            int offset = 0;
            int length = fileData.Length;

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

            return fileData;
        }
    }
}

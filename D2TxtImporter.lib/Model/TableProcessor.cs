using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace D2TxtImporter.lib.Model
{
    // Code taken from: https://github.com/kambala-decapitator/QTblEditor
    public class TableProcessor
    {
        [StructLayout(LayoutKind.Explicit)]
        private struct TblHeader
        {
            [FieldOffset(0x00)]
            public ushort CRC;             // +0x00 - CRC value for string table

            [FieldOffset(0x02)]
            public ushort NodesNumber;     // +0x02 - size of Indices array

            [FieldOffset(0x04)]
            public uint HashTableSize;   // +0x04 - size of TblHashNode array

            [FieldOffset(0x08)]
            public byte Version;         // +0x08 - file version, either 0 or 1, doesn't matter

            [FieldOffset(0x09)]
            public uint DataStartOffset; // +0x09 - string table start offset

            [FieldOffset(0x0D)]
            public uint HashMaxTries;    // +0x0D - max number of collisions for string key search based on its hash value

            [FieldOffset(0x011)]
            public uint FileSize;        // +0x11 - size of the file

            public const int size = 0x15;
        };

        [StructLayout(LayoutKind.Explicit)]
        private struct TblHashNode // node of the hash table in string *.tbl file
        {
            [FieldOffset(0x00)]
            public byte Active;          // +0x00 - shows if the entry is used. if 0, then it has been "deleted" from the table

            [FieldOffset(0x01)]
            public ushort Index;           // +0x01 - index in Indices array

            [FieldOffset(0x03)]
            public uint HashValue;       // +0x03 - hash value of the current string key

            [FieldOffset(0x07)]
            public uint StringKeyOffset; // +0x07 - offset of the current string key

            [FieldOffset(0x0B)]
            public uint StringValOffset; // +0x0B - offset of the current string value

            [FieldOffset(0x0F)]
            public ushort StringValLength; // +0x0F - length of the current string value

            public const int size = 0x11;
        };

        private struct TableList
        {
            public string Key { get; set; }
            public string Value { get; set; }
            public int Index { get; set; }

            public override string ToString()
            {
                return Key;
            }
        };

        public static Dictionary<string, string> ReadTablesFile(string path)
        {
            var result = new Dictionary<string, string>();

            using (var fs = new FileStream(path, FileMode.Open))
            {
                using (var br = new BinaryReader(fs, Encoding.UTF8))
                {
                    // Read the header
                    var header = GetHeader(br);

                    // number of bytes to read without header
                    var numElem = header.FileSize - TblHeader.size;

                    // Check we can read the entire file
                    var byteArray = br.ReadBytes((int)numElem);
                    if (byteArray.Length == numElem)
                    {
                        br.BaseStream.Position = TblHeader.size;

                        // Read the table
                        result = GetStringTable(br, header);
                    }
                    else
                    {
                        throw new Exception($"Table '{path}' seems to be corrupt");
                    }
                }
            }

            return result;
        }

        private static Dictionary<string, string> GetStringTable(BinaryReader br, TblHeader header)
        {
            var result = new Dictionary<string, string>();
            var tableList = new List<TableList>();

            br.BaseStream.Position += header.NodesNumber * sizeof(ushort);
            var hashNodes = new List<TblHashNode>();

            for (uint i = 0; i < header.HashTableSize; i++)
            {
                hashNodes.Add(GetHashNode(br));
            }

            br.BaseStream.Position = 0;

            var byteArray = ReadAllBytes(br);
            foreach (var hashNode in hashNodes)
            {

                if (hashNode.Active == 0)
                {
                    continue;
                }
                else if (hashNode.Active != 1)
                {
                    continue;
                }

                string val = null;
                string key;

                val = Encoding.UTF8.GetString(byteArray, (int)hashNode.StringValOffset, hashNode.StringValLength).Trim('\0');
                key = Encoding.UTF8.GetString(byteArray, (int)hashNode.StringKeyOffset, (int)hashNode.StringValOffset - (int)hashNode.StringKeyOffset).Trim('\0');

                tableList.Add(new TableList { Key = key, Value = val ?? "", Index = hashNode.Index });
            }

            tableList = tableList.OrderBy(x => x.Index).ToList();

            foreach (var tableValue in tableList)
            {
                result[tableValue.Key] = tableValue.Value;
            }

            return result;
        }

        private static TblHeader GetHeader(BinaryReader br)
        {
            byte[] readBuffer = new byte[TblHeader.size];

            readBuffer = br.ReadBytes(TblHeader.size);
            GCHandle handle = GCHandle.Alloc(readBuffer, GCHandleType.Pinned);
            var header = (TblHeader)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(TblHeader));
            handle.Free();

            return header;
        }

        private static TblHashNode GetHashNode(BinaryReader br)
        {
            byte[] readBuffer = new byte[TblHashNode.size];

            readBuffer = br.ReadBytes(TblHashNode.size);
            GCHandle handle = GCHandle.Alloc(readBuffer, GCHandleType.Pinned);
            var result = (TblHashNode)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(TblHashNode));
            handle.Free();

            return result;
        }

        private static byte[] ReadAllBytes(BinaryReader reader)
        {
            const int bufferSize = 4096;
            using (var ms = new MemoryStream())
            {
                byte[] buffer = new byte[bufferSize];
                int count;
                while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
                {
                    ms.Write(buffer, 0, count);
                }
                return ms.ToArray();
            }
        }
    }
}

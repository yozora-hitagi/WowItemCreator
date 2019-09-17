using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Lib_Creator
{
   public class MPQWriter
    {
        [DllImport("SFmpq.dll")]
        public static extern bool MpqAddFileToArchive(int hMPQ, string lpSourceFileName, string lpDestFileName, int dwFlags);
        [DllImport("SFmpq.dll")]
        public static extern bool MpqCompactArchive(int hMPQ);
        [DllImport("SFmpq.dll")]
        public static extern int MpqOpenArchiveForUpdate(string lpFileName, int dwFlags, int dwMaximumFilesInArchive);
        [DllImport("SFmpq.dll")]
        public static extern bool SFileCloseArchive(int hMPQ);
    }
}

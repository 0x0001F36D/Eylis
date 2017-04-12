
namespace Eylis.Plugin.Command.Common
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public static class Extension
    {
        private static int? cache = null;
        public static byte[] ToBytes<TStruct>(this TStruct structure) where TStruct : class
        {
            var size = (cache = cache ?? Marshal.SizeOf(structure)).Value;
            var array = new byte[size];
            var pointer = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(structure, pointer, true);
            Marshal.Copy(pointer, array, 0, size);
            Marshal.FreeHGlobal(pointer);
            return array;//
        }
        public static TStruct To<TStruct>(this byte[] data) where TStruct : class
        {
            try
            {
                var t = typeof(TStruct);
                var size = (cache = cache ?? Marshal.SizeOf(t)).Value;
                var pointer = Marshal.AllocHGlobal(size);
                Marshal.Copy(data, 0, pointer, size);
                var structure = Marshal.PtrToStructure(pointer, t);
                Marshal.FreeHGlobal(pointer);
                return (TStruct)structure;
            }
            catch
            {
                return default(TStruct);
            }
        }

        public static string JoinString<T>(this IEnumerable<T> collection)
        {
            return string.Join(",", collection);
        }
    }

    public class Header
    {



        public enum Version : byte
        {
            None,
            V1
        }
        
        public enum Method : byte
        {
            Anonymous_User ,
            Sign_Up ,
            Sign_In ,
            Sign_Out = Sign_In << 1,

            Online_User_Query = Sign_Out << 2,

        }


    }
}

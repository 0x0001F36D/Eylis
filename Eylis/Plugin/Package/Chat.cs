
namespace Eylis.Plugin.Package
{
    using System.Runtime.InteropServices;
    using Command.Common;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class Chat : Session
    {
        
        [MarshalAs(UnmanagedType.U4)]
        private uint part;

        [MarshalAs(UnmanagedType.U4)]
        private uint total;

        [MarshalAs(UnmanagedType.ByValTStr , SizeConst = 32)]
        private string name;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
        private string payload;

        private Chat():base()
        {
                
        }

        public Chat(Header.Version version, Header.Method method , uint part , uint total , string name , string payload) : base(version, method)
        {
            this.part = part;
            this.total = total;
            this.name = method == Header.Method.Anonymous_User ? "<Anonymous User>" : name;
            this.payload = payload;
        }

        public uint Part => this.part;
        public uint Total => this.total;
        public string Name => this.name;
        public string Payload => this.payload;
    }
}

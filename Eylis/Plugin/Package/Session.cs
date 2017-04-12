
namespace Eylis.Plugin.Package
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;
    using Command.Common;


    [StructLayout(LayoutKind.Sequential , CharSet = CharSet.Unicode)]
    public abstract class Session : ISession
    {
        [MarshalAs(UnmanagedType.U1)]
        private Header.Version version;

        [MarshalAs(UnmanagedType.U1)]
        private Header.Method method;

        protected Session()
        {

        }

        public Session(Header.Version version , Header.Method method)
        {
            this.version = version;
            this.method = method;
        }

        public Header.Method Method => this.method;

        public Header.Version Version => this.version;
    }
}

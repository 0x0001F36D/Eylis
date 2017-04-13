
namespace Eylis.TinyServer.Test
{
    using Eylis.Core;
    using Eylis.Core.Protocol;
    using Eylis.Plugin.Command;
    using Eylis.Plugin.Command.Common;
    using Eylis.Plugin.Package;
    using System;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Net;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using System.Threading;

    class Program
    {
        static void Main(string[] args)
        {
            var host = new EylisHostConsole();
            host.Start();

        }

    }
}
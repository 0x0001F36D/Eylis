
namespace EylisProtocol.Object
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class EylisMessage
    {
        public EylisMessage(string message , Encoding encoding = null)
        {
            this.Encoding = encoding ?? Encoding.UTF8;
            this.Message = this.Encoding.GetBytes(message);
        }
        public Encoding Encoding { get; private set; }
        public byte[] Message { get;private set; }
    }
}

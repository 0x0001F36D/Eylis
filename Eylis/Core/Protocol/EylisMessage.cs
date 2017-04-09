
namespace Eylis.Core.Protocol
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class EylisMessage
    {
        public static EylisMessage operator +(EylisMessage l, EylisMessage r)
            => new EylisMessage(l.Data.Concat(r.Data).ToArray());

        public byte[] Data { get; private set; }
        public Encoding Encoding => Encoding.UTF8;
        public static implicit operator EylisMessage(string message)
            => new EylisMessage(Encoding.UTF8.GetBytes(message));
        public static implicit operator EylisMessage(byte[] data)
            => new EylisMessage(data);

        public EylisMessage(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            this.Data = data;
        }
        public override string ToString()
            => this.Encoding.GetString(this.Data);

    }
}

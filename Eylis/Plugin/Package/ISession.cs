
namespace Eylis.Plugin.Package
{
    using Eylis.Plugin.Command.Common;

    public interface ISession
    {
        Header.Method Method { get; }
        Header.Version Version { get; }
    }
}
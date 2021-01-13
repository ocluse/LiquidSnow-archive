
namespace Thismaker.Aba.Common
{
    public interface IPackage
    {
        string ToJson();

        T FromJson<T>();
    }
}

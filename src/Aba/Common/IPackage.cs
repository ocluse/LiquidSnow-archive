
namespace Thismaker.Aba.Common
{
    public interface IPackage<T>
    {
        string ToJson();

        T FromJson();
    }
}

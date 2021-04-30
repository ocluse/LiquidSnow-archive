
namespace Thismaker.Aba.Common
{
    public interface IAbaPackage<T>
    {
        string ToJson();

        T FromJson();
    }
}

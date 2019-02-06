namespace BigInt.Entities
{
    public interface ICloneable<T>
    {
        T DeepClone();

        T ShallowClone();
    }
}

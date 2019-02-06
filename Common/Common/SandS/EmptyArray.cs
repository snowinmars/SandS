namespace Common.SandS
{
    public static class EmptyArray<T>
    {
        private static readonly T[] InstanceInternal = new T[0];

        public static T[] Instance
        {
            get { return InstanceInternal; }
        }
    }
}
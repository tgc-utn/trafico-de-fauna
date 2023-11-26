using System;

namespace Trafico.De.Fauna.Model.Debug
{
    class TextureMemory : IComparable
    {
        public string FilePath;
        public float Length;
        public int References;

        public int CompareTo(object obj)
        {
            return Length.CompareTo(((TextureMemory)obj).Length);
        }
    }
}

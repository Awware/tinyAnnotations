using System;

namespace tinyAnnotations.ConfigCreator
{
    public class RandStructure
    {
        public RandStructure(int min, int max)
        {
            MIN = min;
            MAX = max;
            if (min > max)
                throw new Exception("Min value must be < Max value");
        }
        public int MIN = 0;
        public int MAX = 0;
        public override string ToString()
        {
            return $"rand {MIN} {MAX}";
        }
    }
}

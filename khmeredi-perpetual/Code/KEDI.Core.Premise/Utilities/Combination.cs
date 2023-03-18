using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Models.Validation
{
    public class Combination
    {
        public static double GetFactorial(int n)
        {
            if (n <= 1) { return 1; }
            return GetFactorial(n - 1) * n;
        }

        public static double GetCombination(int n, int r, bool repeated = false)
        {
            if (repeated)
            {
                return GetFactorial(r + n - 1) / (GetFactorial(r) * GetFactorial(n - r));
            }
            return GetFactorial(n) / (GetFactorial(r) * GetFactorial(n - r));
        }

        public static double GetPermutation(int n, int r, bool repeated = false)
        {
            if (repeated)
            {
                return Math.Pow(n, r);
            }
            return GetFactorial(n) / GetFactorial(n - r);
        }
    }
}

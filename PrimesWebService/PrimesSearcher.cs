using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimesWebService
{
    public class PrimesSearcher : IPrimesSearcher
    {
        public PrimesSearcher()
        {

        }

        public bool IsPrime(int numb)
        {
            if (Enumerable.Range(-1, 3).Contains(numb))
            {
                return false;
            }

            bool isPrime = Enumerable.All(Enumerable.Range(2,Math.Abs(numb) - 2), n => numb % n != 0);
            return isPrime;
        }
        public Task<List<int>> FindPrimesAsync(int from, int to)
        {
            from = from < 2 ? 2 : from;
            return Task.FromResult(to < from 
                ? new List<int>(0) 
                : Enumerable.Range(from, to - from + 1).AsParallel().AsOrdered()
                .Where(numb => IsPrime(numb)).ToList()); 
        }
    }
}

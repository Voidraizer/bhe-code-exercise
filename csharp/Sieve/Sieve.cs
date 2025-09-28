using System.Diagnostics;

namespace Sieve;

public interface ISieve
{
    long NthPrime( long n );
}

public class SieveImplementation : ISieve
{
    public long NthPrime( long n )
    {
        // Base slow method
        HashSet<int> primes = new HashSet<int>();

        bool found = false;
        int counter = 2;

        while( !found )
        {
            bool isPrime = primes.All( i => counter % i != 0 );

            if( isPrime )
            {
                primes.Add( counter );
            }

            if( primes.Count - 1 == n )
            {
                found = true;
                List<string> suffixes = new List<string> { "th", "st", "nd", "rd", "th", "th", "th", "th", "th", "th" };
                int val = (int)n + 1;
                val = int.Parse( val.ToString().Substring( val.ToString().Length - 1 ) );

                Debug.Print( $"Found the {n + 1}{suffixes[val]} prime: {counter}" );
            }
            else
            {
                counter++;
            }
        }

        return counter;
    }
}
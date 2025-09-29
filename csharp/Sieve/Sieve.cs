using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;

namespace Sieve;

public interface ISieve
{
    long NthPrime( long n );
}

public class SieveImplementation : ISieve
{
    private static readonly List<string> suffixes = new List<string> { "th", "st", "nd", "rd", "th", "th", "th", "th", "th", "th" };

    /// <summary>
    /// Version 1.0<br/>
    /// In order to utilize the theorem effectively, we need to estimate an upper limit for the nth prime
    /// <para>
    /// https://en.wikipedia.org/wiki/Prime_number_theorem#Statement states that the nth prime is approximately nlog(n). So let's take that value and double it to ensure it contains the nth prime
    /// </para>
    /// </summary>
    public long NthPrime( long n )
    {
        if( n < 0 )
        {
            throw new ArgumentOutOfRangeException( nameof( n ), "n must be 0 or greater" );
        }

        // Log(n) is asymptotic negative infinity at 0 so we'll just hardcode the limit for the first 8 or so primes
        // This is hard limited by the 32-bit integer max which means the furthest prime we can calculate with this method
        // is roughly the 59,950,299th which will have a rough upper limit calculated to 2147302912.
        // You could squeeze out some higher values by increasing the accuracy of the limit calculation, but it will inevitably overflow
        int limit = n < 9 ? 20 : (int)( n * MathF.Log( n ) * 2 );
        // Debug.Print( "Estimated Upper Limit: " + limit.ToString() );
        bool[] isPrime = new bool[limit];

        // We start at 3 and increment by 2 to skip all evens as they're all composite
        for( int i = 3; i < limit; i += 2 )
        {
            isPrime[i] = true;
        }

        // Don't forget 2 is prime
        isPrime[2] = true;

        for( int j = 3; j * j < limit; j += 2 )
        {
            if( isPrime[j] )
            {
                // Start at the square of the main counter and mark all multiples of j as non-prime because everything
                // before j * j has already been marked off by smaller primes. Also increment by 2 * j to skip evens since
                // they're all already false
                for( int k = j * j; k < limit; k += ( 2 * j ) )
                {
                    isPrime[k] = false;
                }
            }
        }

        List<int> primes = new List<int>();

        // Whatever remains must be prime from which we can pull the nth
        for( int i = 2; i < limit; i++ )
        {
            if( isPrime[i] )
            {
                primes.Add( i );
            }
        }

        int wantedPrime = primes[(int)n];
        int val = (int)n + 1;
        val = int.Parse( val.ToString().Substring( val.ToString().Length - 1 ) );

        Debug.Print( $"Found the {n + 1}{suffixes[val]} prime: {wantedPrime}" );

        return wantedPrime;
    }

    /// <summary>
    /// Version 0.5<br/>
    /// Counting upwards isn't really using the sieve formula and is horrendously slow for large n<br/>
    /// It does return the correct primes after an eternity
    /// </summary>
    //public long NthPrime( long n )
    //{
    //    if( n == 0 )
    //    {
    //        return 2;
    //    }

    //    HashSet<int> primes = new HashSet<int>() { 2 };

    //    bool found = false;
    //    int counter = 1;

    //    while( !found )
    //    {
    //        counter += 2;

    //        bool isPrime = primes.All( i => counter % i != 0 );

    //        if( isPrime )
    //        {
    //            primes.Add( counter );
    //        }

    //        if( primes.Count - 1 == n )
    //        {
    //            found = true;
    //            List<string> suffixes = new List<string> { "th", "st", "nd", "rd", "th", "th", "th", "th", "th", "th" };
    //            int val = (int)n + 1;
    //            val = int.Parse( val.ToString().Substring( val.ToString().Length - 1 ) );

    //            Debug.Print( $"Found the {n + 1}{suffixes[val]} prime: {counter}" );
    //            Debug.Print( $"Worse Estimated Upper Limit: {(int)( n * MathF.Log( n ) )}" );
    //            Debug.Print( $"Better Estimated Upper Limit: {(int)( n * MathF.Log( n * MathF.Log( n ) ) )}" );
    //        }
    //    }

    //    return counter;
    //}

}
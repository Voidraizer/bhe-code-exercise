using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Globalization;
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
    /// Version 2.0<br/>
    /// This version is intended to handle exceptionally large nth primes without requiring 128-bit integers.
    /// </summary>
    public long NthPrime( long n )
    {
        if( n < 0 )
        {
            throw new ArgumentOutOfRangeException( nameof( n ), "n must be 0 or greater" );
        }
        else if( n == 0 )
        {
            Debug.Print( $"Found the first prime: 2" );
            return 2;
        }

        int chunkSize = 100000000; // 100 million - *must be even*
        // Even though we're potentially getting to ridiculously far out there primes, the total count of the primes shoudln't exceed 32-bit integer limits
        // because, according to the prime number theorem, the further out you go, the less frequent primes become
        List<long> primes = new List<long>() { 2 };
        long limit = n < 14 ? 45 : (long)( n * Math.Log( n * Math.Log( n ) ) );
        Debug.Print( $"Estimated Upper Limit: {limit}" );
        int chunkCounter = 0;

        for( long chunkStart = 0; chunkStart < limit; chunkStart += chunkSize )
        {
            chunkCounter++;
            // Make sure we don't exceed the overall limit on the last chunk
            int size = (int)Math.Min( chunkSize, limit - chunkStart );
            Debug.Print( $"Starting {chunkCounter}{( ( chunkCounter ) == 11 ? "th" : suffixes[chunkCounter % 10] )} chunk - ChunkStart: {chunkStart}, ChunkSize: {size}" );
            bool[] isPrime = new bool[size];

            // Start at 1 and increment by 2 to only hit odds
            for( int i = 1; i < size; i += 2 )
            {
                isPrime[i] = true;
            }

            // First cross out all multiples of previously found primes
            foreach( long p in primes )
            {
                //Debug.Print( $"Crossing out multiples of {p}" );

                if( p == 2 )
                {
                    continue;
                }

                /*
                 * We need to find a multiple of p within the current chunk. We always want to start at with p^2 to save time, or the first multiple of p
                 * that is in the chunk if p^2 is less than chunkStart
                 * 
                 * chunkStart + p - 1: This ensures that if chunkStart isn’t already a multiple of p, you round up to the next multiple
                 * / p: Integer division, so it floors the result
                 * * p: Multiplies back to get the actual multiple of p
                 * 
                 * Example:
                 * If chunkStart = 103, p = 7
                 * 103 + 7 - 1 
                 * 109 / 7 = 15 ( integer division )
                 * 15 * 7 = 105
                 */
                long firstPrimeMultiple = Math.Max( p * p, ( ( chunkStart + p - 1 ) / p ) * p );

                // Ensure the starting multiple is odd
                if( firstPrimeMultiple % 2 == 0 )
                {
                    firstPrimeMultiple += p;
                }

                for( long j = firstPrimeMultiple; j < chunkStart + size; j += ( 2 * p ) )
                {
                    if( j >= chunkStart )
                    {
                        isPrime[j - chunkStart] = false;
                    }
                }
            }

            // Now we hunt for new primes
            for( int i = 1; i < size; i += 2 )
            {
                long num = chunkStart + i;

                if( num < 2 ) continue;

                //Debug.Print( $"i: {i}, num: {num}, isPrime: {isPrime[i]}" );

                if( isPrime[i] )
                {
                    primes.Add( num );

                    if( primes.Count - 1 == n )
                    {
                        long val = n + 1;
                        val = int.Parse( val.ToString().Substring( val.ToString().Length - 1 ) );
                        Debug.Print( $"Found the {n + 1}{( ( n + 1 ) == 11 ? "th" : suffixes[(int)val] )} prime: {num}" );
                        return num;
                    }
                    else
                    {
                        for( long j = num * num; j < chunkStart + size; j += ( 2 * num ) )
                        {
                            if( j >= chunkStart )
                            {
                                isPrime[j - chunkStart] = false;
                            }
                        }
                    }
                }
            }
        }


        return 0;
    }

    /// <summary>
    /// Version 1.0<br/>
    /// In order to utilize the theorem effectively, we need to estimate an upper limit for the nth prime
    /// <para>
    /// https://en.wikipedia.org/wiki/Prime_number_theorem#Statement states that the nth prime is approximately nlog(n). So let's take that value and double it to ensure it contains the nth prime
    /// </para>
    /// </summary>
    //public long NthPrime( long n )
    //{
    //    if( n < 0 )
    //    {
    //        throw new ArgumentOutOfRangeException( nameof( n ), "n must be 0 or greater" );
    //    }

    //    // Log(n) is asymptotic negative infinity at 0 so we'll just hardcode the limit for the first 8 or so primes
    //    // This is hard limited by the 32-bit integer max which means the furthest prime we can calculate with this method
    //    // is roughly the 59,950,299th which will have a rough upper limit calculated to 2147302912.
    //    // You could squeeze out some higher values by increasing the accuracy of the limit calculation, but it will inevitably overflow
    //    int limit = n < 9 ? 20 : (int)( n * MathF.Log( n ) * 2 );
    //    // Debug.Print( "Estimated Upper Limit: " + limit.ToString() );
    //    bool[] isPrime = new bool[limit];

    //    // We start at 3 and increment by 2 to skip all evens as they're all composite
    //    for( int i = 3; i < limit; i += 2 )
    //    {
    //        isPrime[i] = true;
    //    }

    //    // Don't forget 2 is prime
    //    isPrime[2] = true;

    //    for( int j = 3; j * j < limit; j += 2 )
    //    {
    //        if( isPrime[j] )
    //        {
    //            // Start at the square of the main counter and mark all multiples of j as non-prime because everything
    //            // before j * j has already been marked off by smaller primes. Also increment by 2 * j to skip evens since
    //            // they're all already false
    //            for( int k = j * j; k < limit; k += ( 2 * j ) )
    //            {
    //                isPrime[k] = false;
    //            }
    //        }
    //    }

    //    List<int> primes = new List<int>();

    //    // Whatever remains must be prime from which we can pull the nth
    //    for( int i = 2; i < limit; i++ )
    //    {
    //        if( isPrime[i] )
    //        {
    //            primes.Add( i );
    //        }
    //    }

    //    int wantedPrime = primes[(int)n];
    //    int val = (int)n + 1;
    //    val = int.Parse( val.ToString().Substring( val.ToString().Length - 1 ) );

    //    Debug.Print( $"Found the {n + 1}{suffixes[val]} prime: {wantedPrime}" );

    //    return wantedPrime;
    //}

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
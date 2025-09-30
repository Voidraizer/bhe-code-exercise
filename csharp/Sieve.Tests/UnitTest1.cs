namespace Sieve.Tests

{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestNthPrime()
        {
            ISieve sieve = new SieveImplementation();
            Assert.AreEqual( 2, sieve.NthPrime( 0 ) );
            Assert.AreEqual( 3, sieve.NthPrime( 1 ) );
            Assert.AreEqual( 5, sieve.NthPrime( 2 ) );
            Assert.AreEqual( 31, sieve.NthPrime( 10 ) );
            Assert.AreEqual( 41, sieve.NthPrime( 12 ) );
            Assert.AreEqual( 43, sieve.NthPrime( 13 ) );
            Assert.AreEqual( 47, sieve.NthPrime( 14 ) );
            Assert.AreEqual( 71, sieve.NthPrime( 19 ) );
            Assert.AreEqual( 541, sieve.NthPrime( 99 ) );
            Assert.AreEqual( 3581, sieve.NthPrime( 500 ) );
            Assert.AreEqual( 7793, sieve.NthPrime( 986 ) );
            Assert.AreEqual( 17393, sieve.NthPrime( 2000 ) );
            Assert.AreEqual( 104729, sieve.NthPrime( 9999 ) );
            Assert.AreEqual( 1299709, sieve.NthPrime( 99999 ) );
            Assert.AreEqual( 15485867, sieve.NthPrime( 1000000 ) );
            Assert.AreEqual( 179424691, sieve.NthPrime( 10000000 ) );
            Assert.AreEqual( 1189457257, sieve.NthPrime( 59950298 ) ); // max version 1 can handle before int overflow
            Assert.AreEqual( 2038074751, sieve.NthPrime( 100000000 ) ); // not required, just a fun challenge
            Assert.AreEqual( 2902958803, sieve.NthPrime( 140000000 ) ); // bigger challenge
        }
    }
}
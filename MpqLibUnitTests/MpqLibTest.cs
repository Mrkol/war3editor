using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MpqLib;

namespace MpqLibUnitTests
{
    [TestClass]
    public class MpqLibTest
    {
        [TestMethod]
        public void TestCryptTable()
        {
            Assert.AreEqual(1439053538u, Mpq.CryptTable[0]);
            Assert.AreEqual(1996014001u, Mpq.CryptTable[0x100]);
            Assert.AreEqual(1027335626u, Mpq.CryptTable[0x222]);
            Assert.AreEqual(3638880060u, Mpq.CryptTable[0x3f0]);
            Assert.AreEqual(1929586796u, Mpq.CryptTable[0x4ff]);
        }

        [TestMethod]
        public void TestHash()
        {
            Assert.AreEqual(3798558537u, Mpq.Hash(@"a", 3));
            Assert.AreEqual(1020835722u, Mpq.Hash(@"arr\units.dat", 3));
            Assert.AreEqual(2216028777u, Mpq.Hash(@"unit\neutral\acritter.grp", 3));
            //TODO: more tests just to be sure
        }

        [TestMethod]
        public void TestDecryptionEncryptionCoherence()
        {
            string[] strs = {"a", "aaaa", "abacabac", @"unit\neutral\acritter.grp",
                @"abcdefghijklmnopqrstuvwxyz1234567890!@#$%^&*()\|/?,.<>;':""[]{}"};
            foreach (string s in strs)
            {
                byte[] converted = Encoding.UTF8.GetBytes(s);
                Mpq.Encrypt(ref converted, 12345);
                Mpq.Decrypt(ref converted, 12345);
                Assert.AreEqual(s, Encoding.UTF8.GetString(converted));
            }
        }
    }
}
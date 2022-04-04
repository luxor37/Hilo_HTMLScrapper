using HtmlScrapper;
using NUnit.Framework;
using System;
using System.IO;

namespace HtmlScrapperTests
{
    public class UtilsTest
    {
        [Test]
        public void GetUrl_Test_PartialURL_Valid()
        {
            Console.SetIn(new StringReader("www.google.com"));

            var actual = Utils.GetUrl();

            var expected = "https://www.google.com";

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetUrl_Test_FullURL_Valid()
        {
            Console.SetIn(new StringReader("https://www.google.com"));

            var actual = Utils.GetUrl();

            var expected = "https://www.google.com";

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ValidateUrl_Test_BadURI_Invalid()
        {
            var actual = Utils.ValidateUrl("google");

            Assert.False(actual);
        }

        [Test]
        public void ValidateUrl_Test_NoHost_Invalid()
        {
            var actual = Utils.ValidateUrl("https://google");

            Assert.False(actual);
        }

        [Test]
        public void ValidateUrl_Test_Valid()
        {
            var actual = Utils.ValidateUrl("https://google.com");

            Assert.True(actual);
        }

        [Test]
        public void GetWord_Test_Valid()
        {
            Console.SetIn(new StringReader("MAISON"));

            var actual = Utils.GetWord();

            var expected = "maison";

            Assert.AreEqual(expected, actual);
        }
    }
}
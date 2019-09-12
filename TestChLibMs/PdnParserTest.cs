using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PdnLib;

namespace TestChLibMs
{
    [TestClass]
    public class PdnParserTest
    {
        private const string _simplePdn = @"[Event ""Simpele test""]
[Round ""1""]
[Result ""1/2-1/2""]
1. 11-15 22-18 2. 15x22 1/2-1/2

[Event ""Simpele test""]
[Round ""2""]
[Result ""*""]
1. 11-16 *";

        private const string _pdnWithEscapedQuote = @"[Event ""Simpele test""]
[Round ""1\\"" <-- dit is een escaped quote!""]
[Result ""1/2-1/2""]
1. 11-15 22-18 2. 15x22 1/2-1/2

[Event ""Simpele test""]
[Round ""2""]
[Result ""*""]
1. 11-16 *";


        private const string _pdnWithComments = @"
[Event ""Simpele test""]
[Round ""1""]
[Result ""1/2-1/2""]
1. 11-15 22-18 2. 15x22 {[een testje]} 1/2-1/2

[Event ""Simpele test""]
[Round ""2""]
[Result ""*""]
1. 11-16 *";

        private const string _pdnWithLineComments = @"
[Event ""Simpele test""]
[Round ""1""]
[Result ""1/2-1/2""]
1. 11-15 22-18 2. 15x22 {[een testje]} 
% [Tag etc.]
1/2-1/2

[Event ""Simpele test""]
[Round ""2""]
[Result ""*""]
% Line comment {see what happens
1. 11-16 *";

        private const string _pdnMalFormed = @"["" *";


        [TestMethod()]
        public void TestSimpleParsing()
        {
            var p = new PdnParser();
            int bodyCount = 0;
            int gameCount = 0;
            int tagCount = 0;
            p.OnBodyFound += (sender, args) => bodyCount++;
            p.OnStartNewGame += (sender, args) => gameCount++;
            p.OnTagFound += (sender, args) => tagCount++;
            p.Parse(_simplePdn);

            Assert.AreEqual(2, bodyCount, "BodyCount");
            Assert.AreEqual(2, gameCount, "GameCount");
            Assert.AreEqual(6, tagCount, "TagCount");
        }

        [TestMethod()]
        public void TestCommentsParsing()
        {
            var p = new PdnParser();
            int bodyCount = 0;
            int gameCount = 0;
            int tagCount = 0;
            p.OnBodyFound += (sender, args) => bodyCount++;
            p.OnStartNewGame += (sender, args) => gameCount++;
            p.OnTagFound += (sender, args) => tagCount++;
            p.Parse(_pdnWithComments);

            Assert.AreEqual(2, bodyCount, "BodyCount");
            Assert.AreEqual(2, gameCount, "GameCount");
            Assert.AreEqual(6, tagCount, "TagCount");
        }

        [TestMethod()]
        public void TestLineCommentsParsing()
        {
            var p = new PdnParser();
            int bodyCount = 0;
            int gameCount = 0;
            int tagCount = 0;
            p.OnBodyFound += (sender, args) => bodyCount++;
            p.OnStartNewGame += (sender, args) => gameCount++;
            p.OnTagFound += (sender, args) => tagCount++;
            p.Parse(_pdnWithLineComments);

            Assert.AreEqual(2, bodyCount, "BodyCount");
            Assert.AreEqual(2, gameCount, "GameCount");
            Assert.AreEqual(6, tagCount, "TagCount");
        }

        [TestMethod()]
        public void TestEscapedQuotes()
        {
            var p = new PdnParser();
            int bodyCount = 0;
            int gameCount = 0;
            int tagCount = 0;
            p.OnBodyFound += (sender, args) => bodyCount++;
            p.OnStartNewGame += (sender, args) => gameCount++;
            p.OnTagFound += (sender, args) => tagCount++;
            p.Parse(_pdnWithEscapedQuote);

            Assert.AreEqual(2, bodyCount, "BodyCount");
            Assert.AreEqual(2, gameCount, "GameCount");
            Assert.AreEqual(6, tagCount, "TagCount");
        }


        [TestMethod()]
        public void TestMalformedPdn()
        {
            var p = new PdnParser();
            int bodyCount = 0;
            int gameCount = 0;
            int tagCount = 0;
            p.OnBodyFound += (sender, args) => bodyCount++;
            p.OnStartNewGame += (sender, args) => gameCount++;
            p.OnTagFound += (sender, args) => tagCount++;
            p.Parse(_pdnMalFormed);

            Assert.AreEqual(0, bodyCount, "BodyCount");
            Assert.AreEqual(0, gameCount, "GameCount");
            Assert.AreEqual(0, tagCount, "TagCount");
        }

        [TestMethod()]
        public void TestEvents()
        {
            List<string> bodies = new List<string>();
            List<string> tagNames = new List<string>();
            List<string> tagValues = new List<string>();
            var p = new PdnParser();

            p.OnBodyFound += (sender, args) => bodies.Add(args.Value);
            p.OnTagFound += (sender, args) => { tagNames.Add(args.Name); tagValues.Add(args.Value); };
            p.Parse(_simplePdn);

            Assert.AreEqual(2, bodies.Count, "BodyCount");
            Assert.AreEqual(6, tagNames.Count, "TagCount");
            Assert.AreEqual("Event", tagNames[0]);
            Assert.AreEqual("Round", tagNames[1]);
            Assert.AreEqual("Result", tagNames[2]);
            Assert.AreEqual("Simpele test", tagValues[0]);
            Assert.AreEqual("1", tagValues[1]);
            Assert.AreEqual("1/2-1/2", tagValues[2]);
            Assert.AreEqual("1. 11-16 *", bodies[1]);
        }

        [TestMethod()]
        public void TestPerformance()
        {
            const string testFilename = @"D:\Users\dg\Documents\Algemeen\PdnFiles\OCA_2.0.pdn";
            var p = new PdnParser();
            int bodyCount = 0;
            int gameCount = 0;
            int tagCount = 0;
            p.OnBodyFound += (sender, args) => bodyCount++;
            p.OnStartNewGame += (sender, args) => gameCount++;
            p.OnTagFound += (sender, args) => tagCount++;
            p.Parse(File.ReadAllText(testFilename));

            Assert.AreEqual(22621, bodyCount, "BodyCount");
            Assert.AreEqual(22621, gameCount, "GameCount");
            Assert.AreEqual(122768, tagCount, "TagCount");
        }

        [TestMethod()]
        public void TestPerformance2()
        {
            const string testFilename = @"D:\Users\dg\Documents\Algemeen\PdnFiles\OCA_2.0.pdn";
            List<string> bodies = new List<string>();
            List<string> tagNames = new List<string>();
            List<string> tagValues = new List<string>();

            var p = new PdnParser();
            p.OnBodyFound += (sender, args) => bodies.Add(args.Value);
            p.OnTagFound += (sender, args) => { tagNames.Add(args.Name); tagValues.Add(args.Value); };

            p.Parse(File.ReadAllText(testFilename));

            Assert.AreEqual(22621, bodies.Count, "BodyCount");
            Assert.AreEqual(122768, tagNames.Count, "TagCount");
        }



    }
}

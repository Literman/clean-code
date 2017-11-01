using System;
using System.IO;
using NUnit.Framework;

namespace Chess
{
	[TestFixture]
	public class ChessProblem_Test
	{
		private static void TestOnFile(string filename)
		{
			var lines = File.ReadAllLines(filename);
		    var board = new BoardParser().ParseBoard(lines);

            var ch = new ChessProblem(board);
			var expectedAnswer = File.ReadAllText(Path.ChangeExtension(filename, ".ans")).Trim();
			ch.GetChessStatusFor(PieceColor.White);
			Assert.AreEqual(expectedAnswer, ch.GetChessStatusFor(PieceColor.White).ToString().ToLower(), "Failed test " + filename);
		}

		[Test]
		public void RepeatedMethodCallDoNotChangeBehaviour()
		{
			var lines = new[]
			{
				"        ",
				"        ",
				"        ",
				"   q    ",
				"    K   ",
				" Q      ",
				"        ",
				"        ",
			};
		    var board = new BoardParser().ParseBoard(lines);
		    var ch = new ChessProblem(board);
			
			Assert.AreEqual(ChessStatus.Check, ch.GetChessStatusFor(PieceColor.White));
			
			// Now check that internal board modifictions during the first call do not change answer
			ch.GetChessStatusFor(PieceColor.White);
			Assert.AreEqual(ChessStatus.Check, ch.GetChessStatusFor(PieceColor.White));
		}

		[Test]
		public void FullTests()
		{
			var dir = TestContext.CurrentContext.TestDirectory;
			var testsCount = 0;
			foreach (var filename in Directory.GetFiles(Path.Combine(dir, "ChessTests"), "*.in"))
			{
				TestOnFile(filename);
				testsCount++;
			}
			Console.WriteLine("Tests passed: " + testsCount);
		}
	}
}
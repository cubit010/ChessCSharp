﻿
using System.Runtime.CompilerServices;

namespace ChessC_
{
    /*
	 
	 copy pasta for inlining stuff

	[MethodImpl(MethodImplOptions.NoInlining)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	 
	 
	 */
    class Program
	{
		public static int MaxDepth = 20;

		static void Main(string[] args)
		{
			// 1) Initialize board from FEN or default start
			Board board = new();

			//custom debug fen

			//pin -7
			//Fen.LoadFEN(board, "6k1/8/8/8/8/4K3/5N2/6q1 w - - 0 1");
			//pin +9
			//Fen.LoadFEN(board, "6k1/8/8/6q1/5N2/4K3/8/8 w - - 0 1");
			//Fen.LoadFEN(board, " rn1qkbnr/pppbpQp1/3p3p/8/2B1P3/8/PPPP1PPP/RNB1K1NR b KQkq - 0 4");

			//Fen.LoadFEN(board, "4k1n1/3pppp1/8/8/8/8/3PPPP1/4K1N1 w - - 0 1");

			//Fen.LoadFEN(board, "4k3/3pppp1/8/8/8/8/3PPPP1/4K3 w - - 0 1"); 
			//Fen.LoadFEN(board, "4k3/3pp3/5p2/6p1/3PP3/8/5PP1/4K3 w - - 0 1");
			//Fen.LoadFEN(board, "r1bqkbnr/p1pppppp/1pn5/8/4P3/2N5/PPPP1PPP/R1BQKBNR w KQkq - 2 3");
			//Fen.LoadFEN(board, "rnbqkb1r/1ppppp2/8/p5pp/4nP1K/8/PPPP2PP/RNBQ1BNR w - - 0 1");

			//Fen.LoadFEN(board, "r1bqkbnr/p1p1pppp/1pn5/3p4/4P1P1/2N5/PPPP1P1P/R1BQKBNR w KQkq d6 0 4");
			//black pawn enpassant
			//Fen.LoadFEN(board, "4k3/8/8/8/5p2/8/4P3/4K3 w - - 0 1");

			//black pawn regular take
			//Fen.LoadFEN(board, "4k3/8/8/5p2/8/8/4P3/4K3 w - - 0 1");

			//black pawn not capature
			//Fen.LoadFEN(board, "4k3/8/5p2/8/8/8/4P3/4K3 w - - 0 1");

			// En Passant White Test
			//Fen.LoadFEN(board, "rnbqkbnr/pppp1ppp/8/3Pp3/8/8/PPP2PPP/RNBQKBNR w KQkq e6 0 3");

			// En Passant Black Test (try e2e4, then Black plays d4xe3 e.p.)
			//Fen.LoadFEN(board, "rnbqkbnr/pppppppp/8/8/3p4/8/PPP1P1PP/RNBQKBNR w KQkq - 0 2");

			// K B Q vs k
			//Fen.LoadFEN(board, "4k3/8/8/8/8/8/8/2BQK3 w - - 0 1");

			// Castling Not Allowed (through check or out of check) — test by placing piece attacking f1/f8 or e1/e8
			// Fen.LoadFEN(board, "r3k2r/pppq1ppp/2n2n2/3pp3/3PP3/2N2N2/PPPQ1PPP/R3K2R w KQkq - 0 1"); // try O-O and O-O-O

			// Discovered Check Test — play e4 to open bishop's diagonal to give check
			// Fen.LoadFEN(board, "r1b2rk1/pp1n1ppp/2p1pn2/3p4/3P4/2N1PN2/PP1N1PPP/R1B2RK1 w - - 0 1");

			// Queen & Rook Threats — verify sliding checks and pins
			// Fen.LoadFEN(board, "r4rk1/ppp2ppp/2n5/3q4/3P4/4Q3/PPP2PPP/R3R1K1 w - - 0 1");

			// Promotion Test — try d8=Q+, d8=R+, d8=N+, or d8=B+
			//Fen.LoadFEN(board, "8/3P4/3k4/8/8/3K4/8/8 w - - 0 1");

			//M1 white Qa8#
			//Fen.LoadFEN(board, "6k1/5ppp/5ppp/8/8/8/5PPP/Q5K1 w - - 0 1");
			//M1 black Re1#
			//Fen.LoadFEN(board, "r5k1/8/8/8/8/6PP/5PPP/7K w - - 0 1");


			//---------------------------//
			// Regular Opening Position  //
			//---------------------------//


			Fen.LoadFEN(board, "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");

			//make sure all data tables are initialized and ready to go at the start
            RuntimeHelpers.RunClassConstructor(typeof(MoveTables).TypeHandle);
            RuntimeHelpers.RunClassConstructor(typeof(Bitboard).TypeHandle);
            RuntimeHelpers.RunClassConstructor(typeof(Magics).TypeHandle);
            var stopwatch = new System.Diagnostics.Stopwatch();

			bool playerPlaysWhite;

			// 2) Create transposition table (e.g. 16 MB)
			var tt = new TranspositionTable(256);
			tt.NewSearch();
			Utils.PrintBoard(board);
			//parameters

			 
			int msThink = 5000; // Default thinking time in milliseconds
			//int threads = 8;
			//bool useSMP = false;
			

			// 3) Ask player if they play as White or Black
			while (true)
			{
				Console.Write("Select color to play as (W/B): ");
				string colorInput = Console.ReadLine()?.Trim().ToLowerInvariant();
				if (colorInput == "w" || colorInput == "white" )
				{
					playerPlaysWhite = true;
					break;
				}
				else if (colorInput == "b" || colorInput == "black")
				{
					playerPlaysWhite = false;
					break;
				}
				else
				{
					Console.WriteLine("Please enter 'w'/'white' for White or 'b'/'black' for Black.");
				}
			}
			while (true)
			{
				Console.Write("How many seconds should the engine think for each move? (default 5): ");
				string secInput = Console.ReadLine()?.Trim();
				if (string.IsNullOrEmpty(secInput))
				{
					msThink = 5000;
					break;
				}
				if (int.TryParse(secInput, out int seconds) && seconds > 0 && seconds < 121)
				{
					msThink = seconds * 1000;
					break;
				}
				Console.WriteLine("Please enter a valid number of seconds (1-120), or press Enter for default.");
			}

			
			if (!playerPlaysWhite)
			{
				if (board.sideToMove == Color.White)
				{
                    // If the player plays black, we need to make the first move for white
                    // This is just a placeholder; you can change it to any opening move you like
                    Move firstMove = Search.FindBestMove(board, tt, msThink);
                    board.MakeRealMove(firstMove);
                    Console.WriteLine($"Engine> {MoveNotation.ToAlgebraicNotation(firstMove)}\n");
                }

			} else
			{
				if (board.sideToMove == Color.Black)
				{
                    // If the player plays black, we need to make the first move for white
                    // This is just a placeholder; you can change it to any opening move you like
                    Move firstMove = Search.FindBestMove(board, tt, msThink);
                    board.MakeRealMove(firstMove);
                    Console.WriteLine($"Engine> {MoveNotation.ToAlgebraicNotation(firstMove)}\n");
                }
            }

				Move[] playerBuffer = new Move[256];
            Console.WriteLine("Enter moves in SAN (e.g. e4, Nf3, O-O). Type 'quit' to exit.\n");

			while (true)
			{
				Utils.PrintBoard(board);
				Console.WriteLine();
				// [A] Generate all legal moves at the start of this turn
				bool isWhiteToMove = (board.sideToMove == Color.White);
				var span = new Span<Move>(playerBuffer);
				int legalCount = 0;

				// fill it
				MoveGen.FilteredLegalMoves(board, span, ref legalCount, isWhiteToMove);

				// now you have the first legalCount elements as your “legal moves”
				Span<Move> legalMoves = span.Slice(0, legalCount);
				/**
				foreach (Move move in legalMoves) {
					Console.WriteLine(MoveNotation.ToAlgebraicNotation(move, legalMoves));
				}/**/
				//MoveGen.FlagCheckAndMate(board, legalMoves, isWhiteToMove);
				//Utils.PrintBoard(board);

				if (legalCount == 0)
				{
					Console.WriteLine("No legal moves available.");
					if (MoveGen.IsInCheck(board, isWhiteToMove))
					{
						Console.WriteLine("Checkmate! " + (isWhiteToMove ? "Black" : "White") + " wins.");
					}
					else
					{
						Console.WriteLine("Stalemate! The game is a draw.");
					}
				}

				var sanToMove = new Dictionary<string, Move>(StringComparer.Ordinal);
				foreach (var m in legalMoves)
				{
					string san = MoveNotation.ToAlgebraicNotation(m, legalMoves);
					if (!sanToMove.ContainsKey(san))
						sanToMove[san] = m;
				}

				// [B] Input move from player
				Move userMove;
				while (true)
				{
					Console.Write(board.sideToMove == Color.White ? "White> " : "Black> ");
					string input = Console.ReadLine()?.Trim();
					if (input == "quit")
						return;

					if (sanToMove.TryGetValue(input, out userMove))
						break;

					Console.WriteLine("Invalid SAN or ambiguous—try again.\n");
				}

				board.MakeRealMove(userMove);
				if ((userMove.Flags & MoveFlags.Checkmate) != 0)
				{
					Console.WriteLine("Checkmate! " + (isWhiteToMove ? "White" : "Black") + " wins.");
					break;
				}
				//Utils.PrintBoard(board);

				// [C] Engine responds
				Search.NodesVisited = 0;
				stopwatch.Restart();

                Move[] engLegalBuffer = new Move[256];

                var engSpan = new Span<Move>(engLegalBuffer);
				int engLegalCount = 0;

				// fill it
				MoveGen.FilteredLegalMoves(board, engSpan, ref engLegalCount, !isWhiteToMove);
				
				Span<Move> legalEngineMoves = engSpan.Slice(0, engLegalCount);
				/**
				foreach (Move move in legalEngineMoves)
				{
					Console.WriteLine(MoveNotation.ToAlgebraicNotation(move, legalEngineMoves));
				}
				/**/
				Move engineMove = Search.FindBestMove(board, tt, msThink);

				string engineSan = MoveNotation.ToAlgebraicNotation(engineMove, legalEngineMoves);

				Console.WriteLine($"Engine> {engineSan}\n");
				board.MakeRealMove(engineMove);
				//stopwatch to time search time
				stopwatch.Stop();
				Console.WriteLine($"Nodes: {Search.NodesVisited}, Time: {stopwatch.ElapsedMilliseconds}ms");

				if ((engineMove.Flags & MoveFlags.Checkmate) != 0)
				{
					Console.WriteLine("Checkmate! " + (!isWhiteToMove ? "White" : "Black") + " wins.");
					break;
				}

			}
		}

	}
}

import sys
import chess
import chess.engine

def get_best_move(fen):
    stockfish_path = "stockfish\stockfish-windows-x86-64-avx2.exe"
    board = chess.Board(fen)

    with chess.engine.SimpleEngine.popen_uci(stockfish_path) as engine:
        result = engine.play(board, chess.engine.Limit(time=2.0))  # Adjust time for move calculation
        best_move = result.move
        return best_move.uci()

if __name__ == "__main__":
    # Get FEN from command line argument
    fen = sys.argv[1]
    best_move = get_best_move(fen)  # Call the function to get the best move
    print(best_move)  # Print the best move, which will be captured by C#

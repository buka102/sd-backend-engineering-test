using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tictactoe_service.Logic
{
    public class BasicGameEvaluator : IGameEvaluator
    {

        private int[,,] WinningLines = new int[8, 3, 2] {
            { {0,0}, {1,0 }, {2,0 } },
            { {0,1}, {1,1 }, {2,1 } },
            { {0,2}, {1,2 }, {2,2 } },
            { {0,0}, {1,1 }, {2,2 } },
            { {0,2}, {1,1 }, {2,0 } },
            { {0,0}, {0,1 }, {0,2 } },
            { {1,0}, {1,1 }, {1,2 } },
            { {2,0}, {2,1 }, {2,2 } }}; 

        /// <summary>
        /// Evaluates TicTacToe board for winning state;
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public string EvaluateBoard(string[,] board)
        {
            int rowsOrHeight = board.GetLength(0);
            int colsOrWidth = board.GetLength(1);
            if (rowsOrHeight!=3 || colsOrWidth != 3)
            {
                throw new ArgumentOutOfRangeException("board is out of range");
            }

            for(var lineIndex = 0; lineIndex < WinningLines.GetLength(0); lineIndex++)
            {
                var firstCellValue = GetBoardCell(board, lineIndex, 0);
                if (firstCellValue == string.Empty)
                {
                    //if cell is empty, then it cannot be winning
                    continue;
                }
                var matchAllCellsInLine = true;
                for(var c=1; c<=2; c++)
                {
                    var currentCellValue = GetBoardCell(board, lineIndex, c);
                    if (currentCellValue != firstCellValue)
                    {
                        //value does not match firstValue
                        matchAllCellsInLine = false;
                        c = 3;
                    }
                }

                if (matchAllCellsInLine)
                {
                    return firstCellValue;
                }

            }

            //check for draws
            for(var i=0; i<=2; i++)
            {
                for(var j=0; j<=2; j++)
                {
                    if (string.IsNullOrEmpty(board[i, j]))
                    {
                        return string.Empty; //game is still in progress
                    }
                }
            }


            return "draw";

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="board">game board</param>
        /// <param name="line">line Index in WinningLines</param>
        /// <param name="sequence">Position in winning board</param>
        /// <returns></returns>
        private string GetBoardCell(string[,] board, int line, int sequence)
        {
            var x = WinningLines[line, sequence, 0];
            var y = WinningLines[line, sequence, 1];
            return board[x, y];
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tictactoe_service.Logic
{
    public interface IGameEvaluator
    {
        string EvaluateBoard(string[,] board);
    }
}

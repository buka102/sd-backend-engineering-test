using FluentValidation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace tictactoe_service.CQRS.Commands.UpdateGameCommand
{
    public class UpdateGameRequestValidator : AbstractValidator<UpdateGameRequest>
    {

        public UpdateGameRequestValidator()
        {
            RuleFor(m => m.Id).NotEmpty().WithMessage("id is required");
            RuleFor(m => m.Player).InclusiveBetween(1, 2).WithMessage("'player' can be either 1 or 2");
            RuleFor(m => m.MoveCoordinate).Must(c => IsValidData(c)).WithMessage("move_coordinates must in pattern \"x,y\" where x/y is between 0 and 2");
        }

        private bool IsValidData(string coord)
        {
            try
            {
                if (UpdateGameRequestHandler.ParseCoordinate(coord, out int x, out int y))
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}

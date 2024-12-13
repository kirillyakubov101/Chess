using UnityEngine;

public class HumanPlayer : Profile
{
    public override void PlayTurn()
    {
        base.PlayTurn();
        print(" player plays");
    }
}

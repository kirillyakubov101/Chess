
public class AIPlayer : Profile
{
    public override void PlayTurn()
    {
        base.PlayTurn();
        GameMode.Instance.CompleteTurn();
    }
}

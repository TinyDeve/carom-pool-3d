using strange.extensions.context.impl;

public class GameRoot : ContextView
{
    public static GameRoot Instance;
    
    private void Awake()
    {
        Instance = this;
        context = new GameContext(this);
    }
}
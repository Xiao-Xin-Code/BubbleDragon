using QMVC;

public class BubbleDragon : Architecture<BubbleDragon>
{
    protected override void Init()
    {
		RegisterSystem<AssetSystem>(new AssetSystem());
		RegisterSystem<PoolSystem>(new PoolSystem());
		RegisterModel<LevelModel>(new LevelModel());
        
	}
}

using CustomContainer;
using Assets.Scripts.Factories;

public class MainInstaller : Installer
{
    private void Awake()
    {
        _container.Register<EventService>();
        _container.Register<BalanceService>();
        _container.Register<MainFactory>();
        _container.Register<GamePlayController>();
    }
}

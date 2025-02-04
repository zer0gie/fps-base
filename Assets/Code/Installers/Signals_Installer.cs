using Zenject;

namespace Code.Installers
{
    public class SignalsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);

            Container.DeclareSignal<FireActionCancelledSignal>().OptionalSubscriber();

            Container.DeclareSignal<FireActionStartedSignal>().OptionalSubscriber();

            Container.DeclareSignal<ReloadPerformedSignal>().OptionalSubscriber();

            Container.DeclareSignal<FirstSlotActivatedSignal>();

            Container.DeclareSignal<SecondSlotActivatedSignal>();

            Container.DeclareSignal<ThirdSlotActivatedSignal>();
        }
    }
}
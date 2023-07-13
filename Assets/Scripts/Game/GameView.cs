using Base;
using Zenject;

namespace Game
{
    public class GameView : BaseView
    {
        private GamePresenter _presenter;
        
        [Inject]
        private void Construct(GamePresenter gamePresenter)
        {
            _presenter = gamePresenter;
        }

        private void Start()
        {
            _presenter.StartGame();
        }

        private void OnDestroy()
        {
            _presenter.Dispose();
        }
    }
}

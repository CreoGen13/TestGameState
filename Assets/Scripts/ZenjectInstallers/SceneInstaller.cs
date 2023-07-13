using Game;
using Mono;
using Other;
using Services;
using UI.SkillTree;
using UnityEngine;
using Zenject;

namespace ZenjectInstallers
{
    public class SceneInstaller : MonoInstaller
    {
        [Header("References")]
        [SerializeField] private GameView gameView;
        [SerializeField] private SkillTreeView skillTreeView;
        [SerializeField] private GameLoader gameLoader;
        
        // [Header("Prefabs")]
        // [SerializeField] private GameObject linePrefab;
        public override void InstallBindings()
        {
            BindGame();
            BindServices();
            BindSkillTree();
            // BindLineFactory();
        }

        private void BindGame()
        {
            
            Container
                .Bind<GameView>()
                .FromInstance(gameView)
                .AsSingle();
            Container
                .Bind<GameModel>()
                .FromInstance(new GameModel())
                .AsSingle();
            Container
                .Bind<GamePresenter>()
                .AsSingle();
        }
        private void BindSkillTree()
        {
            var skillTreePresenter = PresenterFactory.Create<SkillTreeModel, SkillTreePresenter, SkillTreeView>(skillTreeView);
            Container
                .Bind<SkillTreePresenter>()
                .FromInstance(skillTreePresenter)
                .AsSingle()
                .NonLazy();
        }
        private void BindServices()
        {
            Container
                .Bind<GameLoader>()
                .FromInstance(gameLoader)
                .AsSingle();
        }
        // private void BindLineFactory()
        // {
        //     Container
        //         .Bind<LineFactory>()
        //         .AsSingle()
        //         .NonLazy();
        // }
    }
}
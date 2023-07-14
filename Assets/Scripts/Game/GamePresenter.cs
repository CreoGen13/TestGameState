using System;
using Base;
using Other;
using Services;
using UI.SkillTree;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game
{
    public class GamePresenter : BasePresenter <GameModel, GameView>
    {
        private readonly GameLoader _gameLoader;
        private readonly SkillTreePresenter _skillTreePresenter;
        
        private IDisposable _skillsSubscription;
        private IDisposable _pointsSubscription;

        [Inject]
        public GamePresenter(GameLoader gameLoader, SkillTreePresenter skillTreePresenter, GameModel gameModel, GameView gameView)
            : base(gameModel, gameView)
        {
            _gameLoader = gameLoader;
            _skillTreePresenter = skillTreePresenter;

            Init();
        }

        protected sealed override void Init()
        {
            InitActions();
            InitObservables();
           

            void InitActions()
            {
                
                _skillTreePresenter.OnSkillUpdated += (state, index, points) =>
                {
                    UpdateUndirectedSkillState(state, index, points);
                    _skillTreePresenter.SetCurrentNodeState(
                        new SkillTreePresenter.CurrentNodeState(
                            Model.UndirectedSkills[index].State,
                            Model.UndirectedIsForgetBlocked(index),
                            Model.IsUnlockBlocked(index, points))
                    );
                };
                _skillTreePresenter.OnSkillSelected += (index, points) =>
                {
                    _skillTreePresenter.SetCurrentNodeState(
                        new SkillTreePresenter.CurrentNodeState(
                            Model.UndirectedSkills[index].State,
                            Model.UndirectedIsForgetBlocked(index),
                            Model.IsUnlockBlocked(index, points))
                    );
                };
                _skillTreePresenter.OnSkillsForgetAll += (index, points) =>
                {
                    Model.UndirectedResetAll();
                    Model.UndirectedResolveSkill(Model.StartIndex);
                    Model.Update();
                    
                    _skillTreePresenter.SetCurrentNodeState(
                        new SkillTreePresenter.CurrentNodeState(
                            Model.UndirectedSkills[index].State,
                            Model.UndirectedIsForgetBlocked(index),
                            Model.IsUnlockBlocked(index, points))
                    );
                };
                _skillTreePresenter.OnGetPoint += (index, points) =>
                {
                    Model.Points++;
                    Model.Update();
                    
                    _skillTreePresenter.SetCurrentNodeState(
                        new SkillTreePresenter.CurrentNodeState(
                            Model.UndirectedSkills[index].State,
                            Model.UndirectedIsForgetBlocked(index),
                            Model.IsUnlockBlocked(index, points))
                    );
                };
            }
            void InitObservables()
            {
                SequenceComparer<UndirectedSkill[]> comparer = new SequenceComparer<UndirectedSkill[]>();
                _skillsSubscription = Model.Observe()
                    .Select(model => model.UndirectedSkills)
                    .DistinctUntilChanged(comparer)
                    .Subscribe(
                        value =>
                        {
                            _skillTreePresenter.UpdateSkillsView(value);
                        })
                    .AddTo(Disposable);
                
                _pointsSubscription = Model.Observe()
                    .Select(model => model.Points)
                    .DistinctUntilChanged(points => points.GetHashCode())
                    .Subscribe(
                        value =>
                        {
                            _skillTreePresenter.UpdatePointsView(value);
                        })
                    .AddTo(Disposable);
            }
        }

        public void StartGame()
        {
            Model.StartIndex = _gameLoader.LoadStartSkillsStateUndirectedGraph(ref Model.UndirectedSkills, ref Model.AdjacencyMatrix);
            Model.UndirectedResolveSkill(Model.StartIndex);
            Model.Update();
        }

        // Логика направленного графа
        #region Directed

        private void UpdateDirectedSkillsState(DirectedSkill[] skills, int startIndex)
        {
            Model.DirectedSkills = skills;
            Model.DirectedResolveSkill(startIndex);
            Model.Update();
        }
        private void UpdateDirectedSkillState(SkillState state, int index)
        {
            DirectedSkill[] newSkills = new DirectedSkill[Model.DirectedSkills.Length];
            Model.DirectedSkills.CopyTo(newSkills, 0);
            newSkills[index] = new DirectedSkill(Model.DirectedSkills[index], state);

            Model.DirectedSkills = newSkills;
            Model.DirectedResolveSkill(index);
            Model.Update();
        }

        #endregion
        
        private void UpdateUndirectedSkillState(SkillState state, int index, int points)
        {
            UndirectedSkill[] newSkills = new UndirectedSkill[Model.UndirectedSkills.Length];
            Model.UndirectedSkills.CopyTo(newSkills, 0);
            newSkills[index] = new UndirectedSkill(Model.UndirectedSkills[index], state);

            Model.UndirectedSkills = newSkills;
            Model.UndirectedResolveSkill(index);
            Model.ChangePoints(state, points);
            Model.Update();
        }
        
        public enum SkillState
        {
            Inactive,
            Available,
            Active,
        }
    }
}

using System;
using Base;
using Mono;
using Other;
using UniRx;
using UnityEngine;
using Zenject;
using static Game.GamePresenter;

namespace UI.SkillTree
{
    public class SkillTreePresenter : BasePresenter<SkillTreeModel, SkillTreeView>
    {
        public Action<SkillState, int, int> OnSkillUpdated;
        public Action<int, int> OnSkillSelected;
        public Action<int, int> OnGetPoint;
        public Action OnSkillsForgetAll;

        private IDisposable _currentSkillStateSubscription;
        
        [Inject]
        public SkillTreePresenter(SkillTreeModel model, SkillTreeView view)
            : base(model, view)
        {

            Init();
        }
        protected sealed override void Init()
        {
            _currentSkillStateSubscription = Model.Observe()
                .Select(model => model.CurrentNodeState)
                .DistinctUntilChanged(state => state.GetHashCode())
                .Subscribe(
                    value =>
                    {
                        switch (value.State)
                        {
                            case SkillState.Active:
                            {
                                View.SetBottomButtonsState(false,  !value.ForgetBlocked);
                                break;
                            }
                            case SkillState.Available:
                            {
                                View.SetBottomButtonsState(!value.UnlockBlocked, false);
                                break;
                            }
                            case SkillState.Inactive:
                            {
                                View.SetBottomButtonsState(false, false);
                                break;
                            }
                        }
                    })
                .AddTo(Disposable);
        }

        public void SetCurrentNodeState(CurrentNodeState currentNodeState)
        {
            Model.CurrentNodeState = currentNodeState;
            Model.Update();
        }
        public SkillNode[] GenerateSkills()
        {
            var skillNodes = View.GenerateSkillNodes();
            View.InstantiateLines();
            
            return skillNodes;
        }
        public void UpdateSkillsView(UndirectedSkill[] skills)
        {
            View.UpdateSkillsView(skills);
        }
        public void UpdatePointsView(int points)
        {
            View.UpdatePointsView(points);
        }
        
        public readonly struct CurrentNodeState
        {
            public readonly SkillState State;
            public readonly bool UnlockBlocked;
            public readonly bool ForgetBlocked;

            public CurrentNodeState(SkillState state, bool forgetBlocked, bool unlockBlocked)
            {
                State = state;
                ForgetBlocked = forgetBlocked;
                UnlockBlocked = unlockBlocked;
            }

            public override int GetHashCode()
            {
                return (ForgetBlocked ? 1 : 0) + (UnlockBlocked ? 1 : 0) * 10 + (int)State * 100;
            }
        }
    }
}
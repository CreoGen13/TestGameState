using System;
using Base;
using Game;
using UniRx;
using static UI.SkillTree.SkillTreePresenter;

namespace UI.SkillTree
{
    public class SkillTreeModel : BaseModel
    {
        private readonly BehaviorSubject<SkillTreeModel> _subject;

        public CurrentNodeState CurrentNodeState = new(GamePresenter.SkillState.Active, true, true);

        public SkillTreeModel()
        {
            _subject = new BehaviorSubject<SkillTreeModel>(this);
        }

        public IObservable<SkillTreeModel> Observe()
        {
            return _subject.AsObservable();
        }

        public void Update()
        {
            _subject.OnNext(this);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Game;
using ModestTree;
using Mono;
using Other;
using Scriptables;
using TMPro;
using UI.BaseUI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.SkillTree
{
    public class SkillTreeView : BaseUIView
    {
        [SerializeField] private Transform linesParent;
        [SerializeField] private GameObject linePrefab;
        
        [SerializeField] private Button buttonGetPoint;
        [SerializeField] private Button buttonUnlock;
        [SerializeField] private Button buttonForget;
        [SerializeField] private Button buttonForgetAll;
        
        [SerializeField] private TextMeshProUGUI pointsText;
        
        private SkillTreePresenter _presenter;
        private ScriptableSettings _settings;
        private LineFactory _lineFactory;
        
        private SkillNode[] _skillNodes;
        private SkillNode _currentSkillNode;
        private int _currentSkillIndex;

        [Inject]
        private void Construct(SkillTreePresenter skillTreePresenter, ScriptableSettings settings)
        {
            _presenter = skillTreePresenter;
            _settings = settings;
        }

        private void Start()
        {
            _skillNodes = window.GetComponentsInChildren<SkillNode>(true);
            _lineFactory = new LineFactory(linePrefab);
            
            InitButtons();
        }
        private void InitButtons()
        {
            for(int i = 0; i< _skillNodes.Length; i++)
            {
                var index = i;
                _skillNodes[i].SetChooseImage(false);
                _skillNodes[i].SkillButton.onClick.AddListener(() =>
                {
                    _currentSkillIndex = index;
                        
                    if(_currentSkillNode != null)
                    {
                        _currentSkillNode.SetChooseImage(false);
                    }
                    _currentSkillNode = _skillNodes[index];
                    _currentSkillNode.SetChooseImage(true);
                    _presenter.OnSkillSelected?.Invoke(index, _currentSkillNode.Points);
                });
            }
            
            buttonUnlock.onClick.AddListener(() =>
            {
                _presenter.OnSkillUpdated?.Invoke(GamePresenter.SkillState.Active, _currentSkillIndex, _currentSkillNode.Points);
            });
            buttonForget.onClick.AddListener(() =>
            {
                _presenter.OnSkillUpdated?.Invoke(GamePresenter.SkillState.Available, _currentSkillIndex, _currentSkillNode.Points);
            });
            buttonForgetAll.onClick.AddListener(() =>
            {
                _presenter.OnSkillsForgetAll?.Invoke();
            });
            buttonGetPoint.onClick.AddListener(() =>
            {
                if (_currentSkillNode == null)
                {
                    _presenter.OnGetPoint?.Invoke(0, 0);
                    return;
                }
                _presenter.OnGetPoint?.Invoke(_currentSkillIndex, _currentSkillNode.Points);
            });
        }
        public void SetBottomButtonsState(bool buttonUnlockState, bool buttonForgetState)
        {
            buttonUnlock.interactable = buttonUnlockState;
            buttonForget.interactable = buttonForgetState;
        }

        public SkillNode[] GetSkillNodes()
        {
            return _skillNodes;
        }
        public void InstantiateLines()
        {
            foreach (var skillNode in _skillNodes)
            {
                if (!skillNode.NextSkills.IsEmpty())
                {
                    foreach (var nextNode in skillNode.NextSkills)
                    {
                        if(nextNode == null)
                            continue;
                        var line = _lineFactory.Create(linesParent);
                        var difference = skillNode.transform.localPosition - nextNode.transform.localPosition;
                        var pos = nextNode.transform.localPosition + difference / 2;
                        var length = Mathf.Abs(difference.magnitude);
                        var angle = (difference.x > 0 ? -1 : 1) * Quaternion.LookRotation(difference).eulerAngles.x;
                        line.SetTransform(
                            new Vector2(length, _settings.lineHeight),
                            pos,
                            angle);
                    }
                }
            }
        }
        public void UpdateSkillsView(UndirectedSkill[] skills)
        {
            foreach (var skill in skills)
            {
                switch (skill.State)
                {
                    case GamePresenter.SkillState.Active:
                    {
                        skill.SetColor(_settings.skillColorActive);
                        break;
                    }
                    case GamePresenter.SkillState.Available:
                    {
                        skill.SetColor(_settings.skillColorAvailable);
                        break;
                    }
                    case GamePresenter.SkillState.Inactive:
                    {
                        skill.SetColor(_settings.skillColorInactive);
                        break;
                    }
                }
            }
        }
        public void UpdatePointsView(int points)
        {
            pointsText.text = points.ToString();
        }

        private void OnDestroy()
        {
            _presenter.Dispose();
        }
    }
}
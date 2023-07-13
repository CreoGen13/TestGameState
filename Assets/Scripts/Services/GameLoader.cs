using ModestTree;
using Other;
using UI.SkillTree;
using UnityEngine;
using Zenject;
using static Game.GamePresenter;

namespace Services
{
    public class GameLoader : MonoBehaviour
    {
        private SkillTreePresenter _skillTreePresenter;
        private int _startNumber;
        

        [Inject]
        private void Construct(SkillTreePresenter skillTreePresenter)
        {
            _skillTreePresenter = skillTreePresenter;
        }

        // Загрузка направленного графа
        public DirectedSkill[] LoadStartSkillsStateDirectedGraph()
        {
            var skillNodes = _skillTreePresenter.GenerateSkills();
            DirectedSkill[] skills = new DirectedSkill[skillNodes.Length];
            for (int i = 0; i < skillNodes.Length; i++)
            {
                if (skillNodes[i].IsStartNode)
                {
                    _startNumber = i;
                    skills[i] = new DirectedSkill(skillNodes[i], SkillState.Active);
                }
                else
                {
                    skills[i] = new DirectedSkill(skillNodes[i], SkillState.Inactive);
                }
            }
            
            for (int i = 0; i < skillNodes.Length; i++)
            {
                skills[i].NextSkillsIndex = new int[skillNodes[i].NextSkills.Length];

                for (int j = 0; j < skillNodes[i].NextSkills.Length; j++)
                {
                    var nextNode = skillNodes[i].NextSkills[j];
                    var index = skillNodes.IndexOf(nextNode);

                    skills[i].NextSkillsIndex[j] = index;
                    skills[index].PrevSkillsIndex.Add(i);
                }
            }

            return skills;
        }

        // Загрузка ненаправленного графа
        public int LoadStartSkillsStateUndirectedGraph(ref UndirectedSkill[] skills, ref bool[,] adjacencyMatrix)
        {
            var skillNodes = _skillTreePresenter.GenerateSkills();
            skills = new UndirectedSkill[skillNodes.Length];
            adjacencyMatrix = new bool[skillNodes.Length, skillNodes.Length];
            
            for (int i = 0; i < skillNodes.Length; i++)
            {
                if (skillNodes[i].IsStartNode)
                {
                    _startNumber = i;
                    skills[i] = new UndirectedSkill(skillNodes[i], SkillState.Active);
                }
                else
                {
                    skills[i] = new UndirectedSkill(skillNodes[i], SkillState.Inactive);
                }
                
                for (int j = 0; j < skillNodes[i].NextSkills.Length; j++)
                {
                    var nextNode = skillNodes[i].NextSkills[j];
                    var index = skillNodes.IndexOf(nextNode);

                    adjacencyMatrix[i, index] = true;
                    adjacencyMatrix[index, i] = true;
                }
            }

            return _startNumber;
        }
    }
}

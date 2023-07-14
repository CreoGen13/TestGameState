using System;
using System.Collections.Generic;
using System.Linq;
using Base;
using Other;
using UniRx;
using UnityEngine;
using static Game.GamePresenter;

namespace Game
{
    public class GameModel : BaseModel
    {
        private readonly BehaviorSubject<GameModel> _subject;

        public DirectedSkill [] DirectedSkills = Array.Empty<DirectedSkill>();
        public UndirectedSkill [] UndirectedSkills = Array.Empty<UndirectedSkill>();
        public int StartIndex = -1;

        public int Points = 0;
        private int _allPointsSpend = 0;
        
        public bool[,] AdjacencyMatrix;
        private bool[,] _activeMatrix;

        public void ChangePoints(SkillState state, int points)
        {
            switch (state)
            {
                case SkillState.Active:
                {
                    Points -= points;
                    _allPointsSpend += points;
                    
                    break;
                }
                case SkillState.Available:
                {
                    Points += points;
                    _allPointsSpend -= points;
                    
                    break;
                }
            }
        }

        public bool IsUnlockBlocked(int index, int points)
        {
            if (index == StartIndex)
                return true;
            return Points < points;
        }

        // Логика направленного графа
        #region Directed
        
        public void DirectedResolveSkill(int index)
        {
            for(int i = 0; i < DirectedSkills[index].NextSkillsIndex.Length; i++)
            {
                switch (DirectedSkills[index].State)
                {
                    case SkillState.Active:
                    {
                        if (DirectedSkills[DirectedSkills[index].NextSkillsIndex[i]].State == SkillState.Inactive)
                        {
                            
                            DirectedSkills[DirectedSkills[index].NextSkillsIndex[i]] = new DirectedSkill(DirectedSkills[DirectedSkills[index].NextSkillsIndex[i]], SkillState.Available);
                        }
                        break;
                    }
                    case SkillState.Available:
                    {
                        if (DirectedSkills[DirectedSkills[index].NextSkillsIndex[i]].State == SkillState.Available)
                        {
                            if(DirectedSkills[DirectedSkills[index].NextSkillsIndex[i]].PrevSkillsIndex.Any(x => DirectedSkills[x].State == SkillState.Active))
                            {
                                continue;
                            }

                            DirectedSkills[DirectedSkills[index].NextSkillsIndex[i]] = new DirectedSkill(DirectedSkills[DirectedSkills[index].NextSkillsIndex[i]], SkillState.Inactive);
                        }
                        break;
                    }
                }
            }
        }
        public void DirectedResetAll()
        {
            DirectedSkill[] newSkills = new DirectedSkill[DirectedSkills.Length];
            DirectedSkills.CopyTo(newSkills, 0);
            
            for(int i = 0; i < DirectedSkills.Length; i ++)
            {
                if(i == StartIndex)
                {
                    newSkills[i] = new DirectedSkill(DirectedSkills[i], SkillState.Active);
                }
                else
                {
                    newSkills[i] = new DirectedSkill(DirectedSkills[i], SkillState.Inactive);
                }
            }

            DirectedSkills = newSkills;
        }
        public bool DirectedHasAllNextActive(int index)
        {
            if (DirectedSkills[index].State == SkillState.Active &&
                DirectedSkills[index].NextSkillsIndex.Length > 0 && 
                DirectedSkills[index].NextSkillsIndex.All(x => DirectedSkills[x].State == SkillState.Active))
            {
                return true;
            }

            return false;
        }

        #endregion
        // Логика ненаправленного графа
        #region Undirected
        
        private void UpdateActiveMatrix(ref bool[,] matrix, int index, SkillState state)
        {
            if (_activeMatrix == null)
            {
                _activeMatrix = new bool[UndirectedSkills.Length, UndirectedSkills.Length];
            }
            
            var next = GetAllNextIndices(index);

            switch (state)
            {
                case SkillState.Active:
                {
                    var nextReachable = next.FindAll(x => UndirectedSkills[x].State == SkillState.Active);

                    foreach (var reachableIndex in nextReachable)
                    {
                        matrix[reachableIndex, index] = true;
                        matrix[index, reachableIndex] = true;
                    }
                    
                    break;
                }
                case SkillState.Available:
                {
                    for (int i = 0; i < matrix.GetUpperBound(0) - 1; i++)
                    {
                        matrix[index, i] = false;
                        matrix[i, index] = false;
                    }
                    
                    break;
                }
            }
        }
        
        public void UndirectedResolveSkill(int index)
        {
            List<int> nextIndices = GetAllNextIndices(index);
            
            for(int i = 0; i < nextIndices.Count; i++)
            {
                if(nextIndices[i] == StartIndex)
                    continue;
                switch (UndirectedSkills[index].State)
                {
                    case SkillState.Active:
                    {
                        if (UndirectedSkills[nextIndices[i]].State == SkillState.Inactive)
                        {
                            UndirectedSkills[nextIndices[i]] = new UndirectedSkill(UndirectedSkills[nextIndices[i]], SkillState.Available);
                        }

                        break;
                    }
                    case SkillState.Available:
                    {
                        var nextNextIndices = GetAllNextIndices(nextIndices[i]);
                        
                        if (nextNextIndices.Any(x => UndirectedSkills[x].State == SkillState.Active))
                        {
                            continue;
                        }
                        UndirectedSkills[nextIndices[i]] = new UndirectedSkill(UndirectedSkills[nextIndices[i]], SkillState.Inactive);
                        
                        break;
                    }
                }
            }
            
            UpdateActiveMatrix(ref _activeMatrix, index, UndirectedSkills[index].State);
        }
        public void UndirectedResetAll()
        {
            _activeMatrix = new bool[UndirectedSkills.Length, UndirectedSkills.Length];
            Points += _allPointsSpend;
            _allPointsSpend = 0;
            
            UndirectedSkill[] newSkills = new UndirectedSkill[UndirectedSkills.Length];
            UndirectedSkills.CopyTo(newSkills, 0);
            
            for(int i = 0; i < UndirectedSkills.Length; i ++)
            {
                if(i == StartIndex)
                {
                    newSkills[i] = new UndirectedSkill(UndirectedSkills[i], SkillState.Active);
                }
                else
                {
                    newSkills[i] = new UndirectedSkill(UndirectedSkills[i], SkillState.Inactive);
                }
            }

            UndirectedSkills = newSkills;
        }
        public bool UndirectedIsForgetBlocked(int index)
        {
            if (index == StartIndex)
                return true;
            
            if (UndirectedSkills[index].State != SkillState.Active)
                return true;

            int nextCount = 0;

            for (int i = 0; i < _activeMatrix.GetUpperBound(0) - 1; i++)
            {
                if (_activeMatrix[index, i])
                {
                    nextCount++;
                }
            }

            if (nextCount == 1)
                return false;
            
            var next = GetAllNextIndices(index);
            bool [,] newActiveMatrix = (bool [,])_activeMatrix.Clone();
            UpdateActiveMatrix(ref newActiveMatrix, index, SkillState.Available);

            bool canReach = true;
            foreach (var nextIndex in next)
            {
                canReach = CanReach(newActiveMatrix, nextIndex, StartIndex);
                if (!canReach)
                    break;
            }

            return !canReach;
        }

        private List<int> GetAllNextIndices(int index)
        {
            var row = Enumerable.Range(0, AdjacencyMatrix.GetLength(1))
                .Select(x => AdjacencyMatrix[index, x])
                .ToArray();
            List<int> links = new List<int>();
            for (int i = 0; i < row.Length; i++)
            {
                if (row[i])
                {
                    links.Add(i);
                }
            }

            return links;
        }
        private bool CanReach(bool [,] matrix, int startIndex, int endIndex)
        {
            if (startIndex < 0 || startIndex >= matrix.GetLength(0) ||
                endIndex < 0 || endIndex >= matrix.GetLength(0))
            {
                return false;
            }

            Queue<int> queue = new Queue<int>();
            bool[] visited = new bool[matrix.GetLength(0)];

            queue.Enqueue(startIndex);
            visited[startIndex] = true;

            while (queue.Count > 0)
            {
                int currentIndex = queue.Dequeue();
                if (currentIndex == endIndex)
                {
                    return true;
                }
                for (int i = 0; i < matrix.GetLength(0); i++)
                {
                    if (matrix[currentIndex, i] && !visited[i])
                    {
                        queue.Enqueue(i);
                        visited[i] = true;
                    }
                }
            }
            return false;
        }

        #endregion

        public GameModel()
        {
            _subject = new BehaviorSubject<GameModel>(this);
        }

        public IObservable<GameModel> Observe()
        {
            return _subject.AsObservable();
        }

        public void Update()
        {
            _subject.OnNext(this);
        }
    }
}
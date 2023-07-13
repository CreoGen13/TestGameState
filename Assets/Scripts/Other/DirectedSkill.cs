using System;
using System.Collections.Generic;
using Mono;
using UnityEngine;
using UnityEngine.UI;
using static Game.GamePresenter;

namespace Other
{
    public struct DirectedSkill
    {
        public string Name { get; }
        public Image Image { get; }
        public SkillState State { get; set; }

        public int[] NextSkillsIndex;
        public readonly List<int> PrevSkillsIndex;
        
        public DirectedSkill(SkillNode skillNode, SkillState state)
        {
            State = state;
            Name = skillNode.GetName();
            Image = skillNode.GetImage();
            NextSkillsIndex = Array.Empty<int>();
            PrevSkillsIndex = new List<int>();
        }
        public DirectedSkill(DirectedSkill other, SkillState state)
        {
            State = state;
            Name = other.Name;
            Image = other.Image;
            NextSkillsIndex = other.NextSkillsIndex;
            PrevSkillsIndex = other.PrevSkillsIndex;
        }
        public void SetColor(Color color)
        {
            Image.color = color;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            DirectedSkill other;
            try
            {
                other = (DirectedSkill)obj;
            }
            catch (Exception)
            {
                return false;
            }

            //Debug.Log(State + "     " + other.State);
            
            return Name == other.Name && Equals(Image, other.Image) && State == other.State;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Image, (int)State);
        }
    }
}
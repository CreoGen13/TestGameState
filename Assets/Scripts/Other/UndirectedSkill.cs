using System;
using System.Collections.Generic;
using Mono;
using UnityEngine;
using UnityEngine.UI;
using static Game.GamePresenter;

namespace Other
{
    public struct UndirectedSkill
    {
        public string Name { get; }
        public Image Image { get; }
        public SkillState State { get; set; }
        
        
        public UndirectedSkill(SkillNode skillNode, SkillState state)
        {
            State = state;
            Name = skillNode.GetName();
            Image = skillNode.GetImage();
        }
        public UndirectedSkill(UndirectedSkill other, SkillState state)
        {
            State = state;
            Name = other.Name;
            Image = other.Image;
        }
        public void SetColor(Color color)
        {
            Image.color = color;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            UndirectedSkill other;
            try
            {
                other = (UndirectedSkill)obj;
            }
            catch (Exception)
            {
                return false;
            }

            return Name == other.Name && Equals(Image, other.Image) && State == other.State;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Image, (int)State);
        }
    }
}
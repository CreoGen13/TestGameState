using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mono
{
    public class SkillNode : MonoBehaviour
    {
        [SerializeField] private Button skillButton;
        [SerializeField] private Image skillImage;
        [SerializeField] private Image chooseImage;
        [SerializeField] private TextMeshProUGUI skillText;
        [SerializeField] private TextMeshProUGUI pointsText;

        [SerializeField] private SkillNode[] nextSkills;
        [SerializeField] private bool isStartNode;
        [SerializeField] private int points;
        public bool IsStartNode => isStartNode;
        public Button SkillButton => skillButton;
        public SkillNode [] NextSkills => nextSkills;
        public int  Points => points;

        private void Start()
        {
            pointsText.text = points.ToString();
        }

        public string GetName()
        {
            return skillText.text;
        }
        public Image GetImage()
        {
            return skillImage;
        }
        public void SetChooseImage(bool isChosen)
        {
            chooseImage.enabled = isChosen;
        }
    }
}
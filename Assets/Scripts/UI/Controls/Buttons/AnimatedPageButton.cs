using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Controls.Buttons
{
    /// <summary>
    /// Кнопка управления состоянием страницы с проигрыванием анимации
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class AnimatedPageButton: PageButton
    {
        [Header("Настройки анимации")]
        [SerializeField] private Animation pageAnimation;
        [SerializeField] private string animationName;

        protected override void PageAction()
        {
            if (action == PageActionType.Open)
            {
                page.Open();
                StartCoroutine(PlayAnimation(null));
            }
            else
            {
                StartCoroutine(PlayAnimation(page.Close));
            }
        }

        private IEnumerator PlayAnimation([CanBeNull] Action callback)
        {
            pageAnimation.Play(animationName);
            var timeAwait = pageAnimation[animationName].clip.length;
            
            yield return new WaitForSeconds(timeAwait);
            
            callback?.Invoke();
        }
    }
}
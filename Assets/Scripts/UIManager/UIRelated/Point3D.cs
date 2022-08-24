using System;
using System.Collections;
using DG.Tweening;

using TMPro;
using UnityEngine;

    public class Point3D : MonoBehaviour
    {
        public static Point3D lastSpawned;

        public float duration;
        public float delayForFadeOut;
        public Color goodColor;
        public Color badColor;

        private Coroutine _animationCoroutine;
        private int value;
        private float _punchScale;
        private float spawnTime;
        private TextMeshPro _text;

        private void Awake()
        {
            _text = GetComponent<TextMeshPro>();
        }

        

        public void SetTextValues(int amount)
        {
            value = amount;
            _text.color = amount > 0 ? goodColor : badColor;
            _text.text = (amount > 0 ? "+" : "") + value;
            _animationCoroutine = StartCoroutine(TextAnimation(value > 0));
            lastSpawned = this;
        }


        private IEnumerator TextAnimation(bool direction)
        {
            yield return _text.rectTransform.DOPunchScale(_punchScale * Vector3.one, 0.25f).SetRelative(true)
                .WaitForCompletion();
            _text.DOFade(0f, duration - delayForFadeOut).SetDelay(delayForFadeOut)
                .OnComplete(() => Destroy(gameObject));
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
            transform.DOKill();
        }
    }

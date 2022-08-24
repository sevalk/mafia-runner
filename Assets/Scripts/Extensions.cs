using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck;
using UnityEngine;
using Random = UnityEngine.Random;

//this.InvokeAfterSeconds(.5f, (() => { _isTrigger = true; }));


    public static class Extensions
    {
        public static void InvokeAfterSeconds(this MonoBehaviour mono, float seconds, Action target)
        {
            mono.StartCoroutine(InvokeCoroutine(seconds, target));
        }

        private static IEnumerator InvokeCoroutine(float seconds, Action target)
        {
            yield return new WaitForSeconds(seconds);
            target.SafeInvoke();
        }

        public static T RandomElement<T>(this IList<T> list)
        {
            return (T) (list.Count > 0 ? list[Random.Range(0, list.Count)] : default(T));
        }
    }

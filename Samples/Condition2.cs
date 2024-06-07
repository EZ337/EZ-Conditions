using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


namespace EZConditions
{
    public class Condition2 : MonoBehaviour
    {
        [SerializeField]
        private bool inDialogue;

        [SerializeField]
        private int timesDefeated;

        [SerializeField]
        private float inventoryWeight;

        [SerializeField]
        private float maxWeight = 100;

        [Condition(typeof(float))]
        public float GetInventoryRatio()
        {
            return (float)Math.Round(inventoryWeight / maxWeight, 3);
        }

        /// <summary>
        /// Private Conditions show in the condition manager (This was a design choice. Input appreciated)
        /// </summary>
        /// <returns></returns>
        [Condition(typeof(float))]
        private float GetTimesDefeated()
        {
            return timesDefeated;
        }

        [Condition(typeof(bool))]
        public bool InDialogue()
        {
            return inDialogue;
        }
    }

}

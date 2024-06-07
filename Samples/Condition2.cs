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

        public Condition2EnumTest enumTest;

        public Condition2EnumTest2 enumTest2;


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

        [Condition(typeof(Condition2EnumTest))]
        public Condition2EnumTest GetEnum1()
        {
            return enumTest;
        }

        [Condition(typeof(Condition2EnumTest2))]
        public Condition2EnumTest2 GetEnum2()
        {
            return enumTest2;
        }


        public enum Condition2EnumTest
        {
            Choice1,
            Choice2,
            Choice3,
            Choice4,
            Choice5
        }

        public enum Condition2EnumTest2
        {
            ChoiceA,
            ChoiceB,
            ChoiceC,
            ChoiceD,
            ChoiceE,
        }
    }

}

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

        public ConditionManagerComponent conditionManager;

        private void Update()
        {
            if (inDialogue && Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Running conditions for dialogue.");

                if (conditionManager.EvaluateConditions())
                {
                    Debug.Log("Conditions ran successfully");
                    Debug.Log($"Hello. Name's {name}. It looks like the conditions passed to allow me to say this line");
                }
                else
                {
                    Debug.Log("Conditions failed");
                    Debug.Log($"Hello, there seems to be a failure. I don't know what to say");
                }

            }
        }

        private void OnTriggerExit(Collider other)
        {
            Debug.Log("Player left dialogue");
            inDialogue = false;
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("Player entered. Press Space to start dialogue dialogue");
                inDialogue = true;
            }
        }


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

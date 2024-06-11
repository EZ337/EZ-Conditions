using EZConditions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EZConditions
{
    public class EZConditionsSample1 : MonoBehaviour
    {
        [field : SerializeField]
        [Condition(typeof(string))]
        public string Name {get; set;}

        public bool isDead;

        int level = 10;

        [Condition(typeof(int))]
        public int Level {  get => level; private set => level = value; }

        public float DeathXP = 50.0f;

        public Collider Collider;


        [Condition]
        public void VoidFunction()
        {
            Debug.Log($"{Name} called VoidFunction");
        }

        /// <summary>
        /// Function will not show up because it is private
        /// </summary>
        /// <returns></returns>
        [Condition(typeof(int))]
        private int HiddenConditionFunction()
        {
            return 0;
        }

        [Condition(typeof(Collider))]
        public bool HasCollider(Collider collider)
        {
            return this.Collider == collider;
        }

        [Condition(typeof(float))]
        public float GetDeathXP()
        {
            return DeathXP;
        }

    }

}

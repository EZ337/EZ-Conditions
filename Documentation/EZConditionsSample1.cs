using UnityEngine;

namespace EZConditions
{
    public class EZConditionsSample1 : MonoBehaviour
    {
        [field: SerializeField]
        [Condition]
        public string Name {get; set;}

        public bool isDead;

        int level = 10;

        [Condition]
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
        [Condition]
        private int HiddenConditionFunction()
        {
            return 0;
        }

        [Condition]
        public bool HasCollider(Collider collider)
        {
            return this.Collider == collider;
        }

        [Condition]
        public float GetDeathXP()
        {
            return DeathXP;
        }

    }

}

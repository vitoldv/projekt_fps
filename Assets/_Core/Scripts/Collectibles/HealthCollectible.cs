using _Core.Common;
using _Core.Player;
using UnityEngine;

namespace _Core.Collectibles
{
    public class HealthCollectible : CollectibleBase
    {
        public enum Size
        {
            BIG,
            MID,
            SMALL
        }

        [SerializeField] private float smallPackScale;
        [SerializeField] private float bigPackScale;
        [SerializeField] private float healAmount;

        public void Init(Size size)
        {
            healAmount = GameManager.CollectiblesConfig.GetHealthPackSize(size);
            switch (size)
            {
                case Size.BIG: 
                    transform.localScale = new Vector3(bigPackScale, bigPackScale, bigPackScale); break;
                case Size.MID:
                    transform.localScale = Vector3.one; break;
                case Size.SMALL:
                    transform.localScale = new Vector3(smallPackScale, smallPackScale, smallPackScale); break;
                default:
                    break;
            }
        }

        protected override void ApplyEffect(PlayerController player)
        {
            player.Heal(healAmount);
        }
    }
}
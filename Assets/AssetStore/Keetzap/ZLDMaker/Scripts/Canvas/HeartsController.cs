using System.Collections.Generic;
using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    public class HeartsController : MonoBehaviour
    {
        [SerializeField] private GameObject heartPrefab;
        [SerializeField] private List<HeartView> hearts;

        public void AddHeart()
        {
            GameObject heart = Instantiate(heartPrefab, transform);
            hearts.Add(heart.GetComponent<HeartView>());
        }

        public void RecoverAllLifes()
        {
            for (int h = 0; h < hearts.Count; h++)
            {
                hearts[h].SetLife(true);
            }
        }

        public void SetLifes(int index, bool state)
        {
            hearts[index].SetLife(state);
        }
    }
}

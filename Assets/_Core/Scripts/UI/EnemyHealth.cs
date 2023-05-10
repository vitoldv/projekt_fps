using _Core.Common;
using TMPro;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public TextMeshProUGUI text;

    public void SetHealth(float health)
    {
        text.text = health.ToString();
    }

    private void Update()
    {
        if(GameManager.PlayerController != null)
        {
            transform.LookAt(GameManager.PlayerController.transform.position);
        }
    }
}

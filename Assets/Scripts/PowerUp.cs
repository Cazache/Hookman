
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public int id;
    public ParticleSystem particle;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 12 || other.gameObject.layer == 13)
        {
            if (id == 0)
            {
                GameManager.habilities.StartShield();
            }
            else if (id == 1)
            {
                if (GameManager.habilities.CurrentDashes < 2)
                    GameManager.habilities.CurrentDashes++;

                GameManager.manager.UpdateDashText();

            }
            else if (id == 2)
            {
                if (!GameManager.habilities.cdReduction)
                {
                    GameManager.habilities.cdReduction = true;
                    GameManager.weapons.cdShoot = GameManager.weapons.cdShoot / 2f;
                }

            }
            GameManager.manager.PlaySoundWithPrefab(GameManager.manager.powerUpSound, transform, 1f);
            Instantiate(particle, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

    }
}

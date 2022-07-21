using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadTrigger : MonoBehaviour
{
    public GameObject playerPos;
    public Level level;

    float loadDelay = 0.5f;
    public bool isOnCeiling = false;
    public bool hasDefaultDirection = false;
    public bool defaultDirectionRight = false;
    public bool defaultDirectionLeft = false;

    public float exitVelocityY = 30f;
    public float exitVelocityX = 15f;

    Collider2D loadCollider;
    Vector2 playerVelocity;

    private void Start()
    {
        loadCollider = GetComponent<Collider2D>();
        StartCoroutine(LoadDelay());
    }

    IEnumerator LoadDelay()
    {
        loadCollider.enabled = false;
        while(GameManager.instance.isLoading == true)
        {
            yield return null;
        }
        yield return new WaitForSeconds(loadDelay);
        loadCollider.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            if (level != null)
            {
                level.isRevealed = true;
                GameManager.instance.currentLevel = level;
            }
            PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();

            playerVelocity = playerMovement.Velocity;
            float directionalInputX = collision.GetComponent<Player_Input>().directionalInput.x;

            if (isOnCeiling)
            {
                playerMovement.isLoadingVertical = true;
                playerMovement.isLoadingHorizontal = false;

                playerVelocity.y = exitVelocityY;
                playerMovement.exitVelocityY = exitVelocityY;

                if (!hasDefaultDirection) {
                    if (directionalInputX < 0)
                    {
                        playerMovement.exitVelocityX = -exitVelocityX;
                    }
                    else
                    {
                        playerMovement.exitVelocityX = exitVelocityX;
                    }
                }
                else if (defaultDirectionRight)
                {
                    playerMovement.exitVelocityX = exitVelocityX;
                }
                else if (defaultDirectionLeft)
                {
                    playerMovement.exitVelocityX = -exitVelocityX;
                }
            }
            else
            {
                playerMovement.isLoadingHorizontal = true;
                playerMovement.isLoadingVertical = false;

                if (Mathf.Sign(playerVelocity.x) < 0)
                {
                    playerMovement.exitVelocityX = -exitVelocityX;
                    playerVelocity.x = -exitVelocityX;
                }
                else if (Mathf.Sign(playerVelocity.x) > 0)
                {
                    playerMovement.exitVelocityX = exitVelocityX;
                    playerVelocity.x = exitVelocityX;
                }
            }

            GameManager.instance.LoadScenePath(SceneManager.GetActiveScene().path, level.scenePath, playerPos.transform.position);
            playerMovement.Velocity = playerVelocity;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(playerPos.transform.position, 1f);
        BoxCollider2D boxCollider2D = GetComponent<BoxCollider2D>();
        Vector3 boxPos = boxCollider2D.transform.position;
        Vector2 vector = new Vector2(boxPos.x + boxCollider2D.offset.x, boxPos.y + boxCollider2D.offset.y);
        Gizmos.DrawWireCube(vector, GetComponent<BoxCollider2D>().size);
    }
}



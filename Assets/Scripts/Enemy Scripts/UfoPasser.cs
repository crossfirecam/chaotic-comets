using UnityEngine;

/*
 * This class is used for typical UFOs, that chase the player in a straight line and shoot in a straight line
 */

public class UfoPasser : Ufo
{
    private float beginningDirection;
    public float currentDeviation;
    private float passerDirChangeInterval = 1f;
    private float lastDirChangeTime = 0f;

    private new void Start()
    {
        base.Start();

        // If spawned on the left, set beginningDirection to 1. Else, set to -1.
        float whereStarted = transform.position.x;
        beginningDirection = whereStarted < 0f ? 1 : -1;
    }
    private new void Update()
    {
        base.Update();

        CheckUfoScreenWrap(true);
    }

    internal override void ChangeDifficultyStats()
    {
        base.ChangeDifficultyStats();

        // Passer will change directions more often at higher difficulty.
        if (BetweenScenes.Difficulty == 1)
        {
            passerDirChangeInterval *= .8f;
        }
        else if (BetweenScenes.Difficulty >= 2)
        {
            passerDirChangeInterval *= .6f;
        }
    }

    public void SetBeginningDirection(int passedValue)
    {
        if (passedValue == 0)
        {
            beginningDirection = 1f;
        }
        else if (passedValue == 1)
        {
            beginningDirection = -1f;
        }
    }

    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        if (!ufoTeleporting && !deathStarted)
        {
            if (!playerFound)
            {
                FindPlayer();
            }

            // If UFO has more than 10 health, continue moving
            if (alienHealth > 10f)
            {
                MoveLogicPasser();
            }
            // If alien has less than 10 health, it will run away in a single direction and attempt to teleport
            else
            {
                AlienRetreat();
            }
            direction = direction.normalized;
            rb.MovePosition(rb.position + direction * alienSpeedCurrent * Time.fixedDeltaTime);
        }
    }

    // UFO-Passer will move left to right, deviating with up/down/straight on Y axis
    private void MoveLogicPasser()
    {
        if (timer >= lastDirChangeTime + passerDirChangeInterval)
        {
            lastDirChangeTime = timer;
            int whileTimer = 0;
            while(whileTimer < 20)
            {
                whileTimer += 1;
                float rand = Random.Range(0f, 3f);
                if (rand < 1f && PasserMovementIsValid(PasserMove.Up))
                {
                    currentDeviation = Random.Range(0.6f, 1f);
                    break;
                }
                else if (rand > 1f && rand < 2f && PasserMovementIsValid(PasserMove.Straight))
                {
                    currentDeviation = 0f;
                    break;
                }
                else if (rand > 2f && rand < 3f && PasserMovementIsValid(PasserMove.Down))
                {
                    currentDeviation = Random.Range(-1f, -0.6f);
                    break;
                }
            }
            if (whileTimer >= 20)
            {
                print("UFO Passer exceeded 20 while loops");
                currentDeviation = 0f;
            }
        }
        direction = new Vector2(beginningDirection, currentDeviation);
    }
    //
    private enum PasserMove { Up, Down, Straight };
    private bool PasserMovementIsValid(PasserMove attemptedMoveType)
    {
        switch (attemptedMoveType)
        {
            case PasserMove.Up:
                if (transform.position.y < GameManager.i.screenTop - 3f)
                {
                    return true;
                }
                break;
            case PasserMove.Straight:
                if (transform.position.y < GameManager.i.screenTop - 0.5f && transform.position.y > GameManager.i.screenBottom + 0.5f)
                {
                    return true;
                }
                break;
            case PasserMove.Down:
                if (transform.position.y > GameManager.i.screenBottom + 3f)
                {
                    return true;
                }
                break;
        }
        return false;
    }
}

using System.Collections;
using UnityEngine;

public enum StartingPlacement {
    TOP,
    BOTTOM,
    LEFT,
    RIGHT
}

[RequireComponent(typeof(Renderer))]
public class TestScript : MonoBehaviour {
    [SerializeField] private float startDelay = 1;
    [SerializeField] private Camera mainCam = null; // main camera
    [Range(0.0f, 1.0f)] [SerializeField] private float goalPosX; // the % of the viewport of our X
    [Range(0.0f, 1.0f)] [SerializeField] private float goalPosY; // the % of the viewport of our Y

    [SerializeField]
    private float timeToReachGoal = 2.0f; // the time it takes us to reach goal pos from our starting position

    [SerializeField]
    private StartingPlacement placement = StartingPlacement.LEFT; // which direction we are starting from

    [Range(0.0f, 1.0f)] [SerializeField]
    private float
        offset = 0.0f; // the other axis our start position will be % of the view port (ex: top is 1.0f Y, but is this value X)

    [SerializeField] private Renderer rend = null; // renderer of this object

    private float zPosStart = 0f;

    private void Start() {
        if (rend == null)
            rend = GetComponent<Renderer>();

        if (mainCam == null)
            mainCam = Camera.main;

        // save our z position
        zPosStart = transform.position.z;

        // set our starting position based on the bounds of our camera and the bounds of our renderer
        SetInitialPosition();

        // now start our movement to the goal position
        StartCoroutine(MoveToGoalPostiion());
    }

    /// <summary>
    /// Set the initial position of our object based on the viewport position and the size of the renderer
    /// </summary>
    private void SetInitialPosition() {
        // with viewport coordinates, (0,0) is the bottom left and (1,1) is the top right
        // I am also assuming the starting position is half way up or across the screen - change teh 0.5 to alter it

        // we also need to offset our current position by +/- the X or Y of half the bounds of our renderer
        switch (placement) {
            case StartingPlacement.TOP:
                transform.position =
                    mainCam.ViewportToWorldPoint(new Vector3(offset, 1.0f, mainCam.transform.position.z));
                transform.position += new Vector3(0f, rend.bounds.extents.y, 0f);
                break;
            case StartingPlacement.BOTTOM:
                transform.position =
                    mainCam.ViewportToWorldPoint(new Vector3(offset, 0.0f, mainCam.transform.position.z));
                transform.position += new Vector3(0f, -rend.bounds.extents.y, 0f);
                break;
            case StartingPlacement.LEFT:
                transform.position =
                    mainCam.ViewportToWorldPoint(new Vector3(0.0f, offset, mainCam.transform.position.z));
                transform.position += new Vector3(-rend.bounds.extents.x, 0f, 0f);
                break;
            case StartingPlacement.RIGHT:
                transform.position =
                    mainCam.ViewportToWorldPoint(new Vector3(1.0f, offset, mainCam.transform.position.z));
                transform.position += new Vector3(rend.bounds.extents.x, 0f, 0f);
                break;
        }

        // set our start back to our original Z
        transform.position = new Vector3(transform.position.x, transform.position.y, zPosStart);
    }

    /// <summary>
    /// Move from our current position to our goal position
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveToGoalPostiion() {
        yield return new WaitForSeconds(startDelay);
        Vector2 startPos = transform.position;

        float currentTime = 0.0f;

        // create our goal position
        Vector3 goalPos = mainCam.ViewportToWorldPoint(new Vector3(goalPosX, goalPosY, mainCam.transform.position.z));

        goalPos.z = zPosStart;

        while (currentTime <= timeToReachGoal) {
            transform.position = Vector2.Lerp(startPos, goalPos, currentTime / timeToReachGoal);
            currentTime += Time.deltaTime;
            yield return null;
        }

        // set our position in case of floating point precision issues
        transform.position = goalPos;
    }
}
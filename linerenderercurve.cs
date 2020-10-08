using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class linerenderercurve : MonoBehaviour
{
    LineRenderer selfLine; //linerenderer component
    public int linePosCount; //number of points on line, set in inspector
    public float ampLitude; //height of curve
    public float peRiod; //width of curve
    Vector3[] pointPos; //positions of the points
    Vector3 unitVector; //used for the quadratic function
    public GameObject startPosObj; //dummy object for line start position
    public GameObject endPosObj; //dummy object for line end position
    public GameObject endObject; //object to visualise and "mark" end position
    bool hitSomething;

    void Start()
    {
        selfLine = transform.GetComponent<LineRenderer>();
        selfLine.positionCount = linePosCount;
        pointPos = new Vector3[linePosCount];
        pointPos[0] = startPosObj.transform.position;
        pointPos[linePosCount - 1] = endPosObj.transform.position;
        unitVector = (endPosObj.transform.position - startPosObj.transform.position) / (linePosCount-1); //getting the number of 'intervals' between the line
        for(int i = 1;i<linePosCount - 1; i++) //random positioning of the points
        {
            pointPos[i] = startPosObj.transform.position + unitVector * i;
            pointPos[i].y = i * i;
        }
        selfLine.SetPositions(pointPos);
        
    }

    private void FixedUpdate()
    {
        pointPos[0] = startPosObj.transform.position;
        pointPos[linePosCount - 1] = endPosObj.transform.position;
        endObject.transform.position = endPosObj.transform.position;
        unitVector = (endPosObj.transform.position - startPosObj.transform.position) / (linePosCount - 1);
        for (int i = 1; i < linePosCount - 1; i++)
        {
            pointPos[i] = startPosObj.transform.position + unitVector * i; //moving to next point according to the calculated unit vector interval
            pointPos[i].y = -0.1f * ampLitude * (i * i - peRiod * i) + startPosObj.transform.position.y; //quadratic function: f(x) = -ax(x + b) + c, with a maximum point and offset
        }

        for (int i = 0;i<linePosCount -1;i++)
        {

            RaycastHit hitInfo;
            if (Physics.Linecast(pointPos[i], pointPos[i + 1], out hitInfo))
            {
                if (hitInfo.transform.tag == "Untagged" || hitInfo.transform.tag == "ground" || hitInfo.transform.tag == "enemy") //tag of objects that can be hit by linecast
                {
                    //Debug.Log("hit");
                    for (int j = i + 1; j < linePosCount; j++)
                        pointPos[j] = hitInfo.point; //if current points hits, all upcoming points are collapsed to this point

                    endObject.transform.position = hitInfo.point; //move marker to this point

                    break; //stop looping if hit already
                }                 
            }
        }

        selfLine.SetPositions(pointPos); //set all calculated positions


    }

}

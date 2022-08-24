using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SwipeMovement : MonoBehaviour
{ 
    [Header("Speed Settings")]
    [SerializeField] [Range(0.1f, 4.9f)] private float swipeSpeed;

    [Header("Swipe Settings")] 
    [SerializeField] private float minRoadX;
    [SerializeField] private float maxRoadX;
    [Space(5)]
    
    public bool canMove;
    
    private float screenDis;
    private float disDif;
    private float firstMousePosX;



    // Start is called before the first frame update
    void Start()
    {
      
        screenDis = (Screen.width * (10 - swipeSpeed) / 10f) - (Screen.width * swipeSpeed / 10f);
        disDif = screenDis / Mathf.Abs(maxRoadX - minRoadX); // screen / roadX distance

    }

    // Update is called once per frame
    void Update()
    {
       
        if (canMove)
        {
            if (Input.GetMouseButtonDown(0))
            {

                firstMousePosX = Input.mousePosition.x;
                
            }
            else if (Input.GetMouseButton(0))
            {
                
                if (firstMousePosX > Input.mousePosition.x) // Left
                {
                    float minDis = transform.localPosition.x - (firstMousePosX - Input.mousePosition.x) / disDif;
                   
                    
                    
                    if (minDis > minRoadX)
                    {
                        transform.localPosition = new Vector3(minDis, transform.localPosition.y, transform.localPosition.z);
                        firstMousePosX = Input.mousePosition.x;
                    }
                    else
                    {
                        transform.localPosition = new Vector3(minRoadX, transform.localPosition.y, transform.localPosition.z);
                    }
                }
                else if (firstMousePosX < Input.mousePosition.x) // Right
                {
                    float maxDis = transform.localPosition.x + (Input.mousePosition.x - firstMousePosX) / disDif;
                  
                    
                    if (maxDis < maxRoadX)
                    {
                        transform.localPosition = new Vector3(maxDis, transform.localPosition.y, transform.localPosition.z);
                        firstMousePosX = Input.mousePosition.x;
                    }
                    else
                    {
                        transform.localPosition = new Vector3(maxRoadX, transform.localPosition.y, transform.localPosition.z);
                    }
                }
            }
         
        }
    
    }
}
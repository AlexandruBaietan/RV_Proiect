using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    // Start is called before the first frame update

    //these are the arrays containing the text of the frames    
    public Text[] roll_1;
    public Text[] roll_2;
    public Text[] score;

    //this is to know the number of frame and the roll
    public int frameNumb=1;
    public int roll=1;

    //the last roll has to be taken into account separately
    public Text lastRoll;

    //this is the refeence to the main script
    public GoogleARCore.Examples.HelloAR.ARBowling bowlScript;

    //list of points
    public int[] listOfpoints;
    public int[] cumulatedPoints;

    //the pins
    public Pins[] pins;

    //the total rolls of the game
    public int totalRolls = 0;

    void OnEnable()
    {

        /*originalPos = interactionZone.transform.position;

        interactionZone.position = new Vector3(Camera.main.transform.position.x, originalPos.y, Camera.main.transform.position.z);*/

        /* THIS WORKS ONLY IN EDITOR MODE
        //initialize variables roll1
        GameObject[] go = GameObject.FindGameObjectsWithTag("roll1");
        roll_1 = new Text[go.Length];

        for (int ii=0;ii<go.Length;ii++)
        {
            roll_1[ii] = go[ii].GetComponent<Text>();
        }


        //initialize variables roll2
        go = GameObject.FindGameObjectsWithTag("roll2");
        roll_2 = new Text[go.Length];

        for (int ii = 0; ii < go.Length; ii++)
        {
            roll_2[ii] = go[ii].GetComponent<Text>();
        }


        //initialize variables roll2
        go = GameObject.FindGameObjectsWithTag("score");
        score = new Text[go.Length];

        for (int ii = 0; ii < go.Length; ii++)
        {
            score[ii] = go[ii].GetComponent<Text>();
        }

        
        //initialize variables roll2
        GameObject go2 = GameObject.FindGameObjectWithTag("roll3");
        lastRoll = go2.GetComponent<Text>();

        //set pins in the main script
        go = GameObject.FindGameObjectsWithTag("pins");
        pins = new Pins[go.Length];
        for (int ii = 0; ii < go.Length; ii++)
        {
            pins[ii] = go[ii].GetComponent<Pins>();
        }
        */

        //initialization
        listOfpoints = new int[25];

        //intialization
        cumulatedPoints = new int[11];

        GameObject go2 = GameObject.FindGameObjectWithTag("ARcore");
        bowlScript = go2.GetComponent<GoogleARCore.Examples.HelloAR.ARBowling>();

        bowlScript.pins = pins;

        bowlScript.scoreManag = GetComponent<Score>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
                
    }

    public void updateFrame()
    {
        int points = 0;
        //check pins

        for (int ii = 0; ii < pins.Length; ii++)
        {
            if (pins[ii].GetComponent<Pins>().fallen)
            {
                points +=1 ;
            }
        }

        listOfpoints[totalRolls] = points;


        if (frameNumb < 10)
        {
            //strike has happen
            if (points==10 && roll==1)
            {

                //update roll status
                roll_1[frameNumb - 1].text = "X";

                roll += 2;
                totalRolls += 2;


                //reset positions
                for (int ii = 0; ii < pins.Length; ii++)
                {
                    pins[ii].resetPos();
                }

            }
            else if (roll == 1)
            {
                roll_1[frameNumb - 1].text = "" + points;              

                //update roll status
                roll += 1;
                totalRolls += 1;
            }
            else if (roll == 2)
            {

                //spare has happen
                if (points==10)
                {
                    roll_2[frameNumb - 1].text = "/";
                }
                else
                {
                    roll_2[frameNumb - 1].text = "" + (points-listOfpoints[totalRolls-1]);
                                 
                }


     
                //reset positions
                for (int ii = 0; ii < pins.Length; ii++)
                {
                    pins[ii].resetPos();
                }


                //update roll status
                roll += 1;
                totalRolls += 1;


            }

           
            
            // final
            // scoring
            if (frameNumb == 1)
            {
                cumulatedPoints[frameNumb - 1] = points;

                if (roll == 3 && points!=10)
                {
                    score[frameNumb - 1].text = "" + cumulatedPoints[frameNumb - 1];
                }

            }
            else
            {
                ///////////////////
                //     strike
                ///////////////////
                if (roll == 3)
                {
                    if (listOfpoints[totalRolls - 4] == 10 && frameNumb >= 2)
                    {

                        if (frameNumb >= 3)
                        {
                            cumulatedPoints[frameNumb - 2] = 10+points + cumulatedPoints[frameNumb - 3];
                        }
                        else
                        {
                            cumulatedPoints[frameNumb - 2] += points;
                        }
                        cumulatedPoints[frameNumb - 1] = cumulatedPoints[frameNumb - 2] + points;

                        score[frameNumb - 2].text = "" + cumulatedPoints[frameNumb - 2];

                        
                        score[frameNumb - 1].text = "" + cumulatedPoints[frameNumb - 1];
                        

                        Debug.Log("strike computation");
                    }
                    else if(points!=10)
                    {
                        cumulatedPoints[frameNumb - 1] = cumulatedPoints[frameNumb - 2] + points;

                        score[frameNumb - 1].text = "" + cumulatedPoints[frameNumb - 1];

                        Debug.Log("Normal computation");
                    }
                }
                ///////////////////
                //     spare
                ///////////////////
                else if (listOfpoints[totalRolls - 2] == 10 && frameNumb >= 2 && roll == 2)
                {
                    if (frameNumb >= 3)
                    {
                        cumulatedPoints[frameNumb - 2] = 10 + listOfpoints[totalRolls -1] + cumulatedPoints[frameNumb - 3];
                    }
                    else
                    {
                        cumulatedPoints[frameNumb - 2] += points;
                    }


                    score[frameNumb - 2].text = "" + cumulatedPoints[frameNumb - 2];

                    Debug.Log("Spare computation");

                }
           

               
            }
         
          

          
        }

        //last frame
        else if(frameNumb==10)
        {

            //strike has happen
            if (points == 10 && roll == 1)
            {

                //update roll status
                roll_1[frameNumb - 1].text = "X";

                roll += 2;
                totalRolls += 2;


                //reset positions
                for (int ii = 0; ii < pins.Length; ii++)
                {
                    pins[ii].resetPos();
                }

            }
            
            else if (roll == 1)
            {
                roll_1[frameNumb - 1].text = "" + points;

                //update roll status
                roll += 1;
                totalRolls += 1;
            }

            //spare has happen
            else if (roll == 2)
            {
                if (points == 10)
                {
                    roll_2[frameNumb - 1].text = "/";
                }
                else
                {
                    roll_2[frameNumb - 1].text = "" + (points - listOfpoints[totalRolls - 1]);

                }
                
                //reset positions
                for (int ii = 0; ii < pins.Length; ii++)
                {
                    pins[ii].resetPos();
                }


                //update roll status
                roll += 1;
                totalRolls += 1;


            }
            else if(roll==3 && (listOfpoints[totalRolls - 2] == 10 || listOfpoints[totalRolls - 1] == 10))
            {
                lastRoll.text = ""+points;

                //update roll status
                roll += 1;
                totalRolls += 1;
            }



            ///////////////////
            //     strike
            ///////////////////
            if (roll == 3)
            {
                if (listOfpoints[totalRolls - 4] == 10 && frameNumb >= 2)
                {

                   
                    cumulatedPoints[frameNumb - 2] = 10 + points + cumulatedPoints[frameNumb - 3];
                   
                   
                    cumulatedPoints[frameNumb - 1] = cumulatedPoints[frameNumb - 2] + points;

                    score[frameNumb - 2].text = "" + cumulatedPoints[frameNumb - 2];


                    score[frameNumb - 1].text = "" + cumulatedPoints[frameNumb - 1];


                    Debug.Log("strike computation");
                }
                else if (points != 10)
                {
                    cumulatedPoints[frameNumb - 1] = cumulatedPoints[frameNumb - 2] + points;

                    score[frameNumb - 1].text = "" + cumulatedPoints[frameNumb - 1];

                    score[frameNumb].text = "" + cumulatedPoints[frameNumb - 1];

                    Debug.Log("Normal computation");
                }
            }
            ///////////////////
            //     spare
            ///////////////////
            else if (listOfpoints[totalRolls - 2] == 10 && frameNumb >= 2 && roll == 2)
            {
              
                cumulatedPoints[frameNumb - 2] = 10 + listOfpoints[totalRolls - 1] + cumulatedPoints[frameNumb - 3];
             
                score[frameNumb - 2].text = "" + cumulatedPoints[frameNumb - 2];

                Debug.Log("Spare computation");

            }

            else if (listOfpoints[totalRolls - 2] == 10&&roll ==4 || listOfpoints[totalRolls - 3] == 10 && roll == 4)
            {



                cumulatedPoints[frameNumb - 1] = cumulatedPoints[frameNumb - 2]+10 + points;

                score[frameNumb - 1].text = "" + cumulatedPoints[frameNumb - 1];

                score[frameNumb].text = "" + cumulatedPoints[frameNumb - 1];

                Debug.Log("Last computation with strike or spare");

            }

          
        }


        
        if (roll >= 3 && frameNumb<10)
        {
            //update frame status
            frameNumb += 1;

            roll = 1;
        }
        else if(frameNumb==10 && roll>=4)
        {
            //update frame status
            frameNumb += 1;

            roll = 1;
        }
        
        


    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    //Script that applies simple position, rotation, and scale animations to the gameObject it is attatched to
    public class AutoAnimation : MonoBehaviour
    {
        //Position Variables. Intensity means the max range in distance.
        [Range(0.0f, 1.0f)]
        public float HorizontalShakeIntensity;
        [Range(0.0f, 2.0f)]
        public float HorizontalShakeSpeed;
        [Range(0.0f, 1.0f)]
        public float VerticalShakeIntensity;
        [Range(0.0f, 2.0f)]
        public float VerticalShakeSpeed;

        //Rotation Variables.  Intensity means the max range in rotation.
        [Range(0.0f, 10.0f)]
        public float rotationIntensity;
        [Range(0.0f, 100.0f)]
        public float rotationSpeed;

        //rotates in a single direction if set true in inspector before Start() runs
        public bool rotateOnlyClockwise = false;
        public bool rotateOnlyCounterClockwise = false;

        //Scale Variables.  Intensity means the max range in scale on either x or y axis.
        [Range(0.0f, 1.0f)]
        public float scaleIntensityXAxis;
        [Range(0.0f, 5.0f)]
        public float scaleSpeedX;
        [Range(0.0f, 1.0f)]
        public float scaleIntensityYAxis;
        [Range(0.0f, 5.0f)]
        public float scaleSpeedY;

        //animation is stopped by setting animationPlaying to false
        [HideInInspector] public bool animationPlaying = false;

        private Vector3 pivotpoint;
        private Vector3 startposition;
        private Vector3 startrotation;
        private Vector3 startScale;


        //Start is called before the first frame update
        void Start()
        {
            pivotpoint = gameObject.transform.position;
            startposition = gameObject.transform.position;
            startrotation = gameObject.transform.eulerAngles;
            startScale = gameObject.transform.localScale;

            //TEMP: start animation by default
            StartCoroutine("PlayAnimation");
        }


        public IEnumerator PlayAnimation()
        {
            animationPlaying = true;
            if (HorizontalShakeIntensity > 0 && HorizontalShakeSpeed > 0) { StartCoroutine("HorizontalAnim"); }
            if (VerticalShakeIntensity > 0 && VerticalShakeSpeed > 0) { StartCoroutine("VerticalAnim"); }
            if (rotationIntensity > 0 && rotationSpeed > 0) { StartCoroutine("RotateAnim"); }
            if (scaleIntensityXAxis > 0 && scaleSpeedX > 0) { StartCoroutine("ScaleAnimXAxis"); }
            if (scaleIntensityYAxis > 0 && scaleSpeedY > 0) { StartCoroutine("ScaleAnimYAxis"); }

            yield return null;
        }


        //Shakes Object Horizontally
        public IEnumerator HorizontalAnim()
        {
            float xPos = startposition.x;

            while (true)
            {
                //Move Right
                while (xPos <= (startposition.x + HorizontalShakeIntensity))
                {
                    xPos += HorizontalShakeSpeed * Time.deltaTime;
                    transform.position = new Vector3(xPos, transform.position.y, transform.position.z);

                    CheckAnimationStatus();
                    yield return null;
                }

                //Move Left
                while (xPos >= (startposition.x - HorizontalShakeIntensity))
                {
                    xPos -= HorizontalShakeSpeed * Time.deltaTime;
                    transform.position = new Vector3(xPos, transform.position.y, transform.position.z);

                    CheckAnimationStatus();
                    yield return null;
                }
            }
        }


        //Shakes Object Vertically
        public IEnumerator VerticalAnim()
        {
            float yPos = startposition.y;

            while (true)
            {
                //Move Upwards
                while (yPos <= (startposition.y + VerticalShakeIntensity))
                {
                    yPos += VerticalShakeSpeed * Time.deltaTime;
                    transform.position = new Vector3(transform.position.x, yPos, transform.position.z);

                    CheckAnimationStatus();
                    yield return null;
                }

                //Move Downwards
                while (yPos >= (startposition.y - VerticalShakeIntensity))
                {
                    yPos -= VerticalShakeSpeed * Time.deltaTime;
                    transform.position = new Vector3(transform.position.x, yPos, transform.position.z);

                    CheckAnimationStatus();
                    yield return null;
                }
            }
        }


        //wobble the image based on specified intensity and speed, or spin in the specified direction
        public IEnumerator RotateAnim()
        {
            float zRotation = startrotation.z;

            if (rotateOnlyCounterClockwise)
            {
                while (true)
                {
                    transform.RotateAround(pivotpoint, Vector3.forward, rotationSpeed * Time.deltaTime);
                    zRotation += rotationSpeed * Time.deltaTime;

                    CheckAnimationStatus();
                    yield return null;
                }
            }
            else if (rotateOnlyClockwise)
            {
                while (true)
                {
                    transform.RotateAround(pivotpoint, Vector3.forward, (-1 * rotationSpeed * Time.deltaTime));
                    zRotation -= rotationSpeed * Time.deltaTime;

                    CheckAnimationStatus();
                    yield return null;
                }
            }
            else
            {
                while (true)
                {
                    //rotate counterclockwise
                    while (zRotation <= (startrotation.z + rotationIntensity))
                    {
                        transform.RotateAround(pivotpoint, Vector3.forward, rotationSpeed * Time.deltaTime);
                        zRotation += rotationSpeed * Time.deltaTime;

                        CheckAnimationStatus();
                        yield return null;
                    }

                    //rotate clockwise
                    while (zRotation >= (startrotation.z - rotationIntensity))
                    {
                        transform.RotateAround(pivotpoint, Vector3.forward, (-1 * rotationSpeed * Time.deltaTime));
                        zRotation -= rotationSpeed * Time.deltaTime;

                        CheckAnimationStatus();
                        yield return null;
                    }
                }
            }
        }


        //repeatedly grows and shrinks object on x axis
        public IEnumerator ScaleAnimXAxis()
        {
            float xScale = startScale.x;

            while (true)
            {
                //Grow X axis
                while (xScale <= (startScale.x + scaleIntensityXAxis))
                {
                    xScale += scaleSpeedX * Time.deltaTime;

                    transform.localScale = new Vector3(xScale, transform.localScale.y, transform.localScale.z);

                    CheckAnimationStatus();
                    yield return null;
                }

                //Shrink X Axis
                while (xScale >= (startScale.x - scaleIntensityXAxis))
                {
                    xScale -= scaleSpeedX * Time.deltaTime;

                    transform.localScale = new Vector3(xScale, transform.localScale.y, transform.localScale.z);

                    CheckAnimationStatus();
                    yield return null;
                }
            }
        }


        //repeatedly grows and shrinks object on y axis
        public IEnumerator ScaleAnimYAxis()
        {
            float yScale = startScale.y;

            while (true)
            {
                //Grow Y axis
                while (yScale <= (startScale.y + scaleIntensityYAxis))
                {
                    yScale += scaleSpeedY * Time.deltaTime;

                    transform.localScale = new Vector3(transform.localScale.x, yScale, transform.localScale.z);

                    CheckAnimationStatus();
                    yield return null;
                }

                //Shrink Y Axis
                while (yScale >= (startScale.y - scaleIntensityYAxis))
                {
                    yScale -= scaleSpeedY * Time.deltaTime;

                    transform.localScale = new Vector3(transform.localScale.x, yScale, transform.localScale.z);

                    CheckAnimationStatus();
                    yield return null;
                }
            }
        }


        //returns to default position if animation is no longer playing
        private void CheckAnimationStatus()
        {
            if (!animationPlaying)
            {
                //Reset to default transforms
                gameObject.transform.position = startposition;
                gameObject.transform.eulerAngles = startrotation;

                //End all coroutines
                StopCoroutine("HorizontalAnim");
                StopCoroutine("VerticalAnim");
                StopCoroutine("RotateAnim");
                StopCoroutine("ScaleAnimXAxis");
                StopCoroutine("ScaleAnimYAxis");
                StopCoroutine("PlayAnimation");
            }
        }
    }
}
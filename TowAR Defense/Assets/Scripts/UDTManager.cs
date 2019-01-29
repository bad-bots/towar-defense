using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class UDTManager : MonoBehaviour, IUserDefinedTargetEventHandler
{
    // global variables
    UserDefinedTargetBuildingBehaviour udt_targetBuildingBehavior;

    ObjectTracker objectTracker;
    DataSet dataSet; // create a dataset to store our user defined image

    int targetCounter; // counter for naming our target builders uniquely for new targets

    ImageTargetBuilder.FrameQuality udt_FrameQuality; // variable to store current framequality

    public ImageTargetBehaviour targetBehaviour; // public so we can assign in editor

    void Start() 
    {
        udt_targetBuildingBehavior = GetComponent<UserDefinedTargetBuildingBehaviour>(); // get UserDefinedTargetingBehaior from current Game Object
        if(udt_targetBuildingBehavior) 
        { // if behavior is found (!== null)
            udt_targetBuildingBehavior.RegisterEventHandler(this); // register this class as our event handler
        }
    }

    // checks the current image quality of camera if picture were to be taken, returns how viable of a target it is
    public void OnFrameQualityChanged(ImageTargetBuilder.FrameQuality frameQuality) 
    {
        udt_FrameQuality = frameQuality;
    }

    public void OnInitialized() // occurs when class is initialized, specifically our targetbuildingbehavior
    {
        objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>(); // initialize our objectTracker
        if(objectTracker!=null) 
        { // if objectTracker is found (!== null)
            dataSet = objectTracker.CreateDataSet(); // create dataset from our tracker
            objectTracker.ActivateDataSet(dataSet);  // activate the dataset
        }
    }

    public void OnNewTrackableSource(TrackableSource trackableSource)
    {
        targetCounter++;
        objectTracker.DeactivateDataSet(dataSet);
        dataSet.CreateTrackable(trackableSource, targetBehaviour.gameObject ); // create a new trackable image and add it to dataset
        objectTracker.ActivateDataSet(dataSet); // re-activate data set
        udt_targetBuildingBehavior.StartScanning(); // start tracking

    }

    public void BuildTarget()
    {

        if(udt_FrameQuality == ImageTargetBuilder.FrameQuality.FRAME_QUALITY_HIGH) // if current frame quality is high
        {
            udt_targetBuildingBehavior.BuildNewTarget(targetCounter.ToString(), targetBehaviour.GetSize().x); // build new targetBehaviour with the width of our imagetarget
        }
    }
}

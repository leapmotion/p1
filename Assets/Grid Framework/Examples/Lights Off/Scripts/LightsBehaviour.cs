using UnityEngine;
using System.Collections;

/*
	ABOUT THIS SCRIPT
	
This script performs two tasks. For one, if the object it is attached to
gets clicked it will fire off an event (defined in another script) which
will be handled by this script as well. When the event is received the
script compares the object's grid coordinates and the coordinates of the
object that was clicked to decide what to do next.
This example uses delegates and events. While it would have been possible
to do this in another way, delegates and events are the most elegant and
performant solution, since they are native to .NET

This script demonstrates how you can use grid coordinates for game logic
with tiles that seem to work together but don't actually know anything
about each other, giving you great freedom in designing your levels.
*/

public class LightsBehaviour : MonoBehaviour {
	
	//store the materials for the different states
	public Material onMaterial;
	public Material offMaterial;
	
	//the state of the switch (intial set is done in the editor, rest at runtime)
	public bool isOn = false;
	
	//the grid we want to use for our game logic
	public GFGrid connectedGrid;
	
	//cache components for performance
	private Transform cachedTransform;
	private Renderer cachedRenderer;
	
	void Awake(){		
		cachedTransform = transform;
		cachedRenderer = renderer;
		//perform an initial light setting
		SwitchLights();
	}
	
	void OnEnable(){
		//subscribe to the event
		SwitchManager.onHitSwitch += OnHitSwitch;
	}
	
	void OnDisable(){
		//unsubscribe from the event
		SwitchManager.onHitSwitch -= OnHitSwitch;
	}
	
	//this function gets called upon the event "onHitSwitch" (switchPosition is in grid coordinates)
	void OnHitSwitch (Vector3 switchPosition, GFGrid theGrid){
		//don't do anything if this light doesn't belong to the grid we use
		if(theGrid != connectedGrid)
			return;
		
		//check if this light is adjacent to the switch; this is an extenion method that always picks
		//the method that belongs to the specific grid type. The implementation is in another file
		if(theGrid.IsAdjacent(cachedTransform.position, switchPosition)){
			//flip the state of this switch
			isOn = !isOn;
		}
		//change the lights (won't do anything if the state hasn't changed)
		SwitchLights();
	}
	
	public void SwitchLights(){
		if(isOn){
			cachedRenderer.material = onMaterial;
		} else{
			cachedRenderer.material = offMaterial;
		}
	}
	
	void OnMouseUpAsButton(){
		//we don't need an instance here, because this function is static
		SwitchManager.SendSignal(cachedTransform.position, connectedGrid);
	}
}
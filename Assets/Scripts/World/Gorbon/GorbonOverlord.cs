using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GorbonOverlord : MonoBehaviour {

	public List<GorbonAI> gorbons;
	private float neighbourhood = 100.0f;
	private float comfortZone = 10.0f;
	void Start () {
		//gorbons = new List<GorbonAI>();	
	}
	
	void FixedUpdate () {
		List<Vector3> cohesionVectors = Cohesion();
		//Debug.Log("Cohesion vectors" + Functions.print(cohesionVectors));
		List<Vector3> separationVectors = Separation();
		//Debug.Log("Separation vectors" + Functions.print(separationVectors));
		List<Vector3> alignmentVectors = Alignment();
		//Debug.Log("Alignment vectors" + Functions.print(alignmentVectors));

		List<Vector3> flockingVectors = cohesionVectors;
		//Debug.Log("flocking vectors" + Functions.print(flockingVectors));

		for(int i = 0; i < cohesionVectors.Count; i++){
			//Debug.Log(Functions.print(flockingVectors));

			flockingVectors[i] += separationVectors[i];
			//Debug.Log("Added separation" + Functions.print(flockingVectors));

			flockingVectors[i] += alignmentVectors[i];
			//Debug.Log("Added alignment" + Functions.print(flockingVectors));

			//Debug.Log(flockingVectors[i]);
			gorbons[i].GorbonUpdate(flockingVectors[i]);
		}
	}

	private List<Vector3> Cohesion(){
		List<Vector3> cohesionVectors = new List<Vector3>();

		for(int i = 0; i < gorbons.Count; i++){
			Vector3 vector = Vector3.zero;
			int neighbours = 0;

			for(int j = 0; j < gorbons.Count; j++){
				if(i != j){
					Vector3 differenceVector = gorbons[j].transform.position - gorbons[i].transform.position;

					if(Mathf.Abs(differenceVector.magnitude) < neighbourhood){
						neighbours++;

						//vector += gorbons[j].transform.position;
						vector += differenceVector;
					}
				}
			}

			//vector = (vector / neighbours) - gorbons[i].transform.position;
			cohesionVectors.Add(vector.normalized);
		}

		return cohesionVectors;
	}

	private List<Vector3> Separation(){
		List<Vector3> separationVectors = new List<Vector3>();

		for(int i = 0; i < gorbons.Count; i++){
			Vector3 vector = Vector3.zero;

			for(int j = 0; j < gorbons.Count; j++){
				if(i != j){
					Vector3 differenceVector = gorbons[j].transform.position - gorbons[i].transform.position;

					if(Mathf.Abs(differenceVector.magnitude) < comfortZone){

						vector -= differenceVector;
					}
				}
			}

			separationVectors.Add(vector.normalized);
		}

		return separationVectors;
	}

	private List<Vector3> Alignment(){
		List<Vector3> alignmentVectors = new List<Vector3>();

		for(int i = 0; i < gorbons.Count; i++){
			Vector3 vector = Vector3.zero;
			int neighbours = 0;

			for(int j = 0; j < gorbons.Count; j++){
				if(i != j){
					Vector3 differenceVector = gorbons[j].transform.position - gorbons[i].transform.position;

					if(Mathf.Abs(differenceVector.magnitude) < neighbourhood){
						neighbours++;

						vector += gorbons[j].gorbonRB.velocity;
					}
				}
			}

			//if(neighbours > 0) vector = vector / neighbours;
			alignmentVectors.Add(vector.normalized);
		}

		return alignmentVectors;
	}

	public void AddGorbon(GorbonAI gorbon){
		gorbons.Add(gorbon);
	}

	public void RemoveGorbon(GorbonAI gorbon){
		Debug.Log("Removing gorbon");
		gorbons.Remove(gorbon);
	}
}

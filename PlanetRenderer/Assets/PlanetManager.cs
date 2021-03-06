﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.IO;

public class PlanetManager : MonoBehaviour {
	public bool smoothPosition = true;
	public float trailPersistTime = 1000f;

	public int maxSpheres = 11;
	public float iterationsPerSecond = 1f;
	public float systemScale = .00000001f;
	public bool addSunLight = true;
	public GameObject planetPrefab;
	public Material sunMat;
	public GameObject sunLight;

	float trailCurrentTime = -1f;
	float timer = 0;
	public int tick = -1;
	List<StringReader> readers;
	Dictionary<int, PlanetTransform> planets = new Dictionary<int, PlanetTransform>();

	// Use this for initialization
	void Start () {
		//Load text files
		TextAsset[] files = Resources.LoadAll<TextAsset>("Text");
		readers = Array.ConvertAll(files, item => new StringReader(item.text)).ToList();
	}
	class PlanetTransform{
		public Vector3 previousPosition;
		public Vector3 nextPosition;
		public Transform transform;

		public PlanetTransform(Transform t){
			transform = t;
			previousPosition = t.position;
			nextPosition = t.position;
		}
	}

	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		float stepTime = 1f/iterationsPerSecond;

		if(timer >= stepTime){
			timer -= stepTime;
			//Move planets
			for (int i = readers.Count - 1; i >= 0; i--) {
				while(true){
					var stringReader = readers [i];
					string line = stringReader.ReadLine();
					if(line == null){
						//O(1) operation only if last index in List, otherwise O(n)
						readers.RemoveAt(i);
						break;
					}
					if(line.Contains("Tick")){
						//End of step
						break;
					}
					var parts = line.Split(new Char[]{'\t'}, StringSplitOptions.RemoveEmptyEntries);
					//ID,x,y,z
					int id = int.Parse(parts[0]);
					if(!planets.ContainsKey(id)){
						GameObject go = (GameObject)GameObject.Instantiate(planetPrefab,transform,false);
						if(id >= maxSpheres){
							Destroy(go.GetComponent<Renderer>());
							Destroy(go.GetComponent<MeshFilter>());
						}
						else{
							Gradient gradient = new Gradient();
							gradient.mode = GradientMode.Fixed;
							gradient.colorKeys = new GradientColorKey[]{new GradientColorKey(Color.red,0)};
							go.GetComponent<TrailRenderer>().colorGradient = gradient;
						}
						go.name = id.ToString();
						planets.Add(id,new PlanetTransform(go.transform));
						//If the sun
						if(addSunLight && id == 0){
							GameObject.Instantiate(sunLight, go.transform,false);
							go.GetComponent<Renderer>().material = sunMat;
						}
					}
					PlanetTransform pT =  planets[id];
					Vector3 pos = new Vector3(float.Parse(parts[1]),float.Parse(parts[2]),float.Parse(parts[3]));
					pT.previousPosition = pT.nextPosition;
					pT.nextPosition = pos * systemScale;
					if(tick == 0){
						pT.previousPosition = pT.nextPosition;
					}
					pT.transform.position = pT.previousPosition;
				}
			}
			tick++;
		}
		//If value is changed then change times on all trails in children
		if (Math.Abs (trailPersistTime - trailCurrentTime) > float.Epsilon) {
			trailCurrentTime = trailPersistTime;
			var trails = GetComponentsInChildren<TrailRenderer> ();
			for (int i = 0, trailsLength = trails.Length; i < trailsLength; i++) {
				trails [i].time = trailCurrentTime;
			}
		}

		//Move objects
		if(smoothPosition){
			foreach(PlanetTransform pT in planets.Values){
				pT.transform.position = Vector3.Lerp(pT.previousPosition, pT.nextPosition, timer/stepTime);
			}
		}
	}
}

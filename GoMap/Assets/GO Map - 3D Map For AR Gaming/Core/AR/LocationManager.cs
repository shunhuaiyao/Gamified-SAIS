using Assets;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.UI;

namespace GoMap {
	
	public class LocationManager : MonoBehaviour {

		public enum DemoLocation{
			NewYork, 
			Rome,
			NewYork2,
			Venice,
			SanFrancisco,
			Custom
		};

		public enum MotionPreset{
			Walk, 
			Bike,
			Car
		};

		public bool useLocationServices;

		public int zoomLevel = 17;
		public DemoLocation demoLocation;
		public Coordinates demo_CenterWorldCoordinates;
		[HideInInspector]
		public Vector2 demo_CenterWorldTile;

	//	[HideInInspector]
		public Coordinates currentLocation;

		[HideInInspector]
		public static Coordinates CenterWorldCoordinates;

		public float desiredAccuracy ;
		public float updateDistance ;

		[HideInInspector]
		public float updateEvery = 1 / 1000f;

		public MotionPreset simulateMotion = MotionPreset.Walk;
		float demo_WASDspeed = 20;

		public bool useBannerInsideEditor;
		public GameObject banner;
		public Text bannerText;

		public static bool IsOriginSet;
		public static bool UseLocationServices;
		public static LocationServiceStatus status;

		public event OnOriginSet onOriginSet;
		public delegate void OnOriginSet(Coordinates origin);

		public event OnLocationChanged onLocationChanged;
		public delegate void OnLocationChanged(Coordinates current);

		// Use this for initialization
		void Start () {
		
			if (Application.isEditor || !Application.isMobilePlatform) {
				useLocationServices = false;
			}

			if (useLocationServices) {
				Input.location.Start (desiredAccuracy, updateDistance);
			} else { //Demo origin

				switch (demoLocation)
				{
				case DemoLocation.NewYork:
					demo_CenterWorldCoordinates = currentLocation = new Coordinates (40.783435,-73.966249,0);
					break;
				case DemoLocation.NewYork2:
					demo_CenterWorldCoordinates = currentLocation = new Coordinates (40.70193632375534,-74.01628977185595,0);
					break;
				case DemoLocation.Rome:
					demo_CenterWorldCoordinates = currentLocation = new Coordinates (41.910509366663945,12.476284503936768,0);
					break;
				case DemoLocation.Venice:
					demo_CenterWorldCoordinates = currentLocation = new Coordinates (45.433184, 12.336831,0);
					break;
				case DemoLocation.SanFrancisco:
					demo_CenterWorldCoordinates = currentLocation = new Coordinates (37.80724101305343, -122.42086887359619,0);
					break;
				case DemoLocation.Custom:
					currentLocation = demo_CenterWorldCoordinates;
					break;
				default:
					break;
				}

				SetOrigin(demo_CenterWorldCoordinates);

			
			}

			UseLocationServices = useLocationServices;

			StartCoroutine (LocationCheck(updateEvery));

			StartCoroutine(LateStart(0.01f));

		}
			
		IEnumerator LateStart(float waitTime)
		{
			yield return new WaitForSeconds(waitTime);
			if (!useLocationServices) {
				adjust (); //This adjusts the current location just after the initialization
			}
		
		}
			
		IEnumerator LocationCheck (float repeatTime) {

			while (true) {
				
				status = Input.location.status;

				if (!useLocationServices) {
					if (Application.isEditor && useBannerInsideEditor)
						showBannerWithText (true, "GPS is disabled");
					yield return new WaitForSeconds(repeatTime);
				}
				else if (status == LocationServiceStatus.Failed) {
					showBannerWithText (true, "GPS signal not found");
					yield return new WaitForSeconds(repeatTime);
				}
				else if (status == LocationServiceStatus.Stopped) {
					showBannerWithText (true, "GPS signal not found");
					yield return new WaitForSeconds(repeatTime);
				}
				else if (status == LocationServiceStatus.Initializing) {
					showBannerWithText (true, "GPS signal not found");
					yield return new WaitForSeconds(repeatTime);
				} 
				else if (status == LocationServiceStatus.Running) {

					if (Input.location.lastData.horizontalAccuracy > desiredAccuracy) {
						showBannerWithText (true, "GPS signal is weak");
						yield return new WaitForSeconds (repeatTime);
					} else {
						showBannerWithText (false, "GPS signal ok!");

						if (!IsOriginSet) {
							SetOrigin (new Coordinates (Input.location.lastData));
						}
						LocationInfo info = Input.location.lastData;
						if (info.latitude != currentLocation.latitude || info.longitude != currentLocation.longitude) {
							currentLocation.updateLocation (Input.location.lastData);
							if (onLocationChanged != null) {
								onLocationChanged (currentLocation);
							}
						}
					}
				}

				if (!useLocationServices && Application.isEditor) {
					changeLocationWASD ();
				}

				yield return new WaitForSeconds(repeatTime);

			}
		}

		void SetOrigin(Coordinates coords) {
			IsOriginSet = true;
			CenterWorldCoordinates = coords.tileCenter(zoomLevel);
			demo_CenterWorldTile = coords.tileCoordinates(zoomLevel);
			Coordinates.setWorldOrigin (CenterWorldCoordinates);
			if (onOriginSet != null) {
				onOriginSet (CenterWorldCoordinates);
			}
		}

		////UI
		void showBannerWithText(bool show, string text) {

			if (banner == null || bannerText == null) {
				return;
			}

			bannerText.text = text;

			RectTransform bannerRect = banner.GetComponent<RectTransform> ();
			bool alreadyOpen = bannerRect.anchoredPosition.y != bannerRect.sizeDelta.y;

			if (show != alreadyOpen) {
				StartCoroutine (Slide (show, 1));
			}

		}

		private IEnumerator Slide(bool show, float time) {

//			Debug.Log ("Toggle banner");

			Vector2 newPosition;
			RectTransform bannerRect = banner.GetComponent<RectTransform> ();

			if (show) {//Open
				newPosition = new Vector2 (bannerRect.anchoredPosition.x, 0);
			} else { //Close
				newPosition = new Vector2 (bannerRect.anchoredPosition.x, bannerRect.sizeDelta.y);
			} 

			float elapsedTime = 0;
			while (elapsedTime < time)
			{
				bannerRect.anchoredPosition = Vector2.Lerp(bannerRect.anchoredPosition, newPosition, (elapsedTime / time));
				elapsedTime += Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
				
		}
			
		////EDITOR
		/// 
		/// 
		/// 
		/// 

		void adjust () {
		
			Vector3 current = currentLocation.convertCoordinateToVector ();
			Vector3 v = current;
			currentLocation = Coordinates.convertVectorToCoordinates (v);
			v = current + new Vector3(0, 0 , 0.4f);
			currentLocation = Coordinates.convertVectorToCoordinates (v);
			if (onLocationChanged != null) {
				onLocationChanged (currentLocation);
			}
		}

		void changeLocationWASD (){

			switch (simulateMotion)
			{
			case MotionPreset.Car:
				demo_WASDspeed = 4;
				break;
			case MotionPreset.Bike:
				demo_WASDspeed = 2;
				break;
			case MotionPreset.Walk:
				demo_WASDspeed = 0.4f;
				break;
			default:
				break;
			}


			Vector3 current = currentLocation.convertCoordinateToVector ();
			Vector3 v = current;

			if (Input.GetKey (KeyCode.W)){
				v = current + new Vector3(0, 0 , demo_WASDspeed);
			}
			if (Input.GetKey (KeyCode.S)){
				v = current + new Vector3(0, 0 , -demo_WASDspeed);
			}
			if (Input.GetKey (KeyCode.A)){
				v = current + new Vector3(-demo_WASDspeed, 0 , 0);
			}
			if (Input.GetKey (KeyCode.D)){
				v = current + new Vector3(demo_WASDspeed, 0 , 0);
			}

			if (v != current) {
				currentLocation = Coordinates.convertVectorToCoordinates (v);
				if (onLocationChanged != null) {
					onLocationChanged (currentLocation);
				}
			}
		}
	}
}
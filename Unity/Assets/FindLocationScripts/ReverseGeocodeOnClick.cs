// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Geospatial;
using Microsoft.Maps.Unity;
using TMPro;
using UnityEngine;

/// <summary>
/// Instantiates a <see cref="MapPin"/> for each location that is reverse geocoded.
/// The <see cref="MapPin"/> will display the address of the reverse geocoded location.
/// </summary>
[RequireComponent(typeof(MapRenderer))]
public class ReverseGeocodeOnClick : MonoBehaviour
{
    public TextMeshProUGUI latLonText;
    public static double latitude = 90.0;
    public static double longitude = 135.0;
    private MapRenderer _mapRenderer = null;

    /// <summary>
    /// The layer to place MapPins.
    /// </summary>
    [SerializeField]
    private MapPinLayer _mapPinLayer = null;

    /// <summary>
    /// The MapPin prefab to instantiate for each location that is reverse geocoded.
    /// If it uses a TextMeshPro component, the address text will be written to it.
    /// </summary>
    [SerializeField]
    private MapPin _mapPinPrefab = null;

    // removes map pins from the map pin layer, doesn't work
/*
    public void RemoveMapPins()
    {
        _mapPinLayer.MapPins.Clear();
    }
*/

    public void Awake()
    {
        _mapRenderer = GetComponent<MapRenderer>();
        Debug.Assert(_mapRenderer != null);
        Debug.Assert(_mapPinLayer != null);
    }

//    public async void OnTapAndHold(LatLonAlt latLonAlt)
    public void OnTapAndHold(LatLonAlt latLonAlt)
    {
        if (ReferenceEquals(MapSession.Current, null) || string.IsNullOrEmpty(MapSession.Current.DeveloperKey))
        {
            Debug.LogError(
                "Provide a Bing Maps key to use the map services. " +
                "This key can be set on a MapSession component.");
            return;
        }

//        var finderResult = await MapLocationFinder.FindLocationsAt(latLonAlt.LatLon);

/*
        string formattedAddressString = null;
        if (finderResult.Locations.Count > 0)
        {
            formattedAddressString = finderResult.Locations[0].Address.FormattedAddress;
        }
*/

        if (_mapPinPrefab != null)
        {
            // Create a new MapPin instance at the specified location.
            var newMapPin = Instantiate(_mapPinPrefab);
            newMapPin.Location = latLonAlt.LatLon;
            latitude = latLonAlt.LatitudeInDegrees;
            longitude = latLonAlt.LongitudeInDegrees;
            latLonText = GameObject.Find("LatitudeLongitudeText").GetComponent<TextMeshProUGUI>();
            latLonText.text = "Latitude: " + latitude.ToString() + "   Longitude: " + longitude.ToString();
            Debug.Log("What is the latitude?" + latLonAlt.LatitudeInDegrees);
            Debug.Log("What is the longitude?" + latLonAlt.LongitudeInDegrees);
//            var textMesh = newMapPin.GetComponentInChildren<TextMeshPro>();
//            textMesh.text = formattedAddressString ?? "No address found.";

            _mapPinLayer.MapPins.Clear();
            _mapPinLayer.MapPins.Add(newMapPin);
        }
    }
}
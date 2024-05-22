using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrafficSim.WorldManagers
{
    public class TimeManager : MonoBehaviour
    {
        [SerializeField] private float timeScale;
        [SerializeField] private float startHour;
        [SerializeField] private Light sunLight;
        [SerializeField] private float sunriseHour;
        [SerializeField] private float sunsetHour;
        private DateTime currentTime;
        private TimeSpan sunriseTime;
        private TimeSpan sunsetTime;


        // Start is called before the first frame update
        void Start()
        {
            // Set the current time.
            currentTime = DateTime.Now.Date + TimeSpan.FromHours(startHour);

            // Set the sunrise and sunset times.
            sunriseTime = TimeSpan.FromHours(sunriseHour);
            sunsetTime = TimeSpan.FromHours(sunsetHour);
        }

        // Update is called once per frame
        void Update()
        {
            // Update the time of day.
            UpdateTimeOfDay();

            // Apply rotation to the sun.
            RotateSun();
        }

        // Used to update the time of day using deltaTime.
        private void UpdateTimeOfDay()
        {
            // Add deltaTime to the current time.
            currentTime = currentTime.AddSeconds(Time.deltaTime * timeScale);
        }

        // Used to rotate the sun based on the time of day.
        private void RotateSun()
        {
            // Will store the rotation of the sun.
            float sunLightRotation;

            // If the current time is between sunrise and sunset.
            if (currentTime.TimeOfDay > sunriseTime && currentTime.TimeOfDay < sunsetTime)
            {
                // Calculate the percentage of the day that has passed.
                TimeSpan sunriseToSunsetDuration = TimeDifference(sunriseTime, sunsetTime);
                TimeSpan timeSinceSunrise = TimeDifference(sunriseTime, currentTime.TimeOfDay);

                // Calculate the percentage of time passed since sunrise.
                double percentage = timeSinceSunrise.TotalMinutes / sunriseToSunsetDuration.TotalMinutes;

                // Calculate the rotation of the sun based on the percentage of time passed.
                sunLightRotation = Mathf.Lerp(0, 180, (float)percentage);
            }
            else
            {
                // Calculate the percentage of the night that has passed.
                TimeSpan sunsetToSunriseDuration = TimeDifference(sunsetTime, sunriseTime);
                TimeSpan timeSinceSunset = TimeDifference(sunsetTime, currentTime.TimeOfDay);

                // Calculate the percentage of time passed since sunset.
                double percentage = timeSinceSunset.TotalMinutes / sunsetToSunriseDuration.TotalMinutes;

                // Calculate the rotation of the sun based on the percentage of time passed.
                sunLightRotation = Mathf.Lerp(180, 360, (float)percentage);
            }

            // Apply the rotation to the sun.
            sunLight.transform.rotation = Quaternion.AngleAxis(sunLightRotation, Vector3.right);
        }

        // Used to calculate the time difference between two TimeSpans.
        private TimeSpan TimeDifference(TimeSpan fromTime, TimeSpan toTime)
        {
            // Get the difference between the two times.
            TimeSpan difference = toTime - fromTime;

            // If the difference is negative, add 24 hours.
            if (difference.TotalSeconds < 0) difference += TimeSpan.FromHours(24);

            // Return the difference.
            return difference;
        }

        // Used to check if we are in daytime.
        public bool IsDaytime()
        {
            // Return true if the current time is between sunrise and sunset.
            return (currentTime.TimeOfDay > sunriseTime && currentTime.TimeOfDay < sunsetTime);
        }

        /**
         * Used to get the current time as a float in seconds.
         */
        public float GetCurrentMilliseconds()
        {
            // Get the current time as a float in ms
            float currentTimeMilliseconds = (float)currentTime.TimeOfDay.TotalMilliseconds;

            // Return the current time.
            return currentTimeMilliseconds;
        }
    }

}
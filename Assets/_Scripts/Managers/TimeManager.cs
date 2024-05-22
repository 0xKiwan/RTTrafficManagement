using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrafficSim.WorldManagers
{
    public class TimeManager : MonoBehaviour
    {
        // Time scale for controlling the speed of time.
        [SerializeField] private float timeScale;

        // Initial hour of the day.
        [SerializeField] private float startHour;

        // Reference to the directional light representing the sun.
        [SerializeField] private Light sunLight;

        // Hour of sunrise and sunset.
        [SerializeField] private float sunriseHour;
        [SerializeField] private float sunsetHour;

        // Current time in the simulation.
        public DateTime currentTime;

        // Time of sunrise and sunset as TimeSpan.
        private TimeSpan sunriseTime;
        private TimeSpan sunsetTime;

        // Duration of day and night.
        private TimeSpan dayDuration;
        private TimeSpan nightDuration;

        // Event triggered when time changes.
        public event Action<bool> OnTimeChanged;

        // Frequency of time updates (in seconds).
        private float updateFrequency = 0.01f;
        private float timeSinceLastUpdate = 0f;

        private void Start()
        {
            // Initialize current time and time-related variables.
            currentTime = DateTime.Today.AddHours(startHour);
            sunriseTime = TimeSpan.FromHours(sunriseHour);
            sunsetTime = TimeSpan.FromHours(sunsetHour);
            dayDuration = sunsetTime - sunriseTime;
            nightDuration = sunriseTime - sunsetTime + TimeSpan.FromHours(24);
        }

        private void Update()
        {
            // Update the time since the last update.
            timeSinceLastUpdate += Time.deltaTime;

            // Check if it's time for an update.
            if (timeSinceLastUpdate >= updateFrequency)
            {
                // Update the time of day.
                UpdateTimeOfDay();

                // Rotate the sun.
                RotateSun();

                // Reset the time since last update.
                timeSinceLastUpdate = 0f;

                // Notify listeners about time change.
                OnTimeChanged?.Invoke(IsDaytime());
            }
        }

        // Update the current time based on the time scale.
        private void UpdateTimeOfDay()
        {
            currentTime = currentTime.AddSeconds(updateFrequency * timeScale);
        }

        // Rotate the sun based on the current time.
        private void RotateSun()
        {
            float sunLightRotation;
            TimeSpan timeSinceReference;
            double percentage;

            if (IsDaytime())
            {
                timeSinceReference = currentTime.TimeOfDay - sunriseTime;
                percentage = timeSinceReference.TotalMinutes / dayDuration.TotalMinutes;
                sunLightRotation = Mathf.Lerp(0, 180, (float)percentage);
            }
            else
            {
                timeSinceReference = currentTime.TimeOfDay - sunsetTime;
                if (timeSinceReference.TotalSeconds < 0) timeSinceReference += TimeSpan.FromHours(24);
                percentage = timeSinceReference.TotalMinutes / nightDuration.TotalMinutes;
                sunLightRotation = Mathf.Lerp(180, 360, (float)percentage);
            }

            sunLight.transform.rotation = Quaternion.AngleAxis(sunLightRotation, Vector3.right);
        }

        // Check if it's currently daytime.
        public bool IsDaytime()
        {
            var currentTimeOfDay = currentTime.TimeOfDay;
            return currentTimeOfDay >= sunriseTime && currentTimeOfDay < sunsetTime;
        }

        // Get the current time in milliseconds.
        public float GetCurrentMilliseconds()
        {
            return (float)currentTime.TimeOfDay.TotalMilliseconds;
        }
    }
}
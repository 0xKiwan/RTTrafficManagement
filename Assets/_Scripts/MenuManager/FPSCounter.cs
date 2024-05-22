using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    // The TextMeshPro object.
    public TextMeshProUGUI fpsText;

    // Start is called before the first frame update
    void Start()
    {
        // Get the fpsText object.
        fpsText = GetComponent<TextMeshProUGUI>();

        // Set the fpsText to the current FPS.
        fpsText.text = "FPS: " + (1.0f / Time.deltaTime).ToString("0.0");
    }

    // Update is called once per frame
    void Update()
    {
        // Set the fpsText to the current FPS. Once every 60 frames.
        if (Time.frameCount % 60 == 0)
        {
            // Update the fpsText.
            fpsText.text = "FPS: " + (1.0f / Time.deltaTime).ToString("0.0");
        }
    }
}

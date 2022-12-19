/*
 * Copyright (c) 2020 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * Notwithstanding the foregoing, you may not use, copy, modify, merge, publish, 
 * distribute, sublicense, create a derivative work, and/or sell copies of the 
 * Software in any work that is designed, intended, or marketed for pedagogical or 
 * instructional purposes related to programming, coding, application development, 
 * or information technology.  Permission for such use, copying, modification,
 * merger, publication, distribution, sublicensing, creation of derivative works, 
 * or sale is expressly withheld.
 *    
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using UnityEngine;

public class Launcher : MonoBehaviour
{
    // EfxZoom
    public GameObject efxZoomObj;
    private SpriteRenderer _efxZoomRenderer;
    private AnimateController _efxZoomAniController;
    // EfxZoom light
    public GameObject efxLightObj;
    private SpriteRenderer _efxLightRenderer;
    private AnimateController _efxLightAniController;
    // Sound
    private SoundController _sounds;
    private AudioSource _pullSound;
    private AudioSource _shootSound;
    // Touch Listener
    public bool isTouched = false;
    public bool isKeyPress = false;
    // Ready for Launch
    public bool isActive = true;
    // Timers
    private float _pressTime = 0f;
    private float _startTime = 0f;
    private int _powerIndex;

    private SpringJoint2D _springJoint;
    private Rigidbody2D _rb;
    private float _force = 0f;          // current force generated
    public float maxForce = 90f;

    void Start()
    {
        // zoom animation object
        _efxZoomAniController = efxZoomObj.GetComponent<AnimateController>();
        _efxLightAniController = efxLightObj.GetComponent<AnimateController>();
        // zoom light object
        _efxZoomRenderer = efxZoomObj.GetComponent<Renderer>() as SpriteRenderer;
        _efxLightRenderer = efxLightObj.GetComponent<Renderer>() as SpriteRenderer;
        // sounds
        _sounds = GameObject.Find("SoundObjects").GetComponent<SoundController>();
        _pullSound = _sounds.pulldown;
        _shootSound = _sounds.zonar;

        _springJoint = GetComponent<SpringJoint2D>();
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isActive)
        {
            if (Input.GetKeyDown("space"))
            {
                isKeyPress = true;
            }

            if (Input.GetKeyUp("space"))
            {
                isKeyPress = false;
            }

            // on keyboard press or touch hotspot
            if (isKeyPress == true && isTouched == false || isKeyPress == false && isTouched == true)
            {
                if (_startTime == 0f)
                {
                    _startTime = Time.time;
                    _pullSound.Play();
                }
            }

            // on keyboard release 
            if (isKeyPress == false && isTouched == false && _startTime != 0f)
            {
                // #1
                // calculates current force of exertion
                _force = _powerIndex * maxForce;

                _shootSound.Play();
                // reset values & animation
                _pressTime = 0f;
                _startTime = 0f;
                _efxLightRenderer.sprite = _efxLightAniController.spriteSet[1];
                while (_powerIndex >= 0)
                {
                    _efxZoomRenderer.sprite = _efxZoomAniController.spriteSet[_powerIndex];
                    _powerIndex--;
                }
            }
        }

        // Start Press
        if (_startTime != 0f)
        {
            _pressTime = Time.time - _startTime;
            // plays zoom animation on loop
            _powerIndex = (int)Mathf.PingPong(_pressTime * _efxZoomAniController.fps, _efxZoomAniController.spriteSet.Length);
            _efxZoomRenderer.sprite = _efxZoomAniController.spriteSet[_powerIndex];
            // turns on/ off zoom light based on powerIndex
            if (_powerIndex == _efxZoomAniController.spriteSet.Length - 1)
            {
                _efxLightRenderer.sprite = _efxLightAniController.spriteSet[0];
            }
            else
            {
                _efxLightRenderer.sprite = _efxLightAniController.spriteSet[1];
            }
        }
    }

    private void FixedUpdate()
    {
        // When force is not 0
        if (_force != 0)
        {
            // release springJoint and power up
            _springJoint.distance = 1f;
            _rb.AddForce(Vector3.up * _force);
            _force = 0;
        }

        // When the plunger is held down
        if (_pressTime != 0)
        {
            // retract the springJoint distance and reduce the power
            _springJoint.distance = 0.8f;
            _rb.AddForce(Vector3.down * 400);
        }
    }
}

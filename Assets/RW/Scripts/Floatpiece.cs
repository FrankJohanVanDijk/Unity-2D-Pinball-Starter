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
using System.Collections;

public class Floatpiece : MonoBehaviour
{
    // Sound & Animation
    public GameObject handcamObj;
    public GameObject golightObj;
    private SpriteRenderer _handcamRenderer;
    private SpriteRenderer _golightRenderer;
    private AnimateController _handcamAniController;
    private AnimateController _golightAniController;
    private SoundController _sound;

    // Score
    private ScoreBoard _scoreBoard;

    private BuoyancyEffector2D _floatEffector;
    public float floatingTime = 0f; // floating duration
    private float _runningTime = 0f; // the current duration since startTime
    private float _startTime = 0f;

    void Start()
    {
        _floatEffector = GetComponent<BuoyancyEffector2D>(); // assign component to this instance

        // Get scoreboard and sound object
        GameObject obj = GameObject.Find("scoreText");
        _scoreBoard = obj.GetComponent<ScoreBoard>();
        _sound = GameObject.Find("SoundObjects").GetComponent<SoundController>();
        // Animation objects
        _handcamRenderer = handcamObj.GetComponent<Renderer>() as SpriteRenderer;
        _golightRenderer = golightObj.GetComponent<Renderer>() as SpriteRenderer;
        _handcamAniController = handcamObj.GetComponent<AnimateController>();
        _golightAniController = golightObj.GetComponent<AnimateController>();
    }

    void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.name == "ball")
        {
            // on enter zone, start float
            _floatEffector.density = 50;
            // start timer
            if (_startTime == 0f)
            {
                _startTime = Time.time;
                _sound.bonus.Play();
                _scoreBoard.gamescore += 10;
                _golightRenderer.sprite = _golightAniController.spriteSet[0];
                StartCoroutine(BeginFloat());
            }
        }
    }

    IEnumerator BeginFloat()
    {
        while (true)
        {
            // calculate current duration
            _runningTime = Time.time - _startTime;

            // play animation loop
            int index = (int)Mathf.PingPong(_handcamAniController.fps *
                        Time.time, _handcamAniController.spriteSet.Length);
            _handcamRenderer.sprite = _handcamAniController.spriteSet[index];
            yield return new WaitForSeconds(0.1f);

            // when time is up            
            if (_runningTime >= floatingTime)
            {
                // stop float and reset timer
                _floatEffector.density = 0;
                _runningTime = 0f;
                _startTime = 0f;

                // stop sound & animation 
                _sound.bonus.Stop();
                _golightRenderer.sprite = _golightAniController.spriteSet[1];
                _handcamRenderer.sprite = _handcamAniController.spriteSet
                              [_handcamAniController.spriteSet.Length - 1];
                break;
            }
        }
    }
}
